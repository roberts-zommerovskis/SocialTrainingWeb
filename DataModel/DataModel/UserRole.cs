using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataModel.DataModel
{
    public class UserRole
    {
        [Key]
        public long UserRoleId { get; set; }

        [ForeignKey("User")]
        public long UserId { get; set; }

        public Role Role { get; set; }

        public virtual User User { get; set; }
    }
}
