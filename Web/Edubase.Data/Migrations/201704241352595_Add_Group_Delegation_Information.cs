namespace Edubase.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Group_Delegation_Information : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.GroupCollection", "DelegationInformation", c => c.String(maxLength: 1000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.GroupCollection", "DelegationInformation");
        }
    }
}
