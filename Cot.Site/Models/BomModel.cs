using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cot.Entities;

namespace Cot.Site.Models
{
	public class BomModel : Bom
	{
		public IEnumerable<BomItem> Items { get; set; }
	}
}