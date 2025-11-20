using System.Reactive;
using ReactiveUI;

namespace BehaviorsTestApplication.ViewModels;

public class NetworkViewModel : ViewModelBase
{
    private string? _url = "https://httpbin.org/get";
    private string? _responseContent;
    private int _responseStatusCode;
    private bool _isNetworkAvailable;

    public string? Url
    {
        get => _url;
        set => this.RaiseAndSetIfChanged(ref _url, value);
    }

    public string? ResponseContent
    {
        get => _responseContent;
        set => this.RaiseAndSetIfChanged(ref _responseContent, value);
    }

    public int ResponseStatusCode
    {
        get => _responseStatusCode;
        set => this.RaiseAndSetIfChanged(ref _responseStatusCode, value);
    }

    public bool IsNetworkAvailable
    {
        get => _isNetworkAvailable;
        set => this.RaiseAndSetIfChanged(ref _isNetworkAvailable, value);
    }

    public ReactiveCommand<bool, Unit> UpdateNetworkStatusCommand { get; }

    public NetworkViewModel()
    {
        UpdateNetworkStatusCommand = ReactiveCommand.Create<bool>(UpdateNetworkStatus);
    }

    private void UpdateNetworkStatus(bool isAvailable)
    {
        IsNetworkAvailable = isAvailable;
    }
}
