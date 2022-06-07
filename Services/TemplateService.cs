using LiteDB;
using MailQ.Models;
using Microsoft.Extensions.Caching.Memory;
using Mjml.Net;

namespace MailQ.Services;

public class TemplateService
{
    protected readonly MjmlRenderer renderer;
    protected readonly IMemoryCache cache;
    private readonly LiteDatabase db;

    public TemplateService(IMemoryCache cache, LiteDatabase db)
    {
        this.cache = cache;
        this.db = db;
        this.renderer = new MjmlRenderer();
    }

    public MailTemplate AddTemplate(MailTemplate template)
    {
        var templates = db.GetCollection<MailTemplate>("templates");
        templates.Insert(template);

        template.MessageTemplate = renderer.Render(template.MessageTemplate, new MjmlOptions
        {
            Minify = true,
        }).Html;
        cache.Set(template.Key, template);
        db.Commit();

        return template;
    }

    public MailTemplate UpdateTemplate(MailTemplate template)
    {
        var templates = db.GetCollection<MailTemplate>("templates");
        templates.Update(template);
        this.AddTemplate(template);

        return template;
    }

    public MailTemplate? GetTemplate(string key)
    {
        MailTemplate template;

        if (cache.TryGetValue<MailTemplate>(key, out template))
        {
            return template;
        }

        var templates = db.GetCollection<MailTemplate>("templates");
        var dbTemplate = templates.FindOne(e => e.Key == key);

        if (dbTemplate is not null)
        {
            return this.AddTemplate(dbTemplate);
        }

        return null;
    }

    public void RemoveTemplate(string key)
    {
        var templates = db.GetCollection<MailTemplate>("templates");
        templates.DeleteMany(t => t.Key == key);

        cache.Remove(key);
    }
}
