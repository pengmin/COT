namespace Cot.Repositories.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangePoModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Poes", "CustomerCode", c => c.String());
            AddColumn("dbo.Poes", "Code", c => c.String());
            AddColumn("dbo.Poes", "Date", c => c.DateTime(nullable: false));
            AddColumn("dbo.Poes", "ProductCode", c => c.DateTime(nullable: false));
            AddColumn("dbo.Poes", "Delivery", c => c.DateTime(nullable: false));
            AddColumn("dbo.Poes", "ProductSpec", c => c.String());
            AddColumn("dbo.Poes", "ProductName", c => c.String());
            AddColumn("dbo.Poes", "Mold", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Poes", "Mold");
            DropColumn("dbo.Poes", "ProductName");
            DropColumn("dbo.Poes", "ProductSpec");
            DropColumn("dbo.Poes", "Delivery");
            DropColumn("dbo.Poes", "ProductCode");
            DropColumn("dbo.Poes", "Date");
            DropColumn("dbo.Poes", "Code");
            DropColumn("dbo.Poes", "CustomerCode");
        }
    }
}
