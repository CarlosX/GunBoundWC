using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BrokenServer
{
    public partial class Systems
    {
        public class ByteQueue
        {
            private byte[] m_Buffer = new byte[0x800];
            private int m_Head;
            private int m_Size;
            private int m_Tail;

            public void Clear()
            {
                this.m_Head = 0;
                this.m_Tail = 0;
                this.m_Size = 0;
            }

            public int Dequeue(byte[] buffer, int offset, int size)
            {
                if (size > this.m_Size)
                {
                    size = this.m_Size;
                }
                if (size == 0)
                {
                    return 0;
                }
                if (this.m_Head > this.m_Tail)
                {
                    Buffer.BlockCopy(this.m_Buffer, this.m_Head, buffer, offset, size);
                }
                else
                {
                    int count = this.m_Buffer.Length - this.m_Head;
                    if (count >= size)
                    {
                        Buffer.BlockCopy(this.m_Buffer, this.m_Head, buffer, offset, size);
                    }
                    else
                    {
                        Buffer.BlockCopy(this.m_Buffer, this.m_Head, buffer, offset, count);
                        Buffer.BlockCopy(this.m_Buffer, 0, buffer, offset + count, size - count);
                    }
                }
                this.m_Head = (this.m_Head + size) % this.m_Buffer.Length;
                this.m_Size -= size;
                if (this.m_Size == 0)
                {
                    this.m_Head = 0;
                    this.m_Tail = 0;
                }
                return size;
            }

            public void Enqueue(byte[] buffer, int offset, int size)
            {
                if ((this.m_Size + size) > this.m_Buffer.Length)
                {
                    this.SetCapacity(((this.m_Size + size) + 0x7ff) & -2048);
                }
                if (this.m_Head < this.m_Tail)
                {
                    int count = this.m_Buffer.Length - this.m_Tail;
                    if (count >= size)
                    {
                        Buffer.BlockCopy(buffer, offset, this.m_Buffer, this.m_Tail, size);
                    }
                    else
                    {
                        Buffer.BlockCopy(buffer, offset, this.m_Buffer, this.m_Tail, count);
                        Buffer.BlockCopy(buffer, offset + count, this.m_Buffer, 0, size - count);
                    }
                }
                else
                {
                    Buffer.BlockCopy(buffer, offset, this.m_Buffer, this.m_Tail, size);
                }
                this.m_Tail = (this.m_Tail + size) % this.m_Buffer.Length;
                this.m_Size += size;
            }

            public int GetPacketControlCode()
            {
                if (this.m_Size >= 6)
                {
                    return (this.m_Buffer[(this.m_Head + 2) % this.m_Buffer.Length] | (this.m_Buffer[(this.m_Head + 3) % ((int)this.m_Buffer.Length)] << 8));
                }
                return 0;
            }

            public int GetPacketID()
            {
                if (this.m_Size >= 6)
                {
                    return (this.m_Buffer[(this.m_Head + 4) % this.m_Buffer.Length] | (this.m_Buffer[(this.m_Head + 5) % ((int)this.m_Buffer.Length)] << 8));
                }
                return 0;
            }

            public int GetPacketLength()
            {
                if (this.m_Size >= 4)
                {
                    return (this.m_Buffer[this.m_Head % this.m_Buffer.Length] | (this.m_Buffer[(this.m_Head + 1) % ((int)this.m_Buffer.Length)] << 8));
                }
                return 0;
            }

            private void SetCapacity(int size)
            {
                byte[] dst = new byte[size];
                if (this.m_Size > 0)
                {
                    if (this.m_Head < this.m_Tail)
                    {
                        Buffer.BlockCopy(this.m_Buffer, this.m_Head, dst, 0, this.m_Size);
                    }
                    else
                    {
                        Buffer.BlockCopy(this.m_Buffer, this.m_Head, dst, 0, this.m_Buffer.Length - this.m_Head);
                        Buffer.BlockCopy(this.m_Buffer, 0, dst, this.m_Buffer.Length - this.m_Head, this.m_Tail);
                    }
                }
                this.m_Head = 0;
                this.m_Tail = this.m_Size;
                this.m_Buffer = dst;
            }

            public int Length
            {
                get
                {
                    return this.m_Size;
                }
            }

            public static ushort GetPacketID(byte[] buffer)
            {
                if (buffer.Length >= 6)
                {
                    return (ushort)(buffer[4] | (buffer[5] << 8));
                }
                return 0;
            }
        }
    }
}
