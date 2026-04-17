using NBALegende.Models;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace NBALegende
{
    public class UserDataIO
    {
        private string usersFilePath = "users.xml";

        public List<User> LoadUsers()
        {
            if (!File.Exists(usersFilePath))
            {
                List<User> defaultUsers = new List<User>();
                defaultUsers.Add(new User("admin", "admin", UserRole.Admin));
                defaultUsers.Add(new User("visitor", "visitor", UserRole.Visitor));

                SaveUsers(defaultUsers);
                return defaultUsers;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(List<User>));

            using (FileStream fileStream = new FileStream(usersFilePath, FileMode.Open))
            {
                List<User> users = (List<User>)serializer.Deserialize(fileStream);

                if (users == null)
                {
                    users = new List<User>();
                }

                return users;
            }
        }

        public void SaveUsers(List<User> users)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<User>));

            using (FileStream fileStream = new FileStream(usersFilePath, FileMode.Create))
            {
                serializer.Serialize(fileStream, users);
            }
        }
    }
}