using System.Collections.Generic;

namespace Api.Models
{
    public class UserInfoViewModel : SimpleUserInfoViewModel
    {
        public List<UserLanguageViewModel> Languages { get; set; }
        public string Team { get; set; }
        public string Project { get; set; }
        public string Country { get; set; }
        public string PrimaryClientContact { get; set; }
        public string SecondaryClientContact { get; set; }
    }
}