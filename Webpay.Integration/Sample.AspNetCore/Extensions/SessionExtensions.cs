using Microsoft.AspNetCore.Http;


namespace Sample.AspNetCore.Extensions;

using System.Text.Json;

public static class SessionExtensions
{
    public static T GetJson<T>(this ISession session, string key)
    {
        var sessionData = session.GetString(key);
        return sessionData == null
            ? default
            : JsonSerializer.Deserialize<T>(sessionData);
    }


    public static void SetJson(this ISession session, string key, object value)
    {
        session.SetString(key, JsonSerializer.Serialize(value));
    }
}