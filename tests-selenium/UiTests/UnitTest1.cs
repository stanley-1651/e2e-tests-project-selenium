using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace UiTests;

public class SmokeTests
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
        _driver.Navigate().GoToUrl("http://localhost:4200/");
        Thread.Sleep(3000);
        Assert.That(_driver.PageSource, Does.Contain("body"));
    }
}