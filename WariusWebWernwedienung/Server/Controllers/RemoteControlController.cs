using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
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
