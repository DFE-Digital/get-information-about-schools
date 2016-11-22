namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EstabTypeGroupLookup : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LookupEstablishmentTypeGroup",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        DisplayOrder = c.Short(),
                        Code = c.String(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Establishment", "EstablishmentTypeGroupId", c => c.Int());
            CreateIndex("dbo.Establishment", "EstablishmentTypeGroupId");
            AddForeignKey("dbo.Establishment", "EstablishmentTypeGroupId", "dbo.LookupEstablishmentTypeGroup", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Establishment", "EstablishmentTypeGroupId", "dbo.LookupEstablishmentTypeGroup");
            DropIndex("dbo.Establishment", new[] { "EstablishmentTypeGroupId" });
            DropColumn("dbo.Establishment", "EstablishmentTypeGroupId");
            DropTable("dbo.LookupEstablishmentTypeGroup");
        }
    }
}
