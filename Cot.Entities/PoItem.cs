using System;
using Cot.Entities.Values;
using Cot.Infrastructure;

namespace Cot.Entities
{
	/// <summary>
	/// 生产工单项
	/// </summary>
	public class PoItem : Entity<int>
	{
		public int PoId { get; set; }
		public Po Po { get; set; }
		/// <summary>
		/// 物料名称
		/// </summary>
		public string MaterialName { get; set; }
		/// <summary>
		/// 物料宽度
		/// </summary>
		public double Width { get; set; }
		/// <summary>
		/// 物料长度
		/// </summary>
		public double Length { get; set; }
		/// <summary>
		/// 调机长度
		/// </summary>
		public double DebugLength { get; set; }
		/// <summary>
		/// 需求长度
		/// </summary>
		public double NeedLength { get; set; }
		/// <summary>
		/// 需求卷数
		/// </summary>
		public double VolumeCount { get; set; }
		/// <summary>
		/// 倍数
		/// </summary>
		public double Multiple { get; set; }
		/// <summary>
		/// 裁切方式
		/// </summary>
		public CutType CutType { get; set; }
	}
}
