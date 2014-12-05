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
        private static readonly Hashtable Handlers = new Hashtable();
        public class PacketHandlers
        {
            static PacketHandlers()
            {
                Register(0x1310, 0, true, new OnPacketReceive(PacketHandlers.Pack1));
            }
            public static void Empty(Client state, PacketReader2 pv)
            {

            }

            public static void Pack1(Client state, PacketReader2 pv)
            {
                LogConsole.Show(LogType.DEBUG, "OpCode->Pack1()");
                byte[] unk1 = pv.ReadBytes(0x10);
                byte[] buffer2 = pv.ReadBytes(0x10);
                byte[] xd2 = Crypto.DecryptStaticBuffer(unk1);
                string aSCIIZ = Utils.GetASCIIZ(xd2);
                LogConsole.Show(LogType.DEBUG, "Login: {0}", aSCIIZ);

                state.Send(new LoginResponse(LoginResponseCode.Prohibited, -1, "HAHAHAHAHAHAHAHA!!"));
            }

            public static PacketHandler GetHandler(int PacketID)
            {
                return (PacketHandler)Handlers[PacketID];
            }

            public static void Register(int PacketID, int length, bool loggedin, OnPacketReceive onReceive)
            {
                Handlers[PacketID] = new PacketHandler(PacketID, length, loggedin, onReceive);
            }
        }
    }
}
