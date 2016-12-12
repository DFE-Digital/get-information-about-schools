namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenamedCols : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.EstablishmentGroup", name: "TrustGroupUID", newName: "GroupUID");
            RenameIndex(table: "dbo.EstablishmentGroup", name: "IX_TrustGroupUID", newName: "IX_GroupUID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.EstablishmentGroup", name: "IX_GroupUID", newName: "IX_TrustGroupUID");
            RenameColumn(table: "dbo.EstablishmentGroup", name: "GroupUID", newName: "TrustGroupUID");
        }
    }
}
