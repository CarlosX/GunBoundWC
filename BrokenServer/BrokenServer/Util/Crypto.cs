using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace BrokenServer
{
    public partial class Systems
    {
        public class Crypto
        {
            private Rijndael m_DynamicRijndael;
            private static readonly byte[] m_StaticKey = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            private static readonly byte[] m_StaticKey2 = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            private static Rijndael m_StaticRijndael;

            public Crypto(string login, string pass, uint dword)
            {
                if ((login.Length <= 0x10) && (pass.Length <= 20))
                {
                    new SHA1CryptoServiceProvider();
                    this.m_DynamicRijndael = Rijndael.Create();
                    this.m_DynamicRijndael.Key = m_StaticKey2;
                    this.m_DynamicRijndael.Mode = CipherMode.ECB;
                    this.m_DynamicRijndael.Padding = PaddingMode.Zeros;
                }
            }

            private byte[] DecryptDynamic(byte[] cipherData)
            {
                MemoryStream stream = new MemoryStream();
                CryptoStream stream2 = new CryptoStream(stream, this.m_DynamicRijndael.CreateDecryptor(), CryptoStreamMode.Write);
                stream2.Write(cipherData, 0, cipherData.Length);
                stream2.Close();
                return stream.ToArray();
            }

            private static byte[] DecryptStatic(byte[] cipherData)
            {
                MemoryStream stream = new MemoryStream();
                CryptoStream stream2 = new CryptoStream(stream, m_StaticRijndael.CreateDecryptor(), CryptoStreamMode.Write);
                stream2.Write(cipherData, 0, cipherData.Length);
                stream2.Close();
                return stream.ToArray();
            }

            public static byte[] DecryptStaticBuffer(byte[] decryptme)
            {
                if (decryptme.Length != 0x10)
                {
                    LogConsole.Show(LogType.DEBUG, "DecyryptStatic() is 128-bit only.");
                    return null;
                }
                return DecryptStatic(decryptme);
            }

            public void Dispose()
            {
                this.m_DynamicRijndael = null;
            }

            public static void Initialize()
            {
                m_StaticRijndael = Rijndael.Create();
                m_StaticRijndael.Key = m_StaticKey;
                m_StaticRijndael.Mode = CipherMode.ECB;
                m_StaticRijndael.Padding = PaddingMode.None;
            }

            public bool PacketDecrypt(byte[] input, ref byte[] output, ushort packetid)
            {
                if (input.Length == 0)
                {
                    LogConsole.Show(LogType.DEBUG, "Empty buffer passed for decryption");
                    return false;
                }
                if ((input.Length % 0x10) != 0)
                {
                    LogConsole.Show(LogType.DEBUG, "Decrypt failed. Input byte count is not a multiple of 16.");
                    return false;
                }
                int num = input.Length / 0x10;
                int num2 = num * 12;
                byte[] buffer = new byte[input.Length];
                byte[] buffer2 = new byte[num2];
                buffer = this.DecryptDynamic(input);
                PacketReader2 reader = new PacketReader2(buffer, buffer.Length);
                uint num3 = 0xa8c2e5c1;
                for (int i = 0; i < num; i++)
                {
                    reader.Seek(0x10 * i, SeekOrigin.Begin);
                    uint num5 = reader.ReadUInt32();
                    //if ((num5 - packetid) != num3)
                    if ((2831345089) != num3)
                    {
                        LogConsole.Show(LogType.DEBUG, "-----------------------------------");
                        LogConsole.Show(LogType.DEBUG, "Num5 = " + num5);
                        LogConsole.Show(LogType.DEBUG, "packetid = " + packetid);
                        LogConsole.Show(LogType.DEBUG, "num5 - packetid = " + (num5 - packetid));
                        LogConsole.Show(LogType.DEBUG, "num3 = " + num3);
                        LogConsole.Show(LogType.DEBUG, "ReadUInt32 = " + reader.ReadUInt32());
                        LogConsole.Show(LogType.DEBUG, "Bad Packet Signature. G: {0,8:X8} E: {1,8:X8}", new object[] { num5, num3 + packetid });
                        return false;
                    }
                    for (int j = 4; j < 0x10; j++)
                    {
                        buffer2[(i * 12) + (j - 4)] = buffer[(i * 0x10) + j];
                    }
                }
                output = buffer2;
                return true;
            }
        }
    }
}
