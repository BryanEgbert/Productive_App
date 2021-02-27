using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Global.Protos;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace FrontEnd.Pages
{
    public partial class TodoList
    {
        [Inject]
        private User.UserClient UserClient { get; set; }
        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        public string NewDescription { get; set; }
        public bool IsCompleted { get; set; }
        public bool CheckboxValue { get; set; }
        public string Description { get; set; }
        public string serverNameResponse { get; set; }
        public string ToDoDescription { get; set; }
        public int ToDoId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsClicked { get; set; } = false;
        public string editTodo => IsClicked ? "initial" : "none";
        public string iconDisplay => IsClicked ? "none" : "initial"; 
        public RepeatedField<ToDoStructure> ServerToDoResponse { get; set; } = new RepeatedField<ToDoStructure>();
        protected override async Task OnInitializedAsync()
        {
            await GetToDoList();
            foreach (var todo in ServerToDoResponse)
            {
                ToDoDescription = todo.Description;
            }
        }
        public async Task GetUser()
        {
            var request = new UserRequest { Name = this.Name, Email = this.Email, Password = this.Password };
            var response = await UserClient.GetUserAsync(request);
            serverNameResponse = response.Name;
        }

        private async Task GetToDoList()
        {
            var request = new Empty();
            var response = await UserClient.GetToDoListAsync(request);
            ServerToDoResponse = response.ToDoList;
        }
        public async Task AddToDo(KeyboardEventArgs e)
        {

            if (e.Key == "Enter" && !string.IsNullOrWhiteSpace(Description) || 
                e.Key == "NumpadEnter" && !string.IsNullOrWhiteSpace(Description))
            {
                var request = new ToDoStructure()
                    { Id = ServerToDoResponse.Count, Description = this.Description, IsCompleted = false };
                Description = null;

                await UserClient.AddToDoAsync(request);
                await GetToDoList();
            } 
            else
            {}
        }
        public async Task PutToDoIsCompleted(int id, string description, bool isCompleted, MouseEventArgs e)
        {
            if (isCompleted == false && e.Button== 0)
            {
                isCompleted = true;
            } 
            else if (isCompleted == true && e.Button == 0)
            {
                isCompleted = false;
            }

            var request = new ToDoStructure()
                { Id = id, Description = description, IsCompleted = isCompleted };

            await UserClient.PutToDoAsync(request);
            await GetToDoList();
        }
        public async Task EditToDo(int todoId, string description, bool isCompleted)
        {
            int grpcIndex = ServerToDoResponse.IndexOf(new ToDoStructure() 
                { Id = todoId, Description = description, IsCompleted = isCompleted});

            await JSRuntime.InvokeVoidAsync("editMode", "edit-icon", "todo-description", "edit-todo", grpcIndex);
            ToDoDescription = ServerToDoResponse[grpcIndex].Description;
            await JSRuntime.InvokeVoidAsync("focusTextArea", todoId.ToString(), ToDoDescription);
        }
        public async Task PutToDoDescription(int id, string htmlId, string oldDescription, string newDescription, bool isCompleted)
        {
            var request = new ToDoStructure()
                { Id = id, Description = newDescription, IsCompleted = isCompleted };

            int grpcIndex = ServerToDoResponse.IndexOf(new ToDoStructure() 
                { Id = id, Description = oldDescription, IsCompleted = isCompleted});

            await JSRuntime.InvokeVoidAsync("theRealAutoResize", htmlId);
            await JSRuntime.InvokeVoidAsync("initialMode", "edit-icon", "todo-description", "edit-todo", grpcIndex);
            await UserClient.PutToDoAsync(request);
            await GetToDoList();
        }
        public async Task DeleteToDo(int id)
        {
            var request = new DeleteToDoParameter(){ Id = id };
            
            await UserClient.DeleteToDoAsync(request);
            await GetToDoList();
        }
    }
}