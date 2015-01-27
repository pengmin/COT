namespace Cot.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResetPoModel2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PoItems", "PoId", "dbo.Poes");
            DropIndex("dbo.PoItems", new[] { "PoId" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.PoItems", "PoId");
            AddForeignKey("dbo.PoItems", "PoId", "dbo.Poes", "Id", cascadeDelete: true);
        }
    }
}
