namespace TrainingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init9 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "CompanyID", c => c.Int(nullable: false));
            DropColumn("dbo.Users", "CompanyName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "CompanyName", c => c.String());
            DropColumn("dbo.Users", "CompanyID");
        }
    }
}
