namespace DeviceManager
{
    /// Annotates property tracked in logs.<br/>
    /// Position value is for ordering.
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    internal sealed class LoggedAttribute(int position) : Attribute
    {
        public readonly int Position = position;
    }
}
