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
        int[] numbers = MainController.bookList.Select(id => id.ID).ToArray();
        Urls urls = new Urls();
        urls.RegisterPath("/index", typeof(MainController), "Index");
        urls.RegisterPath("/", typeof(MainController), "Index");
        urls.RegisterPath("/add", typeof(MainController), "Add");
        urls.RegisterPath("/addbook", typeof(MainController), "AddBook");
        urls.RegisterPath("/editbook", typeof(MainController), "EditBook");
        Urls.RegisterPathForId("/details", typeof(MainController), "Details", numbers);
        Urls.RegisterPathForId("/delete", typeof(MainController), "Delete", numbers);
        Urls.RegisterPathForId("/edit", typeof(MainController), "Edit", numbers);
        
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
            string[] splitted = new[] { "" };
            splitted = actionName.Split('/');
            
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
                        paramValues[0] = reader;
                        responseBody = (string)actionMethod.Invoke(controllerInstance, paramValues);
                    }
                    else
                    {
                        Console.WriteLine($"Method: {actionMethod} -----------------");
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
