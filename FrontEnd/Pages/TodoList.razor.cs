using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Global.Protos;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace FrontEnd.Pages
{
    public partial class TodoList
    {
        [Inject]
        public User.UserClient UserClient { get; set; }
        public bool IsCompleted { get; set; }
        public bool CheckboxValue { get; set; }
        public string Description { get; set; }
        public string serverNameResponse { get; set; }
        public int ToDoId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public RepeatedField<ToDoStructure> ServerToDoResponse { get; set; } = new RepeatedField<ToDoStructure>();
        protected override async Task OnInitializedAsync()
        {
            await GetToDoList();
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
        public async Task PutToDo()
        {
            var request = new ToDoStructure(){ Id = ToDoId, Description = this.Description, IsCompleted = this.IsCompleted };

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