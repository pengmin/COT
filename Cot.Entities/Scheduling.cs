using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Cot.Infrastructure;

namespace Cot.Entities
{
	public class Scheduling : Entity<int>
	{
		[DisplayName("客户名称")]
		public string CustomerName { get; set; }
		[DisplayName("客户代码")]
		public string CustomerCode { get; set; }
		[DisplayName("产品名称")]
		public string ProductName { get; set; }
		[DisplayName("产品代码")]
		public string ProductCode { get; set; }
		[DisplayName("规格")]
		public string Spec { get; set; }
		[DisplayName("订单量")]
		public int Orders { get; set; }
		/// <summary>
		/// 交期开始日期
		/// </summary>
		public DateTime StartDate { get; set; }
		/// <summary>
		/// 交期结束日期
		/// </summary>
		public DateTime EndDate { get; set; }
		public virtual ICollection<Delivery> Deliveries { get; set; }
	}

	public class Delivery : Entity<int>
	{
		[DisplayName("交期")]
		public DateTime Date { get; set; }
		[DisplayName("订单量")]
		public int Orders { get; set; }
		public int SchedulingId { get; set; }
	}
}
