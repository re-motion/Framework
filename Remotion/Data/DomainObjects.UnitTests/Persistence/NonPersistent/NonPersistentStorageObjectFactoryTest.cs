using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.NonPersistent;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Linq.SqlBackend.SqlPreparation;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.NonPersistent
{
  [TestFixture]
  public class NonPersistentStorageObjectFactoryTest
  {
    private NonPersistentProviderDefinition _storageProviderDefinition;

    [SetUp]
    public void SetUp ()
    {
      _storageProviderDefinition = new NonPersistentProviderDefinition ("Test", MockRepository.GenerateStub<INonPersistentStorageObjectFactory>());
    }

    [Test]
    public void CreatePersistenceModelLoader ()
    {
      var factory = new NonPersistentStorageObjectFactory();

      var result = factory.CreatePersistenceModelLoader (
          _storageProviderDefinition,
          MockRepository.GenerateStub<IStorageProviderDefinitionFinder>());

      Assert.That (result, Is.InstanceOf<NonPersistentPersistenceModelLoader>());
      var nonPersistentPersistenceModelLoader = (NonPersistentPersistenceModelLoader) result;
      Assert.That (nonPersistentPersistenceModelLoader.StorageProviderDefinition, Is.SameAs (_storageProviderDefinition));
    }

    [Test]
    public void CreateStorageProvider ()
    {
      var factory = new NonPersistentStorageObjectFactory();
      var persistenceExtensionStub = MockRepository.GenerateStub<IPersistenceExtension>();

      var result=  factory.CreateStorageProvider (_storageProviderDefinition, persistenceExtensionStub);

     
     Assert.That (result, Is.InstanceOf<NonPersistentProvider>());
     var nonPersistentProvider = (NonPersistentProvider) result;
     Assert.That (nonPersistentProvider.StorageProviderDefinition, Is.SameAs (_storageProviderDefinition));
     Assert.That (nonPersistentProvider.PersistenceExtension, Is.SameAs (persistenceExtensionStub));
    }

    [Test]
    public void CreateDomainObjectQueryGenerator_ThrowsNotSupportedException ()
    {
      var factory = new NonPersistentStorageObjectFactory();
      Assert.That (
          () => factory.CreateDomainObjectQueryGenerator (
              _storageProviderDefinition,
              MockRepository.GenerateStub<IMethodCallTransformerProvider>(),
              new ResultOperatorHandlerRegistry(),
              MockRepository.GenerateStub<IMappingConfiguration>()),
          Throws.TypeOf<NotSupportedException>().With.Message.EqualTo ("Non-persistent DomainObjects do not support querying."));
    }
  }
}