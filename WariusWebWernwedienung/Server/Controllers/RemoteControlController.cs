using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Playwright;
using WariusWebWernwedienung.Shared;

namespace WariusWebWernwedienung.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RemoteControlController : ControllerBase
{
    private const string ScriptFolderName = "ScriptFolder";
    private const string ParameterReplacementsName = "ParameterReplacements";
    private readonly string _scriptFolder;
    private readonly Dictionary<string, string> _parameterReplacements = new();
    private const string DebuggingAddress = "http://localhost:9333";

    public RemoteControlController(IConfiguration configuration)
    {
        _scriptFolder = configuration.GetConnectionString(ScriptFolderName)
            ?? throw new Exception($"Connection {ScriptFolderName} must be defined in appsettings.json");
        foreach (var (key, value) in configuration
            .GetSection(ParameterReplacementsName).GetChildren()
            .Select(c => (c.Key, c.Value))) _parameterReplacements.Add(key, value ?? "");
    }

    [HttpGet]
    public IEnumerable<string> GetExecutableScripts()
    {
        return Directory.GetFiles(_scriptFolder)
            .Select(f => Path.GetFileName(f))
            .Where(p => p.EndsWith(".bat") || p.EndsWith(".exe"));
    }

    [HttpPost("navigate")]
    public async Task Navigate([FromBody] HtmlLink link)
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.ConnectOverCDPAsync(DebuggingAddress);
        var context = browser.Contexts[0];
        var page = await context.NewPageAsync();
        while (context.Pages.Count > 1)
        {
            var i = 0;
            if (context.Pages[i] == page) i++;
            await context.Pages[i].CloseAsync();
        }
        await page.BringToFrontAsync();
        await page.GotoAsync(link.Link);
    }

    [HttpGet("links")]
    public async Task<IEnumerable<HtmlLink>> GetLinks()
    {
        try
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.ConnectOverCDPAsync(DebuggingAddress);
            var context = browser.Contexts[0];
            var page = context.Pages[0];
            await page.BringToFrontAsync();
            var baseUri = new Uri(page.Url).GetLeftPart(UriPartial.Authority);
            var result = new List<HtmlLink>();
            var links = await GetLinksBySite(page.Url, page);
            return await ExtractLinkUrls(baseUri, links);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new List<HtmlLink>();
        }
    }

    private static async Task<(List<IElementHandle> Elements, Func<string, bool> HrefFilter)> GetLinksBySite(string url, IPage page)
    {
        var locator = page.GetByRole(AriaRole.Link);
        if (url.Contains("youtube"))
        {
            Func<string, bool> filter = s => s.Contains("watch");
            return ((await page.Locator(".yt-simple-endpoint",
                        new() { Has = page.Locator("[id=video-title]") }).ElementHandlesAsync()).ToList(), filter);
        }
        else return ((await locator.ElementHandlesAsync()).ToList(), s => !string.IsNullOrEmpty(s));
    }

    private static async Task<IEnumerable<HtmlLink>> ExtractLinkUrls(string baseUri, (List<IElementHandle> Handles, Func<string, bool> HrefFilter) handles)
    {
        var result = new List<HtmlLink>();
        foreach (var element in handles.Handles)
        {
            string href;
            try
            {
                href = await element.GetAttributeAsync("href") ?? "";
                if (!handles.HrefFilter(href)) continue;
            }
            catch (Exception)
            {
                continue;
            }
            var text = await element.InnerTextAsync();
            result.Add(new HtmlLink()
            {
                Link = baseUri.Trim('/') + "/" + href.Trim('/'),
                Name = text
            });
        }
        return result;
    }

    [HttpPost]
    public bool Post([FromBody] RemoteControlParameter parameter)
    {
        foreach (var key in _parameterReplacements.Keys)
            parameter.Parameter = parameter.Parameter.Replace(key, _parameterReplacements[key]);
        var process = new Process
        {
            StartInfo = new ProcessStartInfo(_scriptFolder + "\\" + Path.GetFileName(parameter.FileName),
                                             "\"" + parameter.Parameter + "\"")
        };
#if DEBUG
        Console.WriteLine(process.StartInfo.FileName);
        Console.WriteLine(process.StartInfo.Arguments);
#endif
        return process.Start();
    }
}
