namespace Cot.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRequisitionModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Requisitions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Department = c.String(),
                        Code = c.String(),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RequisitionItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MaterialCode = c.String(),
                        MaterialName = c.String(),
                        Spec = c.String(),
                        Count = c.Double(nullable: false),
                        Delivery = c.DateTime(nullable: false),
                        RequisitionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Requisitions", t => t.RequisitionId, cascadeDelete: true)
                .Index(t => t.RequisitionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RequisitionItems", "RequisitionId", "dbo.Requisitions");
            DropIndex("dbo.RequisitionItems", new[] { "RequisitionId" });
            DropTable("dbo.RequisitionItems");
            DropTable("dbo.Requisitions");
        }
    }
}
