using System.Net;
using System.Net.Mail;

public class MailSenderOptions
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }

    public bool Tls { get; set; } = false;

}

public interface IMailSender
{
    Task SendEmailAsync(string email, string subject, string message, bool isHtml = false);
    Task SendEmailSAsync(string[] emails, string subject, string message, bool isHtml = false);
}

public class MailSender : IMailSender
{
    private readonly MailSenderOptions options = new MailSenderOptions();
    private readonly SmtpClient client;

    public MailSender(Action<MailSenderOptions> setupOptions)
    {
        setupOptions.Invoke(options);

        client = new SmtpClient(this.options.Host, this.options.Port);
        client.UseDefaultCredentials = false;
        client.Credentials = new NetworkCredential(this.options.Email, this.options.Password);
    }

    public async Task SendEmailAsync(string email, string subject, string message, bool isHtml = false)
    {
        MailMessage mailmessage = new MailMessage(options.Email, email);

        mailmessage.Body = message;
        mailmessage.IsBodyHtml = isHtml;
        mailmessage.Subject = subject;

        await client.SendMailAsync(mailmessage);
    }

    public async Task SendEmailSAsync(string[] emails, string subject, string message, bool isHtml = false)
    {
        MailMessage mailmessage = new MailMessage();

        mailmessage.From = new MailAddress(options.Email);
        mailmessage.IsBodyHtml = isHtml;
        mailmessage.Body = message;
        mailmessage.Subject = subject;

        foreach (string email in emails)
        {
            mailmessage.To.Add(email);
        }

        await client.SendMailAsync(mailmessage);
    }
}