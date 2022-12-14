namespace PeptidesTools.Shared.Services;

public interface IErrorLogger
{
    Task LogError(string error, bool throwException = true);
}
