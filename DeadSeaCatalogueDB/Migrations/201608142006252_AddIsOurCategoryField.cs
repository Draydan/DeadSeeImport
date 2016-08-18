namespace DeadSeaCatalogueDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsOurCategoryField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Categories", "isOurCategory", c => c.Boolean(nullable: false));
            AddColumn("dbo.Translations", "isOurCategory", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Translations", "isOurCategory");
            DropColumn("dbo.Categories", "isOurCategory");
        }
    }
}
