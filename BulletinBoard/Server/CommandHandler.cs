using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BulletinBoard.Model;
using static System.String;

namespace BulletinBoard.Server
{
    class CommandHandler
    {
        private static CommandHandler instance;
        private static object syncRoot = new Object();

        protected CommandHandler() { }

        public static CommandHandler GetInstance()
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    instance ??= new CommandHandler();
                }
            }

            return instance;
        }

        public string GetBulletins()
        {
            return "";
        }

        public void DeleteBulletin(params string[] components)
        {
            var nickname = components[0];
            var password = components[1];

            if (CheckAdministrator(nickname, password))
            {

            }
        }

        public void EditBulletin(params string[] components)
        {
            var nickname = components[0];
            var password = components[1];

            if (CheckAdministrator(nickname, password))
            {

            }
        }

        public void AddBulletin(params string[] components)
        {
            var nickname = components[0];
            var password = components[1];

            if (CheckAdministrator(nickname, password))
            {

            }
        }

        private bool CheckAdministrator(string nickname, string password)
        {
            if (password == Empty || nickname == Empty)
            {
                return false;
            }

            ValueTask<List<User>>? users = null;
            using (var fs = new FileStream("Users.json", FileMode.OpenOrCreate))
            {
                users = JsonSerializer.DeserializeAsync<List<User>>(fs);
            }

            return users.Value.Result.Any(x => x.Password == password && x.Nickname == nickname);
        }
    }
}
