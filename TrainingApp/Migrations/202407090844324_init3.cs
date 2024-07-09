namespace TrainingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "UniversityName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "UniversityName");
        }
    }
}
