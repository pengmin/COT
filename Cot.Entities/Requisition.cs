using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Cot.Infrastructure;

namespace Cot.Entities
{
	/// <summary>
	/// 请购单
	/// </summary>
	public class Requisition : Entity<int>
	{
		[DisplayName("部门")]
		public string Department { get; set; }
		[DisplayName("编号")]
		public string Code { get; set; }
		[DisplayName("日期")]
		public DateTime Date { get; set; }
		public virtual ICollection<RequisitionItem> Items { get; set; }
	}
	/// <summary>
	/// 请购单项
	/// </summary>
	public class RequisitionItem : Entity<int>
	{
		[DisplayName("料号")]
		public string MaterialCode { get; set; }
		[DisplayName("品名/型号")]
		public string MaterialName { get; set; }
		[DisplayName("规格")]
		public string Spec { get; set; }
		[DisplayName("数量")]
		public double Count { get; set; }
		[DisplayName("交期")]
		public DateTime Delivery { get; set; }
		public int RequisitionId { get; set; }
	}
}
