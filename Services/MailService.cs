
using System.Collections.Concurrent;
namespace MailQ.Services;

public interface IMailService
{
    Task AddMail(SendMailRequest mail);
    Task<List<SendMailRequest>> GetMailQ();
}

public class MailService : IMailService
{
    private ConcurrentBag<SendMailRequest> MailQueue = new ConcurrentBag<SendMailRequest>();

    public Task AddMail(SendMailRequest mail)
    {
        this.MailQueue.Add(mail);

        return Task.CompletedTask;
    }

    public Task<List<SendMailRequest>> GetMailQ()
    {
        List<SendMailRequest> result;

        result = MailQueue.ToList();
        this.MailQueue.Clear();


        return Task.FromResult(result);
    }

}
