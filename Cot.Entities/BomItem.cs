using System.ComponentModel;
using Cot.Entities.Values;
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
		[DisplayName("厚度")]
		public double Thickness { get; set; }
		/// <summary>
		/// 颜色
		/// </summary>
		[DisplayName("颜色")]
		public string Color { get; set; }
		/// <summary>
		/// 物料编码
		/// </summary>
		[DisplayName("物料编码")]
		public string MaterialCode { get; set; }
		/// <summary>
		/// 物料名称
		/// </summary>
		[DisplayName("物料名称")]
		public string MaterialName { get; set; }
		/// <summary>
		/// 物料宽度
		/// </summary>
		[DisplayName("物料宽度")]
		public double RawMaterialWidth { get; set; }
		/// <summary>
		/// 物料长度
		/// </summary>
		[DisplayName("物料长度")]
		public double RawMaterialLength { get; set; }
		/// <summary>
		/// 宽度
		/// </summary>
		[DisplayName("宽度")]
		public double Width { get; set; }
		/// <summary>
		/// 裁切方式
		/// </summary>
		[DisplayName("裁切方式")]
		public CutType CutType { get; set; }
		/// <summary>
		/// 正公差
		/// </summary>
		[DisplayName("正公差")]
		public double PlusTolerance { get; set; }
		/// <summary>
		/// 负公差
		/// </summary>
		[DisplayName("负公差")]
		public double NegativeTolerance { get; set; }
		/// <summary>
		/// 倍数
		/// </summary>
		[DisplayName("倍数")]
		public double Multiple { get; set; }
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
	}
}
