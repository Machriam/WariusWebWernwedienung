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
    private readonly IConfiguration _configuration;
    private readonly string _scriptFolder;

    public RemoteControlController(IConfiguration configuration)
    {
        _configuration = configuration;
        _scriptFolder = _configuration.GetConnectionString(ScriptFolderName)
            ?? throw new Exception($"Connection {ScriptFolderName} must be defined in appsettings.json");
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
        await using var browser = await playwright.Chromium.ConnectOverCDPAsync("http://localhost:9222");
        var context = browser.Contexts[0];
        var currentPage = context.Pages[0];
        var page = await context.NewPageAsync();
        await currentPage.CloseAsync();
        await page.BringToFrontAsync();
        await page.GotoAsync(link.Link);
    }

    [HttpGet("links")]
    public async Task<IEnumerable<HtmlLink>> GetLinks()
    {
        try
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.ConnectOverCDPAsync("http://localhost:9222");
            var context = browser.Contexts[0];
            var page = context.Pages[0];
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
        var process = new Process
        {
            StartInfo = new ProcessStartInfo(_scriptFolder + "\\" + Path.GetFileName(parameter.FileName),
                                             "\"" + parameter.Parameter + "\"")
        };
        return process.Start();
    }
}
