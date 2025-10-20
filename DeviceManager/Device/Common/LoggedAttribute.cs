namespace DeviceManager
{
    /// Annotates property tracked in logs.<br/>
    /// Position value is for ordering.
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    internal sealed class LoggedAttribute(int position) : Attribute
    {
        public readonly int Position = position;
        
        public LoggedAttribute() : this(-100)
        {
        }
    }
}
