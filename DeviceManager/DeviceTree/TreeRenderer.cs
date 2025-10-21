namespace DeviceManager;

public class TreeRenderer(DeviceTree tree)
{
    public void DisplayTree()
    {
        Console.WriteLine("--- Device Tree Structure ---");
        if (tree.DeviceGroups.Count == 0)
        {
            Console.WriteLine("No groups found.");
            return;
        }

        foreach (var group in tree.DeviceGroups)
        {
            Console.WriteLine($"\n└─ Group: {group.Name}");
            if (group.Devices.Count == 0)
            {
                Console.WriteLine("  No devices in this group.");
                continue;
            }
            foreach (var device in group.Devices)
            {
                Console.WriteLine($"  └── {device.GetCurrentState()}");
            }
        }
        Console.WriteLine("\n--- End of Tree ---");
    }
    
}