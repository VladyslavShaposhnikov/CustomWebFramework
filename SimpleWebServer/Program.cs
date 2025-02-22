using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using SimpleWebServer.Controllers;
using SimpleWebServer.urls;

class Program
{
    static void Main()
    {
        Urls urls = new Urls();
        
        urls.InitializeUrls();
        
        int port = 8080;
        TcpListener server = new TcpListener(IPAddress.Any, port);
        server.Start();
        Console.WriteLine($"Server started on http://localhost:{port}");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient(); // Wait for a client connection
            NetworkStream stream = client.GetStream();

            // Read the request
            StreamReader reader = new StreamReader(stream);
            string requestLine = reader.ReadLine();
            if (requestLine == null) continue;

            Console.WriteLine($"Request: {requestLine}");
            
            string[] splittedRequest = requestLine.Split(' ');
            
            string method = splittedRequest[0];
            
            string url = splittedRequest[1];

            Console.WriteLine($"Method: {method}");
            Console.WriteLine($"Url: {url}");
            
            string responseBody = "";
            string responseHeader = "";

            var (controllerType, actionName) = urls.GetPath(url);
            string[] splitted = actionName.Split('/');
            
            if (controllerType != null)
            {
                object controllerInstance = Activator.CreateInstance(controllerType);
                MethodInfo actionMethod;
                if (splitted.Length == 2)
                {
                    actionMethod = controllerType.GetMethod(splitted[0]);
                }
                else
                {
                    actionMethod = controllerType.GetMethod(actionName);
                }

                if (actionMethod != null)
                {
                    if (splitted.Length == 2)
                    {
                        object[] paramValues = new object[1];
                        paramValues[0] = splitted[1];
                        responseBody = (string)actionMethod.Invoke(controllerInstance, paramValues);
                    }
                    else if (method.ToLower() == "post")
                    {
                        object[] paramValues = new object[1];
                        int length = 0;
                        string line;
                        while (!string.IsNullOrEmpty(line = reader.ReadLine()))
                        {
                            if (line.StartsWith("Content-Length:"))
                            {
                                length = int.Parse(line.Split(" ")[1]);
                            }
                        }
                        char[] buffer = new Char[length];
                        reader.Read(buffer, 0, length);
                        string responseString = new string(buffer);
                        paramValues[0] = responseString;
                        responseBody = (string)actionMethod.Invoke(controllerInstance, paramValues);
                    }
                    else
                    {
                        responseBody = (string)actionMethod.Invoke(controllerInstance, null);
                    }
                    responseHeader = urls.GenerateHeaders(url, responseBody.Length);
                }
                else
                {
                    responseBody = "No controller or action found";
                    responseHeader = urls.GenerateHeaders(null, responseBody.Length);
                }
            }
            else
            {
                responseBody = "No controller or action found";
                responseHeader = urls.GenerateHeaders(null, responseBody.Length);
            }

            // Send the response
            byte[] headerBytes = Encoding.UTF8.GetBytes(responseHeader);
            byte[] bodyBytes = Encoding.UTF8.GetBytes(responseBody);

            stream.Write(headerBytes, 0, headerBytes.Length);
            stream.Write(bodyBytes, 0, bodyBytes.Length);

            // Close connection
            client.Close();
        }
    }
}
