using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace DeviceManager
{
    public abstract partial class Device : ObservableObject
    {
        private static int _nextId = 0;

        [Logged(-2)]
        [ObservableProperty]
        public required partial string Id { get; set; }

        [Logged]
        [ObservableProperty]
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
