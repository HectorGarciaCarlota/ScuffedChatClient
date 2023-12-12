using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

public class ChatClient
{
    private readonly string _host;
    private readonly int _port;
    private TcpClient _client;
    private StreamReader _reader;
    private StreamWriter _writer;

    public ChatClient(string host, int port)
    {
        _host = host;
        _port = port;
    }

    public async Task ConnectToServerAsync()
    {
        _client = new TcpClient();
        await _client.ConnectAsync(_host, _port);
        var networkStream = _client.GetStream();
        _reader = new StreamReader(networkStream);
        _writer = new StreamWriter(networkStream) { AutoFlush = true };
        Console.WriteLine("Connected to chat server.");
        ReceiveMessages();
    }

    private async void ReceiveMessages()
    {
        try
        {
            while (_client.Connected)
            {
                var message = await _reader.ReadLineAsync();
                if (message == null) break; // Server has disconnected
                Console.WriteLine(message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Disconnected from chat server.");
            Console.WriteLine(ex.Message);
        }
    }

    public async Task SendMessageAsync(string message)
    {
        await _writer.WriteLineAsync(message);
    }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        var client = new ChatClient("127.0.0.1", 8000); // Use the server's IP and port
        await client.ConnectToServerAsync();

        Console.WriteLine("Enter messages to send or type 'exit' to quit:");

        string message;

        while ((message = Console.ReadLine()) != "exit" && client != null)
        {
            Console.Write(" - me");
            Console.WriteLine();
            await client.SendMessageAsync(message);
        }
    }
}
