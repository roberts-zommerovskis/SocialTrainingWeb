using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SocialTrainingWebApp.Models
{

    public class Game
    {
        [Key]
        public long MoveId { get; set; }
        public long GameId { get; set; }
        public int PointsSoFar { get; set; }
        public string UnguessedEmployees { get; set; }
        public string GuessedEmployees { get; set; }
        public long EmployeePK { get; set; }
        [ForeignKey("EmployeePK")]
        public virtual Employee Employee { get; set; }
    }
}