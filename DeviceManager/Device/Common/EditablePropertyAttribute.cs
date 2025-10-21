namespace DeviceManager
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class EditablePropertyAttribute : Attribute
    {
    }
}
