namespace TrainingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init8 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "CompanyName", c => c.String(nullable: false));
            DropColumn("dbo.Companies", "UniversityName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Companies", "UniversityName", c => c.String(nullable: false));
            DropColumn("dbo.Companies", "CompanyName");
        }
    }
}
