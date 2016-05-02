namespace DeadSeaCatalogueDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Added_MultipleLinksFromProductToCategory : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Products", new[] { "category_ID" });
            CreateTable(
                "dbo.LinkProductWithCategories",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        category_ID = c.Long(),
                        product_ID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Categories", t => t.category_ID)
                .ForeignKey("dbo.Products", t => t.product_ID)
                .Index(t => t.category_ID)
                .Index(t => t.product_ID);
            
            CreateIndex("dbo.Products", "Category_ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LinkProductWithCategories", "product_ID", "dbo.Products");
            DropForeignKey("dbo.LinkProductWithCategories", "category_ID", "dbo.Categories");
            DropIndex("dbo.LinkProductWithCategories", new[] { "product_ID" });
            DropIndex("dbo.LinkProductWithCategories", new[] { "category_ID" });
            DropIndex("dbo.Products", new[] { "Category_ID" });
            DropTable("dbo.LinkProductWithCategories");
            CreateIndex("dbo.Products", "category_ID");
        }
    }
}
