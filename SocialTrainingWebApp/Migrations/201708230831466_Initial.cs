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
                        DbId = c.Int(nullable: false, identity: true),
                        ImportId = c.Int(nullable: false),
                        FullName = c.String(),
                        Sex = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.DbId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Employees");
        }
    }
}
