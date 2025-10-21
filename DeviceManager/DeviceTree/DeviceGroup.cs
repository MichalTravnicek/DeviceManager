namespace DeviceManager
{
    internal class DeviceGroup
    {
        public string Name { get; set; }
        public List<Device> Devices { get; } = new List<Device>();
        public DeviceGroup(string name)
        {
            Name = name;
        }
    }
}
