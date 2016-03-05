using ProcessWatch;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ProcessTrackerFactory fact = new ProcessTrackerFactory();
            var hook = fact.CreateTracker(new[] { new SimpleTrackableProgram(@"C:\WINDOWS\system32\notepad.exe",
                new Action(() => Console.WriteLine("Start")),
                new Action(() => Console.WriteLine("Stop"))) });

            while (true)
            {
                Console.ReadKey();
                hook.UpdateProcesses();
                Console.WriteLine("updated");
            }
        }
    }
}