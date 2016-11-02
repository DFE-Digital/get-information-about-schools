namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Governors : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GovernorAppointingBody",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GovernorRole",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Governor",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EstablishmentUrn = c.Int(nullable: false),
                        Title = c.String(),
                        Forename1 = c.String(),
                        Forename2 = c.String(),
                        Surname = c.String(),
                        AppointmentStartDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        AppointmentEndDate = c.DateTime(precision: 7, storeType: "datetime2"),
                        RoleId = c.Int(),
                        GovernorAppointingBodyId = c.Int(),
                        CreatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        LastUpdatedUtc = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Establishment", t => t.EstablishmentUrn, cascadeDelete: true)
                .ForeignKey("dbo.GovernorAppointingBody", t => t.GovernorAppointingBodyId)
                .ForeignKey("dbo.GovernorRole", t => t.RoleId)
                .Index(t => t.EstablishmentUrn)
                .Index(t => t.RoleId)
                .Index(t => t.GovernorAppointingBodyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Governor", "RoleId", "dbo.GovernorRole");
            DropForeignKey("dbo.Governor", "GovernorAppointingBodyId", "dbo.GovernorAppointingBody");
            DropForeignKey("dbo.Governor", "EstablishmentUrn", "dbo.Establishment");
            DropIndex("dbo.Governor", new[] { "GovernorAppointingBodyId" });
            DropIndex("dbo.Governor", new[] { "RoleId" });
            DropIndex("dbo.Governor", new[] { "EstablishmentUrn" });
            DropTable("dbo.Governor");
            DropTable("dbo.GovernorRole");
            DropTable("dbo.GovernorAppointingBody");
        }
    }
}
