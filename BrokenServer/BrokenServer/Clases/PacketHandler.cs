using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace BrokenServer
{
    public partial class Systems
    {
        public delegate void OnPacketReceive(Client state, PacketReader2 pr);
        public class PacketHandler
        {
            private int m_Length;
            private OnPacketReceive m_OnReceive;
            private int m_PacketID;
            private bool m_RegisteredOnly;

            public PacketHandler(int packetID, int length, bool registeredonly, OnPacketReceive onReceive)
            {
                this.m_PacketID = packetID;
                this.m_Length = length;
                this.m_RegisteredOnly = registeredonly;
                this.m_OnReceive = onReceive;
            }

            public int Length
            {
                get
                {
                    return this.m_Length;
                }
            }

            public OnPacketReceive OnReceive
            {
                get
                {
                    return this.m_OnReceive;
                }
            }

            public int PacketID
            {
                get
                {
                    return this.m_PacketID;
                }
            }
        }
    }
}
