using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Global.Protos;
using static Global.Protos.UserInfo.Types;

namespace BackEnd
{
    public class UserService : User.UserBase
    
    {
        private readonly ILogger<UserService> _logger;
        public UserService(ILogger<UserService> logger)
        {
            _logger = logger;
        }

        public override Task<UserInfo> GetUser(UserIdentity request, ServerCallContext context)
        {
            return Task.FromResult(new UserInfo
            {
                Name = request.Name,
                Timer={
                    new TimerStructure{ Id = 4, Hours = 0, Minutes = 30 , Seconds = 0 }
                }
            });
        }
    } 
}