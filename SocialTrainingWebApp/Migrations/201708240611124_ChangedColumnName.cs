namespace SocialTrainingWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedColumnName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "UnguessedEmployees", c => c.String());
            AddColumn("dbo.Games", "GuessedEmployees", c => c.String());
            DropColumn("dbo.Games", "MissedGuesses");
            DropColumn("dbo.Games", "CompletedGuesses");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Games", "CompletedGuesses", c => c.String());
            AddColumn("dbo.Games", "MissedGuesses", c => c.String());
            DropColumn("dbo.Games", "GuessedEmployees");
            DropColumn("dbo.Games", "UnguessedEmployees");
        }
    }
}
