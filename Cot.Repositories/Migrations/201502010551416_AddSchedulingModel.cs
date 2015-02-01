namespace Cot.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSchedulingModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Schedulings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomerName = c.String(),
                        CustomerCode = c.String(),
                        ProductName = c.String(),
                        ProductCode = c.String(),
                        Spec = c.String(),
                        Orders = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Deliveries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Orders = c.Int(nullable: false),
                        SchedulingId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Schedulings", t => t.SchedulingId, cascadeDelete: true)
                .Index(t => t.SchedulingId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Deliveries", "SchedulingId", "dbo.Schedulings");
            DropIndex("dbo.Deliveries", new[] { "SchedulingId" });
            DropTable("dbo.Deliveries");
            DropTable("dbo.Schedulings");
        }
    }
}
