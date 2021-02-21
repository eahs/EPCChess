using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ADSBackend.Models.EmailViewModels
{
    public class GenericMessageViewModel
    {
        public string Title { get; set; } = "";
        public string MessageHeading { get; set; } = "";
        public string MessageBody { get; set; } = "";
        public string ActionLinkTitle { get; set; } = "";
        public string ActionLink { get; set; } = "";
    }
}
