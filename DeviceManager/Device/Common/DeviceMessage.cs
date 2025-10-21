namespace DeviceManager
{
    public class DeviceMessage(string deviceid, string message) : EventMessage(message)
    {
        public string DeviceId { get; set; } = deviceid;
    }
}
