namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedEstabInspectorate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Establishment", "InspectorateId", c => c.Int());
            CreateIndex("dbo.Establishment", "InspectorateId");
            AddForeignKey("dbo.Establishment", "InspectorateId", "dbo.LookupInspectorate", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Establishment", "InspectorateId", "dbo.LookupInspectorate");
            DropIndex("dbo.Establishment", new[] { "InspectorateId" });
            DropColumn("dbo.Establishment", "InspectorateId");
        }
    }
}
