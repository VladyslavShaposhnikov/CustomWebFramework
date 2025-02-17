using System.Reflection;
using SimpleWebServer.Models;
using SimpleWebServer.urls;

namespace SimpleWebServer.Requests;

public class HandleGet
{
    public static string SimpleGet(string filename, string directory)
    {
        Urls urls = new();
        return urls.GetFilePath(filename, directory);
    }

    public static string UseForeachWithModel<T>(List<T> booksList, string filename, string directory)
    {
        Urls urls = new();
        string body = urls.GetFilePath(filename, directory);
        string[] splitted = body.Split("\n");
        bool marker = false;
        string result = "";
        int bodyLenth = body.Length;
        string firstHalf = body.Substring(0, body.IndexOf("@foreach"));
        string secondHalf = body.Substring(body.IndexOf("@endforeach"), bodyLenth - body.IndexOf("@endforeach")).Replace("@endforeach", "");
        
        foreach (var line in splitted)
        {
            if (line.Contains("@foreach"))
            {
                marker = true;
                string cut = line.Replace("@foreach(", "");
                cut = cut.Replace(")", "\n");
                result = cut.TrimStart();
                continue;
            }
            else if (line.Contains("@endforeach"))
            {
                marker = false;
                continue;
            }

            if (marker)
            {
                result += line.Trim() + "\n";
            }
        }
        
        string[] items = result.Split("\n");
        
        foreach (var book in booksList)
        {
            foreach (var line in items)
            {
                if (line.Contains("{{") && line.Contains("}}"))
                {
                    string r = GetInside(line);
                    firstHalf += line.Replace("{{" + r + "}}", GetPropert(r.Substring(r.IndexOf('.') + 1), book));
                    firstHalf += "\n";
                    continue;
                }
                if(!line.StartsWith('<'))
                    continue;
                firstHalf += line + "\n";
            }
        }
        firstHalf += secondHalf;

        //Console.WriteLine(firstHalf);
        
        return firstHalf;
    }

    public static string Details(Books book, string filename, string directory)
    {
        Urls urls = new();
        string body = urls.GetFilePath(filename, directory);
        string result = "";
        string[] items = body.Split("\n");
        foreach (var line in items)
        {
            if (line.Contains("{{") && line.Contains("}}"))
            {
                string r = GetInside(line);
                result += line.Replace("{{" + r + "}}", GetPropert(r.Substring(r.IndexOf('.') + 1), book));
                result += "\n";
                continue;
            }
            result += line + "\n";
        }

        Console.WriteLine(result);
        return result;
    }

    static string GetPropert<T>(string property, T book)
    {
        object value = null;
        PropertyInfo prop = book.GetType().GetProperty(property);
        if (prop != null)
        {
            value = prop.GetValue(book);
        }
        return value.ToString();
    }

    static string GetInside(string line)
    {
        int start = line.IndexOf("{{") + 2;
        int end = line.IndexOf("}}");
        return line.Substring(start, end - start);
    }
}