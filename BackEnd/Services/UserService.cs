using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Global.Protos;
using Google.Protobuf.WellKnownTypes;
using Google.Protobuf.Collections;
using Microsoft.EntityFrameworkCore;

namespace BackEnd
{
    public class UserService : User.UserBase
    
    {
        private readonly ILogger<UserService> _logger;
        private readonly UserContext _dataContext;
        public UserService(ILogger<UserService> logger, UserContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }

        public override async Task<UserInfo> GetUser(UserRequest request, ServerCallContext context)
        {

            var user = (from data in _dataContext.UserDb
                        where data.Email == request.Email
                        select data).Single();

            var userList = new UserResponse();

            return await Task.FromResult(new UserInfo()
            {
                Id = userList.UserList.Count,
                Uuid = user.Uuid,
                Name = request.Name,
                Email = request.Email
            });
        }

        public override async Task<Empty> AddUser(UserRequest request, ServerCallContext context)
        {
            Guid uuid = Guid.NewGuid();
            string stringUuid = uuid.ToString();
            var userList = new UserResponse();
            var newUser = new UserInfo(){ Uuid = stringUuid, Name = request.Name, Email = request.Email };
            var response = new Empty();

            _dataContext.UserDb.Add(newUser);
            userList.UserList.Add(newUser);

            await _dataContext.SaveChangesAsync();

            return await Task.FromResult(response);
        }

        public override async Task<ToDoItemList> GetToDoList(Empty request, ServerCallContext context)
        {
            var todoList = new ToDoItemList();
            var userInfo = new UserInfo();

            var getTodo = (from data in _dataContext.ToDoListDb
                           where data.Id == userInfo.Uuid
                           select data).Single();

            foreach(ToDoStructure todo in getTodo.ToDoList)
            {
                todoList.ToDoList.Add(todo);
            }

            return await Task.FromResult(todoList);
        }

        public override async Task<Empty> AddToDo(ToDoStructure request, ServerCallContext context)
        {
            var toDoList = new ToDoItemList();
            var response = new Empty();
            var userInfo = new UserInfo();
            var todoList = _dataContext.ToDoListDb.Where(x => x.Id == userInfo.Uuid).First();
            var newTodo = new ToDoStructure()
            {
                Id = toDoList.ToDoList.Count,
                Uuid = userInfo.Uuid,
                Description = request.Description,
                IsCompleted = false
            };

            todoList.ToDoList.Add(newTodo);
            await _dataContext.SaveChangesAsync();

            return await Task.FromResult(response);
        }

        public override async Task<Empty> PutToDo(ToDoStructure request, ServerCallContext context)
        {
            var response = new Empty();
            _dataContext.ToDoDb.Update(request);
            await _dataContext.SaveChangesAsync();
            return await Task.FromResult(response);
        }

        public override async Task<Empty> DeleteToDo(DeleteToDoParameter request, ServerCallContext context)
        {
            var response = new Empty();
            var item = (from data in _dataContext.ToDoDb
                        where data.Id == request.Id
                        select data).Single();
                        
            _dataContext.ToDoDb.Remove(item);
            var result = await _dataContext.SaveChangesAsync();
            return await Task.FromResult(response);
            
        }
    } 
}