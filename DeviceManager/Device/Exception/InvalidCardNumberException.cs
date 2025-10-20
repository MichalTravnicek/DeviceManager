namespace DeviceManager
{
    public class InvalidCardNumberException : Exception
    {
        public InvalidCardNumberException(string message) : base(message) { }
    }

}
