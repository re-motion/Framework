// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.NonPersistent;
using Remotion.Data.DomainObjects.Persistence.NonPersistent.Model;
using Remotion.Data.DomainObjects.Persistence.NonPersistent.Validation;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Persistence.NonPersistent.Model.NonPersistentPersistenceModelLoaderTestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.NonPersistent
{

  [TestFixture]
  public class NonPersistentPersistenceModelLoaderTest
  {
    private string _storageProviderID;
    private StorageProviderDefinition _storageProviderDefinition;

    private NonPersistentPersistenceModelLoader _persistenceModelLoader;

    [SetUp]
    public void SetUp ()
    {
      _storageProviderID = "DefaultStorageProvider";
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition(_storageProviderID);

      _persistenceModelLoader = new NonPersistentPersistenceModelLoader(_storageProviderDefinition);
    }

    [Test]
    public void CreatePersistenceMappingValidator ()
    {
      var baseClass = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(BaseClass), baseClass: null);
      baseClass.SetDerivedClasses(new ClassDefinition[0]);
      baseClass.SetPropertyDefinitions(new PropertyDefinitionCollection(new PropertyDefinition[0], true));

      var validator = (PersistenceMappingValidator)_persistenceModelLoader.CreatePersistenceMappingValidator(baseClass);

      Assert.That(validator.ValidationRules.Count, Is.EqualTo(2));
      Assert.That(validator.ValidationRules[0], Is.TypeOf(typeof(PropertyStorageClassIsSupportedByStorageProviderValidationRule)));
      Assert.That(validator.ValidationRules[1], Is.TypeOf(typeof(RelationPropertyStorageClassMatchesReferencedTypeDefinitionStorageClassValidationRule)));
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_CreatesEntities_AndProperties_ForClassAndDerivedClasses ()
    {
      var baseClass = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(BaseClass), baseClass: null);
      var derivedClass = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(DerivedClass), baseClass: baseClass);
      baseClass.SetDerivedClasses(new[] { derivedClass });
      derivedClass.SetDerivedClasses(new ClassDefinition[0]);

      var persistentProperty = PropertyDefinitionObjectMother.CreateForFakePropertyInfo(baseClass, "PersistentProperty", StorageClass.Persistent);
      var transactionProperty = PropertyDefinitionObjectMother.CreateForFakePropertyInfo(baseClass, "TransactionProperty", StorageClass.Transaction);
      baseClass.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { persistentProperty, transactionProperty }, true));
      derivedClass.SetPropertyDefinitions(new PropertyDefinitionCollection(new PropertyDefinition[0], true));

      _persistenceModelLoader.ApplyPersistenceModelToHierarchy(baseClass);

      Assert.That(baseClass.StorageEntityDefinition, Is.TypeOf<NonPersistentStorageEntity>());
      Assert.That(derivedClass.StorageEntityDefinition, Is.TypeOf<NonPersistentStorageEntity>());

      Assert.That(persistentProperty.StoragePropertyDefinition, Is.SameAs(NonPersistentStorageProperty.Instance));
      Assert.That(transactionProperty.StorageClass, Is.EqualTo(StorageClass.Transaction));
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_LeavesExistingEntities_AndProperties ()
    {
      var fakeProviderDefinition = new NonPersistentProviderDefinition("Test", new Mock<INonPersistentStorageObjectFactory>().Object);
      var fakeEntityDefinition = new NonPersistentStorageEntity(fakeProviderDefinition);
      var fakeStoragePropertyDefinition = (NonPersistentStorageProperty)Activator.CreateInstance(typeof(NonPersistentStorageProperty), true);

      var baseClass = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(BaseClass), baseClass: null);
      baseClass.SetDerivedClasses(new ClassDefinition[0]);
      var persistentProperty = PropertyDefinitionObjectMother.CreateForFakePropertyInfo(baseClass, "PersistentProperty", StorageClass.Persistent);
      baseClass.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { persistentProperty }, true));

      baseClass.SetStorageEntity(fakeEntityDefinition);
      persistentProperty.SetStorageProperty(fakeStoragePropertyDefinition);

      _persistenceModelLoader.ApplyPersistenceModelToHierarchy(baseClass);

      Assert.That(baseClass.StorageEntityDefinition, Is.SameAs(fakeEntityDefinition));
      Assert.That(persistentProperty.StoragePropertyDefinition, Is.SameAs(fakeStoragePropertyDefinition));
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_Throws_WhenExistingEntityDefinitionDoesNotImplementIEntityDefinition ()
    {
      var invalidStorageEntityDefinition = new Mock<IStorageEntityDefinition>();

      var baseClass = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(BaseClass), baseClass: null);
      baseClass.SetDerivedClasses(new ClassDefinition[0]);

      baseClass.SetStorageEntity(invalidStorageEntityDefinition.Object);
      baseClass.SetPropertyDefinitions(new PropertyDefinitionCollection(new PropertyDefinition[0], true));

      Assert.That(
          () => _persistenceModelLoader.ApplyPersistenceModelToHierarchy(baseClass),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "The storage entity definition of type "
              + "'Remotion.Data.DomainObjects.UnitTests.Persistence.NonPersistent.Model.NonPersistentPersistenceModelLoaderTestDomain.BaseClass' is not of type 'NonPersistentStorageEntity'."));
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy_Throws_WhenExistingPropertyDefinitionDoesNotImplementIColumnDefinition ()
    {
      var baseClass = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(BaseClass), baseClass: null);
      baseClass.SetDerivedClasses(new ClassDefinition[0]);

      var persistentProperty = PropertyDefinitionObjectMother.CreateForFakePropertyInfo(baseClass, "PersistentProperty", StorageClass.Persistent);
      var transactionProperty = PropertyDefinitionObjectMother.CreateForFakePropertyInfo(baseClass, "TransactionProperty", StorageClass.Transaction);
      baseClass.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { persistentProperty, transactionProperty }, true));
      persistentProperty.SetStorageProperty(new FakeStoragePropertyDefinition("Fake"));

      Assert.That(
          () => _persistenceModelLoader.ApplyPersistenceModelToHierarchy(baseClass),
          Throws.InvalidOperationException.With.Message.EqualTo(
              "The property definition 'PersistentProperty' of type "
              + "'Remotion.Data.DomainObjects.UnitTests.Persistence.NonPersistent.Model.NonPersistentPersistenceModelLoaderTestDomain.BaseClass' has a storage property type of "
              + "'Remotion.Data.DomainObjects.UnitTests.Mapping.FakeStoragePropertyDefinition' when only 'NonPersistentStorageProperty' is supported."));
    }
  }
}
