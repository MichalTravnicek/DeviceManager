using CommunityToolkit.Mvvm.Messaging;

namespace DeviceManager;

public class MessengerImpl
{
    public static IMessenger Get()
    {
        return WeakReferenceMessenger.Default;
    }
}
