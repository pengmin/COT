using System.Data.Entity;
using System.Linq;
using Cot.Infrastructure;
using Cot.IRepositories;

namespace Cot.Repositories
{
	internal class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
	{
		protected readonly UnitOfWork UnitOfWork;

		public Repository(UnitOfWork unitOfWork)
		{
			UnitOfWork = unitOfWork;
		}

		public IQueryable<TEntity> Query()
		{
			return UnitOfWork.DbContext.Set<TEntity>().AsNoTracking();
		}
		public void Add(TEntity entity)
		{
			UnitOfWork.RegisterAction(() => PersistCreationOf(entity));
		}
		public void Modify(TEntity entity)
		{
			UnitOfWork.RegisterAction(() => PersistUpdateOf(entity));
		}
		public void Remove(TEntity entity)
		{
			UnitOfWork.RegisterAction(() => PersistDeletionOf(entity));
		}

		protected virtual void PersistCreationOf(TEntity entity)
		{
			UnitOfWork.DbContext.Entry(entity).State = EntityState.Added;
		}
		protected virtual void PersistUpdateOf(TEntity entity)
		{
			UnitOfWork.DbContext.Entry(entity).State = EntityState.Modified;
		}
		protected virtual void PersistDeletionOf(TEntity entity)
		{
			UnitOfWork.DbContext.Entry(entity).State = EntityState.Deleted;
		}
	}
}
