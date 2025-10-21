namespace DeviceManager
{
    public class EventMessage(string message)
    {
        public string Message { get; set; } = message;
        
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public override string ToString()
        {
            return $"[{Timestamp}] {Message}";
        }
    }
}
