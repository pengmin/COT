namespace Cot.Entities
{
	public class Stock
	{
		public Material Material { get; set; }
		public Warehouse Warehouse { get; set; }
		public int Quantity { get; set; }
	}
}
