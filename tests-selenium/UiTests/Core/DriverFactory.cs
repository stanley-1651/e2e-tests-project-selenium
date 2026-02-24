using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace UiTests.Core;

public static class DriverFactory
{
    public static IWebDriver CreateChrome()
    {
        new DriverManager().SetUpDriver(new ChromeConfig());

        var options = new ChromeOptions();
        options.AddArgument("--window-size=1400,900");
        options.AddArgument("--force-device-scale-factor=1");

        return new ChromeDriver(options);
    }
}