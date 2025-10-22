using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace DeviceManager
{
    public abstract partial class Device : ObservableObject
    {
        private static int _nextId = 0;

        private Dictionary<string, Device> _devicesById =  new Dictionary<string, Device>();

        [Logged(-2)]
        [EditableProperty]
        public required string Id { get;
            set
            {
                if (_devicesById != null && _devicesById.ContainsKey(value))
                {
                    Console.WriteLine($"!!!! Error: Id {value} is already in use !!!!");
                    return;
                }

                if (_devicesById != null && field != null)
                {
                    _devicesById.Remove(field);
                    _devicesById.Add(value, this);
                    Console.WriteLine("!!! Updating id....");
                }
                field = value;
                OnPropertyChanged();
            } }

        [Logged]
        [ObservableProperty]
        [EditableProperty]
        public required partial string Name { get; set; }

        [Logged(-1)]
        public abstract string Type { get; init; }

        private readonly IMessenger _messenger = MessengerImpl.Get();

        public virtual void PropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
        {
            var value = ReflectionTool.GetPropertyByName(this, e.PropertyName).GetValue(this);
            Console.WriteLine($"Property {e.PropertyName} changed in device {Id} Value: {value}");
            _messenger.Send(new DeviceMessage(Id, "Device property modified:"+e.PropertyName));
        }

        public void SetDeviceIdLink(Dictionary<string, Device> devices)
        {
            _devicesById =  devices;
        }

        public void RemoveDeviceIdLink()
        {
            _devicesById =  null;
        }

        protected Device(string name)
        {
            Interlocked.Increment(ref _nextId);
            Id = _nextId.ToString();
            Name = name;
            PropertyChanged += PropertyChangedHandler;
        }

        public virtual string GetCurrentState()
        {
            return ReflectionTool.PrintProps(this);
        }
        
        public virtual OrderedDictionary<string,object> GetCurrentStateDetails()
        {
            return ReflectionTool.GetPropertiesValues(this);
        }

        public override string ToString()
        {
            return $"Device: {Name} (" + ReflectionTool.PrintFilteredProps(this) + ")";
        }
    }
}
