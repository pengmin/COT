namespace Cot.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class resetModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BomItems", "Bom_Id", "dbo.Boms");
            DropIndex("dbo.BomItems", new[] { "Bom_Id" });
            AddColumn("dbo.BomItems", "BomId", c => c.Int(nullable: false));
            AddColumn("dbo.PoItems", "NeedLength", c => c.Double(nullable: false));
            AddColumn("dbo.PoItems", "VolumeCount", c => c.Double(nullable: false));
            DropColumn("dbo.BomItems", "Bom_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BomItems", "Bom_Id", c => c.Int());
            DropColumn("dbo.PoItems", "VolumeCount");
            DropColumn("dbo.PoItems", "NeedLength");
            DropColumn("dbo.BomItems", "BomId");
            CreateIndex("dbo.BomItems", "Bom_Id");
            AddForeignKey("dbo.BomItems", "Bom_Id", "dbo.Boms", "Id");
        }
    }
}
