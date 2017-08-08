using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration.Conventions;

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
