namespace TrainingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Reports", "SupervisorID", c => c.Int(nullable: false));
            AlterColumn("dbo.Users", "UniversitySupervisorID", c => c.Int(nullable: false));
            AlterColumn("dbo.Users", "CompanySupervisorID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "CompanySupervisorID", c => c.String());
            AlterColumn("dbo.Users", "UniversitySupervisorID", c => c.String());
            AlterColumn("dbo.Reports", "SupervisorID", c => c.String(nullable: false));
        }
    }
}
