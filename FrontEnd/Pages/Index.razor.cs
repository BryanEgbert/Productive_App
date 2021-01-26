using System;
using FrontEnd.Components;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Web;

namespace FrontEnd.Pages
{
    public partial class Index
    {
        public bool IsCompleted { get; set; }
        public string Description { get; set; }
        public List<string> ToDoListCollection { get; set; }
        public void AddList(KeyboardEventArgs e)
        {
            if (e.Key == "Enter" && !string.IsNullOrWhiteSpace(Description) || 
                e.Key == "NumpadEnter" && !string.IsNullOrWhiteSpace(Description))
            {
                ToDoListCollection.Add($"{Description}");
                Description = null;
            } else
            {}
        }
    }
}