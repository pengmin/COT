namespace Cot.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRawMaterialModel : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Materials", newName: "RawMaterials");
            AddColumn("dbo.RawMaterials", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.RawMaterials", "Count", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RawMaterials", "Count", c => c.Int(nullable: false));
            DropColumn("dbo.RawMaterials", "Discriminator");
            RenameTable(name: "dbo.RawMaterials", newName: "Materials");
        }
    }
}
