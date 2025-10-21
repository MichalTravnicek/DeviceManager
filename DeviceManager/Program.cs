namespace DeviceManager
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            LedPanel panel = new LedPanel("Main Entrance Panel", "Welcome to our facility!");
            LedPanel panel2 = new("Back Entrance Panel", "Authorized Personnel Only");
            Console.WriteLine(panel.Message);
            Console.WriteLine(panel);
            Console.WriteLine(panel2);

            panel.Message = "New Message!";
            panel.Name = "New Panel";

            Thread.Sleep(1000);
            
            // ______________TREE_______________________________________________ 
            
            var tree = new DeviceTree();

            // Přidání skupin
            tree.AddGroup("Office Devices");
            tree.AddGroup("Warehouse Devices");

            // Přidání zařízení
            var ledPanel1 = new LedPanel("Front Desk Display", "Welcome!");
            var ledPanel2 = new LedPanel("Loading Dock Display", "Waiting for cargo...");
            var sensor = new LedPanel("Temperature Sensor 1", "Temperature = Cold"); 
            var ledPanel3 = new LedPanel("Unconnected Display", "Hello...");

            tree.AddDeviceToGroup("Office Devices", ledPanel1);
            tree.AddDeviceToGroup("Warehouse Devices", ledPanel2);
            tree.AddDeviceToGroup("Office Devices", sensor);

            tree.DisplayTree();

            Door door = new Door("Tiny door");
            Console.WriteLine(door);
            tree.AddDeviceToGroup("Office Devices", door);
            door.CurrentState = Door.State.Open | Door.State.OpenedForcibly;
            door.CurrentState = 0;
            door.Locked = true;
            door.Open = true;
            Console.WriteLine(door.GetCurrentState());
            Console.ReadKey();
        }
    }
}
