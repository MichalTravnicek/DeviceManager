using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace DeviceManager
{
    public abstract partial class Device : ObservableObject
    {
        private static int _nextId = 0;

        [Logged(-1)]
        [ObservableProperty]
        public required partial string Id { get; set; }

        [ObservableProperty]
        public required partial string Name { get; set; }

        [Logged(0)]
        public abstract string Type { get; init; }

        public virtual void PropertyChangedHandler(object? sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine($"Property {e.PropertyName} changed in device {Id}");
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
            return ToString();
        }

        public override string ToString()
        {
            return $"Device: {Name} (" + ReflectionTool.PrintProps(this) + ")";
        }
    }
}
