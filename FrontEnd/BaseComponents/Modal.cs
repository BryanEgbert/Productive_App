using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace FrontEnd.BaseComponents
{
    public class Modal : ComponentBase
    {
        [Parameter]
        public string Title { get; set; }
        [Parameter]
        public bool IsOpened { get; set; }
        [Parameter]
        public EventCallback<MouseEventArgs> OnExit { get; set; }
        [Parameter]
        public RenderFragment Content { get; set; }
        public string cssModal => IsOpened ? "show" : "hide"; 
    }
}