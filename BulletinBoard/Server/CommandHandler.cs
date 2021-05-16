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
                    if (list[i].Equals(bulletin))
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
                list.Insert(0, bulletin);
                SerializeBulletinToJson(list);
            }
        }

        public string CheckAdministrator(params string[] components)
        {
            var nickname = components[0];
            var password = components[1];

            return CheckAdministrator(nickname, password).ToString();
        }

        private List<Bulletin> DeserializeBulletinFromJson()
        {
            string content = string.Empty;
            using (StreamReader sr = new StreamReader("Bulletins.json"))
            {
                content = sr.ReadToEnd();
            }

            var bulletins = JsonSerializer.Deserialize<List<Bulletin>>(content);
            
            return bulletins;
        }

        private void SerializeBulletinToJson(List<Bulletin> bulletins)
        {
            var serialized = JsonSerializer.Serialize(bulletins, new JsonSerializerOptions()
            {
                WriteIndented = true
            });

            using (var sw = new StreamWriter("Bulletins.json"))
            {
                sw.Write(serialized);
            }
        }

        private bool CheckAdministrator(string nickname, string password)
        {
            if (password == Empty || nickname == Empty)
            {
                return false;
            }

            string content = string.Empty;
            using (StreamReader sr = new StreamReader("Users.json"))
            {
                content = sr.ReadToEnd();
            }

            var users = JsonSerializer.Deserialize<List<User>>(content);

            return users.Any(x => x.Password == password && x.Nickname == nickname);
        }
    }
}
