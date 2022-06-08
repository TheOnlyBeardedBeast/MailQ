using LiteDB;
using MailQ.Services;

namespace MailQ.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddLiteDatabase(this IServiceCollection services, string? dbLocation = null)
    {
        return services.AddTransient<LiteDatabase>(sp => new LiteDatabase(dbLocation ?? "db.db"));
    }

    public static IServiceCollection AddMailQ(this IServiceCollection services)
    {
        services.AddTransient<IMailSender, MailSender>();
        services.AddTransient<ITemplateService, TemplateService>();
        services.AddSingleton<IMailService, MailService>();
        services.AddHostedService<SchedulerService>();

        return services;
    }
}
