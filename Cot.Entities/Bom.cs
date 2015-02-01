using System.Collections.Generic;
using System.ComponentModel;
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
		[DisplayName("客户代码")]
		public string CustomerCode { get; set; }
		/// <summary>
		/// 品名
		/// </summary>
		[DisplayName("品名")]
		public string ProductName { get; set; }
		[DisplayName("模具编号")]
		public string MoldCode { get; set; }
		/// <summary>
		/// 生产损耗率
		/// </summary>
		[DisplayName("生产损耗率")]
		public double ProductionLossRate { get; set; }
		[DisplayName("产品规格")]
		public string ProductSpec { get; set; }
		[DisplayName("产品类别")]
		public string ProductType { get; set; }

		public IEnumerable<BomItem> Items { get; set; }
		public IEnumerable<BomProcess> BomProcesses { get; set; }
	}
}
