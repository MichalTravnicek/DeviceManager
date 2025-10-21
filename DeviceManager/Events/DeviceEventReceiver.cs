using CommunityToolkit.Mvvm.Messaging;

namespace DeviceManager
{
    internal class DeviceEventReceiver
    {
        private Func<DeviceMessage,bool> AcceptCondition = message => true;
        
        public DeviceEventReceiver(Func<DeviceMessage,bool> acceptCondition) {
            Register(acceptCondition);
        }

        public DeviceEventReceiver()
        {
        }

        public void Register(Func<DeviceMessage,bool> acceptCondition)
        {
            AcceptCondition = acceptCondition;
            MessengerImpl.Get().Register<DeviceMessage>(this, (r, m) => Receive(m));
        }
        private void Receive(DeviceMessage received)
        {
            Console.WriteLine("XXXXXXXXXX Received DeviceMessage");
            var condition = AcceptCondition.Invoke(received);
            if (!condition)
            {
                Console.WriteLine("Device Event: !!! Not in tree"); 
                return;
            }
            Console.WriteLine("Device Event: !!! DeviceMessage:" + received.DeviceId + " " + received.Message);
        }
    }
}
