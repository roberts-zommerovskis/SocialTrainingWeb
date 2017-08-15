using System.Data.Entity;

namespace SocialTrainingWebApp.Models
{
    public class AppDbContext : DbContext
    {

        public AppDbContext() : base("EmployeeDB")
        {
            //Database.SetInitializer(new DropCreateDatabaseAlways<AppDbContext>());

            Database.SetInitializer(new CreateDatabaseIfNotExists<AppDbContext>());

        }

        public DbSet<Employee> Employee { get; set; }
    }
}
