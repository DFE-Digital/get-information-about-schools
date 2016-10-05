namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EstabTypeForeignKey : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Establishment", "TypeId");
            RenameColumn(table: "dbo.Establishment", name: "EstablishmentType_Id", newName: "TypeId");
            RenameIndex(table: "dbo.Establishment", name: "IX_EstablishmentType_Id", newName: "IX_TypeId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Establishment", name: "IX_TypeId", newName: "IX_EstablishmentType_Id");
            RenameColumn(table: "dbo.Establishment", name: "TypeId", newName: "EstablishmentType_Id");
            AddColumn("dbo.Establishment", "TypeId", c => c.Int());
        }
    }
}
