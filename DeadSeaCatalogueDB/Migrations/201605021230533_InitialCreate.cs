namespace DeadSeaCatalogueDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                        NameRus = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        title = c.String(),
                        titleRus = c.String(),
                        price = c.String(),
                        desc = c.String(),
                        details = c.String(),
                        descRus = c.String(),
                        detailsRus = c.String(),
                        imageFileName = c.String(),
                        translated = c.Int(nullable: false),
                        category_ID = c.Long(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Categories", t => t.category_ID)
                .Index(t => t.category_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "category_ID", "dbo.Categories");
            DropIndex("dbo.Products", new[] { "category_ID" });
            DropTable("dbo.Products");
            DropTable("dbo.Categories");
        }
    }
}
