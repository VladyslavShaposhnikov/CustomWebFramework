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
    
    string _controllerName;

    public MainController()
    {
        _controllerName = this.GetType().Name;
    }

    public string Index()
    {
        return HandleGet.UseForeachWithModel(bookList, "/index", _controllerName);
    }

    public string Details(string id)
    {
        Books book = bookList.FirstOrDefault(x => x.ID == int.Parse(id));
        return HandleGet.Details(book, "/details", _controllerName);
    }

    public string Add()
    {
        return HandleGet.SimpleGet("/Add", _controllerName);
    }
    
    public string AddBook(StreamReader reader)
    {
        string responseBody = HandleGet.SimpleGet("/AddBook", _controllerName);
        
        Dictionary<string, string> postData =  HandlePost.GetPostData(reader);
        Books book = new Books
        {
            ID = postData.ContainsKey("Id") ? int.Parse(postData["Id"]) : 0,
            Title = postData.ContainsKey("Title") ? postData["Title"] : null, 
            Author = postData.ContainsKey("Author") ? postData["Author"] : null
        };
        
        bookList.Add(book);
        
        HandlePost.CreatePathById(this.GetType(), postData.ContainsKey("Id") ? int.Parse(postData["Id"]) : 0); // you can use CreatePathById() to register path for actions which require 1 argument named "id" as parameter

        // use method above to not do it manually like follow
        // Urls.RegisterPathForId("/details", typeof(MainController), "Details", int.Parse(postData[0].Substring(postData[0].IndexOf('=') + 1)));
        // Urls.RegisterPathForId("/delete", typeof(MainController), "Delete", int.Parse(postData[0].Substring(postData[0].IndexOf('=') + 1)));
        // Urls.RegisterPathForId("/edit", typeof(MainController), "Edit", int.Parse(postData[0].Substring(postData[0].IndexOf('=') + 1)));
        
        return HandlePost.RenderTemplate(responseBody, postData);
    }

    public string Delete(string id)
    {
        string responseBody = HandleGet.SimpleGet("/Delete", _controllerName);
        bookList.Remove(bookList.FirstOrDefault(x => x.ID == int.Parse(id)));
        return responseBody;
    }
    
    public string Edit( string id)
    {
        string responseBody = HandleGet.SimpleGet("/Edit", _controllerName);
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
        string responseBody = HandleGet.SimpleGet("/EditBook", _controllerName);
        Dictionary<string, string> postData = HandlePost.GetPostData(reader);
        Books b = bookList.FirstOrDefault(x => x.ID == int.Parse(postData["Id"]));
        b.Title = postData["Title"];
        b.Author = postData["Author"];
        return responseBody;
    }
}