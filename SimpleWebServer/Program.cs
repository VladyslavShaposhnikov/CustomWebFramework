using System.Net;
using System.Net.Sockets;
using SimpleWebServer.Requests;
using SimpleWebServer.urls;

class Program
{
    static async Task Main()
    {
        Urls urls = new Urls();
        
        urls.InitializeUrls();
        
        int port = 8080;
        TcpListener server = new TcpListener(IPAddress.Any, port);
        server.Start();
        Console.WriteLine($"Server started on http://localhost:{port}");

        while (true)
        {
            TcpClient client = await server.AcceptTcpClientAsync(); // Wait for a client connection
            _ = Client.HandleClientAsync(client, urls);
        }
    }
}
