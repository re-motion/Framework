using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement
{
  [TestFixture]
  public class DataContainerStateTest
  {
    [Test]
    public void IsUnchanged_WithBuilderDefault_ReturnsFalse ()
    {
      var state = new DataContainerState.Builder().Value;
      Assert.That (state.IsUnchanged, Is.False);
    }

    [Test]
    public void IsUnchanged_WithBuilderSettingOther_ReturnsFalse ()
    {
      var state = new DataContainerState.Builder().SetNew().Value;
      Assert.That (state.IsUnchanged, Is.False);
    }

    [Test]
    public void IsUnchanged_WithBuilderSettingUnchanged_ReturnsTrue ()
    {
      var state = new DataContainerState.Builder().SetUnchanged().Value;
      Assert.That (state.IsUnchanged, Is.True);
    }


    [Test]
    public void IsChanged_WithBuilderDefault_ReturnsFalse ()
    {
      var state = new DataContainerState.Builder().Value;
      Assert.That (state.IsChanged, Is.False);
    }

    [Test]
    public void IsChanged_WithBuilderSettingOther_ReturnsFalse ()
    {
      var state = new DataContainerState.Builder().SetNew().Value;
      Assert.That (state.IsChanged, Is.False);
    }

    [Test]
    public void IsChanged_WithBuilderSettingChanged_ReturnsTrue ()
    {
      var state = new DataContainerState.Builder().SetChanged().Value;
      Assert.That (state.IsChanged, Is.True);
    }


    [Test]
    public void IsNew_WithBuilderDefault_ReturnsFalse ()
    {
      var state = new DataContainerState.Builder().Value;
      Assert.That (state.IsNew, Is.False);
    }

    [Test]
    public void IsNew_WithBuilderSettingOther_ReturnsFalse ()
    {
      var state = new DataContainerState.Builder().SetChanged().Value;
      Assert.That (state.IsNew, Is.False);
    }

    [Test]
    public void IsNew_WithBuilderSettingNew_ReturnsTrue ()
    {
      var state = new DataContainerState.Builder().SetNew().Value;
      Assert.That (state.IsNew, Is.True);
    }


    [Test]
    public void IsDeleted_WithBuilderDefault_ReturnsFalse ()
    {
      var state = new DataContainerState.Builder().Value;
      Assert.That (state.IsDeleted, Is.False);
    }

    [Test]
    public void IsDeleted_WithBuilderSettingOther_ReturnsFalse ()
    {
      var state = new DataContainerState.Builder().SetChanged().Value;
      Assert.That (state.IsDeleted, Is.False);
    }

    [Test]
    public void IsDeleted_WithBuilderSettingDeleted_ReturnsTrue ()
    {
      var state = new DataContainerState.Builder().SetDeleted().Value;
      Assert.That (state.IsDeleted, Is.True);
    }


    [Test]
    public void IsDiscarded_WithBuilderDefault_ReturnsFalse ()
    {
      var state = new DataContainerState.Builder().Value;
      Assert.That (state.IsDiscarded, Is.False);
    }

    [Test]
    public void IsDiscarded_WithBuilderSettingOther_ReturnsFalse ()
    {
      var state = new DataContainerState.Builder().SetChanged().Value;
      Assert.That (state.IsDiscarded, Is.False);
    }

    [Test]
    public void IsDiscarded_WithBuilderSettingInvalid_ReturnsTrue ()
    {
      var state = new DataContainerState.Builder().SetDiscarded().Value;
      Assert.That (state.IsDiscarded, Is.True);
    }

    [Test]
    public void To_String ()
    {
      var state = new DomainObjectState.Builder().SetInvalid().SetDeleted().Value;
      Assert.That (state.ToString(), Is.EqualTo ("DomainObjectState (Deleted, Invalid)"));
    }
  }
}