using System;
using System.Collections.Generic;
using System.ComponentModel;
using Moq;
using NUnit.Framework;

namespace DeviceManager.Tests.Device;

[TestFixture]
[TestOf(typeof(DeviceManager.Device))]
public class CardReaderTest
{
    [Test]
    public void TestCreate()
    {
        var device = new Mock<CardReader>("CardReader-2","A01234DE7FFF") {CallBase = true};
        Assert.That(device.Object, Is.Not.Null);
        Assert.That(device.Object.Id, Is.Not.Null);
        Assert.That(device.Object.Type, Is.EqualTo("CardReader"));
        Assert.That(device.Object.Name, Is.EqualTo("CardReader-2"));
        Assert.That(device.Object.AccessCardNumber, Is.EqualTo("0000FF7FDE3412A0"));
    }

    [Test]
    public void TestUpdate()
    {
        var device = new Mock<CardReader>("CardReader-2","A01234DE7FFF") {CallBase = true};
        Assert.That(device.Object, Is.Not.Null);
        Assert.That(device.Object.AccessCardNumber, Is.EqualTo("0000FF7FDE3412A0"));
        device.Object.AccessCardNumber = "0000FF7FDE3412A0";
        Assert.That(device.Object.AccessCardNumber, Is.EqualTo("A01234DE7FFF0000"));
        device.Object.Name = "CardReader-3";
        device.Verify(dut => dut.PropertyChangedHandler(
            It.IsAny<DeviceManager.Device>(),It.IsAny<PropertyChangedEventArgs>()),Times.Exactly(3));
    }
    
    [Test]
    public void TestException()
    { 
        Assert.Throws<InvalidCardNumberException>(() =>
            {
                try
                {
                    var device = new Mock<CardReader>("CardReader-2", "XXXXXX") { CallBase = true };
                    device.Object.GetType();
                }
                catch (Exception e)
                {
                    if (e.InnerException != null) throw e.InnerException;
                }
            }
        );
    }

    [Test]
    public void TestGetState()
    {
        var device = new Mock<CardReader>("CardReader2","A01234DE7FFF") { CallBase = true };
        var state = device.Object.GetCurrentState();
        var stateDetails = device.Object.GetCurrentStateDetails();
        Assert.That(state, Is.Not.Null);
        Assert.That(state, Does.Match("Name: CardReader2, Id: \\S+, Type: CardReader, AccessCardNumber: 0000FF7FDE3412A0"));
        Assert.That(stateDetails, Is.Not.Null);
        Assert.That(stateDetails, Contains.Key("Id"));
        Assert.That(stateDetails["Id"], Is.Not.Null);
        Assert.That(stateDetails, Contains.Item(new KeyValuePair<string,object>("Name", "CardReader2")));
        Assert.That(stateDetails, Contains.Item(new KeyValuePair<string,object>("Type", "CardReader")));
        Assert.That(stateDetails, Contains.Item(new KeyValuePair<string,object>("AccessCardNumber", "0000FF7FDE3412A0")));
    }
}
