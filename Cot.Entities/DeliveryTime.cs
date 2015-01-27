using System;

namespace Cot.Entities
{
	public class DeliveryTime
	{
		public Customer Customer { get; set; }
		public Product Product { get; set; }
		public DateTime DateTime { get; set; }
		public int Quantity { get; set; }
	}
}
