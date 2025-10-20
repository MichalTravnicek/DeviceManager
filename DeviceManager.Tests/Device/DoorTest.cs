using System.Collections.Generic;
using System.ComponentModel;
using Moq;
using NUnit.Framework;

namespace DeviceManager.Tests.Device;

[TestFixture]
[TestOf(typeof(Door))]
public class DoorTest
{

    [Test]
    public void TestCreate()
    {
        var device = new Mock<Door>("TestDoor1") {CallBase = true};
        Assert.That(device.Object, Is.Not.Null);
        Assert.That(device.Object.Id, Is.Not.Null);
        Assert.That(device.Object.Type, Is.EqualTo("Door"));
        Assert.That(device.Object.Name, Is.EqualTo("TestDoor1"));
        Assert.That(device.Object.Locked, Is.EqualTo(true));
    }
    
    [Test]
    public void TestGetState()
    {
        var device = new Mock<Door>("TestDoor1") {CallBase = true};
        var state = device.Object.GetCurrentStateDetails();
        Assert.That(state, Is.Not.Null);
        Assert.That(state, Contains.Key("Id"));
        Assert.That(state["Id"], Is.Not.Null);
        Assert.That(state, Contains.Item(new KeyValuePair<string,object>("Name","TestDoor1")));
        Assert.That(state, Contains.Item(new KeyValuePair<string,object>("Type","Door")));
        Assert.That(state, Contains.Item(new KeyValuePair<string,object>("Locked", true)));
        Assert.That(state, Contains.Item(new KeyValuePair<string,object>("Open", false)));
        Assert.That(state, Contains.Item(new KeyValuePair<string,object>("OpenForTooLong", false)));
        Assert.That(state, Contains.Item(new KeyValuePair<string,object>("OpenedForcibly", false)));
    }
    
    [Test]
    public void TestChangeState()
    {
        var device = new Mock<Door>("TestDoor1") {CallBase = true};
        Assert.That(device.Object.Locked, Is.EqualTo(true));
        Assert.That(device.Object.Open, Is.EqualTo(false));
        Assert.That(device.Object.OpenForTooLong, Is.EqualTo(false));
        Assert.That(device.Object.OpenedForcibly, Is.EqualTo(false));
        device.Object.Locked = false;
        Assert.That(device.Object.Locked, Is.EqualTo(false));
        Assert.That(device.Object.Open, Is.EqualTo(false));
        Assert.That(device.Object.OpenForTooLong, Is.EqualTo(false));
        Assert.That(device.Object.OpenedForcibly, Is.EqualTo(false));
        device.Verify(dut => dut.PropertyChangedHandler(
            It.IsAny<DeviceManager.Device>(),It.IsAny<PropertyChangedEventArgs>()),Times.Exactly(2));
    }
    
    [Test]
    public void TestChangeState2()
    {
        var device = new Mock<Door>("TestDoor1") {CallBase = true};
        device.Object.Locked = false;
        device.Object.Open = true;
        Assert.That(device.Object.Locked, Is.EqualTo(false));
        Assert.That(device.Object.Open, Is.EqualTo(true));
        Assert.That(device.Object.OpenForTooLong, Is.EqualTo(false));
        Assert.That(device.Object.OpenedForcibly, Is.EqualTo(false));
        device.Verify(dut => dut.PropertyChangedHandler(
            It.IsAny<DeviceManager.Device>(),It.IsAny<PropertyChangedEventArgs>()),Times.Exactly(3));
    }
    
    [Test]
    public void TestChangeState3()
    {
        var device = new Mock<Door>("TestDoor1") {CallBase = true};
        device.Object.OpenForTooLong = true;
        device.Object.OpenedForcibly = true;
        Assert.That(device.Object.Locked, Is.EqualTo(true));
        Assert.That(device.Object.Open, Is.EqualTo(false));
        Assert.That(device.Object.OpenForTooLong, Is.EqualTo(true));
        Assert.That(device.Object.OpenedForcibly, Is.EqualTo(true));
        device.Verify(dut => dut.PropertyChangedHandler(
            It.IsAny<DeviceManager.Device>(),It.IsAny<PropertyChangedEventArgs>()),Times.Exactly(3));
    }
}
