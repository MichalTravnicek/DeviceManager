using System;
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
    public void TestUpdateId()
    {
        var deviceA = new Mock<DeviceManager.Device>("ObjectA") {CallBase = true};
        var deviceB = new Mock<DeviceManager.Device>("ObjectB") {CallBase = true};
        var devicesById =  new Dictionary<string, DeviceManager.Device>
        {
            { deviceB.Object.Id, deviceB.Object }
        };
        deviceA.Object.SetDeviceIdLink(devicesById);
        var objectAId = deviceA.Object.Id;
        var objectBId = deviceB.Object.Id;
        
        deviceA.Object.Id = objectBId;
        Assert.That(deviceA.Object, Is.Not.Null);
        Assert.That(deviceA.Object.Id, Is.EqualTo(objectAId));
        
        deviceA.Object.RemoveDeviceIdLink();
        deviceA.Object.Id = objectBId;
        Assert.That(deviceA.Object.Id, Is.EqualTo(objectBId));
        
        deviceB.Object.SetDeviceIdLink(devicesById);
        deviceB.Object.Id = "NewId";
        Assert.That(devicesById, Does.ContainKey("NewId"));
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
