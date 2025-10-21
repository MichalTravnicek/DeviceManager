namespace DeviceManager
{
    public class DeviceTree : IDeviceTree
    {
        private readonly List<DeviceGroup> _groups = new List<DeviceGroup>();
        private readonly Dictionary<string, Device> _devicesById = new Dictionary<string, Device>();
        
        public DeviceTree()
        {  
        }

        public void AddGroup(string name)
        {
            if (_groups.Any(g => g.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine($"Error: Group '{name}' already exists.");
                return;
            }
            _groups.Add(new DeviceGroup(name));
        }

        public void RemoveGroup(string name)
        {
            var groupToRemove = _groups.FirstOrDefault(g => g.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (groupToRemove == null)
            {
                Console.WriteLine($"Error: Group '{name}' not found.");
                return;
            }

            foreach (var device in groupToRemove.Devices)
            {
                _devicesById.Remove(device.Id);
            }
            _groups.Remove(groupToRemove);
        }

        public void AddDeviceToGroup(string groupName, Device device)
        {
            var group = _groups.FirstOrDefault(g => g.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase));
            if (group == null)
            {
                Console.WriteLine($"Error: Group '{groupName}' not found.");
                return;
            }

            if (_devicesById.ContainsKey(device.Id))
            {
                Console.WriteLine($"Error: Device with ID {device.Id} already exists.");
                return;
            }

            group.Devices.Add(device);
            _devicesById[device.Id] = device;
        }

        public void MoveDeviceToGroup(string groupName, Device device)
        {
            throw new NotImplementedException();
        }

        public void RemoveDeviceFromGroup(string groupName, string deviceId)
        {
            var group = _groups.FirstOrDefault(g => g.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase));
            if (group == null)
            {
                Console.WriteLine($"Error: Group '{groupName}' not found.");
                return;
            }

            var deviceToRemove = group.Devices.FirstOrDefault(d => d.Id == deviceId);
            if (deviceToRemove == null)
            {
                Console.WriteLine($"Error: Device with ID {deviceId} not found in group '{groupName}'.");
                return;
            }

            group.Devices.Remove(deviceToRemove);
            _devicesById.Remove(deviceId);
        }

        public void DisplayTree()
        {
            Console.WriteLine("--- Device Tree Structure ---");
            if (_groups.Count == 0)
            {
                Console.WriteLine("No groups found.");
                return;
            }

            foreach (var group in _groups)
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

        public List<string> GetGroups()
        {
            return _groups.Select(g => g.Name).ToList();
        }

        public Device GetDevice(string currentDeviceId)
        {
            return !_devicesById.TryGetValue(currentDeviceId, out var device) ? 
                throw new KeyNotFoundException($"Device with ID {currentDeviceId} not found.") : device;
        }
    }

}
