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

        public override Task<ToDoItemList> GetToDo(Empty request, ServerCallContext context)
        {
            var toDoList = new ToDoItemList();

            foreach(ToDoStructure todo in _dataContext.ToDoDb)
            {
                toDoList.ToDoList.Add(todo);
            }

            return Task.FromResult(toDoList);

        }

        public override Task<AddToDoResponse> AddToDo(ToDoStructure request, ServerCallContext context)
        {
            _dataContext.ToDoDb.Add(request);
            var result = _dataContext.SaveChanges();
            if(result > 0)
            {
                return Task.FromResult(new AddToDoResponse()
                {
                    StatusMessage = "Added successfully",
                    Status = true,
                    StatusCode = 100
                });
            }
            else
            {
                return Task.FromResult(new AddToDoResponse()
                {
                    StatusMessage = "Issue occured",
                    Status = true,
                    StatusCode = 500
                });
            }
        }
    } 
}