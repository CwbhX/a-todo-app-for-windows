using System.Diagnostics;

namespace Linework.Infrastructure;

public interface IUrlOpener
{
    void Open(string url);
}

public sealed class UrlOpener : IUrlOpener
{
    public void Open(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new ArgumentException("Only http and https URLs can be opened.", nameof(url));
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = uri.ToString(),
            UseShellExecute = true
        });
    }
}
