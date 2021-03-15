using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Global.Protos;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
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
        [CascadingParameter] 
        Task<AuthenticationState> authenticationStateTask { get; set; }
        public string NewDescription { get; set; }
        public bool IsCompleted { get; set; }
        public bool CheckboxValue { get; set; }
        public string Description { get; set; }
        public string serverNameResponse { get; set; }
        public string ServerResponseUuid { get; set; }
        public string ServerResponseName { get; set; }
        public string ToDoDescription { get; set; }
        public int ToDoId { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; }
        public bool IsClicked { get; set; } = false;
        public string editTodo => IsClicked ? "initial" : "none";
        public string iconDisplay => IsClicked ? "none" : "initial"; 
        public RepeatedField<ToDoStructure> ServerToDoResponse { get; set; } = new RepeatedField<ToDoStructure>();

        // Fetch usser from server
        public async Task GetUser()
        {
            var authState = await authenticationStateTask;
            var user = authState.User;
            if (user.Identity.IsAuthenticated)
            {
                var userGuid = user.Claims.FirstOrDefault(c => c.Type == "preferred_username").Value.ToString();
                var subjectId = Convert.ToInt32(user.Claims.FirstOrDefault(c => c.Type == "sub").Value);
                var userEmail = user.Claims.FirstOrDefault(c => c.Type == "email").Value;
                var request = new UserInfo(){ Sub = subjectId, Uuid = userGuid, Email = userEmail };
                var response = await UserClient.GetUserAsync(request);

                await InvokeAsync(StateHasChanged);
                await GetUser();
                Console.WriteLine($"UUID: {userGuid}, sub: {subjectId}, Email: {userEmail}");
            }
        }

        // Fetch to-do list from server
        private async Task GetToDoList()
        {
            var request = new UuidParameter(){ Uuid = ServerResponseUuid };
            var response = await UserClient.GetToDoListAsync(request);
            ServerToDoResponse = response.ToDoList;
        }

        // Add to-do list to the server
        public async Task AddToDo(KeyboardEventArgs e)
        {

            if (e.Key == "Enter" && !string.IsNullOrWhiteSpace(Description) || 
                e.Key == "NumpadEnter" && !string.IsNullOrWhiteSpace(Description))
            {
                var request = new ToDoStructure()
                { 
                    Uuid = ServerResponseUuid, 
                    Description = this.Description, 
                };
                await UserClient.AddToDoAsync(request);
                await GetToDoList();
            } 
            else
            {}
        }

        // Update the checkbox state of the to-do list
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
            { 
                Id = id, 
                Description = description, 
                IsCompleted = isCompleted 
            };

            await UserClient.PutToDoAsync(request);
            await GetToDoList();
        }

        // Edit mode function
        private async Task EditToDo(int todoId, string description, bool isCompleted)
        {
            // Get the index of the to-do list
            int grpcIndex = ServerToDoResponse.IndexOf(new ToDoStructure() 
            { 
                Id = todoId, 
                Uuid = ServerResponseUuid,
                Description = description, 
                IsCompleted = isCompleted
            });

            ToDoDescription = ServerToDoResponse[grpcIndex].Description;

            // Make text area appear and focus on text area and edit icon dissapear based on the to-do list index
            await JSRuntime.InvokeVoidAsync("editMode", "edit-icon", "todo-description", "edit-todo", grpcIndex);
            await JSRuntime.InvokeVoidAsync("focusTextArea", todoId.ToString(), ToDoDescription);
        }

        // Update the to-do description
        public async Task PutToDoDescription(int id, string htmlId, string oldDescription, string newDescription, bool isCompleted)
        {
            var request = new ToDoStructure()
            { 
                Id = id, 
                Uuid = ServerResponseUuid,
                Description = newDescription, 
            };

            int grpcIndex = ServerToDoResponse.IndexOf(new ToDoStructure() 
            { 
                Id = id, 
                Description = oldDescription, 
                IsCompleted = isCompleted
            });

            // Text area auto resize function
            await JSRuntime.InvokeVoidAsync("theRealAutoResize", htmlId);
            // Make text area display to none and edit icon appear base on the to-do list index
            await JSRuntime.InvokeVoidAsync("initialMode", "edit-icon", "todo-description", "edit-todo", grpcIndex);
            await UserClient.PutToDoAsync(request);
            await GetToDoList();
        }

        // Delete to-do
        public async Task DeleteToDo(int id)
        {
            var request = new DeleteToDoParameter(){ Id = id };
            
            await UserClient.DeleteToDoAsync(request);
            await GetToDoList();
        }
    }
}