namespace SocialTrainingWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPointColumnToGameDB : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "PointsSoFar", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Games", "PointsSoFar");
        }
    }
}
