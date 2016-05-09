namespace DeadSeaCatalogueDB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_ingridients_to_Translation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Translations",
                c => new
                    {
                        ID = c.Long(nullable: false, identity: true),
                        titleEng = c.String(),
                        title = c.String(),
                        desc = c.String(),
                        details = c.String(),
                        ingridients = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Translations");
        }
    }
}
