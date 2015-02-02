namespace Cot.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangePoModel2 : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.Poes");
            DropTable("dbo.PoItems");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PoItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PoId = c.Int(nullable: false),
                        MaterialName = c.String(),
                        Width = c.Double(nullable: false),
                        Length = c.Double(nullable: false),
                        DebugLength = c.Double(nullable: false),
                        NeedLength = c.Double(nullable: false),
                        VolumeCount = c.Double(nullable: false),
                        Multiple = c.Double(nullable: false),
                        CutType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Poes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomerCode = c.String(),
                        Code = c.String(),
                        Date = c.DateTime(nullable: false),
                        ProductCode = c.DateTime(nullable: false),
                        Delivery = c.DateTime(nullable: false),
                        ProductSpec = c.String(),
                        ProductName = c.String(),
                        Mold = c.String(),
                        ProductionLossRate = c.Double(nullable: false),
                        FixedLossQuantity = c.Double(nullable: false),
                        Skip = c.Double(nullable: false),
                        Cavity = c.Int(nullable: false),
                        OrderQuantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
    }
}
