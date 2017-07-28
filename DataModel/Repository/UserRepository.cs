using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataModel.DataContext;
using DataModel.DataModel;

namespace DataModel.Repository
{
    public class UserRepository
    {
        private SocialTrainingContext db;

        public UserRepository(SocialTrainingContext context)
        {
            db = context;
        }

        public List<User> GetUsers()
        {
            return db.Users.ToList();
        }

        public async Task<User> SetAura(User updatedUser)
        {
            var user = await db.Users.FindAsync(updatedUser.UserId);
            if (user != null)
            {
                user.UserKarma.Aura = updatedUser.UserKarma.Aura;
                await db.SaveChangesAsync();
            }
            return user;
        }

        public List<UserLanguage> GetUsersLanguage()
        {
            return db.UserLanguages.ToList();
        }

        public List<UserKarma> GetUsersKarma()
        {
            return db.UserKarma.ToList();
        }

        public List<UserKarmaHistory> GetUsersKarmaHistory()
        {
            return db.UserKarmaHistory.ToList();
        }

        public void UpdateUserKarma(UserKarma newUserKarma)
        {
            db.UserKarma.Add(newUserKarma);
            db.SaveChanges();
        }

        public void AddKarmaHisoryRecord(UserKarmaHistory historyRecord)
        {
            db.UserKarmaHistory.Add(historyRecord);
            db.SaveChanges();
        }


    }
}
