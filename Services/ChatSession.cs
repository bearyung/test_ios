using System.Net.Sockets;
using System.Text;
using NetCoreServer;

namespace test_ios.Services;

public class ChatSession : TcpSession
{
    public ChatSession(TcpServer server) : base(server) {}

    protected override void OnConnected()
    {
        Console.WriteLine($"Chat TCP session with Id {Id} connected!");

        // Send invite message
        string message = "Hello from TCP chat! Please send a message or '!' to disconnect the client!";
        SendAsync(message);
    }

    protected override void OnDisconnected()
    {
        Console.WriteLine($"Chat TCP session with Id {Id} disconnected!");
    }

    protected override void OnReceived(byte[] buffer, long offset, long size)
    {
        string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
        Console.WriteLine("Incoming: " + message);

        // Multicast message to all connected sessions
        Server.Multicast(message);
        
        // emit the message event
        //var eventAggregator = Locator.Current.GetService<IEventAggregator>();
        //eventAggregator?.Publish(new MessageEvent(){Message = message});

        // If the buffer starts with '!' the disconnect the current session
        if (message == "!")
            Disconnect();
    }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"Chat TCP session caught an error with code {error}");
    }
}