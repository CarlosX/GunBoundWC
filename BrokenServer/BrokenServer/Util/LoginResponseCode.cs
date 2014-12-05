using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrokenServer
{
    public partial class Systems
    {
        public enum LoginResponseCode : ushort
        {
            BadPass = 0x11,
            IncorrectVersion = 0x60,
            LoginSuccessful = 0,
            NotAuthorized = 0x19,
            Prohibited = 0x30,
            UnregisteredID = 0x10
        }
    }
}
