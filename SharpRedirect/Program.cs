using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SharpRedirect
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: PortRedirector <localPort> <destinationHost> <destinationPort>");
                return;
            }

            if (!int.TryParse(args[0], out int localPort) || !int.TryParse(args[2], out int destinationPort))
            {
                Console.WriteLine("Error: Ports must be valid integers.");
                return;
            }

            string destinationHost = args[1];

            Console.WriteLine($"Redirecting traffic from localhost:{localPort} to {destinationHost}:{destinationPort}");

            var redirector = new PortRedirector(localPort, destinationHost, destinationPort);
            redirector.Start();

            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();

            redirector.Stop();
        }
    }

    class PortRedirector
    {
        private readonly int _localPort;
        private readonly string _destinationHost;
        private readonly int _destinationPort;
        private TcpListener _listener;
        private bool _isRunning;

        public PortRedirector(int localPort, string destinationHost, int destinationPort)
        {
            _localPort = localPort;
            _destinationHost = destinationHost;
            _destinationPort = destinationPort;
        }

        public void Start()
        {
            _listener = new TcpListener(IPAddress.Any, _localPort);
            _listener.Start();
            _isRunning = true;

            Console.WriteLine($"Listening on port {_localPort}...");
            Task.Run(() => AcceptClients());
        }

        public void Stop()
        {
            _isRunning = false;
            _listener?.Stop();
        }

        private async Task AcceptClients()
        {
            while (_isRunning)
            {
                try
                {
                    var client = await _listener.AcceptTcpClientAsync();
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
            using (sourceClient)
            {
                try
                {
                    var destinationClient = new TcpClient();
                    await destinationClient.ConnectAsync(_destinationHost, _destinationPort);
                    Console.WriteLine($"Connected to {_destinationHost}:{_destinationPort}");

                    using (destinationClient)
                    {
                        var sourceStream = sourceClient.GetStream();
                        var destinationStream = destinationClient.GetStream();

                        // Redirect traffic in both directions
                        var forwardTask = CopyStream(sourceStream, destinationStream);
                        var backwardTask = CopyStream(destinationStream, sourceStream);

                        await Task.WhenAll(forwardTask, backwardTask);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error handling client: {ex.Message}");
                }
            }
        }

        private async Task CopyStream(Stream input, Stream output)
        {
            try
            {
                byte[] buffer = new byte[8192];
                int bytesRead;
                while ((bytesRead = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await output.WriteAsync(buffer, 0, bytesRead);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error copying stream: {ex.Message}");
            }
        }
    }
}
