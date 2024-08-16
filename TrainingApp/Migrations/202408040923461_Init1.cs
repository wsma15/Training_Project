namespace TrainingApp.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Init1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reports", "ReportStatus", c => c.Int(nullable: false));
            DropColumn("dbo.Reports", "IsApproved");
        }

        public override void Down()
        {
            AddColumn("dbo.Reports", "IsApproved", c => c.Boolean(nullable: false));
            DropColumn("dbo.Reports", "ReportStatus");
        }
    }
}
