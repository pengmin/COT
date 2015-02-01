using System.Data.Entity;
using Cot.Entities;

namespace Cot.Repositories
{
	internal class CotDbContext : DbContext
	{
		public DbSet<Bom> Boms { get; set; }
		public DbSet<BomItem> BomItems { get; set; }
		public DbSet<Po> Pos { get; set; }
		public DbSet<PoItem> PoItems { get; set; }
		public DbSet<Material> Materials { get; set; }
		public DbSet<Requisition> Requisitions { get; set; }
		public DbSet<RawMaterial> RawMaterials { get; set; }
		public DbSet<Scheduling> Schedulings { get; set; }
		public DbSet<BomProcess> BomProcesses { get; set; }

		public CotDbContext() : base("Data Source=.;Initial Catalog=COT;Integrated Security=False;User ID=sa;Password=azsxdcfvgb;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False") { }
		public CotDbContext(string connectionString) : base(connectionString) { }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<Bom>()
				.Ignore(_ => _.Items);
			modelBuilder.Entity<Po>()
				.Ignore(_ => _.WorkQuantity)
				.Ignore(_ => _.Items);
			modelBuilder.Entity<PoItem>()
				.Ignore(_ => _.Po);
		}
	}
}
