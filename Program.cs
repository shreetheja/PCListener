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
            DataAboutUser.Initialize();
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
                    if (_nextLoop > DateTime.Now)
                    {
                        // If the execution time for the next tick is in the future, aka the server is NOT running behind
                        Thread.Sleep(_nextLoop - DateTime.Now); // Let the thread sleep until it's needed again.
                    }
                }
            }
        }
    }
}
