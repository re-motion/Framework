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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.NonPersistent;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.UnitTests.Mapping;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGeneration
{
  [TestFixture]
  public class RdbmsStructuredTypeDefinitionProviderTest : SchemaGenerationTestBase
  {
    private class RdbmsStructuredTypeDefinitionStub : IRdbmsStructuredTypeDefinition
    {
      public EntityNameDefinition TypeName => throw new NotImplementedException();

      public IReadOnlyCollection<IRdbmsStoragePropertyDefinition> Properties { get; } = [];

      public void Accept (IRdbmsStructuredTypeDefinitionVisitor visitor) => throw new NotImplementedException();
    }

    [Test]
    public void GetTypeDefinitions_UsesSimpleStructuredTypeDefinitionRepository ()
    {
      var structuredTypeDefinitions = new[]
                                      {
                                          new RdbmsStructuredTypeDefinitionStub(),
                                          new RdbmsStructuredTypeDefinitionStub(),
                                          new RdbmsStructuredTypeDefinitionStub()
                                      };

      var simpleStructuredTypeDefinitionRepositoryStub = new Mock<ISingleScalarStructuredTypeDefinitionProvider>();
      simpleStructuredTypeDefinitionRepositoryStub.Setup(_ => _.GetAllStructuredTypeDefinitions()).Returns(structuredTypeDefinitions);

      var structuredTypeDefinitions2 = new[]
                                      {
                                          new RdbmsStructuredTypeDefinitionStub(),
                                          new RdbmsStructuredTypeDefinitionStub(),
                                          new RdbmsStructuredTypeDefinitionStub(),
                                          new RdbmsStructuredTypeDefinitionStub()
                                      };
      var recordDefinitions = structuredTypeDefinitions2
          .Select(e => new RecordDefinition("p", e, [])).ToArray();

      var classDefinition1 = ClassDefinitionObjectMother.CreateClassDefinition();
      classDefinition1.SetStorageEntity(CreateTableDefinitionStub());

      var classDefinition2 = ClassDefinitionObjectMother.CreateClassDefinition();
      classDefinition2.SetStorageEntity(CreateUnionViewDefinitionStub());

      var classDefinitions = new[]
                             {
                                 classDefinition1,
                                 classDefinition2
                             };

      var tableManipulationRecordDefinitionProviderStub = new Mock<ITableManipulationRecordDefinitionProvider>(MockBehavior.Strict);
      tableManipulationRecordDefinitionProviderStub.Setup(_ => _.GetDeleteRecordDefinition(classDefinition1)).Returns(recordDefinitions[0]);
      tableManipulationRecordDefinitionProviderStub.Setup(_ => _.GetLockRecordDefinition(classDefinition1)).Returns(recordDefinitions[1]);
      tableManipulationRecordDefinitionProviderStub.Setup(_ => _.GetInsertRecordDefinition(classDefinition1)).Returns(recordDefinitions[2]);
      tableManipulationRecordDefinitionProviderStub.Setup(_ => _.GetUpdateRecordDefinition(classDefinition1)).Returns(recordDefinitions[3]);

      var storageObjectFactoryStub = new Mock<IRdbmsStorageObjectFactory>();
      var rdbmsProviderDefinition = new RdbmsProviderDefinition("Test", storageObjectFactoryStub.Object, "whatever", "whatever");

      storageObjectFactoryStub
          .Setup(_ => _.CreateSingleScalarStructuredTypeDefinitionProvider(rdbmsProviderDefinition))
          .Returns(simpleStructuredTypeDefinitionRepositoryStub.Object);

      storageObjectFactoryStub
          .Setup(_ => _.CreateTableManipulationRecordDefinitionProvider(rdbmsProviderDefinition))
          .Returns(tableManipulationRecordDefinitionProviderStub.Object);

      storageObjectFactoryStub
          .Setup(_ => _.CreateRdbmsPersistenceModelProvider(rdbmsProviderDefinition))
          .Returns(new RdbmsPersistenceModelProvider());

      var provider = new RdbmsStructuredTypeDefinitionProvider();
      var result = provider.GetTypeDefinitions(rdbmsProviderDefinition, classDefinitions).ToArray();

      Assert.That(result, Is.EqualTo(structuredTypeDefinitions.Concat(structuredTypeDefinitions2)));
    }

    private static ClassDefinition CreateClassDefinitionStub ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      classDefinition.SetStorageEntity(CreateTableDefinitionStub());

      return classDefinition;
    }

    private static TableDefinition CreateTableDefinitionStub ()
    {
      // Yeah, it is a hack. Creating a TableDefinition is quite a lot of work, and we only need the object to pass a type check.
      return (TableDefinition)RuntimeHelpers.GetUninitializedObject(typeof(TableDefinition));
    }

    private static UnionViewDefinition CreateUnionViewDefinitionStub ()
    {
      // Yeah, it is a hack. Creating a TableDefinition is quite a lot of work, and we only need the object to pass a type check.
      return (UnionViewDefinition)RuntimeHelpers.GetUninitializedObject(typeof(UnionViewDefinition));
    }
  }
}
