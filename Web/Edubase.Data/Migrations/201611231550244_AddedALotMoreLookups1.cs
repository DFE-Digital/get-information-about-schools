namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedALotMoreLookups1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Establishment", "FurtherEducationTypeId", c => c.Int());
            CreateIndex("dbo.Establishment", "FurtherEducationTypeId");
            AddForeignKey("dbo.Establishment", "FurtherEducationTypeId", "dbo.LookupFurtherEducationType", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Establishment", "FurtherEducationTypeId", "dbo.LookupFurtherEducationType");
            DropIndex("dbo.Establishment", new[] { "FurtherEducationTypeId" });
            DropColumn("dbo.Establishment", "FurtherEducationTypeId");
        }
    }
}
