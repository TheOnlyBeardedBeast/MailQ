namespace MailQ.Services;

public class MailService
{
    private object Lock = new object();
    private List<SendMailRequest> MailQueue = new List<SendMailRequest>();

    public Task AddMail(SendMailRequest mail)
    {
        lock (Lock)
        {
            this.MailQueue.Add(mail);
        }

        return Task.CompletedTask;
    }

    public Task<List<SendMailRequest>> GetMailQ()
    {
        List<SendMailRequest> result;

        lock (Lock)
        {
            result = MailQueue.ToList();
            this.MailQueue.Clear();
        }

        return Task.FromResult(result);
    }

}
