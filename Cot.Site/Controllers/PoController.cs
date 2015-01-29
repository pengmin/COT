using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cot.Entities;
using Cot.Entities.Values;
using Cot.IRepositories;

namespace Cot.Site.Controllers
{
	public class PoController : BasiceController
	{
		public ActionResult Index()
		{
			var query = UnitOfWork.GetRepository<IRepository<Po>>().Query();
			return View(query);
		}

		public ActionResult Edit(int id)
		{
			var entity = UnitOfWork.GetRepository<IRepository<Po>>().Query().FirstOrDefault(_ => _.Id == id);
			entity.Items = UnitOfWork.GetRepository<IRepository<PoItem>>().Query().Where(_ => _.PoId == entity.Id).ToArray();
			return View(entity);
		}
		[HttpPost]
		public ActionResult Edit(Po po)
		{
			var items = UnitOfWork.GetRepository<IRepository<PoItem>>().Query().Where(_ => _.PoId == po.Id);
			foreach (var item in items)
			{
				item.Po = po;
			}
			po.Items = items.ToArray();
			po.Reset();
			foreach (var item in po.Items)
			{
				UnitOfWork.GetRepository<IRepository<PoItem>>().Modify(item);
			}
			UnitOfWork.GetRepository<IRepository<Po>>().Modify(po);
			UnitOfWork.Commit();
			return RedirectToAction("Edit", po.Id);
		}

		public ActionResult Del(string id)
		{
			var keys = id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var item in keys)
			{
				var po = UnitOfWork.GetRepository<IRepository<Po>>().Query().FirstOrDefault(_ => _.Id.ToString() == item);
				var pis = UnitOfWork.GetRepository<IRepository<PoItem>>().Query().Where(_ => _.PoId.ToString() == item);
				foreach (var pi in pis)
				{
					UnitOfWork.GetRepository<IRepository<PoItem>>().Remove(pi);
				}
				UnitOfWork.GetRepository<IRepository<Po>>().Remove(po);
			}
			UnitOfWork.Commit();
			return RedirectToAction("Index");
		}

		public ActionResult StockNeed(params int[] id)
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
				var rawMaterial = new Material { Width = 1100, Length = 100000 };//原材料，物料最初的规格
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
			return null;
		}
	}
}
