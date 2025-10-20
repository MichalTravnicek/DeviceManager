using System.Diagnostics.CodeAnalysis;

namespace DeviceManager
{
    public class Speaker : Device
    {
        public override string Type { get; init; } = "Speaker";
        public enum SoundType
        {
            None,
            Music,
            Alarm
        }

        [SetsRequiredMembers]
        public Speaker(string name, SoundType sound, double volume) : base (name)
        {
            Sound = sound;
            Volume = volume;
        }

        [Logged(1)]
        public SoundType Sound { get; set => SetProperty(ref field, value); }

        [Logged(2)]
        public double Volume { get; set => SetProperty(ref field, value); }
    }
}
