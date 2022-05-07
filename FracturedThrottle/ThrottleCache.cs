using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Caching.Memory;

namespace FracturedCode.FracturedThrottle
{
    public class ThrottleCache : IThrottleCache
    {
        private IMemoryCache Cache { get; init; }
        private IHttpContextAccessor HttpContextAccessor { get; init; }
        private HttpContext Context => HttpContextAccessor.HttpContext;
        private string CacheKey =>
            Context.GetEndpoint().Metadata.GetMetadata<ControllerActionDescriptor>().MethodInfo
            + Context.Request.Path
            + Context.Connection.RemoteIpAddress;
        public ThrottleCache(IMemoryCache cache, IHttpContextAccessor contextAccessor)
        {
            Cache = cache;
            HttpContextAccessor = contextAccessor;
        }

        public bool AllowConnection(Throttle settings, string cacheModifier = null) =>
            !Cache.TryGetValue<List<DateTime>>(CacheKey + cacheModifier, out var hits)
                || hits.Count(x => x >= DateTime.Now.Subtract(
                        settings.RateType switch
                        {
                            RateTypeEnum.Minute => TimeSpan.FromMinutes(1),
                            RateTypeEnum.Hour => TimeSpan.FromHours(1),
                            _ => TimeSpan.FromDays(1)
                        })
                    ) < settings.Rate;

        public void CacheConnection(string cacheModifier = null) =>
            Cache.GetOrCreate(CacheKey + cacheModifier, ce =>
            {
                ce.SlidingExpiration = TimeSpan.FromDays(2);
                return new List<DateTime>();
            }).Add(DateTime.Now);
    }
}
