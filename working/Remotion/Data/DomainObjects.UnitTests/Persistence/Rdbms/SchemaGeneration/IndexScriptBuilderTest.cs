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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGeneration
{
  [TestFixture]
  public class IndexScriptBuilderTest : SchemaGenerationTestBase
  {
    private IIndexScriptElementFactory _indexScriptElementFactoryStub;
    private IndexScriptBuilder _builder;
    private TableDefinition _tableDefinition1;
    private TableDefinition _tableDefinition2;
    private UnionViewDefinition _unionViewDefinition1;
    private UnionViewDefinition _unionViewDefinition2;
    private FilterViewDefinition _filterViewDefinition1;
    private FilterViewDefinition _filterViewDefinition2;
    private IScriptElement _fakeElement1;
    private IScriptElement _fakeElement2;
    private IScriptElement _fakeElement3;
    private IIndexDefinition _indexDefinition1;
    private IIndexDefinition _indexDefinition2;
    private IIndexDefinition _indexDefinition3;

    public override void SetUp ()
    {
      base.SetUp();

      _indexScriptElementFactoryStub = MockRepository.GenerateStub<IIndexScriptElementFactory>();

      _builder = new IndexScriptBuilder (_indexScriptElementFactoryStub, new SqlCommentScriptElementFactory());

      _indexDefinition1 = MockRepository.GenerateStub<IIndexDefinition>();
      _indexDefinition2 = MockRepository.GenerateStub<IIndexDefinition>();
      _indexDefinition3 = MockRepository.GenerateStub<IIndexDefinition>();

      _tableDefinition1 = TableDefinitionObjectMother.CreateWithIndexes (
          SchemaGenerationFirstStorageProviderDefinition,
          new[] { _indexDefinition1 });
      _tableDefinition2 = TableDefinitionObjectMother.CreateWithIndexes (
          SchemaGenerationFirstStorageProviderDefinition,
          new[] { _indexDefinition2, _indexDefinition3 });
      _unionViewDefinition1 = UnionViewDefinitionObjectMother.CreateWithIndexes (
          SchemaGenerationFirstStorageProviderDefinition,
          new[] { _indexDefinition1 });
      _unionViewDefinition2 = UnionViewDefinitionObjectMother.CreateWithIndexes (
          SchemaGenerationFirstStorageProviderDefinition,
          new[] { _indexDefinition2, _indexDefinition3 });
      _filterViewDefinition1 = FilterViewDefinitionObjectMother.CreateWithIndexes (
          SchemaGenerationFirstStorageProviderDefinition,
          new[] { _indexDefinition1 });
      _filterViewDefinition2 = FilterViewDefinitionObjectMother.CreateWithIndexes (
          SchemaGenerationFirstStorageProviderDefinition,
          new[] { _indexDefinition2, _indexDefinition3 });

      _fakeElement1 = MockRepository.GenerateStub<IScriptElement>();
      _fakeElement2 = MockRepository.GenerateStub<IScriptElement>();
      _fakeElement3 = MockRepository.GenerateStub<IScriptElement>();
    }

    [Test]
    public void GetCreateScript_GetDropScript_NoEntitiesAdded ()
    {
      var createScriptResult = (ScriptElementCollection) _builder.GetCreateScript();
      var dropScriptResult = (ScriptElementCollection) _builder.GetDropScript();

      Assert.That (createScriptResult.Elements.Count, Is.EqualTo (1));
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Create indexes for tables that were created above"));
      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (1));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop all indexes"));
    }

    [Test]
    public void GetCreateScript_GetDropScript_OneTableDefinitionWithOneIndexDefinitionAdded ()
    {
      _indexScriptElementFactoryStub.Stub (stub => stub.GetCreateElement (_indexDefinition1, _tableDefinition1.TableName)).Return (_fakeElement1);
      _indexScriptElementFactoryStub.Stub (stub => stub.GetDropElement (_indexDefinition1, _tableDefinition1.TableName)).Return (_fakeElement2);

      _builder.AddEntityDefinition (_tableDefinition1);

      var createScriptResult = (ScriptElementCollection) _builder.GetCreateScript();
      var dropScriptResult = (ScriptElementCollection) _builder.GetDropScript();

      Assert.That (createScriptResult.Elements.Count, Is.EqualTo (2));
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Create indexes for tables that were created above"));
      Assert.That (createScriptResult.Elements[1], Is.SameAs (_fakeElement1));

      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (2));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop all indexes"));
      Assert.That (dropScriptResult.Elements[1], Is.SameAs (_fakeElement2));
    }

    [Test]
    public void GetCreateScript_GetDropScript_SeveralTableDefinitionsWithSeveralIndexesAdded ()
    {
      _indexScriptElementFactoryStub.Stub (stub => stub.GetCreateElement (_indexDefinition1, _tableDefinition1.TableName)).Return (_fakeElement1);
      _indexScriptElementFactoryStub.Stub (stub => stub.GetDropElement (_indexDefinition1, _tableDefinition1.TableName)).Return (_fakeElement3);
      _indexScriptElementFactoryStub.Stub (stub => stub.GetCreateElement (_indexDefinition2, _tableDefinition2.TableName)).Return (_fakeElement2);
      _indexScriptElementFactoryStub.Stub (stub => stub.GetDropElement (_indexDefinition2, _tableDefinition2.TableName)).Return (_fakeElement2);
      _indexScriptElementFactoryStub.Stub (stub => stub.GetCreateElement (_indexDefinition3, _tableDefinition2.TableName)).Return (_fakeElement3);
      _indexScriptElementFactoryStub.Stub (stub => stub.GetDropElement (_indexDefinition3, _tableDefinition2.TableName)).Return (_fakeElement1);

      _builder.AddEntityDefinition (_tableDefinition1);
      _builder.AddEntityDefinition (_tableDefinition2);

      var createScriptResult = (ScriptElementCollection) _builder.GetCreateScript();
      var dropScriptResult = (ScriptElementCollection) _builder.GetDropScript();

      Assert.That (createScriptResult.Elements.Count, Is.EqualTo (4));
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Create indexes for tables that were created above"));
      Assert.That (createScriptResult.Elements[1], Is.SameAs (_fakeElement1));
      Assert.That (createScriptResult.Elements[2], Is.SameAs (_fakeElement2));
      Assert.That (createScriptResult.Elements[3], Is.SameAs (_fakeElement3));

      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (4));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop all indexes"));
      Assert.That (dropScriptResult.Elements[1], Is.SameAs (_fakeElement3));
      Assert.That (dropScriptResult.Elements[2], Is.SameAs (_fakeElement2));
      Assert.That (dropScriptResult.Elements[3], Is.SameAs (_fakeElement1));
    }

    [Test]
    public void GetCreateScript_GetDropScript_OneUnionViewDefinitionWithOneIndexDefinitionAdded ()
    {
      _indexScriptElementFactoryStub.Stub (stub => stub.GetCreateElement (_indexDefinition1, _unionViewDefinition1.ViewName)).Return (_fakeElement1);
      _indexScriptElementFactoryStub.Stub (stub => stub.GetDropElement (_indexDefinition1, _unionViewDefinition1.ViewName)).Return (_fakeElement2);

      _builder.AddEntityDefinition (_unionViewDefinition1);

      var createScriptResult = (ScriptElementCollection) _builder.GetCreateScript();
      var dropScriptResult = (ScriptElementCollection) _builder.GetDropScript();

      Assert.That (createScriptResult.Elements.Count, Is.EqualTo (2));
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Create indexes for tables that were created above"));
      Assert.That (createScriptResult.Elements[1], Is.SameAs (_fakeElement1));

      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (2));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop all indexes"));
      Assert.That (dropScriptResult.Elements[1], Is.SameAs (_fakeElement2));
    }

    [Test]
    public void GetCreateScript_GetDropScript_SeveralUnionViewDefinitionsWithSeveralIndexesAdded ()
    {
      _indexScriptElementFactoryStub.Stub (stub => stub.GetCreateElement (_indexDefinition1, _unionViewDefinition1.ViewName)).Return (_fakeElement1);
      _indexScriptElementFactoryStub.Stub (stub => stub.GetDropElement (_indexDefinition1, _unionViewDefinition1.ViewName)).Return (_fakeElement3);
      _indexScriptElementFactoryStub.Stub (stub => stub.GetCreateElement (_indexDefinition2, _unionViewDefinition2.ViewName)).Return (_fakeElement2);
      _indexScriptElementFactoryStub.Stub (stub => stub.GetDropElement (_indexDefinition2, _unionViewDefinition2.ViewName)).Return (_fakeElement2);
      _indexScriptElementFactoryStub.Stub (stub => stub.GetCreateElement (_indexDefinition3, _unionViewDefinition2.ViewName)).Return (_fakeElement3);
      _indexScriptElementFactoryStub.Stub (stub => stub.GetDropElement (_indexDefinition3, _unionViewDefinition2.ViewName)).Return (_fakeElement1);

      _builder.AddEntityDefinition (_unionViewDefinition1);
      _builder.AddEntityDefinition (_unionViewDefinition2);

      var createScriptResult = (ScriptElementCollection) _builder.GetCreateScript();
      var dropScriptResult = (ScriptElementCollection) _builder.GetDropScript();

      Assert.That (createScriptResult.Elements.Count, Is.EqualTo (4));
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Create indexes for tables that were created above"));
      Assert.That (createScriptResult.Elements[1], Is.SameAs (_fakeElement1));
      Assert.That (createScriptResult.Elements[2], Is.SameAs (_fakeElement2));
      Assert.That (createScriptResult.Elements[3], Is.SameAs (_fakeElement3));

      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (4));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop all indexes"));
      Assert.That (dropScriptResult.Elements[1], Is.SameAs (_fakeElement3));
      Assert.That (dropScriptResult.Elements[2], Is.SameAs (_fakeElement2));
      Assert.That (dropScriptResult.Elements[3], Is.SameAs (_fakeElement1));
    }

    [Test]
    public void GetCreateScript_GetDropScript_OneFilterViewDefinitionWithOneIndexDefinitionAdded ()
    {
      _indexScriptElementFactoryStub.Stub (stub => stub.GetCreateElement (_indexDefinition1, _filterViewDefinition1.ViewName)).Return (_fakeElement1);
      _indexScriptElementFactoryStub.Stub (stub => stub.GetDropElement (_indexDefinition1, _filterViewDefinition1.ViewName)).Return (_fakeElement2);

      _builder.AddEntityDefinition (_filterViewDefinition1);

      var createScriptResult = (ScriptElementCollection) _builder.GetCreateScript();
      var dropScriptResult = (ScriptElementCollection) _builder.GetDropScript();

      Assert.That (createScriptResult.Elements.Count, Is.EqualTo (2));
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Create indexes for tables that were created above"));
      Assert.That (createScriptResult.Elements[1], Is.SameAs (_fakeElement1));

      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (2));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop all indexes"));
      Assert.That (dropScriptResult.Elements[1], Is.SameAs (_fakeElement2));
    }

    [Test]
    public void GetCreateScript_GetDropScript_SeveralFilterViewDefinitionsWithSeveralIndexesAdded ()
    {
      _indexScriptElementFactoryStub.Stub (stub => stub.GetCreateElement (_indexDefinition1, _filterViewDefinition1.ViewName)).Return (_fakeElement1);
      _indexScriptElementFactoryStub.Stub (stub => stub.GetDropElement (_indexDefinition1, _filterViewDefinition1.ViewName)).Return (_fakeElement3);
      _indexScriptElementFactoryStub.Stub (stub => stub.GetCreateElement (_indexDefinition2, _filterViewDefinition2.ViewName)).Return (_fakeElement2);
      _indexScriptElementFactoryStub.Stub (stub => stub.GetDropElement (_indexDefinition2, _filterViewDefinition2.ViewName)).Return (_fakeElement2);
      _indexScriptElementFactoryStub.Stub (stub => stub.GetCreateElement (_indexDefinition3, _filterViewDefinition2.ViewName)).Return (_fakeElement3);
      _indexScriptElementFactoryStub.Stub (stub => stub.GetDropElement (_indexDefinition3, _filterViewDefinition2.ViewName)).Return (_fakeElement1);

      _builder.AddEntityDefinition (_filterViewDefinition1);
      _builder.AddEntityDefinition (_filterViewDefinition2);

      var createScriptResult = (ScriptElementCollection) _builder.GetCreateScript();
      var dropScriptResult = (ScriptElementCollection) _builder.GetDropScript();

      Assert.That (createScriptResult.Elements.Count, Is.EqualTo (4));
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Create indexes for tables that were created above"));
      Assert.That (createScriptResult.Elements[1], Is.SameAs (_fakeElement1));
      Assert.That (createScriptResult.Elements[2], Is.SameAs (_fakeElement2));
      Assert.That (createScriptResult.Elements[3], Is.SameAs (_fakeElement3));

      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (4));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop all indexes"));
      Assert.That (dropScriptResult.Elements[1], Is.SameAs (_fakeElement3));
      Assert.That (dropScriptResult.Elements[2], Is.SameAs (_fakeElement2));
      Assert.That (dropScriptResult.Elements[3], Is.SameAs (_fakeElement1));
    }

    [Test]
    public void GetCreateScript_GetDropScript_NullEntityDefinitionAdded ()
    {
      var entityDefinition = EmptyViewDefinitionObjectMother.Create (SchemaGenerationFirstStorageProviderDefinition);
      _builder.AddEntityDefinition (entityDefinition);

      var createScriptResult = (ScriptElementCollection) _builder.GetCreateScript();
      var dropScriptResult = (ScriptElementCollection) _builder.GetDropScript();

      Assert.That (createScriptResult.Elements.Count, Is.EqualTo (1));
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Create indexes for tables that were created above"));
      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (1));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop all indexes"));
    }
  }
}