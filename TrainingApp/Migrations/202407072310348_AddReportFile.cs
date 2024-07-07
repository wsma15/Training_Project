namespace TrainingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReportFile : DbMigration
    {
        public override void Up()
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
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ReportFiles");
        }
    }
}
