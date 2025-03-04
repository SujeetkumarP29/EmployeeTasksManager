using Bm.DeskSharing.DataService.ViewModels;
using EmployeeTasksManager.Interfaces;
using EmployeeTasksManager.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EmployeeTasksManager.Implementations
{
    public class MailService:IMailService
    {
        private readonly MailSettingsViewModel _mailSettings;
        public MailService(IOptions<MailSettingsViewModel> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public static string FixBase64ForImage(string Image)
        {
            System.Text.StringBuilder sbText = new System.Text.StringBuilder(Image, Image.Length);
            sbText.Replace("\r\n", string.Empty); sbText.Replace(" ", string.Empty);
            return sbText.ToString();
        }

        public async Task SendEmailAsync(MailRequestViewModel mailRequest)
        {
            MimeMessage? email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;

            BodyBuilder? builder = new BodyBuilder();
            builder.HtmlBody = mailRequest.Body;

            if (!string.IsNullOrEmpty(mailRequest.Image))
            {
                string? ImgData = mailRequest.Image.Substring(22);
                byte[] bytes = Convert.FromBase64String(ImgData);
                builder.Attachments.Add("image.png", bytes, ContentType.Parse("image/png"));
            }

            email.Body = builder.ToMessageBody();

            using SmtpClient? smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }


        public async Task SendEmailAsynctest(MailRequestViewModel request)
        {
            string FilePath = Directory.GetCurrentDirectory() + "\\wwwroot\\Mail template\\mail.html";
            StreamReader str = new StreamReader(FilePath);
            string MailText = str.ReadToEnd();
            str.Close();
            byte[] bytes = Convert.FromBase64String(request.Body);
            BodyBuilder? builder = new BodyBuilder();
            if (request.Body != null)
            {
                builder.Attachments.Add("image.png", bytes, ContentType.Parse("image/png"));
            }
            MimeMessage? email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(request.ToEmail));
            email.Subject = $"Welcome {request.ToEmail}";

            builder.HtmlBody = MailText;
            email.Body = builder.ToMessageBody();
            using SmtpClient? smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

        private IFormFile Base64ToImage(string equipmentFiles)
        {

            byte[] bytes = Convert.FromBase64String(equipmentFiles);
            MemoryStream stream = new MemoryStream(bytes);

            IFormFile file = new FormFile(stream, 0, bytes.Length, "xxx", "xxx");


            return file;
        }

    }
}
