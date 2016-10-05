namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlteredCompanyLinkTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Establishment2Company", "Company_GroupUID", c => c.Int());
            CreateIndex("dbo.Establishment2Company", "Company_GroupUID");
            AddForeignKey("dbo.Establishment2Company", "Company_GroupUID", "dbo.Company", "GroupUID");
            DropColumn("dbo.Establishment2Company", "LinkedUID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Establishment2Company", "LinkedUID", c => c.Int(nullable: false));
            DropForeignKey("dbo.Establishment2Company", "Company_GroupUID", "dbo.Company");
            DropIndex("dbo.Establishment2Company", new[] { "Company_GroupUID" });
            DropColumn("dbo.Establishment2Company", "Company_GroupUID");
        }
    }
}
