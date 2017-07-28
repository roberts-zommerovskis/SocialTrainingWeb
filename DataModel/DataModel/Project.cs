using System.ComponentModel.DataAnnotations;

namespace DataModel.DataModel
{
    public class Project
    {
        [Key]
        public long ProjectId { get; set; }

        public string ProjectTitle { get; set; }
    }
}
