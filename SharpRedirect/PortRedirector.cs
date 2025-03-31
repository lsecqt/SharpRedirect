using System.Net;
using System.Net.Sockets;

namespace SharpRedirect;

class PortRedirector
{
    private readonly int _localPort;
    private readonly string _destinationHost;
    private readonly int _destinationPort;
    private readonly TcpListener _listener;
    private bool _isRunning;

    public PortRedirector(int localPort, string destinationHost, int destinationPort)
    {
        _localPort = localPort;
        _destinationHost = destinationHost;
        _destinationPort = destinationPort;
        _listener = new TcpListener(IPAddress.Any, _localPort);
    }

    public void Start()
    {
        _listener.Start();
        _isRunning = true;
        Console.WriteLine($"Listening on port {_localPort}...");
        Task.Run(AcceptClients);
    }

    public void Stop()
    {
        _isRunning = false;
        _listener.Stop();
    }

    private async Task AcceptClients()
    {
        while (_isRunning)
        {
            try
            {
                using TcpClient client = await _listener.AcceptTcpClientAsync();
                Console.WriteLine("Client connected.");
                _ = HandleClient(client);
            }
            catch (Exception ex)
            {
                if (_isRunning)
                    Console.WriteLine($"Error accepting client: {ex.Message}");
            }
        }
    }

    private async Task HandleClient(TcpClient sourceClient)
    {
        try
        {
            using TcpClient destinationClient = new TcpClient();
            await destinationClient.ConnectAsync(_destinationHost, _destinationPort);
            Console.WriteLine($"Connected to {_destinationHost}:{_destinationPort}");

            Stream sourceStream = sourceClient.GetStream();
            Stream destinationStream = destinationClient.GetStream();

            // Redirect traffic in both directions
            Task forwardTask = sourceStream.CopyToAsync(destinationStream);
            Task backwardTask = destinationStream.CopyToAsync(sourceStream);

            await Task.WhenAll(forwardTask, backwardTask);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling client: {ex.Message}");
        }
    }
}