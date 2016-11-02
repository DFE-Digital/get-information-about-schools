namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Permissions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EstablishmentPermission",
                c => new
                    {
                        PropertyName = c.String(nullable: false, maxLength: 128),
                        RoleName = c.String(nullable: false, maxLength: 128),
                        AllowUpdate = c.Boolean(nullable: false),
                        AllowApproval = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.PropertyName, t.RoleName });
            
        }
        
        public override void Down()
        {
            DropTable("dbo.EstablishmentPermission");
        }
    }
}
