using System;
using System.Threading;

namespace RummyServer
{
    class Program
    {
        public static bool isRunning = false;
        static void Main(string[] args)
        {
            Console.Title = "Rummy Game Server";
            isRunning = true;

            Thread mainThread = new Thread(new ThreadStart(MainThread));
            mainThread.Start();
            Server.Start(5, 26950);
        }

        private static void MainThread()
        {
            Console.WriteLine($"Main Thread Started. Running at {Constants.TICKS_PER_SEC} TickRate. ");
            DateTime _nextLoop = DateTime.Now;
            while(isRunning)
            {
                while(_nextLoop < DateTime.Now)
                {
                    GameLogic.Update();
                   
                    _nextLoop = _nextLoop.AddMilliseconds(Constants.MS_PER_TICK);
                }
            }
        }
    }
}
