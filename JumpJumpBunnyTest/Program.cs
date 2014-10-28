using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading;

using JumpJumpBunny;

namespace JumpJumpBunnyTest
{
    class Program
    {

        static HashSet<int> threadIDs = new HashSet<int>();
        static int lastAmountOfThreads = 0;
        static object thisLock = new Object();

        static void TestTimers(int amount, int maxInterval)
        {
            var rnd = new System.Random();
            
            for (var i = 0; i < amount; ++i)
            {
                var t = new System.Timers.Timer(rnd.Next(maxInterval) + 1);
                t.Elapsed += t_Elapsed;
                t.Start();
            }
        }

        static void t_Elapsed(object sender, ElapsedEventArgs e)
        {

            var id = Thread.CurrentThread.ManagedThreadId;
            if (!threadIDs.Contains(id))
            {
                threadIDs.Add(id);
            }
                
            if (threadIDs.Count > lastAmountOfThreads)
            {
                lock (thisLock)
                {
                    lastAmountOfThreads++;
                }
                Console.Write(threadIDs.Count+"|");
            }
        }

        static void Main(string[] args)
        {
           var grid = new HexGrid(1, 1.0f, 0.1f);
           var testCell = grid.Islands[0];
           while (Console.ReadLine() != "q") {
               testCell.Update();
               Console.Clear();
               Console.SetCursorPosition(0, 0);
               Console.Write(testCell.ToDebugString(), 0);
           }
            
        }
    }
}
