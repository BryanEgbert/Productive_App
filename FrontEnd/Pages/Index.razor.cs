using System;
using FrontEnd.Components;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace FrontEnd.Pages
{
    public partial class Index
    {
        public bool IsCompleted { get; set; }
        public string Description { get; set; }
        public List<string> ToDoListCollection { get; set; } = new List<string>();

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
        // protected override async Task OnAfterRenderAsync(bool firstRender)
        // {
        //     if(firstRender)
        //     {
        //         await 
        //     }
        // }
    }
}