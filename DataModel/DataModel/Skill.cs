using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataModel.DataModel
{
    public class Skill
    {
        [Key]
        public long SkillId { get; set; }

        [ForeignKey("User")]
        public long UserId { get; set; }

        [ForeignKey("Technology")]
        public long TechnologyId { get; set; }

        public SkillLevel Level { get; set; }

        public virtual User User { get; set; }

        public virtual Technology Technology { get; set; }
    }
}
