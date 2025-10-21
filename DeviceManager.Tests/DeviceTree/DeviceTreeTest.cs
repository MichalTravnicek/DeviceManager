using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace DeviceManager.Tests.DeviceTree;

[TestFixture]
[TestOf(typeof(DeviceManager.DeviceTree))]
public class DeviceTreeTest
{
    private IDeviceTree tree;

    private DeviceManager.Device _device;

    [Test, Order(1)]
    public void TestCreate()
    {
        tree = new DeviceManager.DeviceTree();
        Assert.That(tree, Is.Not.Null);
    }
    
    [Test, Order(2)]
    public void TestCreateGroup()
    {
        tree.AddGroup("Group1");
        Assert.That(tree.GetGroups(), Contains.Item("Group1"));
    }
    
    [Test, Order(3)]
    public void TestAddDevice()
    {
        _device = new LedPanel("Panel-1","Good day");
        tree.AddDeviceToGroup("Group1", _device); 
        Assert.That(tree.GetDevice(_device.Id), Is.Not.Null);
    }
    
    [Test, Order(4)]
    public void TestMoveDevice()
    {
        tree.AddGroup("Group2");
        tree.MoveDeviceToGroup("Group2", _device);
        Assert.That(tree.GetDevice(_device.Id), Is.Not.Null);
        Assert.That(tree.GetGroups(), Contains.Item("Group2"));
        Assert.That(tree.GroupContains("Group2", _device.Id), Is.True);
    }
    
    [Test, Order(5)]
    public void TestRemoveDevice()
    {
        Assert.That(tree.GetDevice(_device.Id), Is.Not.Null);
        tree.RemoveDeviceFromGroup("Group2", _device.Id);
        Assert.Throws<KeyNotFoundException>(() =>  tree.GetDevice(_device.Id));
    }
    
    [Test, Order(6)]
    public void TestRemoveGroup()
    {
        Assert.That(tree.GetGroups(), Contains.Item("Group1"));
        tree.RemoveGroup("Group1");
        Assert.That(tree.GetGroups(), !Contains.Item("Group1"));
    }
}
