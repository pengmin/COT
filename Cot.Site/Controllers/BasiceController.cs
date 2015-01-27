using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Cot.Infrastructure;
using Cot.IRepositories;
using Cot.Repositories;

namespace Cot.Site.Controllers
{
	public class BasiceController : Controller
	{
		private static readonly IContainer Ioc;

		protected static IUnitOfWork UnitOfWork
		{
			get { return Ioc.Resolve<IUnitOfWork>(new TypedParameter(typeof(string), "Data Source=.;Initial Catalog=COT;Integrated Security=False;User ID=sa;Password=azsxdcfvgb;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False")); }
		}

		static BasiceController()
		{
			var builder = new ContainerBuilder();
			builder.RegisterAssemblyTypes(Assembly.GetAssembly(typeof(UnitOfWork)))
				.Where(_ => _.GetInterfaces().Contains(typeof(IDependency)))
				.AsImplementedInterfaces()
				.InstancePerLifetimeScope();
			Ioc = builder.Build();
		}
	}
}
