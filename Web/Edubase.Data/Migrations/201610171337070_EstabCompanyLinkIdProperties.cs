namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EstabCompanyLinkIdProperties : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Establishment2Company", "Company_GroupUID", "dbo.Company");
            DropForeignKey("dbo.Establishment2Company", "Establishment_Urn", "dbo.Establishment");
            DropIndex("dbo.Establishment2Company", new[] { "Company_GroupUID" });
            DropIndex("dbo.Establishment2Company", new[] { "Establishment_Urn" });
            RenameColumn(table: "dbo.Establishment2Company", name: "Company_GroupUID", newName: "CompanyGroupUID");
            RenameColumn(table: "dbo.Establishment2Company", name: "Establishment_Urn", newName: "EstablishmentUrn");
            AlterColumn("dbo.Establishment2Company", "CompanyGroupUID", c => c.Int(nullable: false));
            AlterColumn("dbo.Establishment2Company", "EstablishmentUrn", c => c.Int(nullable: false));
            CreateIndex("dbo.Establishment2Company", "CompanyGroupUID");
            CreateIndex("dbo.Establishment2Company", "EstablishmentUrn");
            AddForeignKey("dbo.Establishment2Company", "CompanyGroupUID", "dbo.Company", "GroupUID", cascadeDelete: true);
            AddForeignKey("dbo.Establishment2Company", "EstablishmentUrn", "dbo.Establishment", "Urn", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Establishment2Company", "EstablishmentUrn", "dbo.Establishment");
            DropForeignKey("dbo.Establishment2Company", "CompanyGroupUID", "dbo.Company");
            DropIndex("dbo.Establishment2Company", new[] { "EstablishmentUrn" });
            DropIndex("dbo.Establishment2Company", new[] { "CompanyGroupUID" });
            AlterColumn("dbo.Establishment2Company", "EstablishmentUrn", c => c.Int());
            AlterColumn("dbo.Establishment2Company", "CompanyGroupUID", c => c.Int());
            RenameColumn(table: "dbo.Establishment2Company", name: "EstablishmentUrn", newName: "Establishment_Urn");
            RenameColumn(table: "dbo.Establishment2Company", name: "CompanyGroupUID", newName: "Company_GroupUID");
            CreateIndex("dbo.Establishment2Company", "Establishment_Urn");
            CreateIndex("dbo.Establishment2Company", "Company_GroupUID");
            AddForeignKey("dbo.Establishment2Company", "Establishment_Urn", "dbo.Establishment", "Urn");
            AddForeignKey("dbo.Establishment2Company", "Company_GroupUID", "dbo.Company", "GroupUID");
        }
    }
}
