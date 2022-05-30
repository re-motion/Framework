using NUnit.Framework;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests
{
  [TestFixture]
  public class DomainObjectStateTest
  {
    [Test]
    public void IsUnchanged_WithBuilderDefault_ReturnsFalse ()
    {
      var state = new DomainObjectState.Builder().Value;
      Assert.That(state.IsUnchanged, Is.False);
    }

    [Test]
    public void IsUnchanged_WithBuilderSettingOther_ReturnsFalse ()
    {
      var state = new DomainObjectState.Builder().SetNew().Value;
      Assert.That(state.IsUnchanged, Is.False);
    }

    [Test]
    public void IsUnchanged_WithBuilderSettingUnchanged_ReturnsTrue ()
    {
      var state = new DomainObjectState.Builder().SetUnchanged().Value;
      Assert.That(state.IsUnchanged, Is.True);
    }


    [Test]
    public void IsChanged_WithBuilderDefault_ReturnsFalse ()
    {
      var state = new DomainObjectState.Builder().Value;
      Assert.That(state.IsChanged, Is.False);
    }

    [Test]
    public void IsChanged_WithBuilderSettingOther_ReturnsFalse ()
    {
      var state = new DomainObjectState.Builder().SetNew().Value;
      Assert.That(state.IsChanged, Is.False);
    }

    [Test]
    public void IsChanged_WithBuilderSettingChanged_ReturnsTrue ()
    {
      var state = new DomainObjectState.Builder().SetChanged().Value;
      Assert.That(state.IsChanged, Is.True);
    }


    [Test]
    public void IsNew_WithBuilderDefault_ReturnsFalse ()
    {
      var state = new DomainObjectState.Builder().Value;
      Assert.That(state.IsNew, Is.False);
    }

    [Test]
    public void IsNew_WithBuilderSettingOther_ReturnsFalse ()
    {
      var state = new DomainObjectState.Builder().SetChanged().Value;
      Assert.That(state.IsNew, Is.False);
    }

    [Test]
    public void IsNew_WithBuilderSettingNew_ReturnsTrue ()
    {
      var state = new DomainObjectState.Builder().SetNew().Value;
      Assert.That(state.IsNew, Is.True);
    }


    [Test]
    public void IsDeleted_WithBuilderDefault_ReturnsFalse ()
    {
      var state = new DomainObjectState.Builder().Value;
      Assert.That(state.IsDeleted, Is.False);
    }

    [Test]
    public void IsDeleted_WithBuilderSettingOther_ReturnsFalse ()
    {
      var state = new DomainObjectState.Builder().SetChanged().Value;
      Assert.That(state.IsDeleted, Is.False);
    }

    [Test]
    public void IsDeleted_WithBuilderSettingDeleted_ReturnsTrue ()
    {
      var state = new DomainObjectState.Builder().SetDeleted().Value;
      Assert.That(state.IsDeleted, Is.True);
    }


    [Test]
    public void IsInvalid_WithBuilderDefault_ReturnsFalse ()
    {
      var state = new DomainObjectState.Builder().Value;
      Assert.That(state.IsInvalid, Is.False);
    }

    [Test]
    public void IsInvalid_WithBuilderSettingOther_ReturnsFalse ()
    {
      var state = new DomainObjectState.Builder().SetChanged().Value;
      Assert.That(state.IsInvalid, Is.False);
    }

    [Test]
    public void IsInvalid_WithBuilderSettingInvalid_ReturnsTrue ()
    {
      var state = new DomainObjectState.Builder().SetInvalid().Value;
      Assert.That(state.IsInvalid, Is.True);
    }


    [Test]
    public void IsNotLoadedYet_WithBuilderDefault_ReturnsFalse ()
    {
      var state = new DomainObjectState.Builder().Value;
      Assert.That(state.IsNotLoadedYet, Is.False);
    }

    [Test]
    public void IsNotLoadedYet_WithBuilderSettingOther_ReturnsFalse ()
    {
      var state = new DomainObjectState.Builder().SetChanged().Value;
      Assert.That(state.IsNotLoadedYet, Is.False);
    }

    [Test]
    public void IsNotLoadedYet_WithBuilderSettingNotLoadedYet_ReturnsTrue ()
    {
      var state = new DomainObjectState.Builder().SetNotLoadedYet().Value;
      Assert.That(state.IsNotLoadedYet, Is.True);
    }

    [Test]
    public void SerializeAndDeserialize ()
    {
      var state = new DomainObjectState.Builder().SetChanged().SetNew().Value;
      Assert.That(state.IsChanged, Is.True);
      Assert.That(state.IsNew, Is.True);
      Assert.That(state.IsDeleted, Is.False);

      var deserializedState = Serializer.SerializeAndDeserialize(state);

      Assert.That(deserializedState.IsChanged, Is.True);
      Assert.That(deserializedState.IsNew, Is.True);
      Assert.That(deserializedState.IsDeleted, Is.False);
    }
  }
}
