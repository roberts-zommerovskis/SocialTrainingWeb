using System.Collections.Generic;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Cors;
using Api.Models;
using Api.Service.User;

namespace Api.Controllers
{
    // TODO: Error logging!

    //[OAuth2Authentication]
    //[Authorize]
    /// <summary>
    /// 
    /// </summary>
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserController : ApiController
    {
        private readonly UserService _userService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userService"></param>
        public UserController(UserService userService)
        {
            this._userService = userService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/karma/getProfile")]
        public UserInfoViewModel GetProfile([FromUri] long userId)
        {
            var userInfo = _userService.GetUser(userId);
            return userInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/karma/setAura")]
        public SimpleUserInfoViewModel SetAura([FromBody] SimpleUserInfoViewModel user)
        {
            return _userService.SetAura(user);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/karma/getProfileList")]
        public List<SimpleUserInfoViewModel> GetProfileList([FromUri] int from = 0, [FromUri] int count = 0)
        {
            return _userService.ListUsers(from, count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/karma/getEmployees")]
        public List<UserInfoViewModel> GetEmployees()
        {
            return _userService.ListEmployees();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assigneeUserId"></param>
        /// <param name="karma"></param>
        [HttpPut]
        [Route("api/karma/give")]
        public void GiveKarma([FromUri] long assigneeUserId, [FromUri] int karma)
        {
            string karmaReporterMail = Thread.CurrentPrincipal.Identity.Name;
            _userService.GiveKarma(karmaReporterMail, assigneeUserId, karma);

        }
    }


}
