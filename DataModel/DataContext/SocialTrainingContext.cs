using System.Data.Entity;
using DataModel.DataModel;

namespace DataModel.DataContext
{
    public class SocialTrainingContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<GoogleDoc> GoogleDocs { get; set; }

        public DbSet<Organization> Organizations { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<Skill> Skills { get; set; }

        public DbSet<Team> Teams { get; set; }

        public DbSet<Technology> Technologies { get; set; }

        public DbSet<UserKarma> UserKarma { get; set; }

        public DbSet<UserKarmaHistory> UserKarmaHistory { get; set; }

        public DbSet<Language> Languages { get; set; }

        public DbSet<UserLanguage> UserLanguages { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }


        public SocialTrainingContext() : base("name=DefaultConnection")
        {
            Database.SetInitializer(new SocialTrainingInitializer());
        }
        
    }
}
