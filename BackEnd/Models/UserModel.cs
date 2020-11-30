using System.Collections.Generic;

namespace BackEnd.Models
{
    public class UserModel
    {
        public long Id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public List<int> TimerCollection { get; set; }
    }
}