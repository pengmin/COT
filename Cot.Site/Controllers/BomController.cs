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
		public ActionResult Add(Bom bom)
		{
			UnitOfWork.GetRepository<IRepository<Bom>>().Add(bom);
			UnitOfWork.Commit();
			return Redirect("Add");
		}

		[HttpGet]
		public ActionResult Edit(int id)
		{
			var entity = UnitOfWork.GetRepository<IRepository<Bom>>().Query().FirstOrDefault(_ => _.Id == id);
			var model = Mapper.DynamicMap<BomModel>(entity);
			model.Items = UnitOfWork.GetRepository<IRepository<BomItem>>().Query().Where(_ => _.BomId == entity.Id);

			return View(model);
		}
		[HttpPost]
		public ActionResult Edit(BomModel bom, IEnumerable<BomItem> items)
		{
			var entity = Mapper.DynamicMap<Bom>(bom);
			UnitOfWork.GetRepository<IRepository<Bom>>().Modify(entity);

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

			UnitOfWork.Commit();
			bom.Items = items;
			return View(bom);
		}

		[HttpPost]
		public ActionResult ToPo(string id)
		{
			if (string.IsNullOrWhiteSpace(id)) return RedirectToAction("Index");

			foreach (var item in id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
			{
				var key = int.Parse(item);
				var bom = UnitOfWork.GetRepository<IRepository<Bom>>().Query().SingleOrDefault(_ => _.Id == key);
				if (bom == null) continue;

				bom.Items = UnitOfWork.GetRepository<IRepository<BomItem>>().Query().Where(_ => _.BomId == bom.Id);
				var po = BomToPoScrvice.From(bom);
				UnitOfWork.GetRepository<IRepository<Po>>().Add(po);
				UnitOfWork.Commit();
				foreach (var pi in po.Items)
				{
					pi.PoId = po.Id;
					UnitOfWork.GetRepository<IRepository<PoItem>>().Add(pi);
				}
				UnitOfWork.Commit();
			}

			return RedirectToAction("Index");
		}
	}
}
