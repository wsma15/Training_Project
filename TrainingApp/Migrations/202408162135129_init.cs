namespace TrainingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "OtpCode", c => c.String());
            AddColumn("dbo.Users", "OtpExpiry", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "OtpExpiry");
            DropColumn("dbo.Users", "OtpCode");
        }
    }
}
