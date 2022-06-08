using AspNetCore.Authentication.ApiKey;

namespace MailTrue.Utils;

public class ApiKeyProvider : IApiKeyProvider
{

    public Task<IApiKey> ProvideAsync(string key)
    {
        if (key == Environment.GetEnvironmentVariable("API_KEY"))
        {
            return Task.FromResult(new ApiKey(key, "MailQ") as IApiKey);
        }

        return Task.FromResult(new ApiKey(string.Empty, string.Empty) as IApiKey);
    }
}