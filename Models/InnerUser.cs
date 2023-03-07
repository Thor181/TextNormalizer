using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextNormalizer.Models
{
    public class InnerUser
    {
        public long Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public long RoleId { get; set; }
        public string Role { get; set; }
        public InnerUser(long id, string login, string password, long roleId, string role)
        {
            Crypter crypter = new();
            Id = id;
            Login = crypter.Decrypt(login);
            Password = password;
            RoleId = roleId;
            Role = role;
        }
    }
}
