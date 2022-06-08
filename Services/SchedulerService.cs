using Stubble.Core;
using Stubble.Core.Builders;

namespace MailQ.Services;

public interface ISchedulerService
{
    Task StartAsync(CancellationToken cancellationToken);
    Task StopAsync(CancellationToken cancellationToken);
    Task WorkLoop();
}

public class SchedulerService : IHostedService, ISchedulerService
{
    private readonly PeriodicTimer t;
    private readonly IMailService ms;
    private readonly ITemplateService ts;
    private readonly IMailSender mailer;
    private readonly StubbleVisitorRenderer renderer;

    public SchedulerService(IMailService ms, ITemplateService ts, IMailSender mailer)
    {
        this.t = new PeriodicTimer(TimeSpan.FromSeconds(60));
        this.ms = ms;
        this.ts = ts;
        this.mailer = mailer;
        this.renderer = new StubbleBuilder().Build();
    }

    private async Task Job()
    {
        var mails = await ms.GetMailQ();

        try
        {

            foreach (var mail in mails)
            {
                var template = ts.GetTemplate(mail.TemplateKey);

                if (template is not null)
                {
                    var html = await renderer.RenderAsync(template.MessageTemplate, mail.MailData);
                    var subject = await renderer.RenderAsync(template.SubjectTemplate, mail.SubjectData);

                    await mailer.SendEmailAsync(mail.To, subject, html, true);
                }
            }
        }
        catch (System.Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public async Task WorkLoop()
    {
        while (await this.t.WaitForNextTickAsync())
        {
            await this.Job();
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(WorkLoop);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        this.t.Dispose();
        return Task.CompletedTask;
    }
}
