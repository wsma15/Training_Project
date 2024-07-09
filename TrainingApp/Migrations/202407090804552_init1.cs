namespace TrainingApp.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class init1 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Reports");
            AddColumn("dbo.Reports", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Reports", "Id");
            DropColumn("dbo.Reports", "Id");
        }

        public override void Down()
        {
            AddColumn("dbo.Reports", "Id", c => c.Int(nullable: false, identity: true));
            DropPrimaryKey("dbo.Reports");
            DropColumn("dbo.Reports", "Id");
            AddPrimaryKey("dbo.Reports", "Id");
        }
    }
}
