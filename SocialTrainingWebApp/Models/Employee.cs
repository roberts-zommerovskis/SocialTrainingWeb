using System.ComponentModel.DataAnnotations;

namespace SocialTrainingWebApp.Models
{
    public class Employee
    {
        [Key]
        public int DbId { get; set; }
        public int ImportId { get; set; }
        public string FullName { get; set; }
        public string Sex { get; set; }
    }
}
