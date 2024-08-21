using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Todo.Models;
using Todo.Models.ViewModels;

namespace Todo.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var todoListViewModel = GetAllTodos();
        return View(todoListViewModel);
    }

    public JsonResult PopulateForm(int id)
    {
        var todo = GetById(id);
        return Json(todo);
    }

    internal static TodoItem GetById(int id)
    {
        TodoItem todo = new();
        using SqliteConnection connection = new("Data Source=db.sqlite");
        using var tableCommand = connection.CreateCommand();
        connection.Open();
        tableCommand.CommandText = $"SELECT * FROM todo where Id = '{id}'";
        using var reader = tableCommand.ExecuteReader();
        if (reader.HasRows)
        {
            reader.Read();
            todo.Id = reader.GetInt32(0);
            todo.Name = reader.GetString(1);
        }
        return todo;
    }

    internal static TodoViewModel GetAllTodos()
    {
        List<TodoItem> todoList = [];
        using SqliteConnection connection = new("Data Source=db.sqlite");
        using var tableCommand = connection.CreateCommand();
        connection.Open();
        tableCommand.CommandText = "SELECT * FROM todo";

        using var reader = tableCommand.ExecuteReader();
        if (reader.HasRows)
        {
            while (reader.Read())
            {
                todoList.Add(
                    new TodoItem
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    }
                );
            }
        }
        else
        {
            return new TodoViewModel
            {
                TodoList = todoList
            };
        }
        return new TodoViewModel
        {
            TodoList = todoList
        };
    }

    public RedirectResult Insert(TodoItem todo)
    {
        using SqliteConnection connection = new("Data Source=db.sqlite");
        using var tableCommand = connection.CreateCommand();
        connection.Open();
        tableCommand.CommandText = $"INSERT INTO todo (name) VALUES ('{todo.Name}')";
        try
        {
            tableCommand.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        return Redirect("http://localhost:5001");
    }

    public JsonResult Delete(int id)
    {
        using SqliteConnection connection = new("Data Source=db.sqlite");
        using var tableCommand = connection.CreateCommand();
        connection.Open();
        tableCommand.CommandText = $"DELETE from todo WHERE Id = '{id}'";
        tableCommand.ExecuteNonQuery();
        return Json(new {});
    }

    public RedirectResult Update(TodoItem todo)
    {
        using SqliteConnection connection = new("Data Source=db.sqlite");
        using var tableCommand = connection.CreateCommand();
        connection.Open();
        tableCommand.CommandText = $"UPDATE todo SET name = '{todo.Name}' where Id = '{todo.Id}'";
        try
        {
            tableCommand.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        return Redirect("http://localhost:5001");
    }
}
