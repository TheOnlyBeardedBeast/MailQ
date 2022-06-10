using AspNetCore.Authentication.ApiKey;
using LiteDB;
using MailQ.Services;
using MailTrue.Utils;

namespace MailQ.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddLiteDatabase(this IServiceCollection services, string? dbLocation = null)
    {
        return services.AddTransient<LiteDatabase>(sp => new LiteDatabase(dbLocation ?? "db.db"));
    }

    public static IServiceCollection AddMailQ(this IServiceCollection services)
    {
        var Port = Environment.GetEnvironmentVariable("SMTP_PORT");
        var Host = Environment.GetEnvironmentVariable("SMTP_HOST");
        var Email = Environment.GetEnvironmentVariable("SMTP_EMAIL");
        var Password = Environment.GetEnvironmentVariable("SMTP_PASS");
        var ApiKey = Environment.GetEnvironmentVariable("API_KEY");
        int PortInt;

        if (Port is null || Host is null || Email is null || Password is null || ApiKey is null || !int.TryParse(Port, out PortInt))
        {
            throw new Exception("Missing smtp configuration");
        }

        int MailInterval;
        if (!int.TryParse(Environment.GetEnvironmentVariable("MAIL_INTERVAL"), out MailInterval))
        {
            MailInterval = 60;
        }

        services.AddTransient<IMailSender, MailSender>(_ => new MailSender(options =>
        {
            options.Port = PortInt;
            options.Host = Host;
            options.Email = Email;
            options.Password = Password;
        }));

        services.AddTransient<ITemplateService, TemplateService>();
        services.AddSingleton<IMailService, MailService>();
        services.AddHostedService<SchedulerService>(sp => new SchedulerService(sp.GetRequiredService<IMailService>(), sp.GetRequiredService<ITemplateService>(), sp.GetRequiredService<IMailSender>(), MailInterval));

        services.AddAuthorization().AddAuthentication(ApiKeyDefaults.AuthenticationScheme)
            .AddApiKeyInHeaderOrQueryParams<ApiKeyProvider>(options =>
            {
                options.Realm = "MailQ";
                options.KeyName = "X-API-KEY";
            });

        return services;
    }

    public static GrpcServiceEndpointConventionBuilder MapMailQ(this WebApplication app)
    {
        return app.MapGrpcService<MailComService>();
    }
}
