using Cot.Infrastructure;

namespace Cot.IRepositories
{
	/// <summary>
	/// 工作单元
	/// </summary>
	public interface IUnitOfWork : IDependency
	{
		/// <summary>
		/// 获取仓储对象
		/// </summary>
		/// <typeparam name="TRepository">仓储类型</typeparam>
		/// <returns></returns>
		TRepository GetRepository<TRepository>() where TRepository : IRepository;
		/// <summary>
		/// 提交
		/// </summary>
		void Commit();
		/// <summary>
		/// 回滚
		/// </summary>
		void Rollback();
		/// <summary>
		/// 清理
		/// </summary>
		void Clear();
	}
}
