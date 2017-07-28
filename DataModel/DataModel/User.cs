using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataModel.DataModel
{
    public class User
    {
        [Key]
        public long UserId { get; set; }
        
        [ForeignKey("GoogleDoc")]
        public long? GoogleDocId { get; set; }

        [ForeignKey("Team")]
        public long? TeamId { get; set; }

        [ForeignKey("Project")]
        public long? ProjectId { get; set; }

        public long? ImportId { get; set; }

        public string FullName { get; set; }

        public string Country { get; set; }

        public string Organization { get; set; }
        
        public string PrimaryClientContact { get; set; }

        public string SecondaryClientContact { get; set; }

        public UserStatus Status { get; set; }

        public DateTime ?JoinDate { get; set; }

        public DateTime ?ImportDate { get; set; }

        public string Email { get; set; }

        public virtual GoogleDoc GoogleDoc { get; set; }

        public virtual Team Team { get; set; }

        public virtual Project Project { get; set; }

        public virtual List<Skill> Skills { get; set; }

        public virtual List<UserLanguage> UserLanguages { get; set; }

        public virtual UserKarma UserKarma { get; set; }

    }
}
