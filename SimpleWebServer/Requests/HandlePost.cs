using System.Reflection;
using SimpleWebServer.urls;

namespace SimpleWebServer.Requests;

public class HandlePost
{
    public static Dictionary<string, string> GetPostData(string responseString)
    {
        string decodedData = System.Web.HttpUtility.UrlDecode(responseString);
        
        string[] split = decodedData.Split("&");

        Dictionary<string, string> postDict = new Dictionary<string, string>();

        foreach (string s in split)
        {
            int index = s.IndexOf('=');
            Console.WriteLine($"Adding key: {s.Substring(0, index)}, value: {s.Substring(index + 1)}");
            postDict.Add(s.Substring(0, index), s.Substring(index + 1));
        }
        
        return postDict;
    }

    public static string RenderTemplate(string template, Dictionary<string, string> parameters)
    {
        if (parameters == null || parameters.Count == 0)
        {
            return template;
        }

        foreach (var key in parameters.Keys)
        {
            template = template.Replace("{{" + key + "}}", parameters[key]);
        }
        
        return template;
    }

    // this method register path where url is method name with 1 parameter named "id"
    public static void CreatePathById(Type controllerType, int id)
    {
        List<string> methods = new List<string>();
        foreach (MethodInfo item in controllerType.GetMethods())
        {
            string m = item.Name;
            string parameters = string.Join(", ", item.GetParameters()
                .Select(p => $"{p.ParameterType.Name} {p.Name}"));
            if (parameters.ToLower().Contains(" id") && !parameters.Contains(','))
            {
                methods.Add(m);
            }
        }

        foreach (var url in methods)
        {
            Urls.RegisterPathForId('/'+url.ToLower(), controllerType, url, id);
        }
    }
}