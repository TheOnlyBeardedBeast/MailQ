using Grpc.Core;
using LiteDB;
using MailQ.Models;

namespace MailQ.Services;

public class MailComService : Mailcom.MailcomBase
{
    private readonly IMailSender mailer;
    private readonly LiteDatabase db;
    private readonly TemplateService ts;
    private readonly MailService ms;

    public MailComService(IMailSender mailer, LiteDatabase db, TemplateService ts, MailService ms)
    {
        this.mailer = mailer;
        this.db = db;
        this.ts = ts;
        this.ms = ms;
    }

    public override Task<SendMailResponse> SendMail(SendMailRequest request, ServerCallContext context)
    {
        var template = ts.GetTemplate(request.TemplateKey);

        if (template is null)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Unknown template"));
        }

        ms.AddMail(request);

        return Task.FromResult(new SendMailResponse
        {
            Queued = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow)
        });
    }

    public override Task<AddTemplateResponse> AddTemplate(AddTemplateRequest request, ServerCallContext context)
    {
        var existingTemplate = ts.GetTemplate(request.Key);

        if (existingTemplate is not null)
        {
            throw new RpcException(new Status(StatusCode.AlreadyExists, "Template already exists"));
        }

        var template = new MailTemplate
        {
            Key = request.Key,
            MessageTemplate = request.MailTemplate,
            SubjectTemplate = request.SubjectTemplate
        };

        ts.AddTemplate(template);

        return Task.FromResult(new AddTemplateResponse
        {
            Key = template.Key
        });
    }

    public override Task<AddTemplateResponse> UpdateTemplate(AddTemplateRequest request, ServerCallContext context)
    {
        var template = ts.GetTemplate(request.Key);

        if (template is null)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Template already exists"));
        }

        template.Key = request.Key;
        template.MessageTemplate = request.MailTemplate;
        template.SubjectTemplate = request.SubjectTemplate;

        ts.UpdateTemplate(template);

        return Task.FromResult(new AddTemplateResponse
        {
            Key = template.Key
        });
    }

    public override Task<AddTemplateResponse> RemoveTemplate(RemoveTemplateRequest request, ServerCallContext context)
    {
        var template = ts.GetTemplate(request.Key);

        if (template is null)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Unknown template"));
        }

        ts.RemoveTemplate(request.Key);

        return Task.FromResult(new AddTemplateResponse
        {
            Key = request.Key
        });
    }
}
