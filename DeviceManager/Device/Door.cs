using System.Diagnostics.CodeAnalysis;

namespace DeviceManager
{
    public class Door : Device
    {
        public override string Type { get; init; } = "Door";

        [SetsRequiredMembers]
        public Door(string name) : base(name)
        {
            Locked = true;
        }        
        
        public State CurrentState { get; set; }

        [Logged(1)]
        public bool Locked
        {
            get => CurrentState.HasFlag(State.Locked);
            set => SetFlag(value, State.Locked);
        }

        [Logged(2)]
        public bool Open
        {
            get => CurrentState.HasFlag(State.Open);
            set => SetFlag(value, State.Open);
        }

        [Logged(3)]
        public bool OpenForTooLong
        {
            get => CurrentState.HasFlag(State.OpenForTooLong);
            set => SetFlag(value, State.OpenForTooLong);
        }

        [Logged(4)]
        public bool OpenedForcibly
        {
            get => CurrentState.HasFlag(State.OpenedForcibly);
            set => SetFlag(value, State.OpenedForcibly);
        }

        private void SetFlag(bool value, State state)
        {
            if (value)
            {
                CurrentState |= state;
            }
            else
            {
                CurrentState &= ~state;
            }
            OnPropertyChanged(state.ToString());
        }

        [Flags]
        public enum State
        {
            Locked = 1,
            Open = 2,
            OpenForTooLong = 4,
            OpenedForcibly = 8
        }
    }
}
