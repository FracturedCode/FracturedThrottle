using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FracturedCode.FracturedThrottle
{
    public class ThrottleMiddleware
    {
        private RequestDelegate Next { get; init; }
        public ThrottleMiddleware(RequestDelegate rd)
        {
            Next = rd;
        }
        public async Task InvokeAsync(HttpContext context, IThrottleCache throttleCache)
        {
            Endpoint endpoint = context.GetEndpoint();
            ControllerActionDescriptor cad = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();

            if (cad is null)
            {
                await Next(context);
                return;
            }

            if (cad.MethodInfo
                .GetCustomAttributes(true)
                .Where(a => a.GetType() == typeof(Throttle))
                .Any(t => !throttleCache.AllowConnection((Throttle)t)))
            {
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                return;
            }

            throttleCache.CacheConnection();
            await Next(context);
        }
    }
}
