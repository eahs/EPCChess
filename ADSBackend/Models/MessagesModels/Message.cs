using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models.MessagesModels
{
    public class Message
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public DateTime PublishDate { get; set; }

        public string Author { get; set; }

        private string description;
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value.Replace("<br>", "\n");
            }
        }
    }
}
