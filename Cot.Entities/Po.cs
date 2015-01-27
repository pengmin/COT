using System;
using System.Collections.Generic;
using Cot.Infrastructure;

namespace Cot.Entities
{
	/// <summary>
	/// 生产工单
	/// </summary>
	public class Po : Entity<int>
	{
		/// <summary>
		/// 生产损耗率
		/// </summary>
		public double ProductionLossRate { get; set; }
		/// <summary>
		/// 固定损耗数量
		/// </summary>
		public double FixedLossQuantity { get; set; }
		/// <summary>
		/// 跳距
		/// </summary>
		public double Skip { get; set; }
		/// <summary>
		/// 模穴数
		/// </summary>
		public int Cavity { get; set; }
		/// <summary>
		/// 订单数量
		/// </summary>
		public int OrderQuantity { get; set; }
		/// <summary>
		/// 工单数量
		/// </summary>
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
