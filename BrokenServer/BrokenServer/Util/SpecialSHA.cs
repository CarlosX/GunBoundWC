using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace BrokenServer.Util
{
    public class SpecialSHA
    {
        private byte[] m_ByteArray;
        private long m_HighBound;
        private long m_HighByte;

        private DWORD AddW(DWORD w1, DWORD w2)
        {
            DWORD dword;
            int num = w1.B3 + w2.B3;
            dword.B3 = (byte)(num % 0x100);
            num = (w1.B2 + w2.B2) + (num / 0x100);
            dword.B2 = (byte)(num % 0x100);
            num = (w1.B1 + w2.B1) + (num / 0x100);
            dword.B1 = (byte)(num % 0x100);
            num = (w1.B0 + w2.B0) + (num / 0x100);
            dword.B0 = (byte)(num % 0x100);
            return dword;
        }

        private DWORD AndW(DWORD w1, DWORD w2)
        {
            return new DWORD { B0 = (byte)(w1.B0 & w2.B0), B1 = (byte)(w1.B1 & w2.B1), B2 = (byte)(w1.B2 & w2.B2), B3 = (byte)(w1.B3 & w2.B3) };
        }

        private void Append(byte data)
        {
            if ((1L + this.m_HighByte) > this.m_HighBound)
            {
                this.m_HighBound += 0x400L;
            }
            this.m_ByteArray[(int)((IntPtr)this.m_HighByte)] = data;
            this.m_HighByte += 1L;
        }

        private void Append(string data)
        {
            long length = data.Length;
            if ((length + this.m_HighByte) > this.m_HighBound)
            {
                this.m_HighBound += 0x400L;
            }
            byte[] bytes = Systems.Utils.GetBytes(data);
            for (int i = 0; i < length; i++)
            {
                this.m_ByteArray[(int)((IntPtr)(this.m_HighByte + i))] = bytes[i];
            }
            this.m_HighByte += length;
        }

        public char Chr(byte i)
        {
            return Systems.Utils.Encoding.GetChars(new byte[] { i }, 0, 1)[0];
        }

        private DWORD CircShiftLeftW(DWORD w, int n)
        {
            uint num = this.DWORDToUINT(w);
            uint num2 = num;
            num = (uint)(num * Math.Pow(2.0, (double)n));
            num2 = (uint)(((double)num2) / Math.Pow(2.0, (double)(0x20 - n)));
            return this.OrW(this.ToDWORD(num), this.ToDWORD(num2));
        }

        private uint DWORDToUINT(DWORD w)
        {
            return (uint)((((w.B0 << 0x18) | (w.B1 << 0x10)) | (w.B2 << 8)) | w.B3);
        }

        private DWORD F(int t, DWORD b, DWORD c, DWORD d)
        {
            if (t <= 0x13)
            {
                return this.OrW(this.AndW(b, c), this.AndW(this.NotW(b), d));
            }
            if ((t > 0x27) && (t <= 0x3b))
            {
                return this.OrW(this.OrW(this.AndW(b, c), this.AndW(b, d)), this.AndW(c, d));
            }
            return this.XOrW(this.XOrW(b, c), d);
        }

        private byte[] GData()
        {
            byte[] buffer = new byte[this.m_HighByte];
            for (int i = 0; i < this.m_HighByte; i++)
            {
                buffer[i] = this.m_ByteArray[i];
            }
            return buffer;
        }

        private DWORD NotW(DWORD w)
        {
            DWORD dword;
            dword.B0 = w.B0;
            dword.B1 = w.B1;
            dword.B2 = w.B2;
            dword.B3 = w.B3;
            return dword;
        }



        private DWORD OrW(DWORD w1, DWORD w2)
        {
            return new DWORD { B0 = (byte)(w1.B0 | w2.B0), B1 = (byte)(w1.B1 | w2.B1), B2 = (byte)(w1.B2 | w2.B2), B3 = (byte)(w1.B3 | w2.B3) };
        }

        private void Reset()
        {
            this.m_HighByte = 0L;
            this.m_HighBound = 0x400L;
            this.m_ByteArray = new byte[0x400];
        }

        public byte[] SHA1(string inMsg)
        {
            DWORD[] dwordArray = new DWORD[4];
            DWORD[] dwordArray2 = new DWORD[5];
            DWORD[] dwordArray3 = new DWORD[80];
            long length = inMsg.Length;
            DWORD dword6 = this.ToDWORD((uint)(length * 8L));
            this.Reset();
            int num3 = ((int)((0x80L - (length % 0x40L)) - 9L)) % 0x40;
            int num4 = (inMsg.Length + 9) + num3;
            this.Append(inMsg);
            this.Append((byte)0x80);
            for (int i = 0; i < (num3 + 4); i++)
            {
                this.Append((byte)0);
            }
            this.Append(dword6.B0);
            this.Append(dword6.B1);
            this.Append(dword6.B2);
            this.Append(dword6.B3);
            byte[] buffer = this.GData();
            this.Reset();
            long num2 = buffer.Length / 0x40;
            dwordArray[0] = this.ToDWORD(0x5a827999);
            dwordArray[1] = this.ToDWORD(0x6ed9eba1);
            dwordArray[2] = this.ToDWORD(0x8f1bbcdc);
            dwordArray[3] = this.ToDWORD(0xca62c1d6);
            dwordArray2[0] = this.ToDWORD(0x67452301);
            dwordArray2[1] = this.ToDWORD(0xefcdab89);
            dwordArray2[2] = this.ToDWORD(0x98badcfe);
            dwordArray2[3] = this.ToDWORD(0x10325476);
            dwordArray2[4] = this.ToDWORD(0xc3d2e1f0);
            for (int j = 0; j < num2; j++)
            {
                byte[] buffer2 = new byte[0x40];
                for (int k = j * 0x40; k < ((j + 1) * 0x40); k++)
                {
                    buffer2[k % 0x40] = buffer[k];
                }
                int index = 0;
                while (index <= 15)
                {
                    dwordArray3[index].B0 = buffer2[index * 4];
                    dwordArray3[index].B1 = buffer2[(index * 4) + 1];
                    dwordArray3[index].B2 = buffer2[(index * 4) + 2];
                    dwordArray3[index].B3 = buffer2[(index * 4) + 3];
                    index++;
                }
                index = 0x10;
                while (index <= 0x4f)
                {
                    dwordArray3[index] = this.XOrW(this.XOrW(this.XOrW(dwordArray3[index - 3], dwordArray3[index - 8]), dwordArray3[index - 14]), dwordArray3[index - 0x10]);
                    index++;
                }
                DWORD w = dwordArray2[0];
                DWORD b = dwordArray2[1];
                DWORD c = dwordArray2[2];
                DWORD d = dwordArray2[3];
                DWORD dword5 = dwordArray2[4];
                for (index = 0; index <= 0x4f; index++)
                {
                    DWORD dword7 = this.AddW(this.AddW(this.AddW(this.AddW(this.CircShiftLeftW(w, 5), this.F(index, b, c, d)), dword5), dwordArray3[index]), dwordArray[index / 20]);
                    dword5 = d;
                    d = c;
                    c = this.CircShiftLeftW(b, 30);
                    b = w;
                    w = dword7;
                }
                dwordArray2[0] = this.AddW(dwordArray2[0], w);
                dwordArray2[1] = this.AddW(dwordArray2[1], b);
                dwordArray2[2] = this.AddW(dwordArray2[2], c);
                dwordArray2[3] = this.AddW(dwordArray2[3], d);
                dwordArray2[4] = this.AddW(dwordArray2[4], dword5);
            }
            return new byte[] { dwordArray2[0].B3, dwordArray2[0].B2, dwordArray2[0].B1, dwordArray2[0].B0, dwordArray2[1].B3, dwordArray2[1].B2, dwordArray2[1].B1, dwordArray2[1].B0, dwordArray2[2].B3, dwordArray2[2].B2, dwordArray2[2].B1, dwordArray2[2].B0, dwordArray2[3].B3, dwordArray2[3].B2, dwordArray2[3].B1, dwordArray2[3].B0 };
        }

        private string Space(long count)
        {
            string str = "";
            for (int i = 0; i < count; i++)
            {
                str = str + " ";
            }
            return str;
        }

        public string String(long times, string repeat)
        {
            string str = "";
            for (long i = 0L; i < times; i += 1L)
            {
                str = str + repeat;
            }
            return str;
        }

        private DWORD ToDWORD(uint n)
        {
            DWORD dword;
            dword.B0 = (byte)(n >> 0x18);
            dword.B1 = (byte)(n >> 0x10);
            dword.B2 = (byte)(n >> 8);
            dword.B3 = (byte)n;
            return dword;
        }

        private DWORD XOrW(DWORD w1, DWORD w2)
        {
            return new DWORD { B0 = (byte)(w1.B0 ^ w2.B0), B1 = (byte)(w1.B1 ^ w2.B1), B2 = (byte)(w1.B2 ^ w2.B2), B3 = (byte)(w1.B3 ^ w2.B3) };
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DWORD
        {
            public byte B0;
            public byte B1;
            public byte B2;
            public byte B3;
        }
    }
}
