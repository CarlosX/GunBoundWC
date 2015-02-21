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

                byte[] desuser = pv.ReadBytes(0x10);
                byte[] bffuser = Crypto.DecryptStaticBuffer(desuser);
                string username = Utils.GetASCIIZ(bffuser);
                LogConsole.Show(LogType.DEBUG, "Username: {0}", username);

                byte[] buffer2 = pv.ReadBytes(0x10);


                state.UserInfo = new UserInfo(username, state);
                if (state.UserInfo.Exist() >= 1)
                {
                    byte[] buff2 = Crypto.DecryptStaticBuffer(buffer2);
                    LogConsole.HexDump(buff2, "", 16);

                    PacketReader2 reader = new PacketReader2(buff2, 0x10);
                    reader.Seek(0, SeekOrigin.Begin);
                    state.AuthDWORD = reader.ReadUInt32();
                    LogConsole.Show(LogType.DEBUG, "AuthDWORD: {0}", state.AuthDWORD );

                    //state.Crypto = new Crypto(username, state.UserInfo.Password, state.AuthDWORD);
                    state.Crypto = new Crypto(username, "123456", state.AuthDWORD);
                    byte[] input = pv.ReadBytes(0x20);
                    byte[] output = new byte[0x18];

                    LogConsole.HexDump(input, "", 16);

                    if (!state.Crypto.PacketDecrypt(input, ref output, 0x1012))
                    {
                        LogConsole.Show(LogType.DEBUG, (string.Concat(new object[] { "Failed login: '", state.UserInfo.Username })));
                        state.Send(new LoginResponse(LoginResponseCode.BadPass));
                    }
                    else
                    {
                        reader = new PacketReader2(output, output.Length);
                        reader.Seek(0, SeekOrigin.Begin);
                        byte[] buffpas = reader.ReadBytes(20);
                        string password = Utils.GetASCIIZ(buffpas);
                        uint num = reader.ReadUInt32();
                        LogConsole.Show(LogType.DEBUG, "password: {0}", password);

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
