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


                state.UserInfo = new UserInfo(aSCIIZ, state);
                if (state.UserInfo.Exist() >= 1)
                {
                    PacketReader2 reader = new PacketReader2(Crypto.DecryptStaticBuffer(buffer2), 0x10);
                    reader.Seek(0, SeekOrigin.Begin);
                    state.AuthDWORD = reader.ReadUInt32();

                    state.Crypto = new Crypto(aSCIIZ, state.UserInfo.Password, state.AuthDWORD);
                    byte[] input = pv.ReadBytes(0x20);
                    byte[] output = new byte[0x18];

                    if (!state.Crypto.PacketDecrypt(input, ref output, 0x1012))
                    {
                        LogConsole.Show(LogType.DEBUG, (string.Concat(new object[] { "Failed login: '", state.UserInfo.Username })));
                        state.Send(new LoginResponse(LoginResponseCode.BadPass));
                    }
                    else
                    {
                        reader = new PacketReader2(output, output.Length);
                        reader.Seek(0, SeekOrigin.Begin);
                        string password = Utils.GetASCIIZ(reader.ReadBytes(20));
                        uint num = reader.ReadUInt32();
                        if (state.UserInfo.Password == password)
                        {
                            if (num >= Program.VER)
                            {
                                if (state.UserInfo.Ban)
                                {
                                    state.Send(new LoginResponse(LoginResponseCode.Prohibited, -1, state.UserInfo.BanReason));
                                }
                                else
                                {
                                    state.Send(new LoginResponse(LoginResponseCode.LoginSuccessful));
                                }
                            }
                            else
                            {
                                state.Send(new LoginResponse(LoginResponseCode.IncorrectVersion));
                            }
                        }
                        else
                        {
                            state.Send(new LoginResponse(LoginResponseCode.BadPass));
                        }
                    }
                }
                else
                {
                    state.Send(new LoginResponse(LoginResponseCode.UnregisteredID));
                }

                //state.Send(new LoginResponse(LoginResponseCode.Prohibited, -1, "HAHAHAHAHAHAHAHA!!"));
                //state.Send(new LoginResponse(LoginResponseCode.LoginSuccessful));
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
