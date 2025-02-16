using SimpleWebServer.Models;

namespace SimpleWebServer.urls;

public class Urls
{
    private static Dictionary<string, (Type controllerType, string method)> _urls = new();
    
    public void RegisterPath(string url, Type controllerType, string method)
    {
        _urls[url] = (controllerType, method);
        Console.WriteLine($"Registered URL: {url}");
    }
    
    // to register path for existing in db model
    public static void RegisterPathForId(string url, Type controllerType, string method, int[] numbers) 
    {
        foreach (var number in numbers)
        {
            _urls[url+'/'+number] = (controllerType, method+'/'+number);
            Console.WriteLine($"Success registered path: {url}/{number}");
        }
    }
    
    public static void RegisterPathForId(string url, Type controllerType, string method, int id)
    {
        _urls[url+'/' + id] = (controllerType, method+'/'+id);
        Console.WriteLine($"Success registered path: {url}/{id}");
    }
    
    public (Type controllerType, string method) GetPath(string url)
    {
        if(_urls.TryGetValue(url, out var result))
        {
            return result;
        }
        return (null, null);
    }
    public string GenerateHeaders(string url, int len)
    {
        string res = "";
        bool valid = string.IsNullOrEmpty(url);
        
        if (!valid)
        {
             res = "HTTP/1.1 200 OK\r\n" +
                         "Content-Type: text/html\r\n" +
                         $"Content-Length: {len}\r\n" +
                         "Connection: close\r\n\r\n";
        }
        else
        {
            res = "HTTP/1.1 404 Not Found\r\n" +
                  "Content-Type: text/html\r\n" +
                  $"Content-Length: {len}\r\n" +
                  "Connection: close\r\n\r\n";
        }
        return res;
    }
    
    public string GetFilePath(string url, string controllerTemplate)
    {
        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        string rootDir = Directory.GetParent(basePath).Parent.Parent.Parent.ToString();
        rootDir = Path.Combine(rootDir, "templates");
        string temp = url.Substring(1) + ".html";
        return File.ReadAllText(Path.Combine(rootDir, controllerTemplate.Replace("Controller", ""), temp));
    }
}