namespace DeviceManager
{
    public class Program
    {
        private static DeviceEventReceiver _deviceEventReceiver;
        private static TreeEventReceiver _treeEventReceiver;
        
        private static void Init()
        {
            _deviceEventReceiver = new DeviceEventReceiver();
            _treeEventReceiver = new TreeEventReceiver();

        }

        private static bool skip = false;

        private static void Wait(int time)
        {
            if (!skip)
            {
                Thread.Sleep(time);
            }
        }

        private static void MonitorKeypress()
        {
            new Thread(() => 
            {
                Thread.CurrentThread.IsBackground = true; 
                Console.ReadKey();
                skip = true;
                Console.WriteLine("Skipping..."); 
            }).Start();
        }
        
        static void Main(string[] args)
        {
            Init();
            Console.WriteLine("Hello World!");
            LedPanel panel = new LedPanel("Main Entrance Panel", "Welcome to our facility!");
            LedPanel panel2 = new("Back Entrance Panel", "Authorized Personnel Only");
            Console.WriteLine(panel.Message);
            Console.WriteLine(panel);
            Console.WriteLine(panel2);

            panel.Message = "New Message!";
            panel.Name = "New Panel";

            MonitorKeypress();
            Wait(1000);
            
            // ______________TREE_______________________________________________ 
            
            var tree = new DeviceTree();
            _deviceEventReceiver.Register(tree.DeviceEventCondition);
            _treeEventReceiver.Register(tree.TreeEventCondition);

            // Přidání skupin
            tree.AddGroup("Office Devices");
            Wait(1000);
            tree.AddGroup("Warehouse Devices");

            // Přidání zařízení
            var ledPanel1 = new LedPanel("Front Desk Display", "Welcome!");
            var ledPanel2 = new LedPanel("Loading Dock Display", "Waiting for cargo...");
            var sensor = new LedPanel("Temperature Sensor 1", "Temperature = Cold"); 
            var ledPanel3 = new LedPanel("Unconnected Display", "Hello...");

            tree.AddDeviceToGroup("Office Devices", ledPanel1);
            Wait(1000);
            tree.AddDeviceToGroup("Warehouse Devices", ledPanel2);
            Wait(1000);
            tree.AddDeviceToGroup("Office Devices", sensor);

            Door door = new Door("Tiny door");
            Console.WriteLine(door);
            Wait(2000);
            tree.AddDeviceToGroup("Office Devices", door);
            Wait(2000);
            tree.MoveDeviceToGroup("Warehouse Devices", door);
            Wait(2000);
            tree.RemoveDeviceFromGroup("Warehouse Devices", door.Id);
            Wait(2000);
            var speaker = new Speaker("Big Speaker", Speaker.SoundType.Music,2.0);
            tree.AddDeviceToGroup("Office Devices", speaker);
            Wait(2000);
            speaker.Sound = Speaker.SoundType.Alarm;
            speaker.Volume = 3.5;
            var reader = new CardReader("FastReader", "A01234DE7FFF");
            tree.AddDeviceToGroup("Office Devices", reader);
            tree.DisplayTree();
            door.CurrentState = Door.State.Open | Door.State.OpenedForcibly;
            door.CurrentState = 0;
            door.Locked = true;
            door.Open = true;
            Console.WriteLine(door.GetCurrentState());
            ledPanel1.Id = "100";
            ledPanel1.Id = "8";
            Console.WriteLine("Press any key to exit");
            tree.RemoveDeviceFromGroup("Office Devices", ledPanel1.Id);
            ledPanel1.Id = "8";
            Wait(1000);
            CommandPrompt.Run(tree);
        }
    }
}
