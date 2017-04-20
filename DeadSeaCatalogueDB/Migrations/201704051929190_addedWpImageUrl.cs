namespace DeadSeaCatalogueDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedWpImageUrl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "wpImageUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "wpImageUrl");
        }
    }
}
