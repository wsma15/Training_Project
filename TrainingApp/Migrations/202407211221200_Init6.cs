namespace TrainingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "UniversityID", c => c.Int(nullable: false));
            DropColumn("dbo.Users", "UniversityName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "UniversityName", c => c.String());
            DropColumn("dbo.Users", "UniversityID");
        }
    }
}
