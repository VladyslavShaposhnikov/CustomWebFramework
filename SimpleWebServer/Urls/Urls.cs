using SimpleWebServer.Models;

namespace SimpleWebServer.urls;

public class Urls
{
    private static Dictionary<string, (Type controllerType, string method)> _urls = new();
    
    public void RegisterPath(string url, Type controllerType, string method)
    {
        _urls[url] = (controllerType, method);
    }
    
    public static void RegisterPathForDetails(string url, Type controllerType, string method, List<Books> books)
    {
        foreach (var book in books)
        {
            _urls[url+'/'+book.ID] = (controllerType, method+'/'+book.ID);
            Console.WriteLine($"Success registered path: {url}/{book.ID}");
        }
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
    
    public string getPath(string url)
    {
        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        string rootDir = Directory.GetParent(basePath).Parent.Parent.Parent.ToString();
        string temp = url.Substring(1) + ".html";
        return File.ReadAllText(Path.Combine(rootDir, "templates", temp));
    }

    public void getUrl()
    {
        foreach (var key in _urls.Keys)
        {
            Console.Write(key + "---next---");
        }
    }
}