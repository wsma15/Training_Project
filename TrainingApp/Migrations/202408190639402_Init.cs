namespace TrainingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "EmailConfirmationToken", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "EmailConfirmationToken");
        }
    }
}
