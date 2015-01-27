using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cot.Entities;
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
	}
}
