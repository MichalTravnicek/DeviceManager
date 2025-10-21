namespace DeviceManager
{
    public class TreeMessage(string treeId, string message) : EventMessage(message)
    {
        public string TreeId { get; set; } = treeId;
    }
}
