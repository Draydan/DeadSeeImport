namespace DeadSeaCatalogueDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PriceIsNowPriceFull : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "priceFull", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "priceFull");
        }
    }
}
