using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using Autofac;
using Cot.Infrastructure;
using Cot.IRepositories;

namespace Cot.Repositories
{
	public class UnitOfWork : IUnitOfWork
	{
		private CotDbContext _dbContext;
		private readonly string _connStr;
		private static readonly IContainer Ioc;
		private readonly IList<Action> _actions;

		internal CotDbContext DbContext
		{
			get
			{
				if (_dbContext != null) return _dbContext;

				_dbContext = new CotDbContext(_connStr);
				_dbContext.Configuration.AutoDetectChangesEnabled = false;
				_dbContext.Configuration.ProxyCreationEnabled = false;
				_dbContext.Configuration.ValidateOnSaveEnabled = false;
				return _dbContext;
			}
		}

		static UnitOfWork()
		{
			var builder = new ContainerBuilder();

			builder.RegisterAssemblyTypes(Assembly.GetCallingAssembly())
				.Where(_ => _.GetInterfaces().Contains(typeof(IDependency)))
				.AsImplementedInterfaces()
				.InstancePerLifetimeScope();
			builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>));

			Ioc = builder.Build();
		}
		public UnitOfWork(string connectionString = null)
		{
			_connStr = connectionString;
			_actions = new List<Action>();
		}

		public TRepository GetRepository<TRepository>() where TRepository : IRepository
		{
			return Ioc.Resolve<TRepository>(new TypedParameter(typeof(UnitOfWork), this));
		}

		public void Commit()
		{
			foreach (var item in DbContext.ChangeTracker.Entries())
			{
				item.State = EntityState.Detached;
			}
			foreach (var action in _actions)
			{
				action.Invoke();
			}
			DbContext.SaveChanges();
			Clear();
		}

		public void Rollback()
		{
			Clear();
		}

		public void Clear()
		{
			_actions.Clear();
		}

		internal void RegisterAction(Action action)
		{
			_actions.Add(action);
		}
	}
}
