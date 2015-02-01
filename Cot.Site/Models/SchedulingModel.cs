using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cot.Entities;

namespace Cot.Site.Models
{
	public class SchedulingModel : Scheduling
	{
		public new IEnumerable<DeliveryModel> Deliveries { get; set; }
	}

	public class DeliveryModel : Delivery
	{
		public bool IsDel { get; set; }
	}
}