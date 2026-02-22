using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using UiTests.Core;

namespace UiTests;

public class SmokeTests : TestBase
{
    private IWebDriver _driver = null!;

    [SetUp]
    public void SetUp()
    {
        new DriverManager().SetUpDriver(new ChromeConfig());

        var options = new ChromeOptions();
        _driver = new ChromeDriver(options);
    }

    [TearDown]
    public void TearDown()
    {
        _driver.Quit();
        _driver.Dispose();
    }

    [Test]
    public void App_Should_Open()
    {
        _driver.Navigate().GoToUrl($"{BaseUrl}/");
        Thread.Sleep(3000);
        Assert.That(_driver.PageSource, Does.Contain("body"));
    }
}