using System;
using FrontEnd.Components;
using System.Collections.Generic;

namespace FrontEnd.Pages
{
    public partial class Index
    {
        public bool IsCompleted { get; set; }
        public List<string> ToDoListCollection { get; set; } = new List<string>()
        {
            "Walk",
            "Eat",
            "Sleep"
        };
        public void AddList()
        {

        }
    }
}