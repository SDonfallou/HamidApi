using MailKit.Security;
using Microsoft.OpenApi.Extensions;
using MimeKit;
using System.Net;
using System.Net.Mail;

namespace bookShareBEnd.Database.Net
{
    public class EmailService
    {

        //private readonly SmtpClient _smtpClient;

        
     
       
        //public EmailService(string host, int port, string username, string password)
        //{
        //    _smtpClient = new SmtpClient(host, port)
        //    {
        //        Credentials = new NetworkCredential(username, password),
        //        EnableSsl = false // Enable SSL/TLS if required
        //    };
        //}

        //public void SendRegistrationEmail(string toEmail)
        //{
        //    var from = "crawford.turcotte98@ethereal.email";
        //    var subject = "Welcome to BookShare!";
        //    var body = "Thank you for registering with BookShare.";

        //    var message = new MailMessage(from, toEmail, subject, body);

        //    try
        //    {
        //        _smtpClient.Send(message);
        //        Console.WriteLine("Email sent successfully.");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Failed to send email: {ex.Message}");
        //    }
        //    finally
        //    {
        //        message.Dispose();
        //    }
        //}

        ////public void SendEmail(Email request)
        ////{
        ////    var email = new MimeMessage();
        ////    email.From.Add(MailboxAddress.Parse("crawford.turcotte98@ethereal.email"));
        ////    email.To.Add(MailboxAddress.Parse(request.To));
        ////    email.Subject = "Welcome to BookShare!";
        ////    email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = request.Body };

        ////    using var smtp = new SmtpClient();
        ////    smtp.Connect("crawford.turcotte98@ethereal.email", 587, SecureSocketOptions.StartTls);
        ////    smtp.Authenticate("crawford.turcotte98@ethereal.email", "6txUnmVzpGphr74nVN");
        ////    smtp.Send(email);
        ////    smtp.Dispose();
          
        ////}

    }
}
