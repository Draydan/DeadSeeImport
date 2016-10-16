namespace DeadSeaCatalogueDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AntiKeyWords : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Translations", "antiKeyWords", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Translations", "antiKeyWords");
        }
    }
}
