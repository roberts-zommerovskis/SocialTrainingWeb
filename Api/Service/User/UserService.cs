using System.Collections.Generic;
using System.Linq;
using Api.Models;
using AutoMapper;
using DataAccess.Dto;
using DataAccess.Manager;

namespace Api.Service.User
{
    public class UserService
    {
        private readonly UserManager _userManager;

        public UserService(UserManager userManager)
        {
            _userManager = userManager;
        }

        public UserInfoViewModel GetUser(long userId)
        {
            var user = _userManager.GetUser(userId);
            return Mapper.Map<FullUserDto, UserInfoViewModel>(user);
        }

        public List<UserInfoViewModel> ListEmployees()
        {
            var users = _userManager.ListEmployees();
            return users.Select(Mapper.Map<FullUserDto, UserInfoViewModel>).ToList();
        }

        public SimpleUserInfoViewModel SetAura(SimpleUserInfoViewModel simpleUserInfoViewModel)
        {
            var fullUserDto = Mapper.Map<SimpleUserInfoViewModel, FullUserDto>(simpleUserInfoViewModel);
            var updatedUser = _userManager.SetAura(fullUserDto);
            return Mapper.Map<FullUserDto, SimpleUserInfoViewModel>(updatedUser);
        }

        public List<SimpleUserInfoViewModel> ListUsers(int from, int count)
        {
            var users = _userManager.ListUsers(from, count);
            return users.Select(Mapper.Map<SimpleUserDto, SimpleUserInfoViewModel>).ToList();
        }

        public void GiveKarma(string karmaReporterMail, long assigneeUserId, int karma)
        {
            _userManager.GiveKarma(karmaReporterMail, assigneeUserId, karma);
        }
    }
}