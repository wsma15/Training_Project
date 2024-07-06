namespace TrainingApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Students", name: "Supervisors_SupervisorID", newName: "SupervisorID");
            RenameIndex(table: "dbo.Students", name: "IX_Supervisors_SupervisorID", newName: "IX_SupervisorID");
            DropColumn("dbo.Students", "StudentSupervisor");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Students", "StudentSupervisor", c => c.String(nullable: false));
            RenameIndex(table: "dbo.Students", name: "IX_SupervisorID", newName: "IX_Supervisors_SupervisorID");
            RenameColumn(table: "dbo.Students", name: "SupervisorID", newName: "Supervisors_SupervisorID");
        }
    }
}
