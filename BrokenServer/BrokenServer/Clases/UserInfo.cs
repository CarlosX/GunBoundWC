using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrokenServer
{
    public partial class Systems
    {
        public class UserInfo
        {
            private int m_Authority;
            private string m_BanReason;
            private string m_Password;
            private int m_RestrictTime;
            private string m_Username;
            private Client m_State;
            private bool m_ban;

            public UserInfo(string username, Client state)
            {
                this.m_Username = username;
                this.m_State = state;
                LoadInfo();
            }

            private void LoadInfo(){ }

            public int Exist() { return 1; }

            public string BanReason
            {
                get { return this.m_BanReason; }
            }
            public bool Ban
            {
                get { return this.m_ban; }
            }
            public string Password
            {
                get { return this.m_Password; }
            }
            public string Username
            {
                get { return this.m_Username; }
            }

            public void Dispose()
            {
                this.m_Username = null;
                this.m_Password = null;
                this.m_Authority = 0;
                GC.SuppressFinalize(this);
            }
        }
    }
}
