namespace SharpRedirect;

internal class Program
{
    public static void Main(string[] args)
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

        PortRedirector redirector = new PortRedirector(localPort, destinationHost, destinationPort);
        redirector.Start();

        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();

        redirector.Stop();
    }
}