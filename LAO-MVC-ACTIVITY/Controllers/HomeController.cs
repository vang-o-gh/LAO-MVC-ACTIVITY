using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LAO_MVC_ACTIVITY.Models;
using Microsoft.Data.Sqlite;
using Todo.Models.ViewModels;

namespace LAO_MVC_ACTIVITY.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        List<ToDoItem> items = new();
        List<string> categories = new();
        List<string> priorities = new();

        using (var con = new SqliteConnection("Data Source=db.sqlite"))
        {
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "SELECT Id, Name, Category, Priority, isCompleted FROM todo";
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var item = new ToDoItem
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Category = reader.GetString(2),
                        Priority = reader.GetString(3),
                        IsCompleted = reader.GetInt32(4) == 1 
                    };


                    items.Add(item);
                    categories.Add(item.Category);
                    priorities.Add(item.Priority); 
                }
            }
        }

        var viewModel = new ToDoViewModel
        {
            TodoList = items,
            Categories = categories.Distinct().ToList(),
            Priorities = priorities.Distinct().ToList() 
        };

        return View(viewModel);
    }

    [HttpGet]
    public JsonResult PopulateForm(int id)
    {
        var todo = GetById(id);
        return Json(todo);
    }


    internal ToDoItem GetById(int id)
    {
        ToDoItem todo = new();

        using (SqliteConnection con = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var cmd = con.CreateCommand())
            {
                con.Open();
                cmd.CommandText = "SELECT * FROM todo WHERE id = $id";
                cmd.Parameters.AddWithValue("$id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new ToDoItem
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Category = reader.GetString(2),
                            Priority = reader.GetString(3) 
                        };
                    }
                }
            }
        }

        return todo;
    }


    public IActionResult Update(ToDoItem todo)
    {
        if (string.IsNullOrWhiteSpace(todo.Name) ||
            string.IsNullOrWhiteSpace(todo.Category) ||
            string.IsNullOrWhiteSpace(todo.Priority))
        {
            TempData["ErrorMessage"] = "All fields are required!";
            return RedirectToAction("Index");
        }

        using (SqliteConnection con = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = con.CreateCommand())
            {
                con.Open();
                
                tableCmd.CommandText = "UPDATE todo SET name = @name, category = @category, priority = @priority WHERE Id = @id";

                tableCmd.Parameters.AddWithValue("@name", todo.Name);
                tableCmd.Parameters.AddWithValue("@category", todo.Category);
                tableCmd.Parameters.AddWithValue("@priority", todo.Priority);
                tableCmd.Parameters.AddWithValue("@id", todo.Id);


                try
                {
                    tableCmd.ExecuteNonQuery();
                    TempData["SuccessUpdateMessage"] = "Task UPDATED successfully!";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        return Redirect("/");
    }

   internal ToDoViewModel GetAllTodos()
   {
        List<ToDoItem> todoList = new();

        using (SqliteConnection con = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = con.CreateCommand())
            {
                con.Open();
                tableCmd.CommandText = "SELECT * FROM todo";

                using (var reader = tableCmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            todoList.Add(
                                new ToDoItem
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Category = reader.GetString(2),
                                    Priority = reader.GetString(3)
                                }
                            );
                        }
                    }
                }
            }
        }

        return new ToDoViewModel
        {
            TodoList = todoList
        };
    }


    [HttpPost]
    public IActionResult Insert(ToDoItem todo)
    {
        if (string.IsNullOrWhiteSpace(todo.Name) ||
            string.IsNullOrWhiteSpace(todo.Category) ||
            string.IsNullOrWhiteSpace(todo.Priority))
        {
            TempData["ErrorMessage"] = "All fields are required!";
            return RedirectToAction("Index");
        }

        using (SqliteConnection con = new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = con.CreateCommand())
            {
                con.Open();
                tableCmd.CommandText = "INSERT INTO todo (name, category, priority) VALUES (@name, @category, @priority)";
                tableCmd.Parameters.AddWithValue("@name", todo.Name);
                tableCmd.Parameters.AddWithValue("@category", todo.Category);
                tableCmd.Parameters.AddWithValue("@priority", todo.Priority);  

                try
                {
                    tableCmd.ExecuteNonQuery();
                    TempData["SuccessMessage"] = "Task ADDED successfully!";
                }
                catch (Exception ex)
                {
                     TempData["ErrorMessage"] = "Error: " + ex.Message;
                }
            }
        }
        return Redirect("/");
    }


     [HttpPost]
    public JsonResult Delete(int id)
    {
        using (SqliteConnection con =
                new SqliteConnection("Data Source=db.sqlite"))
        {
            using (var tableCmd = con.CreateCommand())
            {
                con.Open();
                tableCmd.CommandText = $"DELETE from todo WHERE Id = '{id}'";
                tableCmd.ExecuteNonQuery();
                TempData["SuccessDeleteMessage"] = "Task DELETED successfully!";
            }
        }
        return Json(new {});
    }

    [HttpPost]
    public IActionResult ToggleComplete(int id, bool isChecked)
    {
        using (var con = new SqliteConnection("Data Source=db.sqlite"))
        {
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "UPDATE todo SET isCompleted = $isCompleted WHERE Id = $id";
            cmd.Parameters.AddWithValue("$isCompleted", isChecked ? 1 : 0);
            cmd.Parameters.AddWithValue("$id", id);
            cmd.ExecuteNonQuery();
        }

        return Ok();
    }


}
