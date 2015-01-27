namespace Cot.Repositories.Migrations
{
	using System.Data.Entity.Migrations;
    
    public partial class ModifyPoItemModel : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.PoItems", "NeedLength");
            DropColumn("dbo.PoItems", "VolumeCount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PoItems", "VolumeCount", c => c.Double(nullable: false));
            AddColumn("dbo.PoItems", "NeedLength", c => c.Double(nullable: false));
        }
    }
}
