﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SocialTrainingWebApp.Models
{
    public class Employee
    {
        [Key]
        public long EmployeePK { get; set; }
        public long ImportId { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        [JsonIgnore]
        [IgnoreDataMember]
        public virtual ICollection<Game> Games { get; set; }
    }
}
