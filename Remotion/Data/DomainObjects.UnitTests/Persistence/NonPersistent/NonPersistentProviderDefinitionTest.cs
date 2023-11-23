using System;
using NUnit.Framework;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.NonPersistent;
using Remotion.Development.UnitTesting.Configuration;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.NonPersistent
{
  [TestFixture]
  public class NonPersistentProviderDefinitionTest : StandardMappingTest
  {
    private StorageProviderDefinition _definition;
    private NonPersistentStorageObjectFactory _nonPersistentStorageObjectFactory;

    public override void SetUp ()
    {
      base.SetUp();

      _nonPersistentStorageObjectFactory = new NonPersistentStorageObjectFactory();
      _definition = new NonPersistentProviderDefinition("StorageProviderID", _nonPersistentStorageObjectFactory);

      FakeConfigurationWrapper configurationWrapper = new FakeConfigurationWrapper();
      ConfigurationWrapper.SetCurrent(configurationWrapper);
    }

    [Test]
    public void Initialize_FromArguments ()
    {
      var provider = new NonPersistentProviderDefinition("Provider", _nonPersistentStorageObjectFactory);

      Assert.That(provider.Name, Is.EqualTo("Provider"));
      Assert.That(provider.Factory, Is.TypeOf(typeof(NonPersistentStorageObjectFactory)));
    }

    [Test]
    public void IsIdentityTypeSupportedFalse ()
    {
      Assert.That(_definition.IsIdentityTypeSupported(typeof(int)), Is.False);
    }

    [Test]
    public void IsIdentityTypeSupportedTrue ()
    {
      Assert.That(_definition.IsIdentityTypeSupported(typeof(Guid)), Is.True);
    }

    [Test]
    public void IsIdentityTypeSupportedNull ()
    {
      Assert.That(() => _definition.IsIdentityTypeSupported(null), Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void CheckValidIdentityType ()
    {
      _definition.CheckIdentityType(typeof(Guid));
    }

    [Test]
    public void CheckInvalidIdentityType ()
    {
      var exception = Assert.Throws<IdentityTypeNotSupportedException>(() => _definition.CheckIdentityType(typeof(string)));

      Assert.That(
          exception.Message,
          Is.EqualTo("The storage provider defined by 'NonPersistentProviderDefinition' does not support identity values of type 'System.String'."));
      Assert.That(exception.InvalidIdentityType, Is.EqualTo(typeof(string)));
    }
  }
}
