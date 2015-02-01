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
	}
}
