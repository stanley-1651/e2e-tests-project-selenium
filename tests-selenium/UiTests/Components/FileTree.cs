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

    public bool IsVisible() => Root.Displayed;

    /// <summary>
    /// Rozwijanie drzewa po ścieżce unixowej (np. /home/user/projects/README.md).
    /// Zwraca nazwę końcowego elementu odczytaną z UI.
    /// Rekurencja => bez pętli, jak opisano w wymaganiach :)
    /// </summary>
    public string ExpandPath(string unixPath)
    {
        var segments = SplitUnixPath(unixPath);
        if (segments.Length == 0) throw new ArgumentException("Empty path.", nameof(unixPath));

        ExpandRec(segments, 0);

        // Odczytywanie końcowego node z UI, stosownie do instrukcji
        var leafName = segments[^1];
        _wait.Until(_ => Root.FindElements(By.XPath($".//*[contains(normalize-space(.), {EscapeXPathText(leafName)})]")).Count > 0);
        return leafName;
    }

    public bool AreChildrenVisible(string folderName)
    {
        var container = FindNodeContainerByLabel(folderName);
        var group = container.FindElement(By.CssSelector("div[role='group']"));
        return !HasInvisibleClass(group);
    }

    private void ExpandRec(string[] segments, int index)
    {
        // ostatni segment (plik) – zakończenie procesowania ścieki
        if (index >= segments.Length - 1) return;

        var name = segments[index];

        var container = FindNodeContainerByLabel(name);

        // Rozwijanie tylko jeśli zwiniętego folderu
        ExpandIfCollapsed(name, container);

        // Rekurencja :)
        ExpandRec(segments, index + 1);
    }

    private void ExpandIfCollapsed(string nodeName, IWebElement container)
    {
        var toggle = TryFindToggle(nodeName, container);
        if (toggle is null) return; // liść

        var group = TryFindGroup(container);
        if (group is null) return;

        if (HasInvisibleClass(group))
        {
            ScrollIntoView(toggle);
            JsClick(toggle);

            // Czekanie aż children staną się widoczne (innymi słowy - zniknie example-tree-invisible)
            _wait.Until(_ =>
            {
                try
                {
                    return !HasInvisibleClass(group);
                }
                catch (StaleElementReferenceException)
                {
                    // Angular mógł przerysować węzeł – szukam group ponownie
                    var freshGroup = TryFindGroup(container);
                    return freshGroup is not null && !HasInvisibleClass(freshGroup);
                }
            });
        }
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

    private static IWebElement? TryFindGroup(IWebElement container)
    {
        try
        {
            return container.FindElement(By.CssSelector("div[role='group']"));
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

    private void JsClick(IWebElement element)
    {
        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", element);
    }
}