using Bm.DeskSharing.DataService.ViewModels;
using Microsoft.AspNetCore.Mvc;
using EmployeeTasksManager.Interfaces;
using EmployeeTasksManager.Models;
using EmployeeTasksManager.Implementations;

namespace EmployeeTasksManager.Controllers
{
    public class MailController : Controller
    {
        private readonly IMailService mailService;
        public MailController(IMailService mailService)
        {
            this.mailService = mailService;
        }
        [HttpPost("send")]
        public async Task<IActionResult> SendMail(MailRequestViewModel testmail)
        {
            try
            {
                await mailService.SendEmailAsync(testmail);
                return Json(new { data = true });
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
