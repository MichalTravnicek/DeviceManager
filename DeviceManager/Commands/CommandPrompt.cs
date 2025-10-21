using Sharprompt;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;

namespace DeviceManager
{
    internal static class CommandPrompt
    {
        private static readonly OrderedDictionary ActiveCommands = new ()
        {
            { Command.AddGroup, true },
            { Command.SelectGroup, true },
            { Command.RemoveGroup, true },
            { Command.SelectDevice, false },
            { Command.AddDevice, false },
            { Command.EditDevice, false },
            { Command.MoveDevice, false },
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
            MoveDevice = 7,
            RemoveDevice = 8,
            PrintTree  = 9,
            Exit = 16
        }

        private static readonly Dictionary<Command, Action<DeviceTree>> CommandActions = new ();

        private static bool _exitRequested = false;

        private static string _currentDeviceId = string.Empty;

        private static string _currentGroup = string.Empty;


        static CommandPrompt()
        {
            CommandActions[Command.AddGroup] = AddGroup; 
            CommandActions[Command.SelectGroup] = SelectGroup;
            CommandActions[Command.RemoveGroup] = RemoveGroup;
            
            CommandActions[Command.AddDevice] = AddDevice;
            CommandActions[Command.SelectDevice] = SelectDevice;
            CommandActions[Command.EditDevice] = EditDevice;
            CommandActions[Command.MoveDevice] = MoveDeviceToGroup;
            CommandActions[Command.RemoveDevice] = RemoveDevice;
            
            CommandActions[Command.PrintTree] = (x) => x.DisplayTree();
            CommandActions[Command.Exit] = (x) => { 
                _exitRequested = Prompt.Confirm("Really exit?", defaultValue: true); 
            };
        }

        private static void SelectGroup(DeviceTree x)
        {
            _currentGroup = Prompt.Select<string>("Select a group to edit:", x.GetGroups());
            ActiveCommands[Command.AddDevice] = true;
            ActiveCommands[Command.SelectDevice] = x.DeviceGroups.FirstOrDefault(g => g.Name == _currentGroup)?.Devices.Count > 0;
        }
        
        private static void SelectDevice(DeviceTree tree)
        {
                _currentDeviceId = Prompt.Select<Device>("Select device to edit:",
                    tree.DeviceGroups.First(g => g.Name == _currentGroup).Devices).Id;
                ActiveCommands[Command.EditDevice] = true;
                ActiveCommands[Command.RemoveDevice] = true;
        }

        private static void RemoveDevice(DeviceTree x)
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
            ActiveCommands[Command.EditDevice] = false;
            ActiveCommands[Command.RemoveDevice] = false;
            _currentDeviceId = string.Empty;
            ActiveCommands[Command.SelectDevice] = x.DeviceGroups.FirstOrDefault(g => g.Name == _currentGroup)?.Devices.Count > 0;
        }

        internal static void Run(DeviceTree tree)
        {
            // Console.OutputEncoding = Encoding.UTF8;
            while (!_exitRequested)
            {
                CheckAvailableCommands(tree);
                
                Console.ForegroundColor = ConsoleColor.Yellow;

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

                Console.WriteLine(String.Join(",", GetAvailableCommands(tree)
                    .Select(x=>x.ToString()))
                );
                Console.ForegroundColor = ConsoleColor.White;
                var command = Prompt.Select<Command>("Select a command:", GetAvailableCommands(tree));
                InvokeCommand(command, tree);
            }
        }

        private static void InvokeCommand(Command command, DeviceTree tree)
        {
            try
            {
                CommandActions[command].Invoke(tree);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception in {command}: " + e.GetBaseException().Message);
            }
        }

        private static void CheckAvailableCommands(DeviceTree tree)
        {
            var devicesPresent = tree.GetDeviceCount() > 0;
            var groupsPresent = tree.GetGroups().Count > 0;

            ActiveCommands[Command.MoveDevice] = devicesPresent && tree.GetGroups().Count > 1;
            ActiveCommands[Command.AddDevice] = 
            ActiveCommands[Command.SelectGroup] = 
            ActiveCommands[Command.RemoveGroup] = groupsPresent;
        }

        private static List<Command> GetAvailableCommands(DeviceTree tree)
        {
            var result = ActiveCommands.Cast<DictionaryEntry>()
                .Where(x => x.Value.Equals(true))
                .Select(x => x.Key);
            return result.Cast<Command>().ToList();
        }

        private static void AddGroup(DeviceTree tree)
        {
            Console.WriteLine("Adding new group...");
            var groupName = Prompt.Input<string>("Enter group name:");
            tree.AddGroup(groupName);
        }
        
