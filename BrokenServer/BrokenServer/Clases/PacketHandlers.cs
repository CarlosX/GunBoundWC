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
                Register(0x0000, 0, true, new OnPacketReceive(PacketHandlers.Empty));
            }
            public static void Empty(Client state, PacketReader2 pv)
            {

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
