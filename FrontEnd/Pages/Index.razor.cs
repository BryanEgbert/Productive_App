using System;
using FrontEnd.Components;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;
using Global;

namespace FrontEnd.Pages
{
    public partial class Index
    {
        public bool IsCompleted { get; set; }
        public bool CheckboxValue { get; set; }
        public string Description { get; set; }
        public List<ToDo> ToDoListCollection { get; set; } = new List<ToDo>();

        public void AddList(KeyboardEventArgs e)
        {
            if (e.Key == "Enter" && !string.IsNullOrWhiteSpace(Description) || 
                e.Key == "NumpadEnter" && !string.IsNullOrWhiteSpace(Description))
            {
                ToDoListCollection.Add(new ToDo() 
                    {ID = ToDoListCollection.Count + 1, IsCompleted = false, Description = $"{this.Description}"});
                Description = null;
            } 
            else
            {}
        }
        public void PrintCheckboxValue()
        {
            foreach (ToDo ToDoList in ToDoListCollection)
            {
                Console.WriteLine($"{ToDoList.ID}, {ToDoList.Description}, {ToDoList.IsCompleted}");
            }
        }
    }
}