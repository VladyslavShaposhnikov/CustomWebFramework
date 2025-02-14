namespace SimpleWebServer.Requests;

public class HandlePost
{
    private readonly StreamReader _reader;

    public HandlePost(StreamReader reader)
    {
        _reader = reader;
    }

    public string GetPostData()
    {
        int length = 0;
        string line;
        
        while (!string.IsNullOrEmpty(line = _reader.ReadLine()))
        {
            if (line.StartsWith("Content-Length:"))
            {
                length = int.Parse(line.Split(" ")[1]);
            }
        }
        char[] buffer = new Char[length];
        _reader.Read(buffer, 0, length);
        string postData = new string(buffer);
        return postData;
    }

    public string RenderTemplate(string template, Dictionary<string, string> parameters)
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
    
}