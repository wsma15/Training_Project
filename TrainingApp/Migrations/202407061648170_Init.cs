namespace TrainingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Students");
            AlterColumn("dbo.Students", "StudentID", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.Students", "StudentID");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.Students");
            AlterColumn("dbo.Students", "StudentID", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.Students", "StudentID");
        }
    }
}
