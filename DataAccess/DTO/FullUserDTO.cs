using System.Collections.Generic;

namespace DataAccess.Dto
{
    public class FullUserDto : SimpleUserDto
    {
        public List<UserLanguageDto> Languages { get; set; }
        public string Team { get; set; }
        public string Project { get; set; }
        public string Country { get; set; }
        public string PrimaryClientContact { get; set; }
        public string SecondaryClientContact { get; set; }

    }
}
