namespace TrainingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init14 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "AddedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "AddedBy");
        }
    }
}
