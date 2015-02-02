using System;
using System.Collections.Generic;
using System.Linq;

namespace Cot.Entities.Services
{
	/// <summary>
	/// Bom表转生产工单
	/// </summary>
	public static class BomToPoScrvice
	{
		/// <summary>
		/// 转单
		/// </summary>
		/// <param name="bom">Bom</param>
		/// <returns>Po</returns>
		public static Po From(Bom bom, Scheduling scheduling, Delivery delivery)
		{
			var po = new Po
			{
				CustomerCode = bom.CustomerCode,
				Code = "C" + DateTime.Now.ToString("yyMMdd") + new Random().Next(9999).ToString().PadLeft(4, '0'),
				Date = DateTime.Now.Date,
				ProductCode = scheduling.ProductCode,
				ProductName = scheduling.ProductName,
				ProductSpec = scheduling.Spec,
				OrderQuantity = delivery.Orders,
				Mold = bom.MoldCode,
				ProductionLossRate = bom.ProductionLossRate,
				Delivery = delivery.Date,
				Skip = bom.Items.First().Skip,
				Cavity = bom.Items.First().Cavity
			};
			var items = new List<PoItem>();
			foreach (var pi in bom.Items.Select(BomItemToPoItem))
			{
				pi.Po = po;
				items.Add(pi);
			}
			po.Items = items;
			return po;
		}
		/// <summary>
		/// BomItem转PoItem
		/// </summary>
		/// <param name="item">BomItem</param>
		/// <returns>PoItem</returns>
		private static PoItem BomItemToPoItem(BomItem item)
		{
			return new PoItem
			{
				MaterialName = item.MaterialName,
				Width = item.Width,
				Length = item.RawMaterialLength,
				Multiple = item.Multiple,
				CutType = item.CutType
			};
		}
	}
}
