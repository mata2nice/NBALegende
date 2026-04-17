using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBALegende.Models
{

    [Serializable]
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }

        public User()
        {
        }

        public User(string username, string password, UserRole role)
        {
            Username = username;
            Password = password;
            Role = role;
        }
    }
}
