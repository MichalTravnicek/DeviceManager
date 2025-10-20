using System;
using System.Collections.Specialized;
using System.Threading.Channels;

namespace DeviceManager
{
    public class Program
    {
        static void Main(string[] args)
        {
            LedPanel panel = new LedPanel("6CC47701-AA90-4656-A964-4DFD92BFBF60","Hello World");
            panel.Message = "Watch this";
            Console.WriteLine(panel);
            Door door = new Door("Main door");
            door.Locked = false;
            door.OpenedForcibly = true;
            Console.WriteLine(door);
            Console.WriteLine("Device Manager started.");
            Console.ReadKey();
        }
    }
}
