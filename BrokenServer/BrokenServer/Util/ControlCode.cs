using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrokenServer
{
    public partial class Systems
    {
        public class ControlCode
        {
            private ushort m_ControlCode;

            public ControlCode(bool send)
            {
                if (send)
                {
                    this.m_ControlCode = 0xac03;
                }
                else
                {
                    this.m_ControlCode = 0x35f1;
                }
            }

            public int Peek(int length)
            {
                return (ushort)(((this.m_ControlCode + 0xff4) + (length * 0x43fd)) % 0x10000);
            }

            public int Update(int length)
            {
                this.m_ControlCode = (ushort)(((this.m_ControlCode + 0xff4) + (length * 0x43fd)) % 0x10000);
                return this.m_ControlCode;
            }

            public ushort Value
            {
                get
                {
                    return this.m_ControlCode;
                }
            }
        }
    }
}
