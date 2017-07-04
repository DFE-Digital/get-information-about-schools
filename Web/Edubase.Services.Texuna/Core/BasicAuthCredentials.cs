using Edubase.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.Services.Texuna.Core
{
    public class BasicAuthCredentials
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public BasicAuthCredentials(string username, string password)
        {
            Guard.IsNotNull(username, () => new ArgumentNullException(nameof(username)));
            Guard.IsNotNull(password, () => new ArgumentNullException(nameof(password)));
            Username = username;
            Password = password;
        }

        public override string ToString() => Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Username}:{Password}"));
    }
}
