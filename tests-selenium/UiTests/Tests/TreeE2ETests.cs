using NUnit.Framework;
using OpenQA.Selenium;
using UiTests.Core;
using UiTests.Pages;
using UiTests.Utils;

namespace UiTests.Tests;

public class TreeE2ETests : TestBase
{
    private IWebDriver _driver = null!;
    private TreePage _page = null!;

    [SetUp]
    public void SetUp()
    {
        _driver = DriverFactory.CreateChrome();
        _page = new TreePage(_driver);
    }

    [TearDown]
    public void TearDown()
    {
        _driver.Quit();
        _driver.Dispose();
    }

    [Test]
    public void Tree_Should_Be_Visible_And_Screenshot_Taken()
    {
        _page.Open(BaseUrl);

        Assert.That(_page.Title, Is.EqualTo("Tree with nested nodes (childrenAccessor)"));

        Assert.That(_page.FileTree.IsVisible(), Is.True);

        Screenshots.SaveElementPng(_driver, _page.FileTree.Root, "initial-tree");

        Assert.Pass("Tree is visible and screenshot saved.");
    }
}