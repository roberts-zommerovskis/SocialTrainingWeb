namespace SocialTrainingWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedGenderColumnName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "Gender", c => c.String());
            DropColumn("dbo.Employees", "Sex");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Employees", "Sex", c => c.String());
            DropColumn("dbo.Employees", "Gender");
        }
    }
}
