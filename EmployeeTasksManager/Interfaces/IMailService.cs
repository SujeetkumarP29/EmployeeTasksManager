using Bm.DeskSharing.DataService.ViewModels;

namespace EmployeeTasksManager.Interfaces
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequestViewModel mailRequest);
    }
}
