using System.Collections.Generic;
using System.ComponentModel;
using Moq;
using NUnit.Framework;

namespace DeviceManager.Tests.Device;

[TestFixture]
[TestOf(typeof(LedPanel))]
public class LedPanelTest
{
    [Test]
    public void TestCreate()
    {
        var device = new Mock<LedPanel>("TestPanel1","Watch this") {CallBase = true};
        Assert.That(device.Object, Is.Not.Null);
        Assert.That(device.Object.Id, Is.Not.Null);
        Assert.That(device.Object.Type, Is.EqualTo("LedPanel"));
        Assert.That(device.Object.Name, Is.EqualTo("TestPanel1"));
        Assert.That(device.Object.Message, Is.EqualTo("Watch this"));
    }

    [Test]
    public void TestUpdate()
    {
        var device = new Mock<LedPanel>("TestPanel1","Watch this") {CallBase = true};
        device.Object.Message = "My message";
        device.Object.Message = "My message";
        device.Object.Message = "My message";
        //Test behaves bit off - when invoking only constructor it registers 0 updates
        //but on modifying Message after object is created it reports 2 updates altogether (1 create and 1 update) 
        device.Verify(dut => dut.PropertyChangedHandler(
            It.IsAny<DeviceManager.Device>(),It.IsAny<PropertyChangedEventArgs>()),Times.Exactly(2));
    }

    [Test]
    public void TestGetState()
    {
        var device = new Mock<LedPanel>("TestPanel1","Watch this") { CallBase = true };
        var state = device.Object.GetCurrentStateDetails();
        Assert.That(state, Is.Not.Null);
        Assert.That(state, Contains.Key("Id"));
        Assert.That(state["Id"], Is.Not.Null);
        Assert.That(state, Contains.Item(new KeyValuePair<string,object>("Name","TestPanel1")));
        Assert.That(state, Contains.Item(new KeyValuePair<string,object>("Type","LedPanel")));
        Assert.That(state, Contains.Item(new KeyValuePair<string,object>("Message", "Watch this")));
    }
}
