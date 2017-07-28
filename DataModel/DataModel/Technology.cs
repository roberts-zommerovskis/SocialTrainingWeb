using System.ComponentModel.DataAnnotations;

namespace DataModel.DataModel
{
    public class Technology
    {
        [Key]
        public long TechnologyId { get; set; }

        public string TechnologyName { get; set; }
    }
}
