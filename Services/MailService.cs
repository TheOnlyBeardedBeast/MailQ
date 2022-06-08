
using System.Collections.Concurrent;
namespace MailQ.Services;

public class MailService
{
    private object Lock = new object();
    private ConcurrentQueue<SendMailRequest> MailQueue = new ConcurrentQueue<SendMailRequest>();
    // private List<SendMailRequest> MailQueue = new List<SendMailRequest>();

    public Task AddMail(SendMailRequest mail)
    {
        this.MailQueue.Enqueue(mail);

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
