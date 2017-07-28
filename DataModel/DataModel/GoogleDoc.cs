using System.ComponentModel.DataAnnotations;

namespace DataModel.DataModel
{
    public class GoogleDoc
    {
        [Key]
        public long GoogleDocId { get; set; }

        public string GoogleFileId { get; set; }

        public string Uri { get; set; }

    }
}
