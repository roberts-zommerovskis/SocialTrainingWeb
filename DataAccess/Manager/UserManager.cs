using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DataAccess.Dto;
using DataModel.DataModel;
using DataModel.Repository;

namespace DataAccess.Manager
{
    public class UserManager
    {
        private const int GIVEKARMALIMITDEFAULT = 5;

        private readonly UserRepository _userRepository;

        public UserManager(UserRepository repository)
        {
            _userRepository = repository;
        }

        /// <summary>
        /// Gives a List of all Users, except Unemployed ones.
        /// </summary>
        /// <returns>Returns list of SimpleUserDto with Users ID, FullName and Current karma points (if karma points doesn't exist in db column, karma point will be set to 0)</returns>
        public List<SimpleUserDto> ListUsers(int from = 0, int count = 0)
        {
            var users = _userRepository.GetUsers()
                .Where(x => x.Status == UserStatus.Employed)
                .OrderByDescending(y => y.UserKarma?.Current ?? 0)
                .Skip(from).ToList();

            if (count > 0)
            {
                users = users.Take(count).ToList();
            }

            return users.Select(Mapper.Map<User, SimpleUserDto>).ToList();
        }



        public List<FullUserDto> ListEmployees()
        {
            var users = _userRepository.GetUsers();
            return users.Select(Mapper.Map<User, FullUserDto>).ToList();
        }
        
        public FullUserDto GetUser(long userId)
        {
            var user = _userRepository.GetUsers().FirstOrDefault(u => u.UserId == userId && u.Status == UserStatus.Employed);
            return user != null ? Mapper.Map<User, FullUserDto>(user) : null;
        }

        public FullUserDto SetAura(FullUserDto fullUserDto)
        {
            var user = Mapper.Map<FullUserDto, User>(fullUserDto);
            var updatedUser = _userRepository.SetAura(user);
            return Mapper.Map<User, FullUserDto>(updatedUser.Result);
        }

        public void GiveKarma(string karmaReporterMail, long karmaAssigneeId, int karma)
        {
            long karmaReporterId = _userRepository.GetUsers().FirstOrDefault(x => x.Email == karmaReporterMail).UserId;
            if (!IsGiveLimiteExceeded(karmaReporterId, karma))
            {
                UpdateUserKarma(karmaAssigneeId, karma);
                AddKarmaHisoryRecord(karmaReporterId, karmaAssigneeId, karma);
            }
        }

        private void UpdateUserKarma(long karmaAssigneeId, int karma)
        {
            UserKarma dbUserKarma = _userRepository.GetUsersKarma().FirstOrDefault(x => x.UserId == karmaAssigneeId);
            if (dbUserKarma == null)
            {
                UserKarma newUserKarma = new UserKarma();
                newUserKarma.UserId = karmaAssigneeId;
                newUserKarma.Current = karma;
                newUserKarma.GiveLimit = GIVEKARMALIMITDEFAULT;
                _userRepository.UpdateUserKarma(newUserKarma);

            }
            else
            {
                dbUserKarma.Current += karma;
            }
        }

        private void AddKarmaHisoryRecord(long karmaReporterId, long karmaAssigneeId, int karma)
        {
            UserKarmaHistory historyRecord = new UserKarmaHistory();
            historyRecord.KarmaAssigneeId = karmaAssigneeId;
            historyRecord.KarmaReporterId = karmaReporterId;
            historyRecord.Date = DateTime.Now;
            historyRecord.KarmaPoints = karma;
            _userRepository.AddKarmaHisoryRecord(historyRecord);
        }

        private bool IsGiveLimiteExceeded(long karmaReporterId, int karma)
        {
            long karmaLimit = 0;
            try
            {
                karmaLimit = _userRepository.GetUsersKarma().FirstOrDefault(x => x.UserId == karmaReporterId).GiveLimit;
            }
            catch (NullReferenceException e)
            {
                // Probably add error to log and set Give limit to 0 ?
                //throw new NullReferenceException("Give Limit is not set to userID: " + karmaReporterId, e);
            }
            if (karma >= karmaLimit)
            {
                return true;
            }

            else if ((karma + GetAssignedKarmaInCurrentMonth(karmaReporterId)) >= karmaLimit)
            {
                return true;
            }
            return false;
        }

        private long GetAssignedKarmaInCurrentMonth(long karmaReporterId)
        {
            int reportedKarmaInCurrentMonth = 0;
            var startOfTthisMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            IEnumerable<UserKarmaHistory> dbUserKarmaHistory = _userRepository.GetUsersKarmaHistory().Where(x => x.KarmaReporterId == karmaReporterId && x.Date >= startOfTthisMonth);
            foreach (UserKarmaHistory historyRecord in dbUserKarmaHistory)
            {
                reportedKarmaInCurrentMonth += historyRecord.KarmaPoints;
            }
            return reportedKarmaInCurrentMonth;
        }

    }
}
