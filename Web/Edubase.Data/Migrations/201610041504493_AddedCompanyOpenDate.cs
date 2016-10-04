namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedCompanyOpenDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Company", "OpenDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Company", "OpenDate");
        }
    }
}
