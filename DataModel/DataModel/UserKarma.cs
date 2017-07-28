using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataModel.DataModel
{
    public class UserKarma
    {
        [Key]
        [ForeignKey("User")]
        public long UserId { get; set; }

        public long Current { get; set; }

        public long ImportUserId { get; set; }

        public long Aura { get; set; }

        public long GiveLimit { get; set; }

        public virtual User User { get; set; }
    }
}
