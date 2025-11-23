# HttpRequestAction

`HttpRequestAction` sends an HTTP request when invoked and surfaces the response through bindable properties.

## Properties

| Property | Type | Description |
| --- | --- | --- |
| `Url` | `string` | Target URL to request. Required. |
| `Method` | `string` | HTTP method to use (GET, POST, PUT, DELETE, etc.). Defaults to `GET`. |
| `Content` | `string` | Optional request body used for methods that support a payload. |
| `ContentType` | `string` | Content-Type header applied when sending `Content`. Defaults to `application/json`. |
| `ResponseContent` | `string` | Response body as a string. Bind with `Mode=OneWayToSource` to capture the value. |
| `ResponseStatusCode` | `int` | Numeric status code from the response. Set to `0` when the request fails. |

## Usage

Trigger an HTTP GET when a button is clicked and bind the response back to your ViewModel:

```xml
<Button Content="Fetch Data">
    <Interaction.Behaviors>
        <EventTriggerBehavior EventName="Click">
            <HttpRequestAction Url="https://api.example.com/data"
                               Method="GET"
                               ResponseContent="{Binding ResponseContent, Mode=OneWayToSource}"
                               ResponseStatusCode="{Binding ResponseStatusCode, Mode=OneWayToSource}" />
        </EventTriggerBehavior>
    </Interaction.Behaviors>
</Button>
```

### Sending content

Send JSON content with a POST request and capture the status code:

```xml
<EventTriggerBehavior EventName="Click">
    <HttpRequestAction Url="https://api.example.com/items"
                       Method="POST"
                       Content="{Binding JsonPayload}"
                       ContentType="application/json"
                       ResponseStatusCode="{Binding Status, Mode=OneWayToSource}" />
</EventTriggerBehavior>
```

> The request runs asynchronously; responses and errors are posted back to the UI thread. Check `ResponseStatusCode` and `ResponseContent` for the result or failure details.
