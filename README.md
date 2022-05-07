# FracturedThrottle
FracturedThrottle is about giving the programmer an easy-to-implement, easy-to-understand IP-based rate limiting library with a low footprint. It has only 3 classes, and the dependencies are probably already included in your ASP.NET project.
I didn't want to reinvent the wheel, but I couldn't find anything that worked quite the way I wanted, so I made my own.
If you are looking for something with lots of features, very fine control and blazing fast speeds, this ain't it. This was designed to be simple, and in that simplicity is found reasonable performance.
## License
This is shipping with [GPLv3 TODO](TODO). If you would like to use this in proprietary software, [contact me](https://fracturedcode.net/contact) and I'll sell you a license for a few bucks.
## Usage
There are two ways to use FracturedThrottle, and they can be used in tandem, even on the same controller actions.
**You can view some examples in the "Examples" project**
The first method is the classic Attribute-based configuration pattern that uses middleware to achieve the functionality. You can stack as many Throttle attributes as you want on your action. This helps in scenarios where you might want to allow bursts, but don't want to allow a sustained load at those speeds.
Configure the necessary services and enable the middleware in your `Program.cs` or `Startup.cs`:
``` C#
// Service configuration is typically found in Program.cs or Startup.ConfigureServices()
services.AddMemoryCache();
services.AddHttpContextAccessor();
services.AddScoped<IThrottleCache, ThrottleCache>();
...
// IApplicationBuilder (app) is typically found in Program.cs or Startup.Configure()
app.UseMiddleware<ThrottleMiddleware>();
...
```
Then, apply the `Throttle` attribute wherever you need rate limiting:
``` C#
[Throttle(Rate = 20, RateType = RateTypeEnum.Minute)]
[Throttle(Rate = 300, RateType = RateTypeEnum.Day)]
public async Task<IActionResult> ExampleAction() {
    ...
}
```
The second method is to use the `IThrottleCache` service in your action. This is what the `ThrottleMiddleware` uses behind the scenes. There are a few reasons you may want to do this. 
Say you only want to rate limit based on requests that were successful. For example, you want to limit a client to submitting 1 form/day. But if you set a `Throttle` rate to 1 request/day, you might be receiving emails from users who submitted a form with invalid data, needed to make some edits and re-submit, only to find they were locked out of the webpage after making those edits.
``` C#
// Add the service to your constructor
private IThrottleCache RateCache { get; init; }
public ExampleController(IThrottleCache rateCache) {
    RateCache = rateCache;
}
...
// Use the service in your action, you can even use it with other Throttles as well!
[Throttle(Rate = 200, RateType = RateTypeEnum.Hour)]
public IActionResult ExampleAction(FormModel form) {
    const string separateCache = "SuccessCache";
    Throttle successThrottle = new() { Rate = 1, RateType = RateTypeEnum.Day }
    
    if (RateCache.AllowConnection(separateCache)) {
        if (ModelState.IsValid) {
            form.Process();
            RateCache.CacheConnection(separateCache);
            return Json(new { Message = "You have successfully submitted your application." });
        } else {
            return Json(new { Message = "Your application is invalid." });
        }
    } else {
        return Json(new { Message = "You have already submitted an application today." })
    }
}
```
Note that the `IThrottleCache` methods take an optional parameter `cacheModifier`, which is useful for maintaining multiple caches on one action for more advanced logic. This helps if you are using `IThrottleCache` in tandem with the attributes. Otherwise the piece of code above would work the same as if you weren't using `IThrottleCache`; `IThrottleCache` would be pulling from the same cache as `ThrottleMiddleware`.
## Installation
You can grab [the Nuget package TODO]() or compile it yourself:
``` Bash
dotnet build -c Release
```
Then add the dll to your project. In VS right click on project > Add > Project Reference > Browse > Browse > Navigate to the FracturedThrottle Release folder and select the dll.
## Support
This is a pretty simple library, so if you find any bugs I would appreciate a PR.
If you submit an issue I will eventually get around to it.
To really get my attention see [my contact info](https://fracturedcode.net/contact).
