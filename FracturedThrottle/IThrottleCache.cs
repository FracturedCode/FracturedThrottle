using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FracturedCode.FracturedThrottle
{
    public interface IThrottleCache
    {
        /// <summary>
        /// Determines whether the client connection should be allowed based on cached history and the Throttle attribute.
        /// </summary>
        /// <param name="cacheModifier">Optional. Useful to create multiple caches for each action.</param>
        /// <returns>boolean, whether the client has exceeded the rate given in "settings" parameter.</returns>
        public bool AllowConnection(Throttle settings, string cacheModifier = null);
        /// <summary>
        /// Saves a new record of the client's connection in the cache.
        /// </summary>
        /// <param name="cacheModifier">Optional. Useful to create multiple caches for each action.</param>
        public void CacheConnection(string cacheModifier = null);
    }
}
