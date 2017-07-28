using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataModel.DataModel
{
    public class UserKarmaHistory
    {
        [Key]
        public long UserKarmaHistoryId { get; set; }

        [ForeignKey("KarmaAssignee")]
        public long? KarmaAssigneeId { get; set; }

        [ForeignKey("KarmaReporter")]
        public long? KarmaReporterId { get; set; }

        public DateTime Date { get; set; }

        public int KarmaPoints { get; set; }

        public virtual User KarmaAssignee { get; set; }

        public virtual User KarmaReporter { get; set; }

    }
}
