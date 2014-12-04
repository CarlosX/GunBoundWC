using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BrokenServer
{
    public partial class Systems
    {
        public class PacketReader2
        {
            private byte[] m_Buffer;
            private int m_Index;
            private int m_Length;

            public PacketReader2(byte[] buffer, int length)
            {
                this.m_Buffer = buffer;
                this.m_Length = length;
                this.m_Index = 6;
            }

            public bool ReadBoolean()
            {
                if ((this.m_Index + 1) > this.m_Length)
                {
                    return false;
                }
                return (this.m_Buffer[this.m_Index++] != 0);
            }

            public byte ReadByte()
            {
                if ((this.m_Index + 1) > this.m_Length)
                {
                    return 0;
                }
                return this.m_Buffer[this.m_Index++];
            }

            public byte[] ReadBytes(int size)
            {
                int length = this.m_Index + size;
                int num2 = length;
                if (length > this.m_Length)
                {
                    length = this.m_Length;
                }
                byte[] dst = new byte[size];
                System.Buffer.BlockCopy(this.m_Buffer, this.m_Index, dst, 0, length - this.m_Index);
                this.m_Index = num2;
                return dst;
            }

            public short ReadInt16()
            {
                if ((this.m_Index + 2) > this.m_Length)
                {
                    return 0;
                }
                return (short)(this.m_Buffer[this.m_Index++] | (this.m_Buffer[this.m_Index++] << 8));
            }

            public int ReadInt32()
            {
                if ((this.m_Index + 4) > this.m_Length)
                {
                    return 0;
                }
                return (((this.m_Buffer[this.m_Index++] | (this.m_Buffer[this.m_Index++] << 8)) | (this.m_Buffer[this.m_Index++] << 0x10)) | (this.m_Buffer[this.m_Index++] << 0x18));
            }

            public sbyte ReadSByte()
            {
                if ((this.m_Index + 1) > this.m_Length)
                {
                    return 0;
                }
                return (sbyte)this.m_Buffer[this.m_Index++];
            }

            public string ReadString()
            {
                byte num;
                StringBuilder builder = new StringBuilder();
                while ((this.m_Index < this.m_Length) && ((num = this.m_Buffer[this.m_Index++]) != 0))
                {
                    builder.Append((char)num);
                }
                return builder.ToString();
            }

            public string ReadString(int size)
            {
                byte num;
                int length = this.m_Index + size;
                int num3 = length;
                if (length > this.m_Length)
                {
                    length = this.m_Length;
                }
                StringBuilder builder = new StringBuilder();
                while ((this.m_Index < length) && ((num = this.m_Buffer[this.m_Index++]) != 0))
                {
                    builder.Append((char)num);
                }
                this.m_Index = num3;
                return builder.ToString();
            }

            public ushort ReadUInt16()
            {
                if ((this.m_Index + 2) > this.m_Length)
                {
                    return 0;
                }
                return (ushort)(this.m_Buffer[this.m_Index++] | (this.m_Buffer[this.m_Index++] << 8));
            }

            public uint ReadUInt32()
            {
                if ((this.m_Index + 4) > this.m_Length)
                {
                    return 0;
                }
                return (uint)(((this.m_Buffer[this.m_Index++] | (this.m_Buffer[this.m_Index++] << 8)) | (this.m_Buffer[this.m_Index++] << 0x10)) | (this.m_Buffer[this.m_Index++] << 0x18));
            }

            public void Seek(int offset, SeekOrigin origin)
            {
                switch (origin)
                {
                    case SeekOrigin.Begin:
                        this.m_Index = offset;
                        return;

                    case SeekOrigin.Current:
                        this.m_Index += offset;
                        return;

                    case SeekOrigin.End:
                        this.m_Index = this.m_Length - offset;
                        return;
                }
            }

            public byte[] Buffer
            {
                get
                {
                    return this.m_Buffer;
                }
            }

            public int Length
            {
                get
                {
                    return this.m_Length;
                }
            }
        }
    }
}
