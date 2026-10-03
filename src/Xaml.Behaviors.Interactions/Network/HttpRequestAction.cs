using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
#if UNO
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
#else
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Network;
#else
namespace Avalonia.Xaml.Interactions.Network;
#endif

/// <summary>
/// An action that performs an HTTP request.
/// </summary>
public partial class HttpRequestAction : StyledElementAction
{
    private static readonly HttpClient _client = new HttpClient();

    /// <summary>
    /// Gets or sets the URL.
    /// </summary>
    [StyledProperty]
    public partial string? Url { get; set; }

    /// <summary>
    /// Gets or sets the HTTP method.
    /// </summary>
    [StyledProperty(DefaultValue = "GET")]
    public partial string? Method { get; set; }

    /// <summary>
    /// Gets or sets the content to send.
    /// </summary>
    [StyledProperty]
    public partial string? Content { get; set; }

    /// <summary>
    /// Gets or sets the content type.
    /// </summary>
    [StyledProperty(DefaultValue = "application/json")]
    public partial string? ContentType { get; set; }

    /// <summary>
    /// Gets or sets the response content.
    /// </summary>
    [StyledProperty]
    public partial string? ResponseContent { get; set; }

    /// <summary>
    /// Gets or sets the response status code.
    /// </summary>
    [StyledProperty]
    public partial int ResponseStatusCode { get; set; }

    /// <inheritdoc />
    public override object? Execute(object? sender, object? parameter)
    {
        var url = Url;
        if (string.IsNullOrEmpty(url))
        {
            return false;
        }

        _ = MakeRequest(url!);
        return true;
    }

    private async Task MakeRequest(string url)
    {
        try
        {
            var method = new HttpMethod(Method?.ToUpperInvariant() ?? "GET");
            var request = new HttpRequestMessage(method, url);

            if (!string.IsNullOrEmpty(Content))
            {
                request.Content = new StringContent(Content, Encoding.UTF8, ContentType ?? "application/json");
            }

            var response = await _client.SendAsync(request);
            var responseString = await response.Content.ReadAsStringAsync();

            Dispatcher.UIThread.Post(() =>
            {
                ResponseStatusCode = (int)response.StatusCode;
                ResponseContent = responseString;
            });
        }
        catch (Exception ex)
        {
            Dispatcher.UIThread.Post(() =>
            {
                ResponseContent = ex.Message;
                ResponseStatusCode = 0;
            });
        }
    }
}
