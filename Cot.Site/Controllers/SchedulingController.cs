using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cot.Entities;
using Cot.Entities.Services;
using Cot.Entities.Values;
using Cot.IRepositories;

namespace Cot.Site.Controllers
{
	public class SchedulingController : BasiceController
	{
		public ActionResult Index()
		{
			return View(new Scheduling[0]);
		}
		[HttpPost]
		public ActionResult Index(DateTime start, DateTime end)
		{
			ViewBag.Start = start;
			ViewBag.End = end;
			var entities = UnitOfWork.GetRepository<IRepository<Scheduling>>().Query()
				.Include(_ => _.Deliveries)
				.Where(_ => (_.StartDate >= start && _.StartDate <= end) || (_.EndDate >= start && _.EndDate <= end));
			return View(entities);
		}

		public ActionResult Add()
		{
			return View("Edit", new Scheduling { Deliveries = new Delivery[0] });
		}
		[HttpPost]
		public ActionResult Add(Scheduling model)
		{
			model.StartDate = model.Deliveries.OrderBy(_ => _.Date).First().Date;
			model.EndDate = model.Deliveries.OrderBy(_ => _.Date).Last().Date;
			UnitOfWork.GetRepository<IRepository<Scheduling>>().Add(model);
			UnitOfWork.Commit();
			return View("Edit", model);
		}

		public ActionResult Edit(int id)
		{
			var entity = UnitOfWork.GetRepository<IRepository<Scheduling>>().Query()
				.Include(_ => _.Deliveries)
				.FirstOrDefault(_ => _.Id == id);
			return View(entity);
		}
		[HttpPost]
		public ActionResult Edit(Scheduling model)
		{
			model.StartDate = model.Deliveries.OrderBy(_ => _.Date).First().Date;
			model.EndDate = model.Deliveries.OrderBy(_ => _.Date).Last().Date;
			UnitOfWork.GetRepository<IRepository<Scheduling>>().Remove(new Scheduling { Id = model.Id });
			UnitOfWork.Commit();
			model.Id = 0;
			UnitOfWork.GetRepository<IRepository<Scheduling>>().Add(model);
			UnitOfWork.Commit();
			return RedirectToAction("Index");
		}

		public ActionResult Del(int id)
		{
			UnitOfWork.GetRepository<IRepository<Scheduling>>().Remove(new Scheduling { Id = id });
			UnitOfWork.Commit();
			return RedirectToAction("Index");
		}

		public ActionResult Run(IEnumerable<int> id, DateTime date, DateTime start)
		{
			var poIds = new List<int>();
			var boms = new List<Tuple<Scheduling, Bom>>();
			var schedulings = UnitOfWork.GetRepository<IRepository<Scheduling>>().Query()
				.Include(_ => _.Deliveries)
				.Where(_ => id.Contains(_.Id))
				.ToArray();
			foreach (var item in schedulings)
			{
				var bom = UnitOfWork.GetRepository<IRepository<Bom>>().Query()
					.FirstOrDefault(
						_ => _.CustomerCode == item.CustomerCode && _.ProductName == item.ProductName && _.ProductSpec == item.Spec);
				bom.Items = UnitOfWork.GetRepository<IRepository<BomItem>>().Query()
					.Where(_ => _.BomId == bom.Id).ToArray();
				bom.BomProcesses = UnitOfWork.GetRepository<IRepository<BomProcess>>().Query()
					.Where(_ => _.BomId == bom.Id).ToArray();
				boms.Add(new Tuple<Scheduling, Bom>(item, bom));
			}
			foreach (var item in boms)
			{
				var bom = item.Item2;
				var scheduling = item.Item1;
				foreach (var delivery in scheduling.Deliveries.OrderBy(_ => _.Date))
				{
					var po = BomToPoScrvice.From(bom, scheduling, delivery);
					po.Reset();
					UnitOfWork.GetRepository<IRepository<Po>>().Add(po);
					UnitOfWork.Commit();
					poIds.Add(po.Id);
					foreach (var pi in po.Items)
					{
						pi.PoId = po.Id;
						UnitOfWork.GetRepository<IRepository<PoItem>>().Add(pi);
					}
					UnitOfWork.Commit();
				}
			}
			StockNeed(poIds.ToArray());
			return RedirectToAction("Index");
		}

		private void StockNeed(params int[] id)
		{
			var requisition = new Requisition { Date = DateTime.Now };//请购单
			var requisitionItems = new List<RequisitionItem>();//请购单项
			var pos = UnitOfWork.GetRepository<IRepository<Po>>().Query()
				.Where(_ => id.Contains(_.Id)).ToArray();//被选中的生产工单
			var poItems = UnitOfWork.GetRepository<IRepository<PoItem>>().Query()
				.Where(_ => id.Contains(_.PoId)).ToArray();//被选中的生产工单项
			var mNames = poItems.Select(_ => _.MaterialName).Distinct();//所需物料的名称

			foreach (var name in mNames)//对每种物料进行库存量计算，对不足量生成请购单项
			{
				var mPoItems = poItems.Where(_ => _.MaterialName == name).OrderBy(_ => _.Width);//需要该物料的生产工单项
				var materials = UnitOfWork.GetRepository<IRepository<Material>>().Query()
					.Where(_ => _.Name == name).OrderBy(_ => _.Width).ToArray();//该名称的物料
				var rawMaterial = UnitOfWork.GetRepository<IRepository<RawMaterial>>().Query().FirstOrDefault(_ => _.Name == name) ?? new Material { Width = 1100, Length = 100000 };//原材料，物料最初的规格
				var requisitionCount = 0d;//该物料请购量
				foreach (var poItem in mPoItems)//依次计算每个生产工单项的库存供需量
				{
					var po = pos.First(_ => _.Id == poItem.PoId);//生产工单项的生产工单
					var cutParam = new CutParam
					{
						Cavity = po.Cavity,
						Multiple = poItem.Multiple,
						Skip = po.Skip,
						Type = poItem.CutType,
						Width = poItem.Width
					};//生产工单项的裁切参数
					var lack = po.WorkQuantity;//库存裁切不足量
					foreach (var material in materials.Where(_ => _.Width >= poItem.Width).ToArray())//从最符合裁切参数的物料开始计算
					{
						int real;
						var cutResult = material.Cut(cutParam, po.WorkQuantity, out real);
						lack -= real;
						var old = materials.First(_ => _.Id == material.Id);//扣减使用量，不修改数据库
						if (cutResult == null) //无剩余
						{
							old.Width = old.Length = 0;
						}
						else
						{
							old.Width = cutResult.Width;
							old.Length = cutResult.Length;
						}
					}
					requisitionCount += rawMaterial.Need(cutParam, lack);
				}
				if (requisitionCount == 0) continue;

				requisitionItems.Add(new RequisitionItem
				{
					Count = requisitionCount,
					MaterialCode = rawMaterial.Code,
					MaterialName = name,
					Spec = rawMaterial.Width + "*" + rawMaterial.Length,
					Delivery = DateTime.Now
				});//添加请购单项
			}
			requisition.Items = requisitionItems;
			//保存请购单
			UnitOfWork.GetRepository<IRepository<Requisition>>().Add(requisition);
			UnitOfWork.Commit();
		}
	}
}
