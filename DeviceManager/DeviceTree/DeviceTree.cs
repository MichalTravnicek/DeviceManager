using System.Collections.Specialized;
using CommunityToolkit.Mvvm.Messaging;

namespace DeviceManager
{
    public class DeviceTree : IDeviceTree
    {
        public readonly string TreeId = "Tree 001";
        private readonly List<DeviceGroup> _groups = new List<DeviceGroup>();
        private readonly Dictionary<string, Device> _devicesById = new Dictionary<string, Device>();
        private readonly Dictionary<Device, string> _deviceGroups = new Dictionary<Device, string>();
        
        private event NotifyCollectionChangedEventHandler TreeChanged;

        internal List<DeviceGroup> Groups => _groups;
        private TreeRenderer _treeRenderer;
        
        private void OnTreeUpdate(object? sender, NotifyCollectionChangedEventArgs e)
        {
            Console.WriteLine("Tree changed$$$:" + e.Action);
            MessengerImpl.Get().Send(new TreeMessage(TreeId, "DeviceTree modified:"+e.Action));
        }

        public bool DeviceEventCondition(DeviceMessage x) => _devicesById.ContainsKey(x.DeviceId);
        public bool TreeEventCondition(TreeMessage x) => TreeId.Equals(x.TreeId);
        
        public DeviceTree()
        {
            _treeRenderer = new TreeRenderer(this);
            TreeChanged += OnTreeUpdate;
        }

        private void AddGroupInternal(string name)
        {
            if (_groups.Any(g => g.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine($"Error: Group '{name}' already exists.");
                return;
            }
            _groups.Add(new DeviceGroup(name));
            TreeChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add, _groups.Last()));
        }

        private DeviceGroup GetGroupInternal(string name)
        {
            var group = _groups.FirstOrDefault(g => g.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (group == null)
            {
                Console.WriteLine($"Error: Group '{name}' not found.");
                throw new Exception("Group not found.");
            }
            return group;
        }
        
        private string GetGroupInternal(Device device)
        {
            return _deviceGroups[device];
        }

        private void RemoveGroupInternal(string name)
        {
            var groupToRemove = GetGroupInternal(name);

            foreach (var device in groupToRemove.Devices)
            {
                _devicesById.Remove(device.Id);
            }
            _groups.Remove(groupToRemove);
            TreeChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Remove, groupToRemove));
        }

        private Device GetDeviceInternal(DeviceGroup group, string deviceId)
        {
            var device = group.Devices.FirstOrDefault(d => d.Id == deviceId);
            if (device == null)
            {
                Console.WriteLine($"Error: Device with ID {deviceId} not found in group '{group.Name}'.");
                throw new Exception("Device not found:" + deviceId);
            }
            return device;
        }

        private void AddDeviceInternal(string groupName, Device device)
        {
            var group = GetGroupInternal(groupName);

            if (_devicesById.ContainsKey(device.Id))
            {
                Console.WriteLine($"Error: Device with ID {device.Id} already exists.");
                return;
            }

            group.Devices.Add(device);
            _deviceGroups.Add(device, groupName);
            _devicesById[device.Id] = device;
            TreeChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Add, device));
        }

        private void MoveDeviceInternal(string sourceGroupName, string targeGroupName, string deviceId)
        {
            var group = GetGroupInternal(sourceGroupName);
            var targetGroup = GetGroupInternal(targeGroupName);

            var deviceToMove = GetDeviceInternal(group,deviceId);

            group.Devices.Remove(deviceToMove);
            _deviceGroups[deviceToMove] = targeGroupName;
            targetGroup.Devices.Add(deviceToMove);
            TreeChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
                // action Move is not supported in handler
                NotifyCollectionChangedAction.Replace, sourceGroupName, targeGroupName));
        }
        
        private void RemoveDeviceInternal(string groupName, string deviceId)
        {
            var group = GetGroupInternal(groupName);
            var deviceToRemove = GetDeviceInternal(group, deviceId);

            group.Devices.Remove(deviceToRemove);
            _deviceGroups.Remove(deviceToRemove);
            _devicesById.Remove(deviceId);  
            TreeChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Remove, deviceToRemove));
        }

        public void AddGroup(string name)
        {
            AddGroupInternal(name);
        }

        public void RemoveGroup(string name)
        {
            RemoveGroupInternal(name);
        }

        public void AddDeviceToGroup(string groupName, Device device)
        {
            AddDeviceInternal(groupName, device);
        }

        public void MoveDeviceToGroup(string groupName, Device device)
        {
            var sourceGroup = GetGroupInternal(device);
            MoveDeviceInternal(sourceGroup, groupName, device.Id);
        }

        public void RemoveDeviceFromGroup(string groupName, string deviceId)
        {
            RemoveDeviceInternal(groupName, deviceId);
            
        }

        public void DisplayTree()
        {
            _treeRenderer.DisplayTree();
        }

        public List<string> GetGroups()
        {
            return _groups.Select(g => g.Name).ToList();
        }

        public bool GroupContains(string groupName, string deviceId)
        {
            var group = GetGroupInternal(groupName);
            var device = GetDeviceInternal(group, deviceId);
            _deviceGroups.TryGetValue(device, out var foundDeviceGroup);
            Console.WriteLine(foundDeviceGroup);
            return foundDeviceGroup != null && foundDeviceGroup.Equals(groupName);
        }

        public Device GetDevice(string deviceId)
        {
            return !_devicesById.TryGetValue(deviceId, out var device) ? 
                throw new KeyNotFoundException($"Device with ID {deviceId} not found.") : device;
        }
    }

}
