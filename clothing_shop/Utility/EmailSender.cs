//using Microsoft.AspNetCore.Identity.UI.Services;
//using MailKit.Net.Smtp;
//using MimeKit;


//namespace clothing_shop.Utility
//{
//    public class EmailSender : IEmailSender
//    {
//        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
//        {
//            var emailToSend = new MimeMessage();
//            emailToSend.From.Add(MailboxAddress.Parse("caligulashop@outlook.com"));
//            emailToSend.To.Add(MailboxAddress.Parse(email));
//            emailToSend.Subject = subject;
//            emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html)
//            {
//                Text = htmlMessage
//            };
//            using(var emailClient = new SmtpClient())
//            {
//                emailClient.Connect("smtp.outlook.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
//                emailClient.Authenticate("caligulashop@outlook.com", "Pimple23!");
//                await emailClient.SendAsync(emailToSend);
//                await emailClient.DisconnectAsync(true);
//            }
//            await Task.CompletedTask;
//        }
        
//    }
//}
