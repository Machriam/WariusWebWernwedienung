using Microsoft.JSInterop;
using PeptidesTools.Shared.Services;

namespace WariusWebWernwedienung.Client;

public interface IJsInterop
{
    Task Prompt(string message);
}

public class JsInterop : IJsInterop, IErrorLogger
{
    private readonly IJSRuntime _jsRuntime;

    public JsInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task LogError(string error, bool throwException = true)
    {
        error = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + Environment.NewLine + error;
        await Prompt(error);
        if (throwException) throw new Exception(error);
    }

    public async Task Prompt(string message)
    {
        await _jsRuntime.InvokeVoidAsync("alert", message.Replace("\\n", "\n"));
    }
}
