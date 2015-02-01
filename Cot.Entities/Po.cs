using System;
using System.Collections.Generic;
using System.ComponentModel;
using Cot.Infrastructure;

namespace Cot.Entities
{
	/// <summary>
	/// 生产工单
	/// </summary>
	public class Po : Entity<int>
	{
		[DisplayName("客户代码")]
		public string CustomerCode { get; set; }
		[DisplayName("工单号")]
		public string Code { get; set; }
		[DisplayName("日期")]
		public DateTime Date { get; set; }
		[DisplayName("产品代码")]
		public DateTime ProductCode { get; set; }
		[DisplayName("订单交期")]
		public DateTime Delivery { get; set; }
		[DisplayName("产品规格")]
		public string ProductSpec { get; set; }
		[DisplayName("名称型号")]
		public string ProductName { get; set; }
		[DisplayName("模具")]
		public string Mold { get; set; }
		/// <summary>
		/// 生产损耗率
		/// </summary>
		[DisplayName("生产损耗率")]
		public double ProductionLossRate { get; set; }
		/// <summary>
		/// 固定损耗数量
		/// </summary>
		[DisplayName("固定损耗数量")]
		public double FixedLossQuantity { get; set; }
		/// <summary>
		/// 跳距
		/// </summary>
		[DisplayName("跳距")]
		public double Skip { get; set; }
		/// <summary>
		/// 模穴数
		/// </summary>
		[DisplayName("模穴数")]
		public int Cavity { get; set; }
		/// <summary>
		/// 订单数量
		/// </summary>
		[DisplayName("订单数量")]
		public int OrderQuantity { get; set; }
		/// <summary>
		/// 工单数量
		/// </summary>
		[DisplayName("工单数量")]
		public int WorkQuantity
		{
			get { return (int)(OrderQuantity * (1 + ProductionLossRate) + FixedLossQuantity); }
		}
		/// <summary>
		/// 工单项
		/// </summary>
		public virtual ICollection<PoItem> Items { get; set; }

		public void Reset()
		{
			foreach (var item in Items)
			{
				item.NeedLength = Math.Round(WorkQuantity * Skip / Cavity * item.Multiple + item.DebugLength, 2);
				item.VolumeCount = Math.Round(item.NeedLength / item.Length, 2);
			}
		}
	}
}
