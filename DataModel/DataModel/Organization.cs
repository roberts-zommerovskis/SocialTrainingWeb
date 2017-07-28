using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataModel.DataModel
{
    public class Organization
    {
        [Key]
        public long OrganizationId { get; set; }

        public string OrganizationName { get; set; }

        public virtual List<Team> Teams { get; set; }
    }
}
