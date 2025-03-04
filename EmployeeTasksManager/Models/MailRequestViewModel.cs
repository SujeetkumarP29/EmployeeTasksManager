using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bm.DeskSharing.DataService.ViewModels
{
    public class MailRequestViewModel
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string ManagerBody { get; set; }
        public string Image { get; set; }
        public List<IFormFile> Attachments { get; set; }

    }
}
