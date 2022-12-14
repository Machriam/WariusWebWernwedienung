using PeptidesTools.Shared.Services;

namespace WariusWebWernwedienung.Shared;

public interface IRestInteropFactory
{
    IRestInterop CreateRestInterop(string baseRoute);
}

public class RestInteropFactory : IRestInteropFactory
{
    private readonly IErrorLogger _errorLogger;
    private readonly HttpClient _httpClient;

    public RestInteropFactory(IErrorLogger errorLogger, HttpClient httpClient)
    {
        _errorLogger = errorLogger;
        _httpClient = httpClient;
    }

    public IRestInterop CreateRestInterop(string baseRoute)
    {
        return new RestInterop(_httpClient, _errorLogger, baseRoute);
    }
}