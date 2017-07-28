using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataModel.DataModel
{
    public class UserLanguage
    {
        [Key]
        public int UserLanguageId { get; set; }
        [ForeignKey("User")]
        public long UserId { get; set; }
        [ForeignKey("Language")]
        public int LanguageId { get; set; }

        public virtual User User { get; set; }

        public virtual Language Language { get; set; }
    }
}
