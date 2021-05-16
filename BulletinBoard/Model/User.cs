using System;
using System.Collections.Generic;
using System.Text;

namespace BulletinBoard.Model
{
    public class User
    {
        public string Nickname { get; set; }
        public string Password { get; set; }

        public User(string nickname, string password)
        {
            Nickname = nickname;
            Password = password;
        }

        public User()
        {

        }
    }
}
