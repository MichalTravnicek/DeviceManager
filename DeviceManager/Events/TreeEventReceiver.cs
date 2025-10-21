using CommunityToolkit.Mvvm.Messaging;

namespace DeviceManager
{
    internal class TreeEventReceiver
    {
        private Func<TreeMessage,bool> AcceptCondition = message => true;
        
        public TreeEventReceiver(Func<TreeMessage,bool> acceptCondition) {
            Register(acceptCondition);
        }

        public TreeEventReceiver()
        {
        }

        public void Register(Func<TreeMessage,bool> acceptCondition)
        {
            AcceptCondition = acceptCondition;
            MessengerImpl.Get().Register<TreeMessage>(this, (r, m) => Receive(m));
        }

        private void Receive(TreeMessage received)
        {
            Console.WriteLine("XXXXXXXXXX Received TreeMessage");
            var condition = AcceptCondition.Invoke(received);
            if (!condition)
            {
                Console.WriteLine("Tree Event: !!! Not in tree"); 
                return;
            }

            Console.WriteLine($"Tree Event: !!! {received.Timestamp})");
            Console.WriteLine(received.TreeId + " " + received.Message);
            Console.WriteLine("New: " + received.EventArgs.NewItems?[0] + " Old: " +received.EventArgs.OldItems?[0]);
        }
    }
}
