namespace DeadSeaCatalogueDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedFlagPriceFromSite : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "priceIsFromSiteNotExtrapolated", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "priceIsFromSiteNotExtrapolated");
        }
    }
}
