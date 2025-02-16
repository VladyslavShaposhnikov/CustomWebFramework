using System.Collections;
using System.Reflection;
using SimpleWebServer.Models;
using SimpleWebServer.Requests;
using SimpleWebServer.urls;

namespace SimpleWebServer.Controllers;



public class MainController
{
    public static List<Books> bookList = new List<Books>
    {
        new Books { ID = 1, Title = "The Catcher in the Rye", Author = "J.D. Salinger" },
        new Books { ID = 2, Title = "1984", Author = "George Orwell" },
        new Books { ID = 3, Title = "To Kill a Mockingbird", Author = "Harper Lee" },
        new Books { ID = 4, Title = "Harry Potter", Author = "J.K. Rowling" },
        new Books { ID = 5, Title = "Forest Gamp", Author = "I dont know" },
    };
    
    Urls _urls = new();
    string _controllerName;

    public MainController()
    {
        _controllerName = this.GetType().Name;
    }

    public string Index()
    {
        Console.WriteLine($"Controller {_controllerName} is running.");
        string methodName = MethodBase.GetCurrentMethod().Name;
        string responseBody = _urls.GetFilePath($"/{methodName.ToLower()}", _controllerName);
        HandleGet hg = new HandleGet(responseBody);
        return hg.AddModel(bookList);
    }

    public string Details(string id)
    {
        string methodName = MethodBase.GetCurrentMethod().Name;
        string responseBody = _urls.GetFilePath($"/{methodName.ToLower()}", _controllerName);
        HandleGet hg = new HandleGet(responseBody);
        Books book = bookList.FirstOrDefault(x => x.ID == int.Parse(id));
        return hg.Details(book);
    }

    public string Add()
    {
        string methodName = MethodBase.GetCurrentMethod().Name;
        string responseBody = _urls.GetFilePath($"/{methodName.ToLower()}", _controllerName);
        return responseBody;
    }
    
    public string AddBook(StreamReader reader)
    {
        Dictionary<string, string> param = new Dictionary<string, string>();
        string methodName = MethodBase.GetCurrentMethod().Name;
        string responseBody = _urls.GetFilePath($"/{methodName.ToLower()}", _controllerName);
        HandlePost hp = new HandlePost(reader);
        string[] postData = hp.GetPostData().Split('&');

        int id = int.Parse(postData[0].Substring(postData[0].IndexOf('=') + 1));
        string title = postData[1].Substring(postData[1].IndexOf('=') + 1).Replace('+',' ');
        string author = postData[2].Substring(postData[2].IndexOf('=') + 1).Replace('+',' ');
        
        Books book = new Books
        {
            ID = id,
            Title = title, 
            Author = author
        };
        bookList.Add(book);
        foreach (var item in postData)
        {
            string[] keyValue = item.Split('=');
            param.Add(keyValue[0], keyValue[1].Replace('+', ' '));
        }
        
        hp.CreatePathById(this.GetType(), id); // you can use CreatePathById() to register path for actions which require 1 argument named "id" as parameter

        // Urls.RegisterPathForId("/details", typeof(MainController), "Details", int.Parse(postData[0].Substring(postData[0].IndexOf('=') + 1)));
        // Urls.RegisterPathForId("/delete", typeof(MainController), "Delete", int.Parse(postData[0].Substring(postData[0].IndexOf('=') + 1)));
        // Urls.RegisterPathForId("/edit", typeof(MainController), "Edit", int.Parse(postData[0].Substring(postData[0].IndexOf('=') + 1)));
        return hp.RenderTemplate(responseBody, param);
    }

    public string Delete(string id)
    {
        string methodName = MethodBase.GetCurrentMethod().Name;
        string responseBody = _urls.GetFilePath($"/{methodName.ToLower()}", _controllerName);
        bookList.Remove(bookList.FirstOrDefault(x => x.ID == int.Parse(id)));
        return responseBody;
    }
    
    public string Edit( string id)
    {
        string methodName = MethodBase.GetCurrentMethod().Name;
        string responseBody = _urls.GetFilePath($"/{methodName.ToLower()}", _controllerName);
        Books book = bookList.FirstOrDefault(x => x.ID == int.Parse(id));
        Type type = typeof(Books);
        PropertyInfo[] properties = type.GetProperties();

        foreach (var prop in properties)
        {
            responseBody = responseBody.Replace("{{"+prop.Name+"}}", prop.GetValue(book).ToString());
        }
        return responseBody;
    }

    public string EditBook(StreamReader reader)
    {
        string methodName = MethodBase.GetCurrentMethod().Name;
        string responseBody = _urls.GetFilePath($"/{methodName.ToLower()}", _controllerName);
        HandlePost hp = new HandlePost(reader);
        string[] postData = hp.GetPostData().Split('&');
        Books b = bookList.FirstOrDefault(x => x.ID == int.Parse(postData[0].Substring(postData[0].IndexOf('=') + 1)));
        b.Title = postData[1].Substring(postData[1].IndexOf('=') + 1).Replace('+', ' ');
        b.Author = postData[2].Substring(postData[2].IndexOf('=') + 1).Replace('+', ' ');
        return responseBody;
    }
}