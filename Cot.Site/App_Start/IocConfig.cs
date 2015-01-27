using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;

namespace Cot.Site
{
	public class IocConfig
	{
		public static void Config()
		{
			var builder = new ContainerBuilder();
			builder.RegisterControllers(Assembly.GetCallingAssembly());

			DependencyResolver.SetResolver(new AutofacDependencyResolver(builder.Build()));
		}
	}
}