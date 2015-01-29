using System.ComponentModel;
using Cot.Entities.Values;
using Cot.Infrastructure;

namespace Cot.Entities
{
	/// <summary>
	/// 物料
	/// </summary>
	public class Material : RawMaterial
	{
		/// <summary>
		/// 数量
		/// </summary>
		[DisplayName("数量")]
		public int Count { get; set; }
	}
}
