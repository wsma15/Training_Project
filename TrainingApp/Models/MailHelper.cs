using System;
using System.Net;
using System.Net.Mail;

public class MailHelper
{
    public static void SendEmail(string recipientAddress, string subject, string body)
    {
        var fromAddress = new MailAddress("sharkawy.1223@gmail.com", "Hussein Alwisi");
        var toAddress = new MailAddress(recipientAddress);

        var smtpClient = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential("sharkawy.1223@gmail.com", "ycijejxepqeevira\r\n") // Use the generated app password
        };

        var mailMessage = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = body,
            IsBodyHtml = false // Set to true if your email body contains HTML
        };

        try
        {
            smtpClient.Send(mailMessage);
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            System.Diagnostics.Debug.WriteLine($"Error sending email: {ex.Message}");
        }
    }
}
