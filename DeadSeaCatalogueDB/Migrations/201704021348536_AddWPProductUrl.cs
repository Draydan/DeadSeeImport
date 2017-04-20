namespace DeadSeaCatalogueDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWPProductUrl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "wpUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "wpUrl");
        }
    }
}
