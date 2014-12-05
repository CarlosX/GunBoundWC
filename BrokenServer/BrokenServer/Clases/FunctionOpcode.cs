using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace BrokenServer
{
    public partial class Systems
    {
        private static ushort[] m_CryptedPackets = new ushort[] { 0x1032, 0x1020, 0x2010, 0x201f, 0x3432, 0x4200, 0x4102, 0x4106, 0x4104, 0x4410, 0x4412 };
        private static BufferPool m_Buffers = new BufferPool(4, 0x400);

        public static void OpCode(Decode state, Client ed)
        {
            try
            {
                LogConsole.Show(LogType.DEBUG, "OpCode()");
                Systems sys = (Systems)state.Packet;
                sys.PacketInformation = state;

                ByteQueue queue = ed.queue;
                
                int length = queue.Length;
                LogConsole.Show(LogType.DEBUG, "OpCode() {0}", length);
                while ((length > 0))
                {
                    byte[] buffer;
                    int packetID = queue.GetPacketID();
                    int packetLength = queue.GetPacketLength();
                    int packetControlCode = queue.GetPacketControlCode();

                    LogConsole.Show(LogType.DEBUG, "PacketControl: {0} PacketID: 0x{1:X2} Length: {2}", packetControlCode, packetID, packetLength );
                    LogConsole.HexDump(state.buffer, "", 16);

                    PacketHandler handler = PacketHandlers.GetHandler(packetID);
                    if (handler == null)
                    {
                        byte[] buffer2 = new byte[length];
                        length = queue.Dequeue(buffer2, 0, length);
                        LogConsole.Show(LogType.DEBUG, "Client: {0}: Unhandled packet 0x{1:X2}", new object[] { state, packetID });
                        break;
                    }

                    int size = handler.Length;
                    if (length >= 4)
                    {
                        size = packetLength;
                        if (packetLength >= 4)
                        {
                            if (length < size)
                            {
                                break;
                            }

                            if (0x400 >= size)
                            {
                                buffer = m_Buffers.AquireBuffer();
                            }
                            else
                            {
                                buffer = new byte[size];
                            }

                            size = queue.Dequeue(buffer, 0, size);

                            ushort packetid = ByteQueue.GetPacketID(buffer);

                            bool flag = IsCrypted(packetid);
                            if (flag)
                            {
                                LogConsole.Show(LogType.DEBUG, "Crypted Packet 0x{0:X4}", new object[] { packetid });
                            }

                            try
                            {
                                PacketReader2 pr = new PacketReader2(buffer, size);
                                handler.OnReceive(ed, pr);
                            }
                            catch { break; }

                            length = queue.Length;
                            if ((0x400 >= size) && !flag)
                            {
                                m_Buffers.ReleaseBuffer(buffer);
                                break;
                            }
                        }
                    }

                    length = 0;
                }
            }
            catch (Exception)
            {
            }
        }

        public static bool IsCrypted(ushort packetid)
        {
            foreach (ushort num in m_CryptedPackets)
            {
                if (packetid == num)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

