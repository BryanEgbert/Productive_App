using Microsoft.AspNetCore.Components;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Global.Protos;
using System.Threading.Tasks;

namespace FrontEnd.Services
{
    public class ToDoService
    {
        [Inject]
        public User.UserClient UserClient { get; set; }
        public RepeatedField<ToDoStructure> serverToDoResponse = new RepeatedField<ToDoStructure>();
        public async Task GetUser(string name, string email, string password)
        {
            var request = new EmailParameter { Email = email };
            var response = await UserClient.GetUserAsync(request);
        }
        // Fetch to-do list
        public void GetToDoList(int id)
        {
            var request = new UuidParameter() { Uuid = "udpi "};
            var response = UserClient.GetToDoListAsync(request);
        }
        // Add to-do list
        public Empty AddToDo(string description)
        {
            var toDoList = new ToDoItemList();
            var request = new ToDoStructure(){Id = toDoList.ToDoList.Count, Description = description, IsCompleted = false};
            toDoList.ToDoList.Add(request);
            UserClient.AddToDoAsync(request);
            return new Empty();
        }
        // Insert to-do list
        public async Task PutToDo(int id, string description, bool isCompleted)
        {
            var toDoList = new ToDoItemList();
            var request = new ToDoStructure(){ Id = id, Description = description, IsCompleted = isCompleted };
            var response = await UserClient.PutToDoAsync(request);
            toDoList.ToDoList.Insert(id, request);
        }
        // Delete to-do in the list
        public async Task DeleteToDo(int id)
        {
            var toDoList = new ToDoItemList();
            var request = new DeleteToDoParameter(){ Id = id };
            toDoList.ToDoList.RemoveAt(id);
            await UserClient.DeleteToDoAsync(request);
        }
    }
}