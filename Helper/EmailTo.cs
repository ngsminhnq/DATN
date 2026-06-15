using System.Net.Mail;
using System.Net;

namespace HRemployee.Helper
{
    public class EmailTo
    {
        public string Mail { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public async Task<string> SendEmailAsync(EmailTo emailTo)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587, Credentials = new NetworkCredential("nqmminh2k3@gmail.com", "rjoj vpah cijg atzd"), EnableSsl = true };

            try
            {
                var message = new MailMessage();
                message.From = new MailAddress("nqmminh2k3@gmail.com");
                message.To.Add(emailTo.Mail);
                message.Subject = emailTo.Subject;
                message.Body = emailTo.Content;
                message.IsBodyHtml = true;
                await smtpClient.SendMailAsync(message);
                return "Gửi email thành công";
            }
            catch (Exception ex)
            {
                return "Lỗi khi gửi email: " + ex.Message;
            }
        }
    }
}