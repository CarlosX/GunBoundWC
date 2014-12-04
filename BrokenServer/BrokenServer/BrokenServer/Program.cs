using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;


namespace BrokenServer
{
    class Program
    {
        private static Thread _LoopThrreading;
        public static int VER = 1;
        public static bool DEBUG = false;
        static void Main(string[] args)
        {
            Program pro = new Program();
            LogConsole._Load();
            Systems.Ini ini = null;

            #region Default Settings
            int LSPort = 00000;
            int IPCPort = 00000;
            string LSIP = "127.0.0.1";
            string IPCIP = "127.0.0.1";

            string m_host = "localhost";
            string m_user = "root";
            string m_pass = "";
            string m_db = "";
            int m_port = 3306;
            #endregion

            #region Load Settings
            try
            {
                if (File.Exists(Environment.CurrentDirectory + @"\Settings\Settings.ini"))
                {
                    ini = new Systems.Ini(Environment.CurrentDirectory + @"\Settings\Settings.ini");
                    LSPort = Convert.ToInt32(ini.GetValue("Server", "port", 12345));
                    LSIP = ini.GetValue("Server", "ip", "127.0.0.1").ToString();
                    VER = ini.GetValue("Server", "ver", 1);

                    IPCPort = Convert.ToInt32(ini.GetValue("IPC", "port", 40706));
                    IPCIP = ini.GetValue("IPC", "ip", "127.0.0.1").ToString();
                    DEBUG = Convert.ToBoolean(ini.GetValue("CONSOLE", "debug", "false"));

                    m_host = ini.GetValue("MySQL", "host", "localhost").ToString();
                    m_user = ini.GetValue("MySQL", "user", "root").ToString();
                    m_pass = ini.GetValue("MySQL", "pass", "").ToString();
                    m_db = ini.GetValue("MySQL", "data", "").ToString();
                    m_port = Convert.ToInt32(ini.GetValue("MySQL", "port", 3306));

                    ini = null;
                    LogConsole.Show(LogType.INFO, "Has loaded your ip settings successfully");
                }
                else
                {
                    LogConsole.Show(LogType.ALERT, "Settings.ini could not be found, using default setting");
                }
            }
            catch (Exception excc)
            {
                LogConsole.Show(LogType.ERROR, " {0}", excc.ToString());
                return;
            }
            #endregion

            //Systems.Crypto.Initialize();
            //Systems.Server net = new Systems.Server();
            //net.OnConnect += new Systems.Server.dConnect(pro._OnClientConnect);
            //net.OnError += new Systems.Server.dError(pro._ServerError);

            //try
            //{
            //    net.Start(LSIP, LSPort);
            //}
            //catch (Exception ex)
            //{
            //    LogConsole.Show(LogType.ERROR, "Starting Server error: {0}", ex);
            //}



            _LoopThrreading = new Thread(new ThreadStart(Program.LoopConsole));
            _LoopThrreading.Priority = ThreadPriority.BelowNormal;
            _LoopThrreading.Start();

            while (true)
            {
                Thread.Sleep(100);
            }

        }

        private static void LoopConsole()
        {
            while (true)
            {
                object[] totalMemory = new object[] { "BrokenServer | Ram Usage: ", GC.GetTotalMemory(false) / (long)1024, " KB | " };
                Console.Title = string.Concat(totalMemory);
                Thread.Sleep(1500);
            }
        }
    }
}
