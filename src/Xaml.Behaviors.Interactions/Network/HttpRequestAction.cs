using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Network;

/// <summary>
/// An action that performs an HTTP request.
/// </summary>
public class HttpRequestAction : StyledElementAction
{
    private static readonly HttpClient _client = new HttpClient();

    /// <summary>
    /// Identifies the <seealso cref="Url"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> UrlProperty =
        AvaloniaProperty.Register<HttpRequestAction, string?>(nameof(Url));

    /// <summary>
    /// Identifies the <seealso cref="Method"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> MethodProperty =
        AvaloniaProperty.Register<HttpRequestAction, string?>(nameof(Method), "GET");

    /// <summary>
    /// Identifies the <seealso cref="Content"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> ContentProperty =
        AvaloniaProperty.Register<HttpRequestAction, string?>(nameof(Content));

    /// <summary>
    /// Identifies the <seealso cref="ContentType"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> ContentTypeProperty =
        AvaloniaProperty.Register<HttpRequestAction, string?>(nameof(ContentType), "application/json");

    /// <summary>
    /// Identifies the <seealso cref="ResponseContent"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> ResponseContentProperty =
        AvaloniaProperty.Register<HttpRequestAction, string?>(nameof(ResponseContent));

    /// <summary>
    /// Identifies the <seealso cref="ResponseStatusCode"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<int> ResponseStatusCodeProperty =
        AvaloniaProperty.Register<HttpRequestAction, int>(nameof(ResponseStatusCode));

    /// <summary>
    /// Gets or sets the URL.
    /// </summary>
    public string? Url
    {
        get => GetValue(UrlProperty);
        set => SetValue(UrlProperty, value);
    }

    /// <summary>
    /// Gets or sets the HTTP method.
    /// </summary>
    public string? Method
    {
        get => GetValue(MethodProperty);
        set => SetValue(MethodProperty, value);
    }

    /// <summary>
    /// Gets or sets the content to send.
    /// </summary>
    public string? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the content type.
    /// </summary>
    public string? ContentType
    {
        get => GetValue(ContentTypeProperty);
        set => SetValue(ContentTypeProperty, value);
    }

    /// <summary>
    /// Gets or sets the response content.
    /// </summary>
    public string? ResponseContent
    {
        get => GetValue(ResponseContentProperty);
        set => SetValue(ResponseContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the response status code.
    /// </summary>
    public int ResponseStatusCode
    {
        get => GetValue(ResponseStatusCodeProperty);
        set => SetValue(ResponseStatusCodeProperty, value);
    }

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
