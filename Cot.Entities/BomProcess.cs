using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Cot.Infrastructure;

namespace Cot.Entities
{
	public class BomProcess : Entity<int>
	{
		[DisplayName("工艺流程")]
		public string Name { get; set; }
		[DisplayName("生产机台")]
		public string Machine { get; set; }
		[DisplayName("产能")]
		public int Capacity { get; set; }
		[DisplayName("调机时间(小时)")]
		public double Debug { get; set; }
		public int BomId { get; set; }
	}
}
