namespace Cot.Entities.Values
{
	/// <summary>
	/// 裁切参数
	/// </summary>
	public struct CutParam
	{
		public double Width { get; set; }
		public double Multiple { get; set; }
		public double Skip { get; set; }
		public int Cavity { get; set; }
		public CutType Type { get; set; }
	}
}
