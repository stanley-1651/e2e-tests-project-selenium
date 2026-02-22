using OpenQA.Selenium;
using UiTests.Components;

namespace UiTests.Pages;

public class TreePage
{
    private readonly IWebDriver _driver;

    public TreePage(IWebDriver driver)
    {
        _driver = driver;
        FileTree = new FileTree(driver);
    }

    public FileTree FileTree { get; }

    public void Open(string baseUrl)
        => _driver.Navigate().GoToUrl($"{baseUrl.TrimEnd('/')}/");

    public string Title => _driver.Title;
}