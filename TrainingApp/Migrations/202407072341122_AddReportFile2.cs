namespace TrainingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReportFile2 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Reports");
            AlterColumn("dbo.Reports", "ReportId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Reports", "ReportId");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Reports");
            AlterColumn("dbo.Reports", "ReportId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Reports", "ReportId");
        }
    }
}
