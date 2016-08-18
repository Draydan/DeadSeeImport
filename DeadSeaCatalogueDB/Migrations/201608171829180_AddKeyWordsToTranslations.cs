namespace DeadSeaCatalogueDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddKeyWordsToTranslations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Translations", "keyWords", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Translations", "keyWords");
        }
    }
}
