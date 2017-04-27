namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedTeacherAppointmentDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Establishment", "HeadAppointmentDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Establishment", "HeadAppointmentDate");
        }
    }
}
