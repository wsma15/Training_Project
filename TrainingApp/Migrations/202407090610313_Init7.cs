namespace TrainingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "UniversitySupervisorID", c => c.String());
            DropColumn("dbo.Users", "SupervisorID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "SupervisorID", c => c.String());
            DropColumn("dbo.Users", "UniversitySupervisorID");
        }
    }
}
