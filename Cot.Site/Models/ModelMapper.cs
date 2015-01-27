using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Cot.Entities;

namespace Cot.Site.Models
{
	public class ModelMapper
	{
		static ModelMapper()
		{
			Mapper.CreateMap<BomModel, Bom>();
			Mapper.CreateMap<Bom, BomModel>();
		}
	}
}