namespace DeviceManager;

public interface IDeviceTree
{
    void AddGroup(string name);
    void RemoveGroup(string name);
    void AddDeviceToGroup(string groupName, Device device);
    void MoveDeviceToGroup(string groupName, Device device);
    void RemoveDeviceFromGroup(string groupName, string deviceId);
    void DisplayTree();
    List<string> GetGroups();
    bool GroupContains(string groupName, string deviceId);
    Device GetDevice(string deviceId);
    int GetDeviceCount();
}
