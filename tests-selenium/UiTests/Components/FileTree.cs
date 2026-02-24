using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
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

    public bool IsVisible() => Root.Displayed;

    private void ExpandRootIfPresent()
    {
        try
        {
            var rootContainer = FindNodeContainerByLabel("/");
            ExpandIfCollapsed("/", rootContainer);
        }
        catch (WebDriverTimeoutException)
        {
        }
    }

    public string ExpandPath(string unixPath)
    {
        var segments = SplitUnixPath(unixPath);
        if (segments.Length == 0) throw new ArgumentException("Empty path.", nameof(unixPath));

        ExpandRootIfPresent();
        ExpandRec(segments, 0);

        var leafName = segments[^1];
        _wait.Until(_ => Root.FindElements(By.XPath(
            $".//*[contains(normalize-space(.), {EscapeXPathText(leafName)})]"
        )).Count > 0);

        return leafName;
    }

    public bool IsNodeVisible(string name)
    {
        var toggleXPath = $".//button[@aria-label={EscapeXPathText($"Toggle {name}")}]";
        var toggles = Root.FindElements(By.XPath(toggleXPath));
        if (toggles.Count > 0) return toggles[0].Displayed;

        var leafXPath = $".//*[contains(normalize-space(.), {EscapeXPathText(name)})]";
        var leaves = Root.FindElements(By.XPath(leafXPath));
        return leaves.Count > 0 && leaves[0].Displayed;
    }

    private void ExpandRec(string[] segments, int index)
    {
        if (index >= segments.Length - 1) return;

        var name = segments[index];
        var next = segments[index + 1];

        var container = FindNodeContainerByLabel(name);
        ExpandIfCollapsed(name, container);

        _wait.Until(_ => IsPathSegmentVisible(next));

        ExpandRec(segments, index + 1);
    }

    private void ExpandIfCollapsed(string nodeName, IWebElement container)
    {
        if (TryFindToggle(nodeName, container) is null) return;
        if (!IsGroupCollapsed(nodeName)) return;

        var toggleBy = By.CssSelector($"button[aria-label='Toggle {EscapeCssString(nodeName)}']");
        var toggle = _wait.Until(_ => Root.FindElement(toggleBy));

        ScrollIntoView(toggle);

        try
        {
            new Actions(_driver).MoveToElement(toggle).Click().Perform();
        }
        catch
        {
            DispatchClick(toggle);
        }

        _wait.Until(_ => !IsGroupCollapsed(nodeName));
    }

    private IWebElement FindNodeContainerByLabel(string label)
    {
        var folderXPath =
            $".//button[@aria-label={EscapeXPathText($"Toggle {label}")}]/ancestor::mat-nested-tree-node[1]";

        var foundFolders = Root.FindElements(By.XPath(folderXPath));
        if (foundFolders.Count > 0)
        {
            return foundFolders[0];
        }

        var leafXPath =
            $".//mat-nested-tree-node[not(.//button) and normalize-space()={EscapeXPathText(label)}]";

        return _wait.Until(_ => Root.FindElement(By.XPath(leafXPath)));
    }

    private static IWebElement? TryFindToggle(string nodeName, IWebElement container)
    {
        try
        {
            return container.FindElement(By.CssSelector($"button[aria-label='Toggle {EscapeCssString(nodeName)}']"));
        }
        catch (NoSuchElementException)
        {
            return null;
        }
    }

    private static bool HasInvisibleClass(IWebElement group)
    {
        var cls = group.GetAttribute("class") ?? "";
        return cls.Contains("example-tree-invisible", StringComparison.Ordinal);
    }

    private static string[] SplitUnixPath(string path)
        => path.Split('/', StringSplitOptions.RemoveEmptyEntries);

    private static string EscapeCssString(string s)
        => s.Replace("\\", "\\\\").Replace("'", "\\'");

    private static string EscapeXPathText(string text)
    {
        if (!text.Contains("'")) return $"'{text}'";
        if (!text.Contains("\"")) return $"\"{text}\"";
        return $"'{text.Replace("'", "")}'";
    }

    private void ScrollIntoView(IWebElement element)
    {
        ((IJavaScriptExecutor)_driver).ExecuteScript(
            "arguments[0].scrollIntoView({block: 'center', inline: 'nearest'});",
            element);
    }

    private bool IsGroupCollapsed(string folderName)
    {
        try
        {
            var group = FindGroupFresh(folderName);
            return HasInvisibleClass(group);
        }
        catch (NoSuchElementException)
        {
            return true;
        }
        catch (StaleElementReferenceException)
        {
            return true;
        }
    }

    private IWebElement FindGroupFresh(string folderName)
    {
        var groupBy = By.XPath(
            $".//button[@aria-label={EscapeXPathText($"Toggle {folderName}")}]" +
            $"/ancestor::mat-nested-tree-node[1]//div[@role='group']"
        );

        return Root.FindElement(groupBy);
    }

    private void DispatchClick(IWebElement element)
    {
        ((IJavaScriptExecutor)_driver).ExecuteScript(@"
            const el = arguments[0];
            el.dispatchEvent(new MouseEvent('mouseover', {bubbles:true}));
            el.dispatchEvent(new MouseEvent('mousemove', {bubbles:true}));
            el.dispatchEvent(new MouseEvent('mousedown', {bubbles:true}));
            el.dispatchEvent(new MouseEvent('mouseup', {bubbles:true}));
            el.dispatchEvent(new MouseEvent('click', {bubbles:true}));
        ", element);
    }

    private bool IsElementVisible(By by)
    {
        try
        {
            var el = Root.FindElement(by);
            return el.Displayed;
        }
        catch (NoSuchElementException)
        {
            return false;
        }
        catch (StaleElementReferenceException)
        {
            return false;
        }
    }

    private bool IsPathSegmentVisible(string name)
    {
        var toggleBy = By.XPath($".//button[@aria-label={EscapeXPathText($"Toggle {name}")}]");
        if (IsElementVisible(toggleBy)) return true;

        var leafBy = By.XPath($".//mat-nested-tree-node[not(.//button) and contains(normalize-space(.), {EscapeXPathText(name)})]");
        return IsElementVisible(leafBy);
    }
}