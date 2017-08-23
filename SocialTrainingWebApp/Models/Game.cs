using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SocialTrainingWebApp.Models
{
    public class Game
    {
        [Key]
        public long MoveId { get; set; }
        public long GameId { get; set; }
        public string MissedGuesses { get; set; }
        public string CompletedGuesses { get; set; }
        public long EmployeePK { get; set; }
        [ForeignKey("EmployeePK")]
        public Employee Employee { get; set; }
    }
}