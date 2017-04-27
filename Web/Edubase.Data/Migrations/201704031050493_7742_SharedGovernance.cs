namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _7742_SharedGovernance : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EstablishmentGovernor",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EstabishmentUrn = c.Int(nullable: false),
                        GovernorId = c.Int(nullable: false),
                        AppointmentStartDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        AppointmentEndDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Establishment", t => t.EstabishmentUrn, cascadeDelete: true)
                .ForeignKey("dbo.Governor", t => t.GovernorId, cascadeDelete: true)
                .Index(t => t.EstabishmentUrn)
                .Index(t => t.GovernorId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EstablishmentGovernor", "GovernorId", "dbo.Governor");
            DropForeignKey("dbo.EstablishmentGovernor", "EstabishmentUrn", "dbo.Establishment");
            DropIndex("dbo.EstablishmentGovernor", new[] { "GovernorId" });
            DropIndex("dbo.EstablishmentGovernor", new[] { "EstabishmentUrn" });
            DropTable("dbo.EstablishmentGovernor");
        }
    }
}
