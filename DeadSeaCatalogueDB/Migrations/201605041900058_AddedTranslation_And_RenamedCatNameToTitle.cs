namespace DeadSeaCatalogueDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTranslation_And_RenamedCatNameToTitle : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.Categories", "NameRus", "titleRus");
            RenameColumn("dbo.Categories", "Name", "title");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.Categories", "titleRus", "NameRus");
            RenameColumn("dbo.Categories", "title", "Name");
        }
    }
}
