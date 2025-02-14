using System.Reflection;
using SimpleWebServer.Models;

namespace SimpleWebServer.Requests;

public class HandleGet
{
    private readonly string _body;

    public HandleGet( string body)
    {
        _body = body;
    }

    public string AddModel(List<Books> booksList) // list books hardcoded
    {
        string[] splitted = _body.Split("\n");
        bool marker = false;
        string result = "";
        int bodyLenth = _body.Length;
        string firstHalf = _body.Substring(0, _body.IndexOf("@foreach"));
        string secondHalf = _body.Substring(_body.IndexOf("@endforeach"), bodyLenth - _body.IndexOf("@endforeach")).Replace("@endforeach", "");
        
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

    public string Details(Books book)
    {
        string result = "";
        string[] items = _body.Split("\n");
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

    string GetPropert(string property, Books book)
    {
        object value = null;
        PropertyInfo prop = book.GetType().GetProperty(property);
        if (prop != null)
        {
            value = prop.GetValue(book);
        }
        return value.ToString();
    }

    string GetInside(string line)
    {
        int start = line.IndexOf("{{") + 2;
        int end = line.IndexOf("}}");
        return line.Substring(start, end - start);
    }
}