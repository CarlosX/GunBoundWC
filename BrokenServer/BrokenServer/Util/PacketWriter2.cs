using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace BrokenServer
{
    public partial class Systems
    {
        public class PacketWriter2
        {
            private static byte[] m_Buffer = new byte[4];
            private int m_Capacity;
            private static Stack m_Pool = new Stack();
            private MemoryStream m_Stream;

            public PacketWriter2()
                : this(0x26)
            {
            }

            public PacketWriter2(int length)
            {
                this.m_Stream = new MemoryStream(length);
                this.m_Capacity = length;
            }

            public static PacketWriter2 CreateInstance()
            {
                return CreateInstance(0x26);
            }

            public static PacketWriter2 CreateInstance(int capacity)
            {
                PacketWriter2 writer = null;
                lock (m_Pool)
                {
                    if (m_Pool.Count > 0)
                    {
                        writer = (PacketWriter2)m_Pool.Pop();
                        if (writer != null)
                        {
                            writer.m_Capacity = capacity;
                            writer.m_Stream.SetLength(0L);
                        }
                    }
                }
                if (writer == null)
                {
                    writer = new PacketWriter2(capacity);
                }
                return writer;
            }

            public void Fill()
            {
                this.Fill(this.m_Capacity - ((int)this.m_Stream.Length));
            }

            public void Fill(int size)
            {
                if (this.m_Stream.Position == this.m_Stream.Length)
                {
                    this.m_Stream.SetLength(this.m_Stream.Length + size);
                    this.m_Stream.Seek(0L, SeekOrigin.End);
                }
                else
                {
                    this.m_Stream.Write(new byte[size], 0, size);
                }
            }

            public static void ReleaseInstance(PacketWriter pw)
            {
                lock (m_Pool)
                {
                    if (!m_Pool.Contains(pw))
                    {
                        m_Pool.Push(pw);
                    }
                    else
                    {
                        LogConsole.Show(LogType.DEBUG, "Packet.ReleaseInstance - Buffer pool already exists!! ");
                    }
                }
            }

            public long Seek(long offset, SeekOrigin origin)
            {
                return this.m_Stream.Seek(offset, origin);
            }

            public byte[] ToArray()
            {
                return this.m_Stream.ToArray();
            }

            public void Write(bool value)
            {
                this.m_Stream.WriteByte(value ? ((byte)1) : ((byte)0));
            }

            public void Write(byte value)
            {
                this.m_Stream.WriteByte(value);
            }

            public void Write(short value)
            {
                m_Buffer[0] = (byte)value;
                m_Buffer[1] = (byte)(value >> 8);
                this.m_Stream.Write(m_Buffer, 0, 2);
            }

            public void Write(int value)
            {
                m_Buffer[0] = (byte)value;
                m_Buffer[1] = (byte)(value >> 8);
                m_Buffer[2] = (byte)(value >> 0x10);
                m_Buffer[3] = (byte)(value >> 0x18);
                this.m_Stream.Write(m_Buffer, 0, 4);
            }

            public void Write(sbyte value)
            {
                this.m_Stream.WriteByte((byte)value);
            }

            public void Write(ushort value)
            {
                m_Buffer[0] = (byte)value;
                m_Buffer[1] = (byte)(value >> 8);
                this.m_Stream.Write(m_Buffer, 0, 2);
            }

            public void Write(uint value)
            {
                m_Buffer[0] = (byte)value;
                m_Buffer[1] = (byte)(value >> 8);
                m_Buffer[2] = (byte)(value >> 0x10);
                m_Buffer[3] = (byte)(value >> 0x18);
                this.m_Stream.Write(m_Buffer, 0, 4);
            }

            public void Write(byte[] buffer, int offset, int count)
            {
                this.m_Stream.Write(buffer, offset, count);
            }

            public void WriteASCIIFixed(string value, int length)
            {
                if (value == null)
                {
                    LogConsole.Show(LogType.DEBUG, "Null string in Network.PacketWriter.WriteASCIIFixed");
                    value = string.Empty;
                }
                byte[] bytes = Utils.GetBytes(value);
                if (bytes.Length >= length)
                {
                    this.m_Stream.Write(bytes, 0, length);
                }
                else
                {
                    this.m_Stream.Write(bytes, 0, bytes.Length);
                    this.Fill(length - bytes.Length);
                }
            }

            public void WriteASCIIZ(string value)
            {
                if (value == null)
                {
                    LogConsole.Show(LogType.DEBUG, "Null string in Network.PacketWriter.WriteASCIIZ");
                    value = string.Empty;
                }
                byte[] bytes = Utils.GetBytes(value);
                this.m_Stream.Write(bytes, 0, bytes.Length);
                this.m_Stream.WriteByte(0);
            }

            public long Length
            {
                get
                {
                    return this.m_Stream.Length;
                }
            }

            public long Position
            {
                get
                {
                    return this.m_Stream.Position;
                }
            }

            public MemoryStream UnderlyingStream
            {
                get
                {
                    return this.m_Stream;
                }
            }
        }
    }
}
