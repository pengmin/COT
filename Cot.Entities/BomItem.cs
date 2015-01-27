﻿using Cot.Entities.Values;
using Cot.Infrastructure;

namespace Cot.Entities
{
	/// <summary>
	/// 物料清单项
	/// </summary>
	public class BomItem : Entity<int>
	{
		public int BomId { get; set; }
		/// <summary>
		/// 厚度
		/// </summary>
		public double Thickness { get; set; }
		/// <summary>
		/// 颜色
		/// </summary>
		public string Color { get; set; }
		/// <summary>
		/// 物料编码
		/// </summary>
		public string MaterialCode { get; set; }
		/// <summary>
		/// 物料名称
		/// </summary>
		public string MaterialName { get; set; }
		/// <summary>
		/// 物料宽度
		/// </summary>
		public double RawMaterialWidth { get; set; }
		/// <summary>
		/// 物料长度
		/// </summary>
		public double RawMaterialLength { get; set; }
		/// <summary>
		/// 宽度
		/// </summary>
		public double Width { get; set; }
		/// <summary>
		/// 裁切方式
		/// </summary>
		public CutType CutType { get; set; }
		/// <summary>
		/// 正公差
		/// </summary>
		public double PlusTolerance { get; set; }
		/// <summary>
		/// 负公差
		/// </summary>
		public double NegativeTolerance { get; set; }
		/// <summary>
		/// 倍数
		/// </summary>
		public double Multiple { get; set; }
		/// <summary>
		/// 跳距
		/// </summary>
		public double Skip { get; set; }
		/// <summary>
		/// 模穴数
		/// </summary>
		public int Cavity { get; set; }
	}
}
