using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Global.Protos;
using Google.Protobuf.WellKnownTypes;

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

        public override Task<UserResponse> GetUser(UserRequest request, ServerCallContext context)
        {
            
            return Task.FromResult(new UserResponse
            {
                Name = request.Name
            });
        }

        public override Task<ToDoItemList> GetToDoList(Empty request, ServerCallContext context)
        {
            var toDoList = new ToDoItemList();

            foreach(ToDoStructure todo in _dataContext.ToDoDb)
            {
                toDoList.ToDoList.Add(todo);
            }

            return Task.FromResult(toDoList);

        }

        public override async Task<Empty> AddToDo(ToDoStructure request, ServerCallContext context)
        {
            var toDoList = new ToDoItemList();
            var response = new Empty();

            _dataContext.ToDoDb.Add(new ToDoStructure()
            {
                Id = toDoList.ToDoList.Count,
                Description = request.Description,
                IsCompleted = false
            });

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