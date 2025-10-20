using System.Collections.Generic;
using System.ComponentModel;
using Moq;
using NUnit.Framework;

namespace DeviceManager.Tests.Device;

[TestFixture]
[TestOf(typeof(DeviceManager.Device))]
public class DeviceTest
{
    [Test]
    public void TestCreate()
    {
        var device = new Mock<DeviceManager.Device>("TestName") {CallBase = true};
        Assert.That(device.Object, Is.Not.Null);
        Assert.That(device.Object.Id, Is.Not.Null);
        Assert.That(device.Object.Type, Is.EqualTo(null));
        Assert.That(device.Object.Name, Is.EqualTo("TestName"));
    }

    [Test]
    public void TestUpdate()
    {
        var device = new Mock<DeviceManager.Device>("TestName") {CallBase = true};
        device.Object.Id = "My device id"; 
        device.Object.Id = "My device id"; // When the value is the same, no update is announced
        //Id and Name are assigned before binding PropertyChangedHandler so init of these is not registered 
        device.Object.Name = "New object name";
        device.Verify(dut => dut.PropertyChangedHandler(
            It.IsAny<DeviceManager.Device>(),It.IsAny<PropertyChangedEventArgs>()),Times.Exactly(2));
    }

    [Test]
    public void TestGetState()
    {
        var device = new Mock<DeviceManager.Device>("TestName") { CallBase = true };
        var state = device.Object.GetCurrentState();
        var stateDetails = device.Object.GetCurrentStateDetails();
        Assert.That(state, Is.Not.Null);
        Assert.That(state, Contains.Substring("Id"));
        Assert.That(state, Contains.Substring("TestName"));
        Assert.That(stateDetails, Is.Not.Null);
        Assert.That(stateDetails, Contains.Key("Id"));
        Assert.That(stateDetails["Id"], Is.Not.Null);
        Assert.That(stateDetails, Contains.Item(new KeyValuePair<string,object>("Name", "TestName")));
        Assert.That(stateDetails, Contains.Item(new KeyValuePair<string,object>("Type", null)));
    }
}
