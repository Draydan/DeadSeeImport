namespace DeadSeaCatalogueDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSupplierTitle : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Suppliers", "title", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Suppliers", "title");
        }
    }
}
