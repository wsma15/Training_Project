namespace TrainingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reports", "ReportDateCreated", c => c.DateTime(nullable: false));
            DropColumn("dbo.Reports", "ReportDateTime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reports", "ReportDateTime", c => c.DateTime(nullable: false));
            DropColumn("dbo.Reports", "ReportDateCreated");
        }
    }
}
