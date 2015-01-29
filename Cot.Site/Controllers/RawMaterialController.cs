using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cot.Entities;
using Cot.IRepositories;

namespace Cot.Site.Controllers
{
	public class RawMaterialController : BasiceController
	{
		public ActionResult Index()
		{
			var entities = UnitOfWork.GetRepository<IRepository<RawMaterial>>().Query().ToList();
			return View(entities);
		}

		public ActionResult Add()
		{
			return View("Edit", new Material());
		}
		[HttpPost]
		public ActionResult Add(RawMaterial model)
		{
			UnitOfWork.GetRepository<IRepository<RawMaterial>>().Add(model);
			UnitOfWork.Commit();
			return RedirectToAction("Edit", model.Id);
		}

		public ActionResult Edit(int id)
		{
			var entity = UnitOfWork.GetRepository<IRepository<RawMaterial>>().Query().First(_ => _.Id == id);
			return View(entity);
		}
		[HttpPost]
		public ActionResult Edit(RawMaterial model)
		{
			UnitOfWork.GetRepository<IRepository<RawMaterial>>().Modify(model);
			UnitOfWork.Commit();
			return View(model);
		}

		public ActionResult Del(params int[] id)
		{
			var rep = UnitOfWork.GetRepository<IRepository<RawMaterial>>();
			foreach (var item in id)
			{
				rep.Remove(new RawMaterial() { Id = item });
			}
			UnitOfWork.Commit();
			return RedirectToAction("Index");
		}
	}
}
