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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGeneration
{
  [TestFixture]
  public class RdbmsStorageEntityDefinitionProviderTest
  {
    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;
    private RdbmsStorageEntityDefinitionProvider _entityDefinitionProvider;
    private ClassDefinition _classDefinition1;
    private ClassDefinition _classDefinition2;
    private ClassDefinition _classDefinition3;

    [SetUp]
    public void SetUp ()
    {
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition ("SPID");
      _entityDefinitionProvider = new RdbmsStorageEntityDefinitionProvider();
      _classDefinition1 = ClassDefinitionObjectMother.CreateClassDefinitionWithTable (_storageProviderDefinition, classType: typeof (Order));
      _classDefinition2 = ClassDefinitionObjectMother.CreateClassDefinitionWithTable (_storageProviderDefinition, classType: typeof (OrderItem));
      _classDefinition3 = ClassDefinitionObjectMother.CreateClassDefinitionWithTable (_storageProviderDefinition, classType: typeof (Customer));
    }

    [Test]
    public void GetEntityDefinitions_NoClassDefinition ()
    {
      Assert.That (_entityDefinitionProvider.GetEntityDefinitions (new ClassDefinition[0]), Is.Empty);
    }

    [Test]
    public void GetEntityDefinitions_OneClassDefinition ()
    {
      var result = _entityDefinitionProvider.GetEntityDefinitions (new[] { _classDefinition1 }).ToList();

      Assert.That (result.Count, Is.EqualTo (1));
      Assert.That (result[0], Is.SameAs (_classDefinition1.StorageEntityDefinition));
    }

    [Test]
    public void GetEntityDefinitions_SeveralClassDefinitions ()
    {
      var result = _entityDefinitionProvider.GetEntityDefinitions (new[] { _classDefinition1, _classDefinition2, _classDefinition3 }).ToList();

      Assert.That (
          result,
          Is.EquivalentTo (
              new[]
              { _classDefinition1.StorageEntityDefinition, _classDefinition2.StorageEntityDefinition, _classDefinition3.StorageEntityDefinition }));
    }

    [Test]
    public void GetEntityDefinitions_OnClassWithNoIEntityDefinition ()
    {
      var storageEntityDefinitionStub = MockRepository.GenerateStub<IStorageEntityDefinition> ();
      storageEntityDefinitionStub.Stub (stub => stub.StorageProviderDefinition).Return (_storageProviderDefinition);
      _classDefinition1.SetStorageEntity (storageEntityDefinitionStub);

      var result = _entityDefinitionProvider.GetEntityDefinitions (new[] { _classDefinition1, _classDefinition2, _classDefinition3 });

      Assert.That (
          result,
          Is.EquivalentTo (
              new[] { _classDefinition2.StorageEntityDefinition, _classDefinition3.StorageEntityDefinition }));
    }
  }
}