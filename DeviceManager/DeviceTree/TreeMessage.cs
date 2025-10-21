using System.Collections.Specialized;

namespace DeviceManager
{
    public class TreeMessage(string treeId, string message, NotifyCollectionChangedEventArgs eventArgs) : EventMessage(message)
    {
        public NotifyCollectionChangedEventArgs EventArgs = eventArgs; 

        public string TreeId { get; set; } = treeId;
    }
}
