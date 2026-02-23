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
    public void Expand_Path_And_Take_Tree_Screenshot_And_Verify_Checksum()
    {
        // Przygotowanie - ściezka
        const string path = "/home/user/projects/README.md";

        _page.Open(BaseUrl);

        // Assert: tytuł strony z index.html
        Assert.That(_page.Title, Is.EqualTo("Tree with nested nodes (childrenAccessor)"));

        // Assert: drzewo jest widoczne
        Assert.That(_page.FileTree.IsVisible(), Is.True);

        // Rozwiń ścieżkę (rekurencja, bez pętli)
        var fileNameFromUi = _page.FileTree.ExpandPath(path);

        // Assert: po drodze foldery są rozwinięte (walidacja stanu UI)
        Assert.That(_page.FileTree.AreChildrenVisible("home"), Is.True);
        Assert.That(_page.FileTree.AreChildrenVisible("user"), Is.True);
        Assert.That(_page.FileTree.AreChildrenVisible("projects"), Is.True);

        // Screenshot drzewa po rozwinięciu
        Screenshots.SaveElementPng(_driver, _page.FileTree.Root, "tree-after-expand");

        // checksum z nazwy pliku odczytanej z elementu
        var checksum = Hashing.Sha256Hex(fileNameFromUi);

        Assert.That(fileNameFromUi, Is.EqualTo("README.md"));
        Assert.That(checksum, Has.Length.EqualTo(64));
        Assert.That(checksum, Is.EqualTo("b335630551682c19a781afebcf4d07bf978fb1f8ac04c6bf87428ed5106870f5"));
    }
}