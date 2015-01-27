using System.Linq;
using Cot.Infrastructure;

namespace Cot.IRepositories
{
	/// <summary>
	/// 仓储基接口
	/// </summary>
	public interface IRepository : IDependency { }
	/// <summary>
	/// 仓储基接口
	/// </summary>
	/// <typeparam name="TEntity">实体类型</typeparam>
	public interface IRepository<TEntity> : IRepository where TEntity : Entity
	{
		/// <summary>
		/// 查询
		/// </summary>
		/// <returns></returns>
		IQueryable<TEntity> Query();
		/// <summary>
		/// 新增
		/// </summary>
		/// <param name="entity">实体</param>
		void Add(TEntity entity);
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="entity">实体</param>
		void Modify(TEntity entity);
		/// <summary>
		/// 删除
		/// </summary>
		/// <param name="entity">实体</param>
		void Remove(TEntity entity);
	}
}
