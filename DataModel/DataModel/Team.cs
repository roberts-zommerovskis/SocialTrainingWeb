using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataModel.DataModel
{
    public class Team
    {
        [Key]
        public long TeamId { get; set; }

        [ForeignKey("Organization")]
        public long OrganizationId { get; set; }

        public string TeamName { get; set; }

        public virtual Organization Organization { get; set; }
    }
}
