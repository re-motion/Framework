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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGeneration
{
  [TestFixture]
  public class ForeignKeyConstraintScriptBuilderTest : SchemaGenerationTestBase
  {
    private Mock<IForeignKeyConstraintScriptElementFactory> _factoryStub;
    private ForeignKeyConstraintScriptBuilder _builder;
    private TableDefinition _tableDefinition1;
    private Mock<IScriptElement> _fakeElement1;
    private Mock<IScriptElement> _fakeElement2;
    private Mock<IScriptElement> _fakeElement3;
    private EntityNameDefinition _tableName;
    private ForeignKeyConstraintDefinition _constraint1;
    private ForeignKeyConstraintDefinition _constraint2;
    private ForeignKeyConstraintDefinition _constraint3;
    private TableDefinition _tableDefinition2;

    public override void SetUp ()
    {
      base.SetUp();

      _factoryStub = new Mock<IForeignKeyConstraintScriptElementFactory>();

      _builder = new ForeignKeyConstraintScriptBuilder(_factoryStub.Object, new SqlCommentScriptElementFactory());

      _tableName = new EntityNameDefinition(null, "Table");
      _constraint1 = new ForeignKeyConstraintDefinition("FK1", _tableName, new ColumnDefinition[0], new ColumnDefinition[0]);
      _constraint2 = new ForeignKeyConstraintDefinition("FK2", _tableName, new ColumnDefinition[0], new ColumnDefinition[0]);
      _constraint3 = new ForeignKeyConstraintDefinition("FK3", _tableName, new ColumnDefinition[0], new ColumnDefinition[0]);

      _tableDefinition1 = TableDefinitionObjectMother.Create(
          SchemaGenerationFirstStorageProviderDefinition,
          _tableName,
          null,
          new[] { _constraint1 });
      _tableDefinition2 = TableDefinitionObjectMother.Create(
          SchemaGenerationFirstStorageProviderDefinition,
          _tableName,
          null,
          new[] { _constraint2, _constraint3 });

      _fakeElement1 = new Mock<IScriptElement>();
      _fakeElement2 = new Mock<IScriptElement>();
      _fakeElement3 = new Mock<IScriptElement>();
    }

    [Test]
    public void AddStructuredTypeDefinition_DoesNothing ()
    {
      var elements = _builder.GetCreateScript().As<ScriptElementCollection>().Elements;
      Assert.That(elements.Count, Is.EqualTo(1));

      var scriptStatement = elements[0].As<ScriptStatement>();
      var before = scriptStatement.Statement;

      _builder.AddStructuredTypeDefinition(Mock.Of<IRdbmsStructuredTypeDefinition>());

      Assert.That(elements.Count, Is.EqualTo(1));
      Assert.That(scriptStatement.Statement, Is.EqualTo(before));
    }

    [Test]
    public void GetCreateScript_GetDropScript_NoEntitiesAdded ()
    {
      var createScriptResult = _builder.GetCreateScript();
      var dropScriptResult = _builder.GetDropScript();

      Assert.That(((ScriptElementCollection)createScriptResult).Elements.Count, Is.EqualTo(1));
      Assert.That(
          ((ScriptStatement)((ScriptElementCollection)createScriptResult).Elements[0]).Statement,
          Is.EqualTo("-- Create foreign key constraints for tables that were created above"));
      Assert.That(((ScriptElementCollection)dropScriptResult).Elements.Count, Is.EqualTo(1));
      Assert.That(
          ((ScriptStatement)((ScriptElementCollection)dropScriptResult).Elements[0]).Statement, Is.EqualTo("-- Drop foreign keys of all tables"));
    }

    [Test]
    public void GetCreateScript_GetDropScript_OneTableDefinitionAdded ()
    {
      _factoryStub.Setup(mock => mock.GetCreateElement(_constraint1, _tableName)).Returns(_fakeElement1.Object);
      _factoryStub.Setup(mock => mock.GetDropElement(_constraint1, _tableName)).Returns(_fakeElement2.Object);

      _builder.AddEntityDefinition(_tableDefinition1);

      var createScriptResult = _builder.GetCreateScript();
      var dropScriptResult = _builder.GetDropScript();

      Assert.That(((ScriptElementCollection)createScriptResult).Elements.Count, Is.EqualTo(2));
      Assert.That(
          ((ScriptStatement)((ScriptElementCollection)createScriptResult).Elements[0]).Statement,
          Is.EqualTo("-- Create foreign key constraints for tables that were created above"));
      Assert.That(((ScriptElementCollection)createScriptResult).Elements[1], Is.SameAs(_fakeElement1.Object));

      Assert.That(((ScriptElementCollection)dropScriptResult).Elements.Count, Is.EqualTo(2));
      Assert.That(
          ((ScriptStatement)((ScriptElementCollection)dropScriptResult).Elements[0]).Statement, Is.EqualTo("-- Drop foreign keys of all tables"));
      Assert.That(((ScriptElementCollection)dropScriptResult).Elements[1], Is.SameAs(_fakeElement2.Object));
    }

    [Test]
    public void GetCreateScript_GetDropScript_SeveralTableDefinitionsAdded ()
    {
      _factoryStub.Setup(mock => mock.GetCreateElement(_constraint1, _tableName)).Returns(_fakeElement1.Object);
      _factoryStub.Setup(mock => mock.GetDropElement(_constraint1, _tableName)).Returns(_fakeElement3.Object);
      _factoryStub.Setup(mock => mock.GetCreateElement(_constraint2, _tableName)).Returns(_fakeElement2.Object);
      _factoryStub.Setup(mock => mock.GetDropElement(_constraint2, _tableName)).Returns(_fakeElement2.Object);
      _factoryStub.Setup(mock => mock.GetCreateElement(_constraint3, _tableName)).Returns(_fakeElement3.Object);
      _factoryStub.Setup(mock => mock.GetDropElement(_constraint3, _tableName)).Returns(_fakeElement1.Object);

      _builder.AddEntityDefinition(_tableDefinition1);
      _builder.AddEntityDefinition(_tableDefinition2);

      var createScriptResult = _builder.GetCreateScript();
      var dropScriptResult = _builder.GetDropScript();

      Assert.That(((ScriptElementCollection)createScriptResult).Elements.Count, Is.EqualTo(4));
      Assert.That(
          ((ScriptStatement)((ScriptElementCollection)createScriptResult).Elements[0]).Statement,
          Is.EqualTo("-- Create foreign key constraints for tables that were created above"));
      Assert.That(((ScriptElementCollection)createScriptResult).Elements[1], Is.SameAs(_fakeElement1.Object));
      Assert.That(((ScriptElementCollection)createScriptResult).Elements[2], Is.SameAs(_fakeElement2.Object));
      Assert.That(((ScriptElementCollection)createScriptResult).Elements[3], Is.SameAs(_fakeElement3.Object));

      Assert.That(((ScriptElementCollection)dropScriptResult).Elements.Count, Is.EqualTo(4));
      Assert.That(
          ((ScriptStatement)((ScriptElementCollection)dropScriptResult).Elements[0]).Statement, Is.EqualTo("-- Drop foreign keys of all tables"));
      Assert.That(((ScriptElementCollection)dropScriptResult).Elements[1], Is.SameAs(_fakeElement3.Object));
      Assert.That(((ScriptElementCollection)dropScriptResult).Elements[2], Is.SameAs(_fakeElement2.Object));
      Assert.That(((ScriptElementCollection)dropScriptResult).Elements[3], Is.SameAs(_fakeElement1.Object));
    }

    [Test]
    public void GetCreateScript_GetDropScript_FilterViewDefinitionAdded ()
    {
      var entityDefinition = FilterViewDefinitionObjectMother.Create(
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition(null, "FilterView"),
          _tableDefinition1);
      _builder.AddEntityDefinition(entityDefinition);

      var createScriptResult = _builder.GetCreateScript();
      var dropScriptResult = _builder.GetDropScript();

      Assert.That(((ScriptElementCollection)createScriptResult).Elements.Count, Is.EqualTo(1));
      Assert.That(
          ((ScriptStatement)((ScriptElementCollection)createScriptResult).Elements[0]).Statement,
          Is.EqualTo("-- Create foreign key constraints for tables that were created above"));
      Assert.That(((ScriptElementCollection)dropScriptResult).Elements.Count, Is.EqualTo(1));
      Assert.That(
          ((ScriptStatement)((ScriptElementCollection)dropScriptResult).Elements[0]).Statement, Is.EqualTo("-- Drop foreign keys of all tables"));
    }

    [Test]
    public void GetCreateScript_GetDropScript_UnionViewDefinitionAdded ()
    {
      var entityDefinition = UnionViewDefinitionObjectMother.Create(
          SchemaGenerationFirstStorageProviderDefinition,
          new EntityNameDefinition(null, "UnionView"),
           _tableDefinition1);
      _builder.AddEntityDefinition(entityDefinition);

      var createScriptResult = _builder.GetCreateScript();
      var dropScriptResult = _builder.GetDropScript();

      Assert.That(((ScriptElementCollection)createScriptResult).Elements.Count, Is.EqualTo(1));
      Assert.That(
          ((ScriptStatement)((ScriptElementCollection)createScriptResult).Elements[0]).Statement,
          Is.EqualTo("-- Create foreign key constraints for tables that were created above"));
      Assert.That(((ScriptElementCollection)dropScriptResult).Elements.Count, Is.EqualTo(1));
      Assert.That(
          ((ScriptStatement)((ScriptElementCollection)dropScriptResult).Elements[0]).Statement, Is.EqualTo("-- Drop foreign keys of all tables"));
    }

    [Test]
    public void GetCreateScript_GetDropScript_EmptyViewDefinitionAdded ()
    {
      var entityDefinition = EmptyViewDefinitionObjectMother.Create(SchemaGenerationFirstStorageProviderDefinition);
      _builder.AddEntityDefinition(entityDefinition);

      var createScriptResult = _builder.GetCreateScript();
      var dropScriptResult = _builder.GetDropScript();

      Assert.That(((ScriptElementCollection)createScriptResult).Elements.Count, Is.EqualTo(1));
      Assert.That(
          ((ScriptStatement)((ScriptElementCollection)createScriptResult).Elements[0]).Statement,
          Is.EqualTo("-- Create foreign key constraints for tables that were created above"));
      Assert.That(((ScriptElementCollection)dropScriptResult).Elements.Count, Is.EqualTo(1));
      Assert.That(
          ((ScriptStatement)((ScriptElementCollection)dropScriptResult).Elements[0]).Statement, Is.EqualTo("-- Drop foreign keys of all tables"));
    }
  }
}
