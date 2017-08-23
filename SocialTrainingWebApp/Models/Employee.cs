using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SocialTrainingWebApp.Models
{
    public class Employee
    {
        [Key]
        public long EmployeePK { get; set; }
        public long ImportId { get; set; }
        public string FullName { get; set; }
        public string Sex { get; set; }
        public string Email { get; set; }
        public virtual ICollection<Game> Games { get; set; }
    }
}
