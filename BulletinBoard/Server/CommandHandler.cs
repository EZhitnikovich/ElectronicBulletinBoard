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
            return JsonSerializer.Serialize(DeserializeBulletinFromJson());
        }

        public void DeleteBulletin(params string[] components)
        {
            var nickname = components[0];
            var password = components[1];

            if (CheckAdministrator(nickname, password))
            {
                var list = DeserializeBulletinFromJson();
                var bulletin = JsonSerializer.Deserialize<Bulletin>(components[2]);
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] == bulletin)
                    {
                        list.RemoveAt(i);
                    }
                }
                SerializeBulletinToJson(list);
            }
        }

        public void AddBulletin(params string[] components)
        {
            var nickname = components[0];
            var password = components[1];

            if (CheckAdministrator(nickname, password))
            {
                var list = DeserializeBulletinFromJson();
                var bulletin = JsonSerializer.Deserialize<Bulletin>(components[2]);
                list.Add(bulletin);
                SerializeBulletinToJson(list);
            }
        }

        private List<Bulletin> DeserializeBulletinFromJson()
        {
            ValueTask<List<Bulletin>>? bulletinValueTask = null;
            using (FileStream fs = new FileStream("Bulletins.json", FileMode.OpenOrCreate))
            {
                bulletinValueTask = JsonSerializer.DeserializeAsync<List<Bulletin>>(fs);
            }

            return bulletinValueTask.Value.Result;
        }

        private void SerializeBulletinToJson(List<Bulletin> bulletins)
        {
            using (FileStream fs = new FileStream("Bulletins.json", FileMode.OpenOrCreate))
            {
                JsonSerializer.SerializeAsync(fs, bulletins);
                Console.WriteLine("Data has been saved to file");
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
