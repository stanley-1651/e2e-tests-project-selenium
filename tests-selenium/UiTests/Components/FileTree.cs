using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace UiTests.Components;

public class FileTree
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    public FileTree(IWebDriver driver, TimeSpan? timeout = null)
    {
        _driver = driver;
        _wait = new WebDriverWait(_driver, timeout ?? TimeSpan.FromSeconds(10));
    }

    public IWebElement Root =>
        _wait.Until(d => d.FindElement(By.CssSelector("mat-tree.example-tree")));

    public bool IsVisible()
        => Root.Displayed;
}