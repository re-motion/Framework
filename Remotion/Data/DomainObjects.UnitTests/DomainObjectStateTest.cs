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
    public void IsNewInHierarchy_WithBuilderDefault_ReturnsFalse ()
    {
      var state = new DomainObjectState.Builder().Value;
      Assert.That(state.IsNewInHierarchy, Is.False);
    }

    [Test]
    public void IsNewInHierarchy_WithBuilderSettingOther_ReturnsFalse ()
    {
      var state = new DomainObjectState.Builder().SetChanged().Value;
      Assert.That(state.IsNewInHierarchy, Is.False);
    }

    [Test]
    public void IsNewInHierarchy_WithBuilderSettingNewInHierarchy_ReturnsTrue ()
    {
      var state = new DomainObjectState.Builder().SetNewInHierarchy().Value;
      Assert.That(state.IsNewInHierarchy, Is.True);
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
    public void IsDataChanged_WithBuilderDefault_ReturnsFalse ()
    {
      var state = new DomainObjectState.Builder().Value;
      Assert.That(state.IsDataChanged, Is.False);
    }

    [Test]
    public void IsDataChanged_WithBuilderSettingOther_ReturnsFalse ()
    {
      var state = new DomainObjectState.Builder().SetNew().Value;
      Assert.That(state.IsDataChanged, Is.False);
    }

    [Test]
    public void IsDataChanged_WithBuilderSettingChanged_ReturnsTrue ()
    {
      var state = new DomainObjectState.Builder().SetDataChanged().Value;
      Assert.That(state.IsDataChanged, Is.True);
    }

    [Test]
    public void SetDataChanged_DoesNotSetIsChanged ()
    {
      var state = new DomainObjectState.Builder().SetDataChanged().Value;
      Assert.That(state.IsChanged, Is.False);
    }


    [Test]
    public void IsPersistentDataChanged_WithBuilderDefault_ReturnsFalse ()
    {
      var state = new DomainObjectState.Builder().Value;
      Assert.That(state.IsPersistentDataChanged, Is.False);
    }

    [Test]
    public void IsPersistentDataChanged_WithBuilderSettingOther_ReturnsFalse ()
    {
      var state = new DomainObjectState.Builder().SetNew().Value;
      Assert.That(state.IsPersistentDataChanged, Is.False);
    }

    [Test]
    public void IsPersistentDataChanged_WithBuilderSettingChanged_ReturnsTrue ()
    {
      var state = new DomainObjectState.Builder().SetPersistentDataChanged().Value;
      Assert.That(state.IsPersistentDataChanged, Is.True);
    }

    [Test]
    public void SetPersistentDataChanged_ImplicitlySetsIsDataChanged ()
    {
      var state = new DomainObjectState.Builder().SetPersistentDataChanged().Value;
      Assert.That(state.IsDataChanged, Is.True);
    }

    [Test]
    public void SetPersistentDataChanged_DoesNotSetIsChanged ()
    {
      var state = new DomainObjectState.Builder().SetPersistentDataChanged().Value;
      Assert.That(state.IsChanged, Is.False);
    }


    [Test]
    public void IsNonPersistentDataChanged_WithBuilderDefault_ReturnsFalse ()
    {
      var state = new DomainObjectState.Builder().Value;
      Assert.That(state.IsNonPersistentDataChanged, Is.False);
    }

    [Test]
    public void IsNonPersistentDataChanged_WithBuilderSettingOther_ReturnsFalse ()
    {
      var state = new DomainObjectState.Builder().SetNew().Value;
      Assert.That(state.IsNonPersistentDataChanged, Is.False);
    }

    [Test]
    public void IsNonPersistentDataChanged_WithBuilderSettingChanged_ReturnsTrue ()
    {
      var state = new DomainObjectState.Builder().SetNonPersistentDataChanged().Value;
      Assert.That(state.IsNonPersistentDataChanged, Is.True);
    }

    [Test]
    public void SetNonPersistentDataChanged_ImplicitlySetsIsDataChanged ()
    {
      var state = new DomainObjectState.Builder().SetNonPersistentDataChanged().Value;
      Assert.That(state.IsDataChanged, Is.True);
    }

    [Test]
    public void SetNonPersistentDataChanged_DoesNotSetIsChanged ()
    {
      var state = new DomainObjectState.Builder().SetNonPersistentDataChanged().Value;
      Assert.That(state.IsChanged, Is.False);
    }


    [Test]
    public void IsRelationChanged_WithBuilderDefault_ReturnsFalse ()
    {
      var state = new DomainObjectState.Builder().Value;
      Assert.That(state.IsRelationChanged, Is.False);
    }

    [Test]
    public void IsRelationChanged_WithBuilderSettingOther_ReturnsFalse ()
    {
      var state = new DomainObjectState.Builder().SetNew().Value;
      Assert.That(state.IsRelationChanged, Is.False);
    }

    [Test]
    public void IsRelationChanged_WithBuilderSettingChanged_ReturnsTrue ()
    {
      var state = new DomainObjectState.Builder().SetRelationChanged().Value;
      Assert.That(state.IsRelationChanged, Is.True);
    }

    [Test]
    public void SetRelationChanged_DoesNotSetIsChanged ()
    {
      var state = new DomainObjectState.Builder().SetRelationChanged().Value;
      Assert.That(state.IsChanged, Is.False);
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
