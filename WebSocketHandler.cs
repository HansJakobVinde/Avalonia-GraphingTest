using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Websocket.Client;

public class DataMessage
{
    public long Timestamp { get; set; }
    public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
}

public class WebSocketHandler
{
    private readonly Uri _uri;
    private readonly WebsocketClient _client;

    public event Action<DataMessage>? OnDataReceived;

    public WebSocketHandler(string url)
    {
        _uri = new Uri(url);
        _client = new WebsocketClient(_uri);
        _client.MessageReceived.Subscribe(OnMessageReceived);
    }

    public async Task Start()
    {
        await _client.Start();
        Console.WriteLine("WebSocket connection started.");
    }

    private void OnMessageReceived(ResponseMessage message)
    {
        if (!string.IsNullOrEmpty(message.Text))
        {
            try
            {
                var dataMessage = JsonSerializer.Deserialize<DataMessage>(message.Text, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (dataMessage != null)
                {
                    OnDataReceived?.Invoke(dataMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to parse WebSocket message: {ex.Message}");
            }
        }
    }

    public void Stop()
    {
        _client.Stop(WebSocketCloseStatus.NormalClosure, "Client disconnected");
        Console.WriteLine("WebSocket connection closed.");
    }
}
