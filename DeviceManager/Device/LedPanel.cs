using System.Diagnostics.CodeAnalysis;

namespace DeviceManager
{
    public class LedPanel : Device
    {
        public override string Type {get; init;} = "LedPanel";
        
        [SetsRequiredMembers]
        public LedPanel(string name, string message) : base(name)
        {
            Message = message;
        }

        [Logged(1)]
        [EditableProperty]
        public string Message
        {
            get;
            set => SetProperty(ref field, value);
        }
    }
}