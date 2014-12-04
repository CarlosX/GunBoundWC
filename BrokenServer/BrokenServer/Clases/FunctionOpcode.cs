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
                /////////////////
                ////////////////
                ///////////////
                //////////////
                /////////////
                ////////////
                ///////////
                //////////
                /////////
                ////////
                ///////
                //////
                /////
                ////
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

