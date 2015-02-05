namespace Cot.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SmallSchedulingModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SmallSchedulings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Mold = c.String(),
                        Date = c.DateTime(nullable: false),
                        Shifts = c.String(),
                        Machine = c.String(),
                        CustomerCode = c.String(),
                        PoCode = c.String(),
                        ProductCode = c.String(),
                        ProductName = c.String(),
                        ProductSpec = c.String(),
                        ProductType = c.String(),
                        Orders = c.Int(nullable: false),
                        WorkOrders = c.Int(nullable: false),
                        HasOrders = c.Int(nullable: false),
                        PlanOrders = c.Int(nullable: false),
                        Cavity = c.Int(nullable: false),
                        ProcessName = c.String(),
                        Capacity = c.Int(nullable: false),
                        DebugTime = c.Double(nullable: false),
                        WorkTime = c.Double(nullable: false),
                        Start = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SmallSchedulings");
        }
    }
}
