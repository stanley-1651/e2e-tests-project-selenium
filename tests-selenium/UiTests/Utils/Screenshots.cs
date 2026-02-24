using OpenQA.Selenium;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace UiTests.Utils;

public static class Screenshots
{
    public static string SaveElementPng(IWebDriver driver, IWebElement element, string fileNameNoExt)
    {
        Directory.CreateDirectory(ArtifactsDir());

        var path = Path.Combine(ArtifactsDir(), $"{Sanitize(fileNameNoExt)}.png");

        var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
        using var img = Image.Load(screenshot.AsByteArray);

        var rect = new Rectangle(
            x: Math.Max(element.Location.X, 0),
            y: Math.Max(element.Location.Y, 0),
            width: Math.Min(element.Size.Width, img.Width - element.Location.X),
            height: Math.Min(element.Size.Height, img.Height - element.Location.Y)
        );

        img.Mutate(x => x.Crop(rect));
        img.Save(path);

        TestContext.AddTestAttachment(path, $"Element screenshot: {fileNameNoExt}");
        return path;
    }

    private static string ArtifactsDir()
        => Path.Combine(TestContext.CurrentContext.WorkDirectory, "artifacts");

    private static string Sanitize(string name)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');
        return name;
    }
}