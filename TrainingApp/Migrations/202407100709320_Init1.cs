namespace TrainingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "UniversitySupervisorID", c => c.Int());
            AlterColumn("dbo.Users", "CompanySupervisorID", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "CompanySupervisorID", c => c.Int(nullable: false));
            AlterColumn("dbo.Users", "UniversitySupervisorID", c => c.Int(nullable: false));
        }
    }
}
