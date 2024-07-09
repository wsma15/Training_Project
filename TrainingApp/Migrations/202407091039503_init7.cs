namespace TrainingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "CompanySupervisorID", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "CompanySupervisorID");
        }
    }
}
