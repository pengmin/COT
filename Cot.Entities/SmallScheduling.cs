using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Cot.Infrastructure;

namespace Cot.Entities
{
	public class SmallScheduling : Entity<int>
	{
		[DisplayName("刀模编号")]
		public string Mold { get; set; }
		[DisplayName("排程日期")]
		public DateTime Date { get; set; }
		[DisplayName("生产班次")]
		public string Shifts { get; set; }
		[DisplayName("机台号")]
		public string Machine { get; set; }
		[DisplayName("客户")]
		public string CustomerCode { get; set; }
		[DisplayName("工单号")]
		public string PoCode { get; set; }
		[DisplayName("产品代码")]
		public string ProductCode { get; set; }
		[DisplayName("品名")]
		public string ProductName { get; set; }
		[DisplayName("规格")]
		public string ProductSpec { get; set; }
		[DisplayName("产品类别")]
		public string ProductType { get; set; }
		[DisplayName("订单量")]
		public int Orders { get; set; }
		[DisplayName("工单量")]
		public int WorkOrders { get; set; }
		[DisplayName("未完成量")]
		public int HasOrders { get; set; }
		[DisplayName("计划量")]
		public int PlanOrders { get; set; }
		[DisplayName("模穴")]
		public int Cavity { get; set; }
		[DisplayName("工序")]
		public string ProcessName { get; set; }
		[DisplayName("产能")]
		public int Capacity { get; set; }
		[DisplayName("调机首检")]
		public double DebugTime { get; set; }
		[DisplayName("工时")]
		public double WorkTime { get; set; }
		[DisplayName("预计开始时间")]
		public DateTime Start { get; set; }
		[DisplayName("预计完成时间")]
		public DateTime End { get; set; }
	}
}
