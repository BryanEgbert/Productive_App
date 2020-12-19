using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace FrontEnd.Components
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
        public EventCallback OnAdd { get; set; }
        public string cssModal => IsOpened ? "show" : "hide"; 
    }
}