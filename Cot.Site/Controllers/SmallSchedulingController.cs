using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cot.Entities;
using Cot.IRepositories;

namespace Cot.Site.Controllers
{
	public class SmallSchedulingController : BasiceController
	{
		public ActionResult Index()
		{
			var entities = UnitOfWork.GetRepository<IRepository<SmallScheduling>>().Query()
				.OrderBy(_ => _.Date).OrderBy(_ => _.Machine).OrderBy(_ => _.Start).ToArray();
			return View(entities);
		}

	}
}
