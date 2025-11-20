# Network Interactions

The `Avalonia.Xaml.Interactions.Network` namespace contains actions and triggers for network operations.

## Actions

### HttpRequestAction

Performs an HTTP request.

```xml
<HttpRequestAction Url="https://api.example.com/data" 
                   Method="GET" 
                   ResponseContent="{Binding ResponseContent, Mode=OneWayToSource}"
                   ResponseStatusCode="{Binding ResponseStatusCode, Mode=OneWayToSource}" />
```

Properties:
- `Url`: The URL to request.
- `Method`: The HTTP method (GET, POST, PUT, DELETE, etc.). Default is GET.
- `Content`: The content to send (for POST/PUT).
- `ContentType`: The content type header. Default is "application/json".
- `ResponseContent`: The response body as string (OneWayToSource).
- `ResponseStatusCode`: The response status code (OneWayToSource).

## Triggers

### NetworkInformationTrigger

Listens to network availability changes.

```xml
<NetworkInformationTrigger>
    <InvokeCommandAction Command="{Binding UpdateNetworkStatusCommand}" PassEventArgsToCommand="True" />
</NetworkInformationTrigger>
```
