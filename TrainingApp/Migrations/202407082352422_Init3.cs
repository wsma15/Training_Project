namespace TrainingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Students", "SupervisorID", "dbo.Supervisors");
            DropIndex("dbo.Students", new[] { "SupervisorID" });
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Email = c.String(nullable: false, maxLength: 50),
                        Password = c.String(nullable: false, maxLength: 50),
                        SupervisorID = c.String(),
                        Roles = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.Admins");
            DropTable("dbo.Students");
            DropTable("dbo.Supervisors");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Supervisors",
                c => new
                    {
                        SupervisorID = c.String(nullable: false, maxLength: 128),
                        SupervisorName = c.String(nullable: false),
                        SupervisorPassword = c.String(nullable: false),
                        SupervisorEmail = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.SupervisorID);
            
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        StudentID = c.String(nullable: false, maxLength: 128),
                        StudentPassword = c.String(nullable: false),
                        StudentName = c.String(nullable: false),
                        StudentEmail = c.String(nullable: false),
                        SupervisorID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.StudentID);
            
            CreateTable(
                "dbo.Admins",
                c => new
                    {
                        AdminId = c.String(nullable: false, maxLength: 128),
                        AdminName = c.String(nullable: false),
                        AdminPassword = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.AdminId);
            
            DropTable("dbo.Users");
            CreateIndex("dbo.Students", "SupervisorID");
            AddForeignKey("dbo.Students", "SupervisorID", "dbo.Supervisors", "SupervisorID");
        }
    }
}
