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

		public ActionResult Clear()
		{
			var pos = UnitOfWork.GetRepository<IRepository<Po>>().Query().ToArray();
			var poIds = pos.Select(_ => _.Id);
			var poItems = UnitOfWork.GetRepository<IRepository<PoItem>>().Query()
				.Where(_ => poIds.Contains(_.PoId)).ToArray();
			var res = UnitOfWork.GetRepository<IRepository<Requisition>>().Query().ToArray();
			var smalls = UnitOfWork.GetRepository<IRepository<SmallScheduling>>().Query().ToArray();
			foreach (var item in pos)
			{
				UnitOfWork.GetRepository<IRepository<Po>>().Remove(item);
			}
			foreach (var item in poItems)
			{
				UnitOfWork.GetRepository<IRepository<PoItem>>().Remove(item);
			}
			foreach (var item in res)
			{
				UnitOfWork.GetRepository<IRepository<Requisition>>().Remove(item);
			}
			foreach (var item in smalls)
			{
				UnitOfWork.GetRepository<IRepository<SmallScheduling>>().Remove(item);
			}
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
			date = date.AddDays(1).AddSeconds(-1);//当天的最后一秒
			SmallScheduling(id.ToArray(), start, date);
			return RedirectToAction("Index", "SmallScheduling");
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

		private void SmallScheduling(int[] ids, DateTime start, DateTime end)
		{
			var schedulings = getSchedulings(ids, start, end);//所有要进行生产排程的交货排程及交期
			var boms = GetBoms(schedulings);//获取关联的BOM
			var pos = GetPos(boms, start, end);//获取bom关联的工单
			var machines = GetAllMachine(boms);//所有需要的机台
			//
			start = start.AddHours(8);//每日上班时间
			end = start.AddHours(18);//每日下班时间
			var detail = GetDetail(schedulings, boms, pos, start, end);//生成工序时序表
			var planInfo = Plan(detail, machines, start, end);
			SaveSmallScheduling(planInfo);
		}

		private IEnumerable<Scheduling> getSchedulings(int[] ids, DateTime start, DateTime end)
		{
			return UnitOfWork.GetRepository<IRepository<Scheduling>>().Query()
				.Where(_ => ids.Contains(_.Id) && !(_.StartDate > end || _.EndDate < start)).ToArray();
		}
		private IEnumerable<Bom> GetBoms(IEnumerable<Scheduling> schedulings)
		{
			var boms = new List<Bom>();
			foreach (var item in schedulings)
			{
				var bom = UnitOfWork.GetRepository<IRepository<Bom>>().Query()
					.FirstOrDefault(
						_ => _.CustomerCode == item.CustomerCode && _.ProductName == item.ProductName && _.ProductSpec == item.Spec);
				if (bom != null)
				{
					//bom.Items = UnitOfWork.GetRepository<IRepository<BomItem>>().Query()
					//	.Where(_ => _.BomId == bom.Id);
					bom.BomProcesses = UnitOfWork.GetRepository<IRepository<BomProcess>>().Query()
						.Where(_ => _.BomId == bom.Id);
					boms.Add(bom);
				}
			}
			return boms;
		}
		private IEnumerable<Po> GetPos(IEnumerable<Bom> boms, DateTime start, DateTime end)
		{
			var list = new List<Po>();
			foreach (var bom in boms)
			{
				var pos = UnitOfWork.GetRepository<IRepository<Po>>().Query()
					.Where(
						_ =>
							_.CustomerCode == bom.CustomerCode && _.ProductName == bom.ProductName && _.ProductSpec == bom.ProductSpec &&
							_.Delivery >= start && _.Delivery <= end)
					.ToArray();
				var po = pos.OrderBy(_ => _.Delivery).First();
				po.OrderQuantity = pos.Sum(_ => _.OrderQuantity);
				list.Add(po);//只返回同一bom关联的工单的汇总工单
			}
			return list;
		}
		private IEnumerable<string> GetAllMachine(IEnumerable<Bom> boms)
		{
			var list = new List<string>();
			foreach (var bom in boms)
			{
				foreach (var process in bom.BomProcesses)
				{
					list.AddRange(process.Machine.Split('/'));
				}
			}
			return list.Distinct();
		}
		private IEnumerable<SchedulingInfo> GetDetail(IEnumerable<Scheduling> schedulings, IEnumerable<Bom> boms, IEnumerable<Po> pos, DateTime start, DateTime end)
		{
			var details = new List<SchedulingInfo>();//工序排程列表
			foreach (var scheduling in schedulings)
			{
				var bom =
					boms.First(
						_ =>
							_.CustomerCode == scheduling.CustomerCode && _.ProductName == scheduling.ProductName &&
							_.ProductSpec == scheduling.Spec);
				var po =
						pos.First(
							_ => _.CustomerCode == bom.CustomerCode && _.ProductName == bom.ProductName && _.ProductSpec == bom.ProductSpec);
				var i = -1;
				var days = 0;
				foreach (var process in bom.BomProcesses.OrderBy(_ => _.Name))
				{
					var detail = new SchedulingInfo
					{
						Scheduling = scheduling,
						Bom = bom,
						Po = po,
						Process = process,
						ProcessName = process.Name,
						WorkTimes = (po.WorkQuantity + 0d) / po.Cavity / process.Capacity + process.Debug,
						Machines = process.Machine.Split('/')
					};
					if (++i == 0)//第一道工序，从上班时间开始
					{
						detail.Start = start;
					}
					else
					{
						detail.Prev = details.Last();//前道工序
						if (detail.Process.Capacity <= detail.Prev.Process.Capacity)//当前工序产能不大于前道工序，两者可以同时开使
						{
							detail.Start = detail.Prev.Start;
						}
						else
						{
							//若上一道工序完成时间已到下班时间，当前工序排到下一天；否则排到前一到工序完成时间之后
							detail.Start = detail.Prev.End >= end ? start.AddDays(++days) : detail.Prev.End;
						}
					}
					details.Add(detail);
				}
			}
			return details;
		}
		//交期、Bom、Po、工序名称、工时、适合机台、是否已排班、key
		private IEnumerable<Tuple<Scheduling, Bom, Po, string, double, IEnumerable<string>, /*bool,*/ int>> GetDetail(
			IEnumerable<Scheduling> schedulings, IEnumerable<Bom> boms, IEnumerable<Po> pos)
		{
			var key = 0;
			var list = new List<Tuple<Scheduling, Bom, Po, string, double, IEnumerable<string>, /*bool,*/ int>>();
			foreach (var scheduling in schedulings)
			{
				var bom =
					boms.First(
						_ =>
							_.CustomerCode == scheduling.CustomerCode && _.ProductName == scheduling.ProductName &&
							_.ProductSpec == scheduling.Spec);
				var thePos =
					pos.Where(
						_ => _.CustomerCode == bom.CustomerCode && _.ProductName == bom.ProductName && _.ProductSpec == bom.ProductSpec);
				foreach (var process in bom.BomProcesses)
				{
					foreach (var po in thePos)
					{
						list.Add(new Tuple<Scheduling, Bom, Po, string, double, IEnumerable<string>, /*bool,*/ int>(
						scheduling,
						bom,
						po,
						process.Name,
						(po.WorkQuantity + 0d) / po.Cavity / process.Capacity + process.Debug,
						process.Machine.Split('/'),
							//false,
						++key
						));
					}
				}
			}
			return list;
		}
		//机台、开始时间、结束时间、detail.key
		private IEnumerable<MachineInfo> Plan(IEnumerable<SchedulingInfo> detail, IEnumerable<string> machines, DateTime start, DateTime end)
		{
			var list = new List<MachineInfo>();
			//所有可排程的机台
			var mList = machines.Select(_ => new MachineScheduling { Name = _, Start = start }).ToArray();
			var days = 0;
			while (detail.Any(_ => _.IsPlan == false))
			{
				foreach (var m in mList)
				{
					if (m.Start >= end)
					{
						m.Start = start.AddDays(++days);
					}
					foreach (var d in detail.Where(_ => _.IsPlan == false))
					{
						if (!d.Machines.Contains(m.Name)) continue;
						if (m.Start >= d.Start)
						{
							list.Add(new MachineInfo
							{
								Name = m.Name,
								SchedulingInfo = d,
								Start = m.Start,
								End = m.Start.AddHours(d.WorkTimes)
							});
							m.Start = m.Start.AddHours(d.WorkTimes);
							d.IsPlan = true;
						}
					}
				}
				if (detail.All(_ => _.IsPlan)) continue;
				//如果所有机台的最后完成时间小于所有未排程工序中最先开始时间，该工序的开始时间调整为机台中最早完成时间
				var firstD = detail.Where(_ => _.IsPlan == false).OrderBy(_ => _.Start).First();//最前面的未排程工序
				if (mList.Where(_ => firstD.Machines.Contains(_.Name)).Max(_ => _.Start) < firstD.Start)
				{
					firstD.Start = mList.Where(_ => firstD.Machines.Contains(_.Name)).Min(_ => _.Start);
				}
			}
			return list;
		}
		//生成并保存生产排程
		private void SaveSmallScheduling(IEnumerable<MachineInfo> planInfo)
		{
			foreach (var info in planInfo)
			{
				var small = new SmallScheduling
				{
					Mold = info.SchedulingInfo.Bom.MoldCode,
					Date = info.Start,
					Machine = info.Name,
					CustomerCode = info.SchedulingInfo.Scheduling.CustomerCode,
					PoCode = info.SchedulingInfo.Po.Code,
					ProductCode = info.SchedulingInfo.Scheduling.ProductCode,
					ProductName = info.SchedulingInfo.Scheduling.ProductName,
					ProductSpec = info.SchedulingInfo.Scheduling.Spec,
					ProductType = info.SchedulingInfo.Bom.ProductType,
					Orders = info.SchedulingInfo.Po.OrderQuantity,
					WorkOrders = info.SchedulingInfo.Po.WorkQuantity,
					HasOrders = info.SchedulingInfo.Po.WorkQuantity,
					PlanOrders = info.SchedulingInfo.Po.WorkQuantity,
					Cavity = info.SchedulingInfo.Po.Cavity,
					ProcessName = info.SchedulingInfo.ProcessName,
					Capacity = info.SchedulingInfo.Process.Capacity,
					DebugTime = info.SchedulingInfo.Process.Debug,
					WorkTime = info.SchedulingInfo.WorkTimes,
					Start = info.Start,
					End = info.End,
				};
				UnitOfWork.GetRepository<IRepository<SmallScheduling>>().Add(small);
			}
			UnitOfWork.Commit();
		}
	}
	public class SchedulingInfo
	{
		public Scheduling Scheduling { get; set; }
		public Bom Bom { get; set; }
		public Po Po { get; set; }
		public BomProcess Process { get; set; }
		public string ProcessName { get; set; }
		public double WorkTimes { get; set; }
		public DateTime Start { get; set; }
		public DateTime End { get { return Start.AddHours(WorkTimes); } }
		public string[] Machines { get; set; }
		public SchedulingInfo Prev { get; set; }
		public bool IsPlan { get; set; }
	}

	public class MachineInfo
	{
		public string Name { get; set; }
		public SchedulingInfo SchedulingInfo { get; set; }
		public DateTime Start { get; set; }
		public DateTime End { get; set; }
	}

	public class MachineScheduling
	{
		public string Name { get; set; }
		public DateTime Start { get; set; }
	}
}
