

namespace DN.WebApi.Infrastructure.Common.Constants;

public static class CacheKeys
{
    public static string GetCacheKey<T>(object id)
    
    {
        return $"{typeof(T).Name}-{id}";
    }

    public static string GetCacheKey(string name, object id)
    {
        return $"{name}-{id}";
    }
}