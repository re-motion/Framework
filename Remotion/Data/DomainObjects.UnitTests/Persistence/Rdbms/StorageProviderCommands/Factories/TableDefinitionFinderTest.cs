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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.StorageProviderCommands.Factories
{
  [TestFixture]
  public class TableDefinitionFinderTest : StandardMappingTest
  {
    private IRdbmsPersistenceModelProvider _rdbmsPersistenceModelProviderStub;
    private TableDefinitionFinder _finder;
    private TableDefinition _tableDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _rdbmsPersistenceModelProviderStub = MockRepository.GenerateStub<IRdbmsPersistenceModelProvider>();
      _finder = new TableDefinitionFinder (_rdbmsPersistenceModelProviderStub);

      _tableDefinition = TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table"));
    }

    [Test]
    public void GetTableDefinition_TableDefinition ()
    {
      var objectID = CreateObjectID (_tableDefinition);
      _rdbmsPersistenceModelProviderStub.Stub (stub => stub.GetEntityDefinition (objectID.ClassDefinition)).Return (_tableDefinition);

      var result = _finder.GetTableDefinition (objectID);

      Assert.That (result, Is.SameAs (_tableDefinition));
    }

    [Test]
    public void GetTableDefinition_FilterViewDefinition ()
    {
      var filterViewDefinition = FilterViewDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "FilterView"), _tableDefinition);

      var objectID = CreateObjectID (filterViewDefinition);
      _rdbmsPersistenceModelProviderStub.Stub (stub => stub.GetEntityDefinition (objectID.ClassDefinition)).Return (filterViewDefinition);

      var result = _finder.GetTableDefinition (objectID);

      Assert.That (result, Is.SameAs (_tableDefinition));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "An ObjectID's EntityDefinition cannot be a UnionViewDefinition.")]
    public void GetTableDefinition_UnionViewDefinition ()
    {
      var unionViewDefinition = UnionViewDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition, new EntityNameDefinition (null, "Table"), _tableDefinition);

      var objectID = CreateObjectID (unionViewDefinition);
      _rdbmsPersistenceModelProviderStub.Stub (stub => stub.GetEntityDefinition (objectID.ClassDefinition)).Return (unionViewDefinition);

      _finder.GetTableDefinition (objectID);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "An ObjectID's EntityDefinition cannot be a EmptyViewDefinition.")]
    public void GetTableDefinition_EmptyViewDefinition()
    {
      var emptyViewDefinition = EmptyViewDefinitionObjectMother.Create (TestDomainStorageProviderDefinition);

      var objectID = CreateObjectID (emptyViewDefinition);
      _rdbmsPersistenceModelProviderStub.Stub (stub => stub.GetEntityDefinition (objectID.ClassDefinition)).Return (emptyViewDefinition);

      _finder.GetTableDefinition (objectID);
    }

    private ObjectID CreateObjectID (IStorageEntityDefinition entityDefinition)
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (classType: typeof (Order), baseClass: null);
      classDefinition.SetStorageEntity (entityDefinition);

      return new ObjectID(classDefinition, Guid.NewGuid ());
    }
  }
}