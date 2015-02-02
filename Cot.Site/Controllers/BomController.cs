using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Cot.Entities;
using Cot.Entities.Services;
using Cot.IRepositories;
using Cot.Site.Models;

namespace Cot.Site.Controllers
{
	public class BomController : BasiceController
	{
		public ActionResult Index()
		{
			var boms = UnitOfWork.GetRepository<IRepository<Bom>>().Query();
			return View(boms);
		}
		[HttpGet]
		public ActionResult Add()
		{
			return View("Edit", new BomModel());
		}
		[HttpPost]
		public ActionResult Add(Bom bom, IEnumerable<BomItem> items, IEnumerable<BomProcess> processes)
		{
			Save(bom, items, processes, true);
			return RedirectToAction("Edit", new { bom.Id });
		}

		[HttpGet]
		public ActionResult Edit(int id)
		{
			var entity = UnitOfWork.GetRepository<IRepository<Bom>>().Query().FirstOrDefault(_ => _.Id == id);
			entity.Items = UnitOfWork.GetRepository<IRepository<BomItem>>().Query()
				.Where(_ => _.BomId == entity.Id)
				.ToList();
			entity.BomProcesses = UnitOfWork.GetRepository<IRepository<BomProcess>>().Query()
				.Where(_ => _.BomId == entity.Id)
				.ToList();

			return View(entity);
		}
		[HttpPost]
		public ActionResult Edit(Bom bom, IEnumerable<BomItem> items, IEnumerable<BomProcess> processes)
		{
			Save(bom, items, processes, false);
			return View(bom);
		}

		public ActionResult Del(int id)
		{
			var itemIds = UnitOfWork.GetRepository<IRepository<BomItem>>().Query()
				.Where(_ => _.BomId == id)
				.Select(_ => _.Id).ToList();
			var proIds = UnitOfWork.GetRepository<IRepository<BomProcess>>().Query()
				.Where(_ => _.BomId == id)
				.Select(_ => _.Id);
			UnitOfWork.GetRepository<IRepository<Bom>>().Remove(new Bom { Id = id });
			foreach (var item in itemIds)
			{
				UnitOfWork.GetRepository<IRepository<BomItem>>().Remove(new BomItem() { Id = item });
			}
			foreach (var item in proIds)
			{
				UnitOfWork.GetRepository<IRepository<BomProcess>>().Remove(new BomProcess { Id = item });
			}
			UnitOfWork.Commit();
			return RedirectToAction("Index");
		}

		private void Save(Bom bom, IEnumerable<BomItem> items, IEnumerable<BomProcess> processes, bool isAdd)
		{
			if (isAdd)
			{
				UnitOfWork.GetRepository<IRepository<Bom>>().Add(bom);
				UnitOfWork.Commit();
				foreach (var item in items)
				{
					item.BomId = bom.Id;
				}
				foreach (var item in processes)
				{
					item.BomId = bom.Id;
				}
			}
			else
			{
				UnitOfWork.GetRepository<IRepository<Bom>>().Modify(bom);
			}

			ResetBomItems(bom, items);
			ResetBomItems(bom, processes);
			UnitOfWork.Commit();

			bom.Items = items;
			bom.BomProcesses = processes;
		}
		private void ResetBomItems(Bom bom, IEnumerable<BomItem> items)
		{
			var oldIds = UnitOfWork.GetRepository<IRepository<BomItem>>().Query()
				.Where(_ => _.BomId == bom.Id)
				.Select(_ => _.Id)
				.ToArray();
			var itemRep = UnitOfWork.GetRepository<IRepository<BomItem>>();

			foreach (var id in oldIds.Where(id => items.Select(_ => _.Id).All(_ => _ != id)))
			{
				itemRep.Remove(new BomItem { Id = id });
			}
			foreach (var item in items.Where(_ => _.Id == 0))
			{
				itemRep.Add(item);
			}
			foreach (var item in items.Where(item => oldIds.Any(_ => _ == item.Id)))
			{
				UnitOfWork.GetRepository<IRepository<BomItem>>().Modify(item);
			}
		}
		private void ResetBomItems(Bom bom, IEnumerable<BomProcess> processes)
		{
			var oldIds = UnitOfWork.GetRepository<IRepository<BomProcess>>().Query()
				.Where(_ => _.BomId == bom.Id)
				.Select(_ => _.Id)
				.ToArray();
			var itemRep = UnitOfWork.GetRepository<IRepository<BomProcess>>();

			foreach (var id in oldIds.Where(id => processes.Select(_ => _.Id).All(_ => _ != id)))
			{
				itemRep.Remove(new BomProcess { Id = id });
			}
			foreach (var item in processes.Where(_ => _.Id == 0))
			{
				itemRep.Add(item);
			}
			foreach (var item in processes.Where(item => oldIds.Any(_ => _ == item.Id)))
			{
				UnitOfWork.GetRepository<IRepository<BomProcess>>().Modify(item);
			}
		}

		[HttpPost]
		public ActionResult ToPo(string id)
		{
			//if (string.IsNullOrWhiteSpace(id)) return RedirectToAction("Index");

			//foreach (var item in id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
			//{
			//	var key = int.Parse(item);
			//	var bom = UnitOfWork.GetRepository<IRepository<Bom>>().Query().SingleOrDefault(_ => _.Id == key);
			//	if (bom == null) continue;

			//	var scheduling = UnitOfWork.GetRepository<IRepository<Scheduling>>().Query()
			//		.FirstOrDefault(
			//			_ => _.ProductName == bom.ProductName && _.Spec == bom.ProductSpec && _.CustomerCode == bom.CustomerCode);
			//	bom.Items = UnitOfWork.GetRepository<IRepository<BomItem>>().Query().Where(_ => _.BomId == bom.Id);
			//	var po = BomToPoScrvice.From(bom, scheduling);
			//	UnitOfWork.GetRepository<IRepository<Po>>().Add(po);
			//	UnitOfWork.Commit();
			//	foreach (var pi in po.Items)
			//	{
			//		pi.PoId = po.Id;
			//		UnitOfWork.GetRepository<IRepository<PoItem>>().Add(pi);
			//	}
			//	UnitOfWork.Commit();
			//}

			return RedirectToAction("Index");
		}

		public ActionResult Scheduling(int id)
		{
			var scheduling = UnitOfWork.GetRepository<IRepository<Scheduling>>().Query()
				.FirstOrDefault(_ => _.Id == id);
			var bom = UnitOfWork.GetRepository<IRepository<Bom>>().Query()
				.FirstOrDefault(_ =>
					_.CustomerCode == scheduling.CustomerCode &&
					_.ProductName == scheduling.ProductName &&
					_.ProductSpec == scheduling.Spec);
			if (bom == null)
			{
				bom = new Bom
				{
					CustomerCode = scheduling.CustomerCode,
					ProductName = scheduling.ProductName,
					ProductSpec = scheduling.Spec
				};
				ViewBag.Action = "/bom/add";
			}
			else
			{
				bom.Items = UnitOfWork.GetRepository<IRepository<BomItem>>().Query()
					.Where(_ => _.BomId == bom.Id);
				bom.BomProcesses = UnitOfWork.GetRepository<IRepository<BomProcess>>().Query()
					.Where(_ => _.BomId == bom.Id);
				ViewBag.Action = "/bom/edit";
			}
			return View("Edit", bom);
		}
	}
}
