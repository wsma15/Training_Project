namespace TrainingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init1 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Reports");
            AlterColumn("dbo.Reports", "ReportId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.Reports", "ReportId");
            DropColumn("dbo.Reports", "FileName");
            DropColumn("dbo.Reports", "ContentType");
            DropColumn("dbo.Reports", "Content");
            DropTable("dbo.ReportFiles");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ReportFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileName = c.String(nullable: false),
                        ContentType = c.String(nullable: false),
                        Content = c.Binary(nullable: false),
                        OwnerId = c.String(),
                        SupervisorID = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Reports", "Content", c => c.Binary());
            AddColumn("dbo.Reports", "ContentType", c => c.String());
            AddColumn("dbo.Reports", "FileName", c => c.String());
            DropPrimaryKey("dbo.Reports");
            AlterColumn("dbo.Reports", "ReportId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Reports", "ReportId");
        }
    }
}
