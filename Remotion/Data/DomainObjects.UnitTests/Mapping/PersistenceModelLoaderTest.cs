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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class PersistenceModelLoaderTest
  {
    private Mock<IStorageSettings> _storageSettingsStub;
    private PersistenceModelLoader _persistenceModelLoader;
    private ClassDefinition _classDefinition;

    [SetUp]
    public void SetUp ()
    {
      _storageSettingsStub = new Mock<IStorageSettings>();
      _persistenceModelLoader = new PersistenceModelLoader(_storageSettingsStub.Object);
      _classDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(Order), baseClass: null);
    }

    [Test]
    public void ApplyPersistenceModelToHierarchy ()
    {
      _classDefinition.SetDerivedClasses(new ClassDefinition[0]);
      _classDefinition.SetPropertyDefinitions(new PropertyDefinitionCollection());
      _classDefinition.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection());
      Assert.That(_classDefinition.HasStorageEntityDefinitionBeenSet, Is.False);

      _storageSettingsStub
          .Setup(stub => stub.GetStorageProviderDefinition(_classDefinition))
          .Returns(new UnitTestStorageProviderStubDefinition("DefaultStorageProvider"));

      _persistenceModelLoader.ApplyPersistenceModelToHierarchy(_classDefinition);

      Assert.That(_classDefinition.HasStorageEntityDefinitionBeenSet, Is.True);
      Assert.That(_classDefinition.StorageEntityDefinition, Is.Not.Null);
    }

    [Test]
    public void CreatePersistenceMappingValidator ()
    {
      _storageSettingsStub
          .Setup(stub => stub.GetStorageProviderDefinition(_classDefinition))
          .Returns(new UnitTestStorageProviderStubDefinition("DefaultStorageProvider"));

      var result = _persistenceModelLoader.CreatePersistenceMappingValidator(_classDefinition);

      Assert.That(result, Is.Not.Null);
      Assert.That(result, Is.TypeOf(typeof(PersistenceMappingValidator)));
    }
  }
}
