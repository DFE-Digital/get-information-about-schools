namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedLinkTables2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Estab2Estab",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LinkName = c.String(),
                        LinkType = c.String(),
                        LinkEstablishedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        Establishment_Urn = c.Int(),
                        LinkedEstablishment_Urn = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Establishment", t => t.Establishment_Urn)
                .ForeignKey("dbo.Establishment", t => t.LinkedEstablishment_Urn)
                .Index(t => t.Establishment_Urn)
                .Index(t => t.LinkedEstablishment_Urn);
            
            CreateTable(
                "dbo.Establishment2Company",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LinkedUID = c.Int(nullable: false),
                        JoinedDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        Establishment_Urn = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Establishment", t => t.Establishment_Urn)
                .Index(t => t.Establishment_Urn);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Establishment2Company", "Establishment_Urn", "dbo.Establishment");
            DropForeignKey("dbo.Estab2Estab", "LinkedEstablishment_Urn", "dbo.Establishment");
            DropForeignKey("dbo.Estab2Estab", "Establishment_Urn", "dbo.Establishment");
            DropIndex("dbo.Establishment2Company", new[] { "Establishment_Urn" });
            DropIndex("dbo.Estab2Estab", new[] { "LinkedEstablishment_Urn" });
            DropIndex("dbo.Estab2Estab", new[] { "Establishment_Urn" });
            DropTable("dbo.Establishment2Company");
            DropTable("dbo.Estab2Estab");
        }
    }
}
