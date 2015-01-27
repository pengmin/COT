namespace Cot.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResetPoModel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PoItems", "Po_Id", "dbo.Poes");
            DropIndex("dbo.PoItems", new[] { "Po_Id" });
            RenameColumn(table: "dbo.PoItems", name: "Po_Id", newName: "PoId");
            AlterColumn("dbo.PoItems", "PoId", c => c.Int(nullable: false));
            CreateIndex("dbo.PoItems", "PoId");
            AddForeignKey("dbo.PoItems", "PoId", "dbo.Poes", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PoItems", "PoId", "dbo.Poes");
            DropIndex("dbo.PoItems", new[] { "PoId" });
            AlterColumn("dbo.PoItems", "PoId", c => c.Int());
            RenameColumn(table: "dbo.PoItems", name: "PoId", newName: "Po_Id");
            CreateIndex("dbo.PoItems", "Po_Id");
            AddForeignKey("dbo.PoItems", "Po_Id", "dbo.Poes", "Id");
        }
    }
}
