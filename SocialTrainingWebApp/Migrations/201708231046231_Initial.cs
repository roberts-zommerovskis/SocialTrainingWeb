namespace SocialTrainingWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        EmployeePK = c.Long(nullable: false, identity: true),
                        ImportId = c.Long(nullable: false),
                        FullName = c.String(),
                        Sex = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.EmployeePK);
            
            CreateTable(
                "dbo.Games",
                c => new
                    {
                        MoveId = c.Long(nullable: false, identity: true),
                        GameId = c.Long(nullable: false),
                        MissedGuesses = c.String(),
                        CompletedGuesses = c.String(),
                        EmployeePK = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.MoveId)
                .ForeignKey("dbo.Employees", t => t.EmployeePK, cascadeDelete: true)
                .Index(t => t.EmployeePK);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Games", "EmployeePK", "dbo.Employees");
            DropIndex("dbo.Games", new[] { "EmployeePK" });
            DropTable("dbo.Games");
            DropTable("dbo.Employees");
        }
    }
}
