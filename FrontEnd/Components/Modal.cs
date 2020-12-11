using System;
using Microsoft.AspNetCore.Components;

namespace FrontEnd.Components
{
    public class Modal : ComponentBase
    {
        [Parameter]
        public bool IsOpened { get; set; }
        public string cssModal => IsOpened ? "show" : "hide"; 
        [Parameter]
        public string Title { get; set; }
    }
}