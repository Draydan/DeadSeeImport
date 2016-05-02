namespace DeadSeaCatalogueDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_SKU : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "artikul", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "artikul");
        }
    }
}
