using System.Collections.Generic;
using Cot.Infrastructure;

namespace Cot.Entities
{
	/// <summary>
	/// 物料清单
	/// </summary>
	public class Bom : Entity<int>
	{
		/// <summary>
		/// 客户代码
		/// </summary>
		public string CustomerCode { get; set; }
		/// <summary>
		/// 品名
		/// </summary>
		public string ProductName { get; set; }
		/// <summary>
		/// 生产损耗率
		/// </summary>
		public double ProductionLossRate { get; set; }

		public IEnumerable<BomItem> Items { get; set; }
	}
}
