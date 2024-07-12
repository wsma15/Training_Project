namespace TrainingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SenderId = c.Int(nullable: false),
                        ReceiverId = c.Int(nullable: false),
                        MessageText = c.String(nullable: false),
                        SenderName = c.String(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Reports",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReportTitle = c.String(nullable: false),
                        ReportDescription = c.String(nullable: false),
                        ReportDateCreated = c.DateTime(nullable: false),
                        OwnerId = c.String(nullable: false),
                        SupervisorID = c.Int(nullable: false),
                        FileName = c.String(nullable: false),
                        ContentType = c.String(),
                        Content = c.Binary(),
                        IsApproved = c.Boolean(nullable: false),
                        Feedback = c.String(),
                        IsFeedbackSubmitted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Email = c.String(nullable: false, maxLength: 50),
                        Password = c.String(nullable: false, maxLength: 50),
                        UniversitySupervisorID = c.Int(),
                        CompanySupervisorID = c.Int(),
                        UniversityName = c.String(),
                        CompanyName = c.String(),
                        Roles = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Users");
            DropTable("dbo.Reports");
            DropTable("dbo.Messages");
        }
    }
}
