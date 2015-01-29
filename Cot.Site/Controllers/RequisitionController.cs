using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cot.Entities;
using Cot.IRepositories;

namespace Cot.Site.Controllers
{
	public class RequisitionController : BasiceController
	{
		public ActionResult Index()
		{
			var entities = UnitOfWork.GetRepository<IRepository<Requisition>>().Query().ToList();
			return View(entities);
		}

		public ActionResult Edit(int id)
		{
			var entity = UnitOfWork.GetRepository<IRepository<Requisition>>().Query()
				.Include(_ => _.Items).First();
			return View(entity);
		}

		public ActionResult Del(params int[] id)
		{
			var rep = UnitOfWork.GetRepository<IRepository<Requisition>>();
			foreach (var item in id)
			{
				rep.Remove(new Requisition() { Id = item });
			}
			UnitOfWork.Commit();
			return RedirectToAction("Index");
		}
	}
}
