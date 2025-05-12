using System.Collections.Generic;
using LAO_MVC_ACTIVITY.Models;

namespace Todo.Models.ViewModels
{
    public class ToDoViewModel
    {
        public List<ToDoItem> TodoList { get; set; } = new();  
        public ToDoItem Todo { get; set; } = new();     
        public List<string> Categories { get; set; } = new(); 
        public List<string> Priorities { get; set; } = new();       
    }
}
