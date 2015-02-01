namespace Cot.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeBomModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Boms", "MoldCode", c => c.String());
            AddColumn("dbo.Boms", "ProductSpec", c => c.String());
            AddColumn("dbo.Boms", "ProductType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Boms", "ProductType");
            DropColumn("dbo.Boms", "ProductSpec");
            DropColumn("dbo.Boms", "MoldCode");
        }
    }
}
