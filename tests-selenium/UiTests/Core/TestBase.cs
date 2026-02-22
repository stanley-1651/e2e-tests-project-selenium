using NUnit.Framework;

namespace UiTests.Core;

public abstract class TestBase
{
    protected string BaseUrl
    {
        get
        {
            var url = Environment.GetEnvironmentVariable("APP_URL");
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("Missing APP_URL. Set it to the application base address, e.g. http://localhost:4200");

            return url.TrimEnd('/');
        }
    }
}