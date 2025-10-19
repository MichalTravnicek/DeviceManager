# DeviceManager
- Manage devices using command-line prompt
- The device tree contains groups with individual devices

Allows:
- Adding and removing groups.
- Adding and removing devices in a group.
- Moving devices between groups.
- Modifying properties of devices.

Modifying device properties => prints the device's current state  
Modifying the tree structure => prints the tree with devices

## Command-line prompt
- Commands for modifying tree
- Commands for modifying devices
- Print tree
- Exit

### Basic Device
- Type (constant) - string
- Id (unique for device tree) - string
- Name (editable) - string
- GetCurrentState

### LedPanel
- Message - string

### Door
- Locked - bool
- Open - bool
- OpenForTooLong - bool
- OpenedForcibly - bool
- State (enum) contains above as bit flags

### Speaker
- Sound - (None, Music, Alarm)
- Volume - double

### CardReader
- AccessCardNumber - string
- ReverseBytesAndPad
- setting AccessCardNumber triggers ReverseBytesAndPad

