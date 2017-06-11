namespace DeadSeaCatalogueDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSuppliers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Suppliers",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.Products", "supplier_ID", c => c.Long());
            CreateIndex("dbo.Products", "supplier_ID");
            AddForeignKey("dbo.Products", "supplier_ID", "dbo.Suppliers", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "supplier_ID", "dbo.Suppliers");
            DropIndex("dbo.Products", new[] { "supplier_ID" });
            DropColumn("dbo.Products", "supplier_ID");
            DropTable("dbo.Suppliers");
        }
    }
}
