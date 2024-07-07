namespace TrainingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReportFile1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reports", "FileName", c => c.String());
            AddColumn("dbo.Reports", "ContentType", c => c.String());
            AddColumn("dbo.Reports", "Content", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reports", "Content");
            DropColumn("dbo.Reports", "ContentType");
            DropColumn("dbo.Reports", "FileName");
        }
    }
}
