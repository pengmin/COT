using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cot.Entities;
using Cot.IRepositories;

namespace Cot.Site.Controllers
{
	public class MaterialController : BasiceController
	{
		public ActionResult Index()
		{
			var entities = UnitOfWork.GetRepository<IRepository<Material>>().Query().ToArray();
			return View(entities);
		}

		public ActionResult Add()
		{
			return View("Edit", new Material());
		}
		[HttpPost]
		public ActionResult Add(Material entity)
		{
			UnitOfWork.GetRepository<IRepository<Material>>().Add(entity);
			UnitOfWork.Commit();
			return RedirectToAction("Index");
		}

		public ActionResult Edit(int id)
		{
			var entity = UnitOfWork.GetRepository<IRepository<Material>>().Query().FirstOrDefault(_ => _.Id == id);
			return View(entity);
		}
		[HttpPost]
		public ActionResult Edit(Material entity)
		{
			UnitOfWork.GetRepository<IRepository<Material>>().Modify(entity);
			UnitOfWork.Commit();
			return View(entity);
		}

		public ActionResult Del(params int[] ids)
		{
			foreach (var id in ids)
			{
				UnitOfWork.GetRepository<IRepository<Material>>().Remove(new Material { Id = id });
			}
			UnitOfWork.Commit();
			return RedirectToAction("Index");
		}
	}
}
