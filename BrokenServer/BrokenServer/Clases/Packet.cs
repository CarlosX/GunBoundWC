using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BrokenServer
{
    public partial class Systems
    {
        public class Packet
        {
            private byte[] m_FinalBuffer;
            private ushort m_Length;
            private ushort m_PacketID;
            protected PacketWriter2 m_Stream;

            public Packet(ushort code)
            {
                this.m_PacketID = code;
            }

            public Packet(ushort code, ushort length)
            {
                this.m_PacketID = code;
                this.m_Length = length;
                this.EnsureCapacity(length);
            }

            public byte[] Compile(Client user, bool encrypt)
            {
                if (this.m_FinalBuffer == null)
                {
                    this.InternalCompile(user, encrypt);
                }
                return this.m_FinalBuffer;
            }

            public void EnsureCapacity(ushort length)
            {
                this.m_Stream = new PacketWriter2(length);
                this.m_Stream.Write(length);
                this.m_Stream.Write((ushort)0);
                this.m_Stream.Write(this.m_PacketID);
            }

            private void InternalCompile(Client state, bool encrypt)
            {
                if (state == null)
                {
                    LogConsole.Show(LogType.DEBUG, "Null NetState passed in Network.Packet.Compile()");
                }
                if (this.m_Length == 0)
                {
                    long num = this.m_Stream.Length;
                    this.m_Stream.Seek(0L, SeekOrigin.Begin);
                    this.m_Stream.Write((ushort)num);
                }
                else if (this.m_Stream.Length != this.m_Length)
                {
                    LogConsole.Show(LogType.DEBUG, "Packet {0:X2}: Bad packet length, Expected {1} , Stream {2}", new object[] { this.m_PacketID, this.m_Length, this.m_Stream.Length });
                }
                MemoryStream underlyingStream = this.m_Stream.UnderlyingStream;
                this.m_FinalBuffer = underlyingStream.GetBuffer();
                int length = (int)underlyingStream.Length;
                if (!encrypt)
                {
                    ControlCode sendControlCode = new ControlCode(true);
                    if (state.FirstPacketSent)
                    {
                        sendControlCode.Update(length - 4);
                    }
                    this.m_Stream.Seek(2L, SeekOrigin.Begin);
                    this.m_Stream.Write(sendControlCode.Value);
                }
                if (this.m_FinalBuffer != null)
                {
                    byte[] finalBuffer = this.m_FinalBuffer;
                    this.m_FinalBuffer = new byte[length];
                    Buffer.BlockCopy(finalBuffer, 0, this.m_FinalBuffer, 0, length);
                }
                this.m_Stream = null;
            }

            public ushort PacketID
            {
                get
                {
                    return this.m_PacketID;
                }
            }

            public PacketWriter2 UnderlyingStream
            {
                get
                {
                    return this.m_Stream;
                }
            }
        }
    }
}