        private static void RemoveGroup(DeviceTree tree)
        {
            Console.WriteLine("Removing group...");
            var groupName = Prompt.Select<string>("Select group to delete:", tree.GetGroups());

            var resetDevice = tree.GroupContains(groupName, _currentDeviceId);
            if (!Prompt.Confirm("Really delete?", defaultValue: true))
            {
                return;
            }
            tree.RemoveGroup(groupName);
            if (resetDevice)
            {
                _currentDeviceId = string.Empty;
            }

            if (_currentGroup == groupName)
            {
                _currentGroup = string.Empty;
                ActiveCommands[Command.EditDevice] = 
                ActiveCommands[Command.RemoveDevice] = 
                ActiveCommands[Command.SelectDevice]  = false;
            }
        }
        
        private static void MoveDeviceToGroup(DeviceTree tree)
        {
            var sourceGroup = Prompt.Select<string>("Select group to move from:", 
                tree.GetGroups());
            var deviceId = Prompt.Select<Device>("Select device to edit:",
                tree.DeviceGroups.First(g => g.Name == sourceGroup).Devices).Id;
            
            var targetGroup = Prompt.Select<string>("Select group to move to:", 
                tree.GetGroups());
            
            if (!Prompt.Confirm("Really move?", defaultValue: true))
            {
                return;
            }

            var id = tree.GetDevice(deviceId);
            tree.MoveDeviceToGroup(targetGroup, id);
        }

        private static void AddDevice(DeviceTree tree)
        {
            Console.WriteLine("Adding new device...");
            var groupName = Prompt.Select<string>("Select group to add device to:", tree.GetGroups());
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
                case "Speaker":
                    var speakerName = Prompt.Input<string>("Enter Speaker name:");
                    var soundTypes = Enum.GetValues(typeof(Speaker.SoundType)).Cast<Speaker.SoundType>();
                    var  speakerSound = Prompt.Select<Speaker.SoundType>("Select Speaker sound:", soundTypes);
                    var  speakerVolume = Prompt.Input<double>("Select Speaker volume:");
                    newDevice = new Speaker(speakerName, speakerSound,speakerVolume);
                    break;
                case "CardReader":
                    var readerName = Prompt.Input<string>("Enter CardReader name:");
                    var cardNumber = Prompt.Input<string>("Enter AccessCard number:");
                    newDevice = new CardReader(readerName, cardNumber);
                    break;
                
                default:
                    Console.WriteLine("Unknown device type.");
                    return;
            }
            tree.AddDeviceToGroup(groupName, newDevice);
            Console.WriteLine($"Device {newDevice} added to group {groupName}.");
        }

        private static void SetValue(Object editedObject, PropertyInfo propertyInfo)
        {
            switch (propertyInfo.PropertyType.Name)
            {
                case "Boolean":
                    var inputBool = Prompt.Select<string>(
                        $"Enter new value for {propertyInfo.Name}:", 
                        new[] { "True", "False" });
                    propertyInfo.SetValue(editedObject, Convert.ChangeType(inputBool, propertyInfo.PropertyType));
                    break;
                
                case "SoundType":
                    var inputSoundType = Prompt.Select<Speaker.SoundType>(
                        $"Enter new value for {propertyInfo.Name}:", 
                        new[] { Speaker.SoundType.None,
                                Speaker.SoundType.Music,
                                Speaker.SoundType.Alarm
                        });
                    propertyInfo.SetValue(editedObject, Convert.ChangeType(inputSoundType, propertyInfo.PropertyType));
                    break;
                
                case "String":
                    var inputString = Prompt.Input<string>($"Enter new value for {propertyInfo.Name}:",
                        editedObject.GetType().GetProperty(propertyInfo.Name)?.GetValue(editedObject)?.ToString());
                    if (inputString.Equals(propertyInfo.GetValue(editedObject)))
                    {
                        break;
                    }
                    propertyInfo.SetValue(editedObject, Convert.ChangeType(inputString, propertyInfo.PropertyType));
                    break;
                
                case "Double":
                    var inputDouble = Prompt.Input<double>($"Enter new value for {propertyInfo.Name}:",
                        double.Parse(editedObject.GetType().GetProperty(propertyInfo.Name)?.GetValue(editedObject)?.ToString()!));
                    if (inputDouble.Equals(propertyInfo.GetValue(editedObject)))
                    {
                        break;
                    }
                    propertyInfo.SetValue(editedObject, Convert.ChangeType(inputDouble, propertyInfo.PropertyType));
                    break;
                default: break;
            }
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
            try
            {
                SetValue(device, propertyInfo);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception while editing {device}: " + e.GetBaseException().Message);
            }
          
        }
    }
}
