namespace TrainingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reports", "IsApproved", c => c.Boolean(nullable: false));
            AddColumn("dbo.Reports", "Feedback", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reports", "Feedback");
            DropColumn("dbo.Reports", "IsApproved");
        }
    }
}
