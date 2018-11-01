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
  public class TableScriptBuilderTest : SchemaGenerationTestBase
  {
    private ITableScriptElementFactory _tableScriptfactoryStub;
    private TableScriptBuilder _builder;
    private TableDefinition _tableDefinition1;
    private TableDefinition _tableDefinition2;
    private TableDefinition _tableDefinition3;
    private IScriptElement _fakeElement1;
    private IScriptElement _fakeElement2;
    private IScriptElement _fakeElement3;
    
    public override void SetUp ()
    {
      base.SetUp();

      _tableScriptfactoryStub = MockRepository.GenerateStub<ITableScriptElementFactory>();
      _builder = new TableScriptBuilder (_tableScriptfactoryStub, new SqlCommentScriptElementFactory());

      _tableDefinition1 = TableDefinitionObjectMother.Create (SchemaGenerationFirstStorageProviderDefinition);
      _tableDefinition2 = TableDefinitionObjectMother.Create (SchemaGenerationFirstStorageProviderDefinition);
      _tableDefinition3 = TableDefinitionObjectMother.Create (SchemaGenerationFirstStorageProviderDefinition);

      _fakeElement1 = MockRepository.GenerateStub<IScriptElement>();
      _fakeElement2 = MockRepository.GenerateStub<IScriptElement>();
      _fakeElement3 = MockRepository.GenerateStub<IScriptElement>();
    }

    [Test]
    public void GetCreateScript_GetDropScript_NoEntitiesAdded ()
    {
      var createScriptResult = _builder.GetCreateScript ();
      var dropScriptResult = _builder.GetDropScript ();

      Assert.That (((ScriptElementCollection) createScriptResult).Elements.Count, Is.EqualTo (1));
      Assert.That (((ScriptStatement) ((ScriptElementCollection) createScriptResult).Elements[0]).Statement, Is.EqualTo ("-- Create all tables"));
      Assert.That (((ScriptElementCollection) dropScriptResult).Elements.Count, Is.EqualTo (1));
      Assert.That (((ScriptStatement) ((ScriptElementCollection) dropScriptResult).Elements[0]).Statement, Is.EqualTo ("-- Drop all tables"));
    }

    [Test]
    public void GetCreateScript_GetDropScript_OneTableDefinitionAdded ()
    {
      _tableScriptfactoryStub.Stub(stub => stub.GetCreateElement (_tableDefinition1)).Return (_fakeElement1);
      _tableScriptfactoryStub.Stub(stub => stub.GetDropElement (_tableDefinition1)).Return (_fakeElement2);
      
      _builder.AddEntityDefinition (_tableDefinition1);

      var createScriptResult = _builder.GetCreateScript ();
      var dropScriptResult = _builder.GetDropScript ();

      Assert.That (((ScriptElementCollection) createScriptResult).Elements.Count, Is.EqualTo (2));
      Assert.That (((ScriptStatement) ((ScriptElementCollection) createScriptResult).Elements[0]).Statement, Is.EqualTo ("-- Create all tables"));
      Assert.That (((ScriptElementCollection) createScriptResult).Elements[1], Is.SameAs(_fakeElement1));

      Assert.That (((ScriptElementCollection) dropScriptResult).Elements.Count, Is.EqualTo (2));
      Assert.That (((ScriptStatement) ((ScriptElementCollection) dropScriptResult).Elements[0]).Statement, Is.EqualTo ("-- Drop all tables"));
      Assert.That (((ScriptElementCollection) dropScriptResult).Elements[1], Is.SameAs (_fakeElement2));
    }

    [Test]
    public void GetCreateScript_GetDropScript_SeveralTableDefinitionsAdded ()
    {
      _tableScriptfactoryStub.Stub (stub => stub.GetCreateElement (_tableDefinition1)).Return (_fakeElement1);
      _tableScriptfactoryStub.Stub (stub => stub.GetDropElement (_tableDefinition1)).Return (_fakeElement3);
      _tableScriptfactoryStub.Stub (stub => stub.GetCreateElement (_tableDefinition2)).Return (_fakeElement2);
      _tableScriptfactoryStub.Stub (stub => stub.GetDropElement (_tableDefinition2)).Return (_fakeElement2);
      _tableScriptfactoryStub.Stub (stub => stub.GetCreateElement (_tableDefinition3)).Return (_fakeElement3);
      _tableScriptfactoryStub.Stub (stub => stub.GetDropElement (_tableDefinition3)).Return (_fakeElement1);
      
      _builder.AddEntityDefinition (_tableDefinition1);
      _builder.AddEntityDefinition (_tableDefinition2);
      _builder.AddEntityDefinition (_tableDefinition3);

      var createScriptResult = _builder.GetCreateScript ();
      var dropScriptResult = _builder.GetDropScript ();

      Assert.That (((ScriptElementCollection) createScriptResult).Elements.Count, Is.EqualTo (4));
      Assert.That (((ScriptStatement) ((ScriptElementCollection) createScriptResult).Elements[0]).Statement, Is.EqualTo ("-- Create all tables"));
      Assert.That (((ScriptElementCollection) createScriptResult).Elements[1], Is.SameAs (_fakeElement1));
      Assert.That (((ScriptElementCollection) createScriptResult).Elements[2], Is.SameAs (_fakeElement2));
      Assert.That (((ScriptElementCollection) createScriptResult).Elements[3], Is.SameAs (_fakeElement3));

      Assert.That (((ScriptElementCollection) dropScriptResult).Elements.Count, Is.EqualTo (4));
      Assert.That (((ScriptStatement) ((ScriptElementCollection) dropScriptResult).Elements[0]).Statement, Is.EqualTo ("-- Drop all tables"));
      Assert.That (((ScriptElementCollection) dropScriptResult).Elements[1], Is.SameAs (_fakeElement3));
      Assert.That (((ScriptElementCollection) dropScriptResult).Elements[2], Is.SameAs (_fakeElement2));
      Assert.That (((ScriptElementCollection) dropScriptResult).Elements[3], Is.SameAs (_fakeElement1));
    }

    [Test]
    public void GetCreateScript_GetDropScript_FilterViewDefinitionAdded ()
    {
      var entityDefinition = FilterViewDefinitionObjectMother.Create (SchemaGenerationFirstStorageProviderDefinition);
      _builder.AddEntityDefinition (entityDefinition);

      var createScriptResult = _builder.GetCreateScript ();
      var dropScriptResult = _builder.GetDropScript ();

      Assert.That (((ScriptElementCollection) createScriptResult).Elements.Count, Is.EqualTo (1));
      Assert.That (((ScriptStatement) ((ScriptElementCollection) createScriptResult).Elements[0]).Statement, Is.EqualTo ("-- Create all tables"));
      Assert.That (((ScriptElementCollection) dropScriptResult).Elements.Count, Is.EqualTo (1));
      Assert.That (((ScriptStatement) ((ScriptElementCollection) dropScriptResult).Elements[0]).Statement, Is.EqualTo ("-- Drop all tables"));
    }

    [Test]
    public void GetCreateScript_GetDropScript_UnionViewDefinitionAdded ()
    {
      var entityDefinition = UnionViewDefinitionObjectMother.Create (SchemaGenerationFirstStorageProviderDefinition);
      _builder.AddEntityDefinition (entityDefinition);

      var createScriptResult = _builder.GetCreateScript ();
      var dropScriptResult = _builder.GetDropScript ();

      Assert.That (((ScriptElementCollection) createScriptResult).Elements.Count, Is.EqualTo (1));
      Assert.That (((ScriptStatement) ((ScriptElementCollection) createScriptResult).Elements[0]).Statement, Is.EqualTo ("-- Create all tables"));
      Assert.That (((ScriptElementCollection) dropScriptResult).Elements.Count, Is.EqualTo (1));
      Assert.That (((ScriptStatement) ((ScriptElementCollection) dropScriptResult).Elements[0]).Statement, Is.EqualTo ("-- Drop all tables"));
    }

    [Test]
    public void GetCreateScript_GetDropScript_EmptyViewDefinitionAdded ()
    {
      var entityDefinition = EmptyViewDefinitionObjectMother.Create (SchemaGenerationFirstStorageProviderDefinition);
      _builder.AddEntityDefinition (entityDefinition);

      var createScriptResult = _builder.GetCreateScript();
      var dropScriptResult = _builder.GetDropScript();

      Assert.That (((ScriptElementCollection) createScriptResult).Elements.Count, Is.EqualTo(1));
      Assert.That (((ScriptStatement) ((ScriptElementCollection) createScriptResult).Elements[0]).Statement, Is.EqualTo ("-- Create all tables"));
      Assert.That (((ScriptElementCollection) dropScriptResult).Elements.Count, Is.EqualTo(1));
      Assert.That (((ScriptStatement) ((ScriptElementCollection) dropScriptResult).Elements[0]).Statement, Is.EqualTo ("-- Drop all tables"));

    }
  }
}