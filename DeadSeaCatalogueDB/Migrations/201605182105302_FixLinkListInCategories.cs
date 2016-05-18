namespace DeadSeaCatalogueDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixLinkListInCategories : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Products", new[] { "Category_ID" });
            DropColumn("dbo.Products", "Category_ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "Category_ID", c => c.Long());
            CreateIndex("dbo.Products", "Category_ID");
        }
    }
}
