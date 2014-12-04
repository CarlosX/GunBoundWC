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
        
        static void Main(string[] args)
        {
            Program pro = new Program();

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
