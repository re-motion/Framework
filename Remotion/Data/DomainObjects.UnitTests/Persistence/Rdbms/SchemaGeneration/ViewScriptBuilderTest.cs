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
  public class ViewScriptBuilderTest : SchemaGenerationTestBase
  {
    private IViewScriptElementFactory<TableDefinition> _tableViewElementFactoryStub;
    private IViewScriptElementFactory<UnionViewDefinition> _unionViewElementFactoryStub;
    private IViewScriptElementFactory<FilterViewDefinition> _filterViewElementFactoryStub;
    private IViewScriptElementFactory<EmptyViewDefinition> _emptyViewElementFactoryStub;
    private ViewScriptBuilder _builder;
    private TableDefinition _tableDefinition1;
    private TableDefinition _tableDefinition2;
    private UnionViewDefinition _unionViewDefinition1;
    private UnionViewDefinition _unionViewDefinition2;
    private FilterViewDefinition _filterViewDefinition1;
    private FilterViewDefinition _filterViewDefinition2;
    private EmptyViewDefinition _emptyViewDefinition1;
    private EmptyViewDefinition _emptyViewDefinition2;
    private IScriptElement _fakeElement1;
    private IScriptElement _fakeElement2;
    private IScriptElement _fakeElement3;

    public override void SetUp ()
    {
      base.SetUp();

      _tableViewElementFactoryStub = MockRepository.GenerateStub<IViewScriptElementFactory<TableDefinition>>();
      _unionViewElementFactoryStub = MockRepository.GenerateStub<IViewScriptElementFactory<UnionViewDefinition>>();
      _filterViewElementFactoryStub = MockRepository.GenerateStub<IViewScriptElementFactory<FilterViewDefinition>>();
      _emptyViewElementFactoryStub = MockRepository.GenerateStub<IViewScriptElementFactory<EmptyViewDefinition>>();

      _builder = new ViewScriptBuilder (
          _tableViewElementFactoryStub,
          _unionViewElementFactoryStub,
          _filterViewElementFactoryStub,
          _emptyViewElementFactoryStub,
          new SqlCommentScriptElementFactory());

      _tableDefinition1 = TableDefinitionObjectMother.Create(SchemaGenerationFirstStorageProviderDefinition);
      _tableDefinition2 = TableDefinitionObjectMother.Create (SchemaGenerationFirstStorageProviderDefinition);
      _unionViewDefinition1 = UnionViewDefinitionObjectMother.Create (SchemaGenerationFirstStorageProviderDefinition);
      _unionViewDefinition2 = UnionViewDefinitionObjectMother.Create (SchemaGenerationFirstStorageProviderDefinition);
      _filterViewDefinition1 = FilterViewDefinitionObjectMother.Create (SchemaGenerationFirstStorageProviderDefinition);
      _filterViewDefinition2 = FilterViewDefinitionObjectMother.Create (SchemaGenerationFirstStorageProviderDefinition);
      _emptyViewDefinition1 = EmptyViewDefinitionObjectMother.Create (SchemaGenerationFirstStorageProviderDefinition);
      _emptyViewDefinition2 = EmptyViewDefinitionObjectMother.Create (SchemaGenerationFirstStorageProviderDefinition);

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
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Create a view for every class"));
      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (1));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop all views"));
    }

    [Test]
    public void GetCreateScript_GetDropScript_OneTableDefinitionAdded ()
    {
      _tableViewElementFactoryStub.Stub (stub => stub.GetCreateElement (_tableDefinition1)).Return (_fakeElement1);
      _tableViewElementFactoryStub.Stub (stub => stub.GetDropElement (_tableDefinition1)).Return (_fakeElement2);

      _builder.AddEntityDefinition (_tableDefinition1);

      var createScriptResult = (ScriptElementCollection) _builder.GetCreateScript ();
      var dropScriptResult = (ScriptElementCollection) _builder.GetDropScript ();

      Assert.That (createScriptResult.Elements.Count, Is.EqualTo (2));
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Create a view for every class"));
      Assert.That (createScriptResult.Elements[1], Is.SameAs (_fakeElement1));

      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (2));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop all views"));
      Assert.That (dropScriptResult.Elements[1], Is.SameAs (_fakeElement2));
    }

    [Test]
    public void GetCreateScript_GetDropScript_SeveralTableDefinitionsAdded ()
    {
      _tableViewElementFactoryStub.Stub (stub => stub.GetCreateElement (_tableDefinition1)).Return (_fakeElement1);
      _tableViewElementFactoryStub.Stub (stub => stub.GetDropElement (_tableDefinition1)).Return (_fakeElement2);
      _tableViewElementFactoryStub.Stub (stub => stub.GetCreateElement (_tableDefinition2)).Return (_fakeElement2);
      _tableViewElementFactoryStub.Stub (stub => stub.GetDropElement (_tableDefinition2)).Return (_fakeElement1);

      _builder.AddEntityDefinition (_tableDefinition1);
      _builder.AddEntityDefinition (_tableDefinition2);

      var createScriptResult = (ScriptElementCollection) _builder.GetCreateScript ();
      var dropScriptResult = (ScriptElementCollection) _builder.GetDropScript ();

      Assert.That (createScriptResult.Elements.Count, Is.EqualTo (3));
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Create a view for every class"));
      Assert.That (createScriptResult.Elements[1], Is.SameAs (_fakeElement1));
      Assert.That (createScriptResult.Elements[2], Is.SameAs (_fakeElement2));

      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (3));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop all views"));
      Assert.That (dropScriptResult.Elements[1], Is.SameAs (_fakeElement2));
      Assert.That (dropScriptResult.Elements[2], Is.SameAs (_fakeElement1));
    }

    [Test]
    public void GetCreateScript_GetDropScript_OneUnionViewDefinitionAdded ()
    {
      _unionViewElementFactoryStub.Stub (stub => stub.GetCreateElement (_unionViewDefinition1)).Return (_fakeElement1);
      _unionViewElementFactoryStub.Stub (stub => stub.GetDropElement (_unionViewDefinition1)).Return (_fakeElement2);

      _builder.AddEntityDefinition (_unionViewDefinition1);

      var createScriptResult = (ScriptElementCollection) _builder.GetCreateScript ();
      var dropScriptResult = (ScriptElementCollection) _builder.GetDropScript ();

      Assert.That (createScriptResult.Elements.Count, Is.EqualTo (2));
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Create a view for every class"));
      Assert.That (createScriptResult.Elements[1], Is.SameAs (_fakeElement1));

      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (2));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop all views"));
      Assert.That (dropScriptResult.Elements[1], Is.SameAs (_fakeElement2));
    }

    [Test]
    public void GetCreateScript_GetDropScript_SeveralUnionViewDefinitionsAdded ()
    {
      _unionViewElementFactoryStub.Stub (stub => stub.GetCreateElement (_unionViewDefinition1)).Return (_fakeElement1);
      _unionViewElementFactoryStub.Stub (stub => stub.GetDropElement (_unionViewDefinition1)).Return (_fakeElement2);
      _unionViewElementFactoryStub.Stub (stub => stub.GetCreateElement (_unionViewDefinition2)).Return (_fakeElement2);
      _unionViewElementFactoryStub.Stub (stub => stub.GetDropElement (_unionViewDefinition2)).Return (_fakeElement1);

      _builder.AddEntityDefinition (_unionViewDefinition1);
      _builder.AddEntityDefinition (_unionViewDefinition2);

      var createScriptResult = (ScriptElementCollection) _builder.GetCreateScript ();
      var dropScriptResult = (ScriptElementCollection) _builder.GetDropScript ();

      Assert.That (createScriptResult.Elements.Count, Is.EqualTo (3));
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Create a view for every class"));
      Assert.That (createScriptResult.Elements[1], Is.SameAs (_fakeElement1));
      Assert.That (createScriptResult.Elements[2], Is.SameAs (_fakeElement2));

      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (3));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop all views"));
      Assert.That (dropScriptResult.Elements[1], Is.SameAs (_fakeElement2));
      Assert.That (dropScriptResult.Elements[2], Is.SameAs (_fakeElement1));
    }

    [Test]
    public void GetCreateScript_GetDropScript_OneFilterViewDefinitionAdded ()
    {
      _filterViewElementFactoryStub.Stub (stub => stub.GetCreateElement (_filterViewDefinition1)).Return (_fakeElement1);
      _filterViewElementFactoryStub.Stub (stub => stub.GetDropElement (_filterViewDefinition1)).Return (_fakeElement2);

      _builder.AddEntityDefinition (_filterViewDefinition1);

      var createScriptResult = (ScriptElementCollection) _builder.GetCreateScript ();
      var dropScriptResult = (ScriptElementCollection) _builder.GetDropScript ();

      Assert.That (createScriptResult.Elements.Count, Is.EqualTo (2));
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Create a view for every class"));
      Assert.That (createScriptResult.Elements[1], Is.SameAs (_fakeElement1));

      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (2));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop all views"));
      Assert.That (dropScriptResult.Elements[1], Is.SameAs (_fakeElement2));
    }

    [Test]
    public void GetCreateScript_GetDropScript_SeveralFilterViewDefinitionsAdded ()
    {
      _filterViewElementFactoryStub.Stub (stub => stub.GetCreateElement (_filterViewDefinition1)).Return (_fakeElement1);
      _filterViewElementFactoryStub.Stub (stub => stub.GetDropElement (_filterViewDefinition1)).Return (_fakeElement2);
      _filterViewElementFactoryStub.Stub (stub => stub.GetCreateElement (_filterViewDefinition2)).Return (_fakeElement2);
      _filterViewElementFactoryStub.Stub (stub => stub.GetDropElement (_filterViewDefinition2)).Return (_fakeElement1);

      _builder.AddEntityDefinition (_filterViewDefinition1);
      _builder.AddEntityDefinition (_filterViewDefinition2);

      var createScriptResult = (ScriptElementCollection) _builder.GetCreateScript ();
      var dropScriptResult = (ScriptElementCollection) _builder.GetDropScript ();

      Assert.That (createScriptResult.Elements.Count, Is.EqualTo (3));
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Create a view for every class"));
      Assert.That (createScriptResult.Elements[1], Is.SameAs (_fakeElement1));
      Assert.That (createScriptResult.Elements[2], Is.SameAs (_fakeElement2));

      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (3));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop all views"));
      Assert.That (dropScriptResult.Elements[1], Is.SameAs (_fakeElement2));
      Assert.That (dropScriptResult.Elements[2], Is.SameAs (_fakeElement1));
    }

    [Test]
    public void GetCreateScript_GetDropScript_SeveralEntityDefinitionsAdded ()
    {
      _tableViewElementFactoryStub.Stub (stub => stub.GetCreateElement (_tableDefinition1)).Return (_fakeElement1);
      _tableViewElementFactoryStub.Stub (stub => stub.GetDropElement (_tableDefinition1)).Return (_fakeElement3);
      _unionViewElementFactoryStub.Stub (stub => stub.GetCreateElement (_unionViewDefinition1)).Return (_fakeElement2);
      _unionViewElementFactoryStub.Stub (stub => stub.GetDropElement (_unionViewDefinition1)).Return (_fakeElement2);
      _filterViewElementFactoryStub.Stub (stub => stub.GetCreateElement (_filterViewDefinition1)).Return (_fakeElement3);
      _filterViewElementFactoryStub.Stub (stub => stub.GetDropElement (_filterViewDefinition1)).Return (_fakeElement1);

      _builder.AddEntityDefinition (_tableDefinition1);
      _builder.AddEntityDefinition (_unionViewDefinition1);
      _builder.AddEntityDefinition (_filterViewDefinition1);

      var createScriptResult = (ScriptElementCollection) _builder.GetCreateScript ();
      var dropScriptResult = (ScriptElementCollection) _builder.GetDropScript ();

      Assert.That (createScriptResult.Elements.Count, Is.EqualTo (4));
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Create a view for every class"));
      Assert.That (createScriptResult.Elements[1], Is.SameAs (_fakeElement1));
      Assert.That (createScriptResult.Elements[2], Is.SameAs (_fakeElement2));
      Assert.That (createScriptResult.Elements[3], Is.SameAs (_fakeElement3));

      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (4));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop all views"));
      Assert.That (dropScriptResult.Elements[1], Is.SameAs (_fakeElement3));
      Assert.That (dropScriptResult.Elements[2], Is.SameAs (_fakeElement2));
      Assert.That (dropScriptResult.Elements[3], Is.SameAs (_fakeElement1));
    }

    [Test]
    public void GetCreateScript_GetDropScript_OneEmptyViewDefinitionAdded ()
    {
      _emptyViewElementFactoryStub.Stub (stub => stub.GetCreateElement (_emptyViewDefinition1)).Return (_fakeElement1);
      _emptyViewElementFactoryStub.Stub (stub => stub.GetDropElement (_emptyViewDefinition1)).Return (_fakeElement2);

      _builder.AddEntityDefinition (_emptyViewDefinition1);

      var createScriptResult = (ScriptElementCollection) _builder.GetCreateScript ();
      var dropScriptResult = (ScriptElementCollection) _builder.GetDropScript ();

      Assert.That (createScriptResult.Elements.Count, Is.EqualTo (2));
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Create a view for every class"));
      Assert.That (createScriptResult.Elements[1], Is.SameAs (_fakeElement1));

      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (2));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop all views"));
      Assert.That (dropScriptResult.Elements[1], Is.SameAs (_fakeElement2));
    }

    [Test]
    public void GetCreateScript_GetDropScript_SeveralEmptyViewDefinitionsAdded ()
    {
      _emptyViewElementFactoryStub.Stub (stub => stub.GetCreateElement (_emptyViewDefinition1)).Return (_fakeElement1);
      _emptyViewElementFactoryStub.Stub (stub => stub.GetDropElement (_emptyViewDefinition1)).Return (_fakeElement2);
      _emptyViewElementFactoryStub.Stub (stub => stub.GetCreateElement (_emptyViewDefinition2)).Return (_fakeElement2);
      _emptyViewElementFactoryStub.Stub (stub => stub.GetDropElement (_emptyViewDefinition2)).Return (_fakeElement1);

      _builder.AddEntityDefinition (_emptyViewDefinition1);
      _builder.AddEntityDefinition (_emptyViewDefinition2);

      var createScriptResult = (ScriptElementCollection) _builder.GetCreateScript ();
      var dropScriptResult = (ScriptElementCollection) _builder.GetDropScript ();

      Assert.That (createScriptResult.Elements.Count, Is.EqualTo (3));
      Assert.That (((ScriptStatement) createScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Create a view for every class"));
      Assert.That (createScriptResult.Elements[1], Is.SameAs (_fakeElement1));
      Assert.That (createScriptResult.Elements[2], Is.SameAs (_fakeElement2));

      Assert.That (dropScriptResult.Elements.Count, Is.EqualTo (3));
      Assert.That (((ScriptStatement) dropScriptResult.Elements[0]).Statement, Is.EqualTo ("-- Drop all views"));
      Assert.That (dropScriptResult.Elements[1], Is.SameAs (_fakeElement2));
      Assert.That (dropScriptResult.Elements[2], Is.SameAs (_fakeElement1));
    }
  }
}