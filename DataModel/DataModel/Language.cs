using System.ComponentModel.DataAnnotations;

namespace DataModel.DataModel
{
    public class Language
    {

        [Key]
        public int LanguageId { get; set; }
        public string LanguageName { get; set; }
    }
}
