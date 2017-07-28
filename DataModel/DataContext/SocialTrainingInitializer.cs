using System.Configuration;
using System.Data.Entity;
using DataModel.DataModel;

namespace DataModel.DataContext
{
    public class SocialTrainingInitializer : DropCreateDatabaseIfModelChanges<SocialTrainingContext> // TODO for development only!
    {
        protected override void Seed(SocialTrainingContext context)
        {
            User admin = new User {Email = ConfigurationManager.AppSettings["AdminEmail"]};

            UserRole role = new UserRole
            {
                User = admin,
                Role = Role.Admin
            };

            context.Users.Add(admin);
            context.UserRoles.Add(role);
            context.SaveChanges();
        }
    }
}
