using System;
using System.Collections.Specialized;
using System.Configuration;
using NUnit.Framework;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.NonPersistent;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms;
using Remotion.Development.UnitTesting.Configuration;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.NonPersistent
{
  [TestFixture]
  public class NonPersistentProviderDefinitionTest : StandardMappingTest
  {
    private StorageProviderDefinition _definition;
    private NonPersistentStorageObjectFactory _nonPersistentStorageObjectFactory;

    public override void SetUp()
    {
      base.SetUp();

      _nonPersistentStorageObjectFactory = new NonPersistentStorageObjectFactory();
      _definition = new NonPersistentProviderDefinition ("StorageProviderID", _nonPersistentStorageObjectFactory);

      FakeConfigurationWrapper configurationWrapper = new FakeConfigurationWrapper();
      ConfigurationWrapper.SetCurrent (configurationWrapper);
    }

    [Test]
    public void Initialize_FromArguments()
    {
      var provider = new NonPersistentProviderDefinition ("Provider", _nonPersistentStorageObjectFactory);

      Assert.That (provider.Name, Is.EqualTo ("Provider"));
      Assert.That (provider.Factory, Is.TypeOf (typeof (NonPersistentStorageObjectFactory)));
    }

    [Test]
    public void Initialize_FromConfig()
    {
      var config = new NameValueCollection();
      config.Add ("description", "The Description");
      config.Add ("factoryType", "Remotion.Data.DomainObjects::Persistence.NonPersistent.NonPersistentStorageObjectFactory");

      var provider = new NonPersistentProviderDefinition ("Provider", config);

      Assert.That (provider.Name, Is.EqualTo ("Provider"));
      Assert.That (provider.Description, Is.EqualTo ("The Description"));
      Assert.That (provider.Factory, Is.TypeOf(typeof (NonPersistentStorageObjectFactory)));
      Assert.That (config, Is.Empty);
    }

    [Test]
    public void Initialize_FromConfig_InvalidFactoryType ()
    {
      var config = new NameValueCollection();
      config.Add ("description", "The Description");
      config.Add ("factoryType", typeof (InvalidRdbmsStorageObjectFactory).AssemblyQualifiedName);

      Assert.That (
          () => new NonPersistentProviderDefinition ("Provider", config),
          Throws.TypeOf<ConfigurationErrorsException>()
              .With.Message.EqualTo (
                  "The factory type for the storage provider defined by 'Provider' must implement the 'INonPersistentStorageObjectFactory' interface. "
                  + "'InvalidRdbmsStorageObjectFactory' does not implement that interface."));
    }

    [Test]
    public void Initialize_FromConfig_WithMissingFactoryType ()
    {
      NameValueCollection config = new NameValueCollection();
      config.Add ("description", "The Description");

      Assert.That (
          () => new NonPersistentProviderDefinition ("Provider", config),
          Throws.TypeOf<ConfigurationErrorsException>()
              .With.Message.EqualTo ("The attribute 'factoryType' is missing in the configuration of the 'Provider' provider."));
    }

    [Test]
    public void IsIdentityTypeSupportedFalse()
    {
      Assert.That (_definition.IsIdentityTypeSupported (typeof (int)), Is.False);
    }

    [Test]
    public void IsIdentityTypeSupportedTrue()
    {
      Assert.That (_definition.IsIdentityTypeSupported (typeof (Guid)), Is.True);
    }

    [Test]
    public void IsIdentityTypeSupportedNull()
    {
      Assert.That (() => _definition.IsIdentityTypeSupported (null), Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void CheckValidIdentityType()
    {
      _definition.CheckIdentityType (typeof (Guid));
    }

    [Test]
    public void CheckInvalidIdentityType ()
    {
      var exception = Assert.Throws<IdentityTypeNotSupportedException> (() => _definition.CheckIdentityType (typeof (string)));

      Assert.That (
          exception.Message,
          Is.EqualTo ("The storage provider defined by 'NonPersistentProviderDefinition' does not support identity values of type 'System.String'."));
      Assert.That (exception.InvalidIdentityType, Is.EqualTo (typeof (string)));
    }
  }
}