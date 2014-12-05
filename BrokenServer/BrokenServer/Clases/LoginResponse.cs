using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrokenServer
{
    public partial class Systems
    {
        public class LoginResponse : Packet
        {
            public LoginResponse(LoginResponseCode code)
                : base(0x1312, 8)
            {
                base.m_Stream.Write((ushort)code);
            }

            public LoginResponse(LoginResponseCode code, int restricttime, string reason)
                : base(0x1312, 0)
            {
                base.m_Stream.Write((ushort)code);
                base.m_Stream.Write(restricttime);
                base.m_Stream.WriteASCIIFixed(reason, reason.Length);
            }
            public LoginResponse(int unk1)
                : base(0x3001, 0)
            {
                base.m_Stream.Write((ushort)unk1);
            }
            public LoginResponse(int unk2, bool unkx = true)
                : base(0x4001, 0)
            {
                base.m_Stream.Write((ushort)unk2);
                base.m_Stream.Write((byte)0x1C);
                base.m_Stream.Write((byte)0x00);
                base.m_Stream.Write((byte)0x00);
                base.m_Stream.Write((byte)0x00);
            }
        }
    }
}
