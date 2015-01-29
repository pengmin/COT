using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Cot.Entities.Values;
using Cot.Infrastructure;

namespace Cot.Entities
{
	public class RawMaterial : Entity<int>
	{
		/// <summary>
		/// 物料代码
		/// </summary>
		[DisplayName("物料代码")]
		public string Code { get; set; }
		/// <summary>
		/// 物料名称
		/// </summary>
		[DisplayName("物料名称")]
		public string Name { get; set; }
		/// <summary>
		/// 物料宽度(mm)
		/// </summary>
		[DisplayName("物料宽度(mm)")]
		public double Width { get; set; }
		/// <summary>
		/// 物料长度(mm)
		/// </summary>
		[DisplayName("物料长度(mm)")]
		public double Length { get; set; }
		/// <summary>
		/// 物料单位
		/// </summary>
		[DisplayName("物料单位")]
		public string Unit { get; set; }

		/// <summary>
		/// 可裁切数量
		/// </summary>
		/// <param name="param">裁切参数</param>
		/// <returns>裁切数量</returns>
		public int CanCutQuantity(CutParam param)
		{
			if (param.Type == CutType.Horizontal)
			{
				return (int)((Width / param.Width) * (Length / param.Skip));//一条有多少个，共有多少条
			}
			else
			{
				return (int)((Length / param.Width) * (Width / param.Skip));//一卷有多少个，共有多少卷
			}
		}

		/// <summary>
		/// 裁切
		/// </summary>
		/// <param name="param">裁切参数</param>
		/// <param name="quantity">需要裁切数量</param>
		/// <param name="realQuantity">裁切到的数量</param>
		/// <returns>剩余物料</returns>
		/// <remarks>裁切不会修改当前对象，若有余料，则返回新对象，否则返回null</remarks>
		public Material Cut(CutParam param, int quantity, out int realQuantity)
		{
			var q = CanCutQuantity(param);
			if (q <= quantity)//无余料
			{
				realQuantity = q;
				return null;
			}
			realQuantity = quantity;
			if (param.Type == CutType.Horizontal)
			{
				var newLen = Length - (quantity / (Width / param.Width) + (quantity % (Width / param.Width) > 1 ? 1 : 0) * param.Skip);
				return new Material { Width = Width, Length = newLen };
			}
			else
			{
				var newWidth = Width - ((quantity / (Length / param.Width) + (quantity % (Length / param.Width) > 1 ? 1 : 0)) * param.Skip);
				return new Material { Width = newWidth, Length = Length };
			}
		}
		/// <summary>
		/// 按指定裁切参数及数量，需要多少物料
		/// </summary>
		/// <param name="param">裁切参数</param>
		/// <param name="quantity">数量</param>
		/// <returns>需求量</returns>
		public double Need(CutParam param, int quantity)
		{
			return quantity / CanCutQuantity(param);
		}
	}
}
