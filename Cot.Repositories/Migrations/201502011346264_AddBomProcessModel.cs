namespace Cot.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBomProcessModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BomProcesses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Machine = c.String(),
                        Capacity = c.Int(nullable: false),
                        Debug = c.Double(nullable: false),
                        BomId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BomProcesses");
        }
    }
}
