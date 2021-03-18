using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Global.Protos;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using BackEnd;
using BackEnd.Data;

namespace BackEnd
{
    [Authorize(Roles="User")]
    public class UserService : User.UserBase
    
    {
        private readonly ILogger<UserService> _logger;
        private readonly ApplicationDbContext _dataContext;
        public UserService(ILogger<UserService> logger, ApplicationDbContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }

        public override async Task<Empty> GetUser(UserInfo request, ServerCallContext context)
        {
            var response = new Empty();
            var userList = new UserResponse();

            if (!_dataContext.UserDb.Any(x => x.Sub == request.Sub))
            {
                var newUser = new UserInfo(){ Id = userList.UserList.Count, Sub = request.Sub, Email = request.Email };

                _dataContext.UserDb.Add(newUser);
                userList.UserList.Add(newUser);

                await _dataContext.SaveChangesAsync();
            }
            else
            {
                var user = _dataContext.UserDb.Single(u => u.Sub == request.Sub);
                userList.UserList.Add(user);
            }
            
            return await Task.FromResult(response);
        }

        public override async Task<ToDoItemList> GetToDoList(UuidParameter request, ServerCallContext context)
        {
            var todoList = new ToDoItemList();
            var userInfo = new UserInfo();

            var getTodo = (from data in _dataContext.ToDoDb
                           where data.Uuid == userInfo.Sub
                           select data).ToList();

            todoList.ToDoList.Add(getTodo);

            return await Task.FromResult(todoList);
        }

        public override async Task<Empty> AddToDo(ToDoStructure request, ServerCallContext context)
        {
            var todoList = new ToDoItemList();
            var userInfo = new UserInfo();
            var newTodo = new ToDoStructure()
            {
                Id = todoList.ToDoList.Count,
                Uuid = request.Uuid,
                Description = request.Description,
                IsCompleted = false
            };

            todoList.ToDoList.Add(newTodo);
            await _dataContext.ToDoDb.AddAsync(newTodo);
            await _dataContext.SaveChangesAsync();

            return await Task.FromResult(new Empty());
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
            var item = (from data in _dataContext.ToDoDb
                        where data.Id == request.Id
                        select data).First();
                        
            _dataContext.ToDoDb.Remove(item);
            var result = await _dataContext.SaveChangesAsync();

            return await Task.FromResult(new Empty());
            
        }
    } 
}