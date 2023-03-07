using System;
using System.Collections.Generic;

namespace TextNormalizer.Models
{
    public partial class User
    {
        public long Id { get; set; }
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public long RoleId { get; set; }

        public virtual Role Role { get; set; } = null!;
    }
}
