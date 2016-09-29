namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedOldTables : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.SchoolMAT", "LinkedUID", "dbo.MAT");
            DropIndex("dbo.SchoolMAT", new[] { "LinkedUID" });
            DropTable("dbo.MAT");
            DropTable("dbo.SchoolMAT");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SchoolMAT",
                c => new
                    {
                        URN = c.Int(nullable: false, identity: true),
                        LinkedUID = c.Short(nullable: false),
                        JoinedDate = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.URN);
            
            CreateTable(
                "dbo.MAT",
                c => new
                    {
                        GroupUID = c.Short(nullable: false, identity: true),
                        GroupID = c.String(),
                        GroupName = c.String(),
                        CompaniesHouseNumber = c.String(),
                        GroupTypeCode = c.String(),
                        GroupType = c.String(),
                        ClosedDate = c.String(),
                        GroupStatusCode = c.String(),
                        GroupStatus = c.String(),
                    })
                .PrimaryKey(t => t.GroupUID);
            
            CreateIndex("dbo.SchoolMAT", "LinkedUID");
            AddForeignKey("dbo.SchoolMAT", "LinkedUID", "dbo.MAT", "GroupUID", cascadeDelete: true);
        }
    }
}
