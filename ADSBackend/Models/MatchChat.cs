using ADSBackend.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models
{
    public class MatchChat
    {
        [Key]
        public int MatchChatId { get; set; }

        public int MatchId { get; set; }
        public Match Match { get; set; }

        public int? UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string Message { get; set; }
        public DateTime MessageDate { get; set; } = DateTime.Now;

        public bool Deleted { get; set; }
    }
}
