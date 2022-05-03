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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2014;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests.EnablingClassIDsInAllForeignKeys.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests.EnablingClassIDsInAllForeignKeys
{
  [TestFixture]
  public class DefaultBehaviorTest : CustomStorageObjectFactoryTestBase
  {
    public DefaultBehaviorTest ()
        : base(CreateEmptyTestDataFileName)
    {
    }

    protected override SqlStorageObjectFactory CreateSqlStorageObjectFactory ()
    {
      return new SqlStorageObjectFactory();
    }

    [Test]
    public void EndPointWithoutInheritanceHierarchy_HasNoClassIDColumn ()
    {
      var endPointWithoutInheritanceHierarchy = ReleationEndPointTestHelper.GetRelationEndPointDefinition(
          MappingConfiguration,
          (ClassWithRelations obj) => obj.RelationWithoutInheritanceHierarchy);

      var storagePropertyDefinition = (IRdbmsStoragePropertyDefinition)endPointWithoutInheritanceHierarchy.PropertyDefinition.StoragePropertyDefinition;
      Assert.That(storagePropertyDefinition, Is.TypeOf<ObjectIDWithoutClassIDStoragePropertyDefinition>());
      var columnDefinitions = storagePropertyDefinition.GetColumns().ToArray();
      Assert.That(columnDefinitions, Has.Length.EqualTo(1));
      Assert.That(columnDefinitions.Any(cd => cd.Name.EndsWith("ClassID")), Is.False);
    }

    [Test]
    public void EndPointWithInheritanceHierarchy_HasClassIDColumn ()
    {
      var endPointWithInheritanceHierarchy = ReleationEndPointTestHelper.GetRelationEndPointDefinition(
          MappingConfiguration,
          (ClassWithRelations obj) => obj.RelationWithInheritanceHierarchy);

      var storagePropertyDefinition = (IRdbmsStoragePropertyDefinition)endPointWithInheritanceHierarchy.PropertyDefinition.StoragePropertyDefinition;
      Assert.That(storagePropertyDefinition, Is.TypeOf<ObjectIDStoragePropertyDefinition>());
      var columnDefinitions = storagePropertyDefinition.GetColumns().ToArray();
      Assert.That(columnDefinitions, Has.Length.EqualTo(2));
      Assert.That(columnDefinitions.Any(cd => cd.Name.EndsWith("ClassID")), Is.True);
    }

    [Test]
    public void SchemaGeneration ()
    {
      var scriptGenerator = new ScriptGenerator(
          pd => pd.Factory.CreateSchemaScriptBuilder(pd),
          new RdbmsStorageEntityDefinitionProvider(),
          new ScriptToStringConverter());

      var script = scriptGenerator.GetScripts(MappingConfiguration.GetTypeDefinitions()).Single();

      const string expectedSetUpScriptFragment =
          @"CREATE TABLE [dbo].[ClassWithRelations]
(
  [ID] uniqueidentifier NOT NULL,
  [ClassID] varchar (100) NOT NULL,
  [Timestamp] rowversion NOT NULL,
  [RelationWithoutInheritanceHierarchyID] uniqueidentifier NULL,
  [RelationWithInheritanceHierarchyID] uniqueidentifier NULL,
  [RelationWithInheritanceHierarchyIDClassID] varchar (100) NULL,";
      Assert.That(script.SetUpScript, Does.Contain(expectedSetUpScriptFragment));
    }
  }
}
