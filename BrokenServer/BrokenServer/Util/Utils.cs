using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace BrokenServer
{
    public partial class Systems
    {

        public class Utils
        {
            private static System.Text.Encoding m_Encoding = System.Text.Encoding.GetEncoding("Latin1");
            public static unsafe string GetASCIIZ(byte[] buffer)
            {
                string str;
                fixed (byte* numRef = buffer)
                {
                    str = new string((sbyte*)numRef);
                }
                return str;
            }
            public static byte[] GetBytes(long s)
            {
                byte[] buffer = new byte[8];
                ulong num = (ulong)s;
                for (int i = 1; i <= 8; i++)
                {
                    buffer[i - 1] = (byte)(num >> (0x40 - (i * 8)));
                }
                return buffer;
            }

            public static byte[] GetBytes(string s)
            {
                byte[] buffer = new byte[s.Length];
                int num = 0;
                foreach (char ch in s)
                {
                    buffer[num++] = (byte)ch;
                }
                return buffer;
            }

            public static byte[] GetBytes(ushort u)
            {
                byte[] buffer = new byte[2];
                for (int i = 1; i <= 2; i++)
                {
                    buffer[i - 1] = (byte)(u >> (0x10 - (i * 8)));
                }
                return buffer;
            }

            public static byte[] GetBytes(uint t)
            {
                byte[] buffer = new byte[4];
                for (int i = 1; i <= 4; i++)
                {
                    buffer[i - 1] = (byte)(t >> (0x20 - (i * 8)));
                }
                return buffer;
            }

            public static System.Text.Encoding Encoding
            {
                get
                {
                    return m_Encoding;
                }
            }
        }
    }
}
