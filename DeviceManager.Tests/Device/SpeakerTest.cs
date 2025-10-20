using System.Collections.Generic;
using System.ComponentModel;
using Moq;
using NUnit.Framework;

namespace DeviceManager.Tests.Device;

[TestFixture]
[TestOf(typeof(Speaker))]
public class SpeakerTest
{

    [Test]
    public void TestCreate()
    {
        var device = new Mock<Speaker>("BigSpeaker1", Speaker.SoundType.Alarm, 2.5) {CallBase = true};
        Assert.That(device.Object, Is.Not.Null);
        Assert.That(device.Object.Id, Is.Not.Null);
        Assert.That(device.Object.Type, Is.EqualTo("Speaker"));
        Assert.That(device.Object.Name, Is.EqualTo("BigSpeaker1"));
        Assert.That(device.Object.Sound, Is.EqualTo(Speaker.SoundType.Alarm));
        Assert.That(device.Object.Volume, Is.EqualTo(2.5));
    }

    [Test]
    public void TestGetState()
    {
        var device = new Mock<Speaker>("BigSpeaker1", Speaker.SoundType.Alarm, 2.5) {CallBase = true};
        Assert.That(device.Object, Is.Not.Null);
        var state = device.Object.GetCurrentStateDetails();
        Assert.That(state, Is.Not.Null);
        Assert.That(state, Contains.Key("Id"));
        Assert.That(state["Id"], Is.Not.Null);
        Assert.That(state, Contains.Item(new KeyValuePair<string,object>("Name","BigSpeaker1")));
        Assert.That(state, Contains.Item(new KeyValuePair<string,object>("Type","Speaker")));
        Assert.That(state, Contains.Item(new KeyValuePair<string,object>("Sound", Speaker.SoundType.Alarm)));
        Assert.That(state, Contains.Item(new KeyValuePair<string,object>("Volume", 2.5)));
    }

    [Test]
    public void TestUpdate()
    {
        var device = new Mock<Speaker>("BigSpeaker1", Speaker.SoundType.Alarm, 2.5) {CallBase = true};
        // +1 registered change from constructor
        device.Object.Volume = 2.5;
        device.Object.Volume = 2.5;
        device.Object.Volume = 3.0;
        device.Object.Volume = 3.0;
        device.Verify(dut => dut.PropertyChangedHandler(
            It.IsAny<DeviceManager.Device>(),It.Is<PropertyChangedEventArgs>(
                args => args.PropertyName == "Volume")),Times.Exactly(2));
        Assert.That(device.Object.Volume, Is.EqualTo(3.0));
    }
    
    [Test]
    public void TestUpdate2()
    {
        var device = new Mock<Speaker>("BigSpeaker1", Speaker.SoundType.Alarm, 2.5) {CallBase = true};
        // +1 registered change from constructor
        device.Object.Sound = Speaker.SoundType.Music;
        device.Object.Sound = Speaker.SoundType.Music;
        Assert.That(device.Object.Sound, Is.EqualTo(Speaker.SoundType.Music));
        device.Object.Sound = Speaker.SoundType.None;
        device.Object.Sound = Speaker.SoundType.None;
        device.Verify(dut => dut.PropertyChangedHandler(
            It.IsAny<DeviceManager.Device>(),It.Is<PropertyChangedEventArgs>(
                args => args.PropertyName == "Sound")),Times.Exactly(3));
        Assert.That(device.Object.Sound, Is.EqualTo(Speaker.SoundType.None));
    }
}
