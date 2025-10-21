using Sharprompt;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;

namespace DeviceManager
{
    internal class CommandPrompt
    {
        static OrderedDictionary CommandMap = new OrderedDictionary()
        {
            { Command.AddGroup, true },
            { Command.SelectGroup, true },
            { Command.RemoveGroup, true },
            { Command.SelectDevice, false },
            { Command.AddDevice, false },
            { Command.EditDevice, false },
            { Command.RemoveDevice, false },
            { Command.PrintTree,  true },
            { Command.Exit, true }
        };

        enum Command
        {
            AddGroup = 1,
            SelectGroup = 2,
            RemoveGroup = 3,
            SelectDevice = 4,
            AddDevice = 5,
            EditDevice = 6,
            RemoveDevice = 7,
            PrintTree  = 8,
            Exit = 16
        }

        static Dictionary<Command, Action<DeviceTree>> CommandActions = new Dictionary<Command, Action<DeviceTree>>();

        static bool exitRequested = false;

        static string _currentDeviceId = string.Empty;

        static string _currentGroup = string.Empty;


        static CommandPrompt()
        {
            CommandActions[Command.AddGroup] = tree => Console.WriteLine(" > > > Not yet implemented !!! 👉"); 
            CommandActions[Command.RemoveGroup] = tree => Console.WriteLine(" > > > Not yet implemented !!! 👉"); 
            CommandActions[Command.AddDevice] = (x) => AddDevice(x);
            CommandActions[Command.EditDevice] = (x) => EditDevice(x);
            CommandActions[Command.RemoveDevice] = (x) =>
            {
                if (string.IsNullOrEmpty(_currentDeviceId))
                {
                    Console.WriteLine("No device selected for removal.");
                    return;
                }
                if (string.IsNullOrEmpty(_currentGroup))
                {
                    Console.WriteLine("No group selected for device removal.");
                    return;
                }
                if (!Prompt.Confirm("Really delete?", defaultValue: true))
                {
                    return;
                }
                x.RemoveDeviceFromGroup(_currentGroup, _currentDeviceId);
                CommandMap[Command.EditDevice] = false;
                CommandMap[Command.RemoveDevice] = false;
                _currentDeviceId = string.Empty;
                CommandMap[Command.SelectDevice] = x.DeviceGroups.FirstOrDefault(g
                    => g.Name == _currentGroup)?.Devices.Count > 0;

            };
            CommandActions[Command.SelectDevice] = (x) => {
                _currentDeviceId = Prompt.Select<Device>("Select device to edit:",
                    x.DeviceGroups.First(g => g.Name == _currentGroup).Devices).Id;
                CommandMap[Command.EditDevice] = true;
                CommandMap[Command.RemoveDevice] = true;
            };
            CommandActions[Command.SelectGroup] = (x) =>
            {              
                _currentGroup = Prompt.Select<string>("Select a group to edit:", x.DeviceGroups.Select(x => x.Name));
                CommandMap[Command.AddDevice] = true;
                CommandMap[Command.SelectDevice] = x.DeviceGroups.FirstOrDefault(g => g.Name == _currentGroup)?.Devices.Count > 0;
            };
            CommandActions[Command.PrintTree] = (x) => x.DisplayTree();
            CommandActions[Command.Exit] = (x) => { 
                exitRequested = Prompt.Confirm("Really exit?", defaultValue: true); 
            };
        }

        internal static void Run(DeviceTree tree)
        {
            Console.OutputEncoding = Encoding.UTF8;

            while (!exitRequested)
            {
                if (tree.GetDeviceCount() == 0)
                {
                    Console.WriteLine("Device tree is empty. Please add devices to the tree before editing.");
                }
                if (!string.IsNullOrEmpty(_currentGroup))
                {
                    Console.WriteLine(">>> Selected group:" + _currentGroup);
                }
                if (!string.IsNullOrEmpty(_currentDeviceId))
                {
                    Console.WriteLine(">>> Selected device:" + tree.GetDevice(_currentDeviceId));
                }
                
                var command = Prompt.Select<Command>("Select a command:", GetAvailableCommands(tree));
                CommandActions[command].Invoke(tree);
            }
        }

        private static List<Command> GetAvailableCommands(DeviceTree tree)
        {
            var result = CommandMap.Cast<DictionaryEntry>()
                .Where(x => x.Value.Equals(true))
                .Select(x => x.Key);
            return result.Cast<Command>().ToList();
        }

        private static void AddDevice(DeviceTree tree)
        {
            Console.WriteLine("Adding new device...");
            var groupName = Prompt.Select<string>("Select group to add device to:", tree.DeviceGroups.Select(x => x.Name));
            var deviceType = Prompt.Select<string>("Select device type to add:", new[] 
                { "LedPanel", "Door", "Speaker", "CardReader" });
            Device newDevice;
            switch (deviceType)
            {
                case "LedPanel":
                    var panelName = Prompt.Input<string>("Enter LED Panel name:");
                    var panelMessage = Prompt.Input<string>("Enter LED Panel message:");
                    newDevice = new LedPanel(panelName, panelMessage);
                    break;
                case "Door":
                    var doorName = Prompt.Input<string>("Enter Door name:");
                    newDevice = new Door(doorName);
                    break;
                default:
                    Console.WriteLine("Unknown device type.");
                    return;
            }
            tree.AddDeviceToGroup(groupName, newDevice);
            Console.WriteLine($"Device {newDevice} added to group {groupName}.");
        }

        private static void EditDevice(DeviceTree tree)
        {
            Console.WriteLine("Editing device...");
            if (string.IsNullOrEmpty(_currentDeviceId))
            {
                Console.WriteLine("No device selected for editing.");
                return;
            }
            var device = tree.GetDevice(_currentDeviceId);
            var editableProps = ReflectionTool.GetEditableProperties(device).ToList();
            if (!editableProps.Any())
            {
                Console.WriteLine("No editable properties found for device: " + _currentDeviceId);
                return;
            }
            var property = Prompt.Select<string>("Select property to edit:", editableProps.Select(x => x.Name));
            PropertyInfo propertyInfo = ReflectionTool.GetPropertyByName(device, property);
            var type = propertyInfo.PropertyType;
            if (type == typeof(bool)) {
                var input = Prompt.Select<string>($"Enter new value for {propertyInfo.Name}:", new[]{"True","False"});
                propertyInfo.SetValue(device, Convert.ChangeType(input, propertyInfo.PropertyType));
            }
            var input2 = Prompt.Input<string>($"Enter new value for {propertyInfo.Name}:", device.GetType().GetProperty(propertyInfo.Name)?.GetValue(device)?.ToString());
            propertyInfo.SetValue(device, Convert.ChangeType(input2, propertyInfo.PropertyType));
        }
    }
}
