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
  public class SynonymScriptBuilderTest : SchemaGenerationTestBase
  {
    private SynonymScriptBuilder _builder;
    private TableDefinition _tableDefinition1;
    private TableDefinition _tableDefinition2;
    private UnionViewDefinition _unionViewDefinition1;
    private UnionViewDefinition _unionViewDefinition2;
    private FilterViewDefinition _filterViewDefinition1;
    private FilterViewDefinition _filterViewDefinition2;
    private EmptyViewDefinition _emptyViewDefinition1;
    private EmptyViewDefinition _emptyViewDefinition2;
    private Mock<IScriptElement> _fakeElement1;
    private Mock<IScriptElement> _fakeElement2;
    private Mock<IScriptElement> _fakeElement3;
    private Mock<ISynonymScriptElementFactory<TableDefinition>> _tableViewElementFactoryStub;
    private Mock<ISynonymScriptElementFactory<UnionViewDefinition>> _unionViewElementFactoryStub;
    private Mock<ISynonymScriptElementFactory<FilterViewDefinition>> _filterViewElementFactoryStub;
    private Mock<ISynonymScriptElementFactory<EmptyViewDefinition>> _emptyViewElementFactoryStub;
    private EntityNameDefinition _synonym1;
    private EntityNameDefinition _synonym2;
    private EntityNameDefinition _synonym3;

    public override void SetUp ()
    {
      base.SetUp();

      _tableViewElementFactoryStub = new Mock<ISynonymScriptElementFactory<TableDefinition>>();
      _unionViewElementFactoryStub = new Mock<ISynonymScriptElementFactory<UnionViewDefinition>>();
      _filterViewElementFactoryStub = new Mock<ISynonymScriptElementFactory<FilterViewDefinition>>();
      _emptyViewElementFactoryStub = new Mock<ISynonymScriptElementFactory<EmptyViewDefinition>>();

      _builder = new SynonymScriptBuilder(
          _tableViewElementFactoryStub.Object,
          _unionViewElementFactoryStub.Object,
          _filterViewElementFactoryStub.Object,
          _emptyViewElementFactoryStub.Object,
          new SqlCommentScriptElementFactory());

      _synonym1 = new EntityNameDefinition(null, "Synonym1");
      _synonym2 = new EntityNameDefinition(null, "Synonym2");
      _synonym3 = new EntityNameDefinition(null, "Synonym3");

      _tableDefinition1 = TableDefinitionObjectMother.CreateWithSynonyms(
          SchemaGenerationFirstStorageProviderDefinition,
          new[] { _synonym1 });
      _tableDefinition2 = TableDefinitionObjectMother.CreateWithSynonyms(
          SchemaGenerationFirstStorageProviderDefinition,
          new[] { _synonym2, _synonym3 });
      _unionViewDefinition1 = UnionViewDefinitionObjectMother.CreateWithSynonyms(
          SchemaGenerationFirstStorageProviderDefinition,
          new[] { _synonym1 });
      _unionViewDefinition2 = UnionViewDefinitionObjectMother.CreateWithSynonyms(
          SchemaGenerationFirstStorageProviderDefinition,
          new[] { _synonym2, _synonym3 });
      _filterViewDefinition1 = FilterViewDefinitionObjectMother.CreateWithSynonyms(
          SchemaGenerationFirstStorageProviderDefinition,
          new[] { _synonym1 });
      _filterViewDefinition2 = FilterViewDefinitionObjectMother.CreateWithSynonyms(
          SchemaGenerationFirstStorageProviderDefinition,
          new[] { _synonym2, _synonym3 });
      _emptyViewDefinition1 = EmptyViewDefinitionObjectMother.CreateWithSynonyms(
          SchemaGenerationFirstStorageProviderDefinition,
          new[] { _synonym1 });
      _emptyViewDefinition2 = EmptyViewDefinitionObjectMother.CreateWithSynonyms(
          SchemaGenerationFirstStorageProviderDefinition,
          new[] { _synonym2, _synonym3 });

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
      var createScriptResult = (ScriptElementCollection)_builder.GetCreateScript();
      var dropScriptResult = (ScriptElementCollection)_builder.GetDropScript();

      Assert.That(createScriptResult.Elements.Count, Is.EqualTo(1));
      Assert.That(((ScriptStatement)createScriptResult.Elements[0]).Statement, Is.EqualTo("-- Create synonyms for tables that were created above"));
      Assert.That(dropScriptResult.Elements.Count, Is.EqualTo(1));
      Assert.That(((ScriptStatement)dropScriptResult.Elements[0]).Statement, Is.EqualTo("-- Drop all synonyms"));
    }

    [Test]
    public void GetCreateScript_GetDropScript_OneTableDefinitionAdded ()
    {
      _tableViewElementFactoryStub
          .Setup(stub => stub.GetCreateElement(_tableDefinition1, _synonym1))
          .Returns(_fakeElement1.Object);
      _tableViewElementFactoryStub
          .Setup(stub => stub.GetDropElement(_tableDefinition1, _synonym1))
          .Returns(_fakeElement2.Object);

      _builder.AddEntityDefinition(_tableDefinition1);

      var createScriptResult = (ScriptElementCollection)_builder.GetCreateScript();
      var dropScriptResult = (ScriptElementCollection)_builder.GetDropScript();

      Assert.That(createScriptResult.Elements.Count, Is.EqualTo(2));
      Assert.That(((ScriptStatement)createScriptResult.Elements[0]).Statement, Is.EqualTo("-- Create synonyms for tables that were created above"));
      Assert.That(createScriptResult.Elements[1], Is.SameAs(_fakeElement1.Object));

      Assert.That(dropScriptResult.Elements.Count, Is.EqualTo(2));
      Assert.That(((ScriptStatement)dropScriptResult.Elements[0]).Statement, Is.EqualTo("-- Drop all synonyms"));
      Assert.That(dropScriptResult.Elements[1], Is.SameAs(_fakeElement2.Object));
    }

    [Test]
    public void GetCreateScript_GetDropScript_SeveralTableDefinitionsAdded ()
    {
      _tableViewElementFactoryStub.Setup(stub => stub.GetCreateElement(_tableDefinition1, _synonym1)).Returns(_fakeElement1.Object);
      _tableViewElementFactoryStub.Setup(stub => stub.GetDropElement(_tableDefinition1, _synonym1)).Returns(_fakeElement3.Object);
      _tableViewElementFactoryStub.Setup(stub => stub.GetCreateElement(_tableDefinition2, _synonym2)).Returns(_fakeElement2.Object);
      _tableViewElementFactoryStub.Setup(stub => stub.GetDropElement(_tableDefinition2, _synonym2)).Returns(_fakeElement2.Object);
      _tableViewElementFactoryStub.Setup(stub => stub.GetCreateElement(_tableDefinition2, _synonym3)).Returns(_fakeElement3.Object);
      _tableViewElementFactoryStub.Setup(stub => stub.GetDropElement(_tableDefinition2, _synonym3)).Returns(_fakeElement1.Object);

      _builder.AddEntityDefinition(_tableDefinition1);
      _builder.AddEntityDefinition(_tableDefinition2);

      var createScriptResult = (ScriptElementCollection)_builder.GetCreateScript();
      var dropScriptResult = (ScriptElementCollection)_builder.GetDropScript();

      Assert.That(createScriptResult.Elements.Count, Is.EqualTo(4));
      Assert.That(((ScriptStatement)createScriptResult.Elements[0]).Statement, Is.EqualTo("-- Create synonyms for tables that were created above"));
      Assert.That(createScriptResult.Elements[1], Is.SameAs(_fakeElement1.Object));
      Assert.That(createScriptResult.Elements[2], Is.SameAs(_fakeElement2.Object));
      Assert.That(createScriptResult.Elements[3], Is.SameAs(_fakeElement3.Object));

      Assert.That(dropScriptResult.Elements.Count, Is.EqualTo(4));
      Assert.That(((ScriptStatement)dropScriptResult.Elements[0]).Statement, Is.EqualTo("-- Drop all synonyms"));
      Assert.That(dropScriptResult.Elements[1], Is.SameAs(_fakeElement3.Object));
      Assert.That(dropScriptResult.Elements[2], Is.SameAs(_fakeElement2.Object));
      Assert.That(dropScriptResult.Elements[3], Is.SameAs(_fakeElement1.Object));
    }

    [Test]
    public void GetCreateScript_GetDropScript_OneUnionViewDefinitionAdded ()
    {
      _unionViewElementFactoryStub.Setup(stub => stub.GetCreateElement(_unionViewDefinition1, _synonym1)).Returns(_fakeElement1.Object);
      _unionViewElementFactoryStub.Setup(stub => stub.GetDropElement(_unionViewDefinition1, _synonym1)).Returns(_fakeElement2.Object);

      _builder.AddEntityDefinition(_unionViewDefinition1);

      var createScriptResult = (ScriptElementCollection)_builder.GetCreateScript();
      var dropScriptResult = (ScriptElementCollection)_builder.GetDropScript();

      Assert.That(createScriptResult.Elements.Count, Is.EqualTo(2));
      Assert.That(((ScriptStatement)createScriptResult.Elements[0]).Statement, Is.EqualTo("-- Create synonyms for tables that were created above"));
      Assert.That(createScriptResult.Elements[1], Is.SameAs(_fakeElement1.Object));

      Assert.That(dropScriptResult.Elements.Count, Is.EqualTo(2));
      Assert.That(((ScriptStatement)dropScriptResult.Elements[0]).Statement, Is.EqualTo("-- Drop all synonyms"));
      Assert.That(dropScriptResult.Elements[1], Is.SameAs(_fakeElement2.Object));
    }

    [Test]
    public void GetCreateScript_GetDropScript_SeveralUnionViewDefinitionsAdded ()
    {
      _unionViewElementFactoryStub.Setup(stub => stub.GetCreateElement(_unionViewDefinition1, _synonym1)).Returns(_fakeElement1.Object);
      _unionViewElementFactoryStub.Setup(stub => stub.GetDropElement(_unionViewDefinition1, _synonym1)).Returns(_fakeElement3.Object);
      _unionViewElementFactoryStub.Setup(stub => stub.GetCreateElement(_unionViewDefinition2, _synonym2)).Returns(_fakeElement2.Object);
      _unionViewElementFactoryStub.Setup(stub => stub.GetDropElement(_unionViewDefinition2, _synonym2)).Returns(_fakeElement2.Object);
      _unionViewElementFactoryStub.Setup(stub => stub.GetCreateElement(_unionViewDefinition2, _synonym3)).Returns(_fakeElement3.Object);
      _unionViewElementFactoryStub.Setup(stub => stub.GetDropElement(_unionViewDefinition2, _synonym3)).Returns(_fakeElement1.Object);

      _builder.AddEntityDefinition(_unionViewDefinition1);
      _builder.AddEntityDefinition(_unionViewDefinition2);

      var createScriptResult = (ScriptElementCollection)_builder.GetCreateScript();
      var dropScriptResult = (ScriptElementCollection)_builder.GetDropScript();

      Assert.That(createScriptResult.Elements.Count, Is.EqualTo(4));
      Assert.That(((ScriptStatement)createScriptResult.Elements[0]).Statement, Is.EqualTo("-- Create synonyms for tables that were created above"));
      Assert.That(createScriptResult.Elements[1], Is.SameAs(_fakeElement1.Object));
      Assert.That(createScriptResult.Elements[2], Is.SameAs(_fakeElement2.Object));
      Assert.That(createScriptResult.Elements[3], Is.SameAs(_fakeElement3.Object));

      Assert.That(dropScriptResult.Elements.Count, Is.EqualTo(4));
      Assert.That(((ScriptStatement)dropScriptResult.Elements[0]).Statement, Is.EqualTo("-- Drop all synonyms"));
      Assert.That(dropScriptResult.Elements[1], Is.SameAs(_fakeElement3.Object));
      Assert.That(dropScriptResult.Elements[2], Is.SameAs(_fakeElement2.Object));
      Assert.That(dropScriptResult.Elements[3], Is.SameAs(_fakeElement1.Object));
    }

    [Test]
    public void GetCreateScript_GetDropScript_OneFilterViewDefinitionAdded ()
    {
      _filterViewElementFactoryStub.Setup(stub => stub.GetCreateElement(_filterViewDefinition1, _synonym1)).Returns(_fakeElement1.Object);
      _filterViewElementFactoryStub.Setup(stub => stub.GetDropElement(_filterViewDefinition1, _synonym1)).Returns(_fakeElement2.Object);

      _builder.AddEntityDefinition(_filterViewDefinition1);

      var createScriptResult = (ScriptElementCollection)_builder.GetCreateScript();
      var dropScriptResult = (ScriptElementCollection)_builder.GetDropScript();

      Assert.That(createScriptResult.Elements.Count, Is.EqualTo(2));
      Assert.That(((ScriptStatement)createScriptResult.Elements[0]).Statement, Is.EqualTo("-- Create synonyms for tables that were created above"));
      Assert.That(createScriptResult.Elements[1], Is.SameAs(_fakeElement1.Object));

      Assert.That(dropScriptResult.Elements.Count, Is.EqualTo(2));
      Assert.That(((ScriptStatement)dropScriptResult.Elements[0]).Statement, Is.EqualTo("-- Drop all synonyms"));
      Assert.That(dropScriptResult.Elements[1], Is.SameAs(_fakeElement2.Object));
    }

    [Test]
    public void GetCreateScript_GetDropScript_SeveralFilterViewDefinitionsAdded ()
    {
      _filterViewElementFactoryStub.Setup(stub => stub.GetCreateElement(_filterViewDefinition1, _synonym1)).Returns(_fakeElement1.Object);
      _filterViewElementFactoryStub.Setup(stub => stub.GetDropElement(_filterViewDefinition1, _synonym1)).Returns(_fakeElement3.Object);
      _filterViewElementFactoryStub.Setup(stub => stub.GetCreateElement(_filterViewDefinition2, _synonym2)).Returns(_fakeElement2.Object);
      _filterViewElementFactoryStub.Setup(stub => stub.GetDropElement(_filterViewDefinition2, _synonym2)).Returns(_fakeElement2.Object);
      _filterViewElementFactoryStub.Setup(stub => stub.GetCreateElement(_filterViewDefinition2, _synonym3)).Returns(_fakeElement3.Object);
      _filterViewElementFactoryStub.Setup(stub => stub.GetDropElement(_filterViewDefinition2, _synonym3)).Returns(_fakeElement1.Object);

      _builder.AddEntityDefinition(_filterViewDefinition1);
      _builder.AddEntityDefinition(_filterViewDefinition2);

      var createScriptResult = (ScriptElementCollection)_builder.GetCreateScript();
      var dropScriptResult = (ScriptElementCollection)_builder.GetDropScript();

      Assert.That(createScriptResult.Elements.Count, Is.EqualTo(4));
      Assert.That(((ScriptStatement)createScriptResult.Elements[0]).Statement, Is.EqualTo("-- Create synonyms for tables that were created above"));
      Assert.That(createScriptResult.Elements[1], Is.SameAs(_fakeElement1.Object));
      Assert.That(createScriptResult.Elements[2], Is.SameAs(_fakeElement2.Object));
      Assert.That(createScriptResult.Elements[3], Is.SameAs(_fakeElement3.Object));

      Assert.That(dropScriptResult.Elements.Count, Is.EqualTo(4));
      Assert.That(((ScriptStatement)dropScriptResult.Elements[0]).Statement, Is.EqualTo("-- Drop all synonyms"));
      Assert.That(dropScriptResult.Elements[1], Is.SameAs(_fakeElement3.Object));
      Assert.That(dropScriptResult.Elements[2], Is.SameAs(_fakeElement2.Object));
      Assert.That(dropScriptResult.Elements[3], Is.SameAs(_fakeElement1.Object));
    }

    [Test]
    public void GetCreateScript_GetDropScript_SeveralEntityDefinitionsAdded ()
    {
      _tableViewElementFactoryStub.Setup(stub => stub.GetCreateElement(_tableDefinition1, _synonym1)).Returns(_fakeElement1.Object);
      _tableViewElementFactoryStub.Setup(stub => stub.GetDropElement(_tableDefinition1, _synonym1)).Returns(_fakeElement3.Object);
      _unionViewElementFactoryStub.Setup(stub => stub.GetCreateElement(_unionViewDefinition1, _synonym1)).Returns(_fakeElement2.Object);
      _unionViewElementFactoryStub.Setup(stub => stub.GetDropElement(_unionViewDefinition1, _synonym1)).Returns(_fakeElement2.Object);
      _filterViewElementFactoryStub.Setup(stub => stub.GetCreateElement(_filterViewDefinition1, _synonym1)).Returns(_fakeElement3.Object);
      _filterViewElementFactoryStub.Setup(stub => stub.GetDropElement(_filterViewDefinition1, _synonym1)).Returns(_fakeElement1.Object);

      _builder.AddEntityDefinition(_tableDefinition1);
      _builder.AddEntityDefinition(_unionViewDefinition1);
      _builder.AddEntityDefinition(_filterViewDefinition1);

      var createScriptResult = (ScriptElementCollection)_builder.GetCreateScript();
      var dropScriptResult = (ScriptElementCollection)_builder.GetDropScript();

      Assert.That(createScriptResult.Elements.Count, Is.EqualTo(4));
      Assert.That(((ScriptStatement)createScriptResult.Elements[0]).Statement, Is.EqualTo("-- Create synonyms for tables that were created above"));
      Assert.That(createScriptResult.Elements[1], Is.SameAs(_fakeElement1.Object));
      Assert.That(createScriptResult.Elements[2], Is.SameAs(_fakeElement2.Object));
      Assert.That(createScriptResult.Elements[3], Is.SameAs(_fakeElement3.Object));

      Assert.That(dropScriptResult.Elements.Count, Is.EqualTo(4));
      Assert.That(((ScriptStatement)dropScriptResult.Elements[0]).Statement, Is.EqualTo("-- Drop all synonyms"));
      Assert.That(dropScriptResult.Elements[1], Is.SameAs(_fakeElement3.Object));
      Assert.That(dropScriptResult.Elements[2], Is.SameAs(_fakeElement2.Object));
      Assert.That(dropScriptResult.Elements[3], Is.SameAs(_fakeElement1.Object));
    }

    [Test]
    public void GetCreateScript_GetDropScript_OneEmptyViewDefinitionAdded ()
    {
      _emptyViewElementFactoryStub.Setup(stub => stub.GetCreateElement(_emptyViewDefinition1, _synonym1)).Returns(_fakeElement1.Object);
      _emptyViewElementFactoryStub.Setup(stub => stub.GetDropElement(_emptyViewDefinition1, _synonym1)).Returns(_fakeElement2.Object);

      _builder.AddEntityDefinition(_emptyViewDefinition1);

      var createScriptResult = (ScriptElementCollection)_builder.GetCreateScript();
      var dropScriptResult = (ScriptElementCollection)_builder.GetDropScript();

      Assert.That(createScriptResult.Elements.Count, Is.EqualTo(2));
      Assert.That(((ScriptStatement)createScriptResult.Elements[0]).Statement, Is.EqualTo("-- Create synonyms for tables that were created above"));
      Assert.That(createScriptResult.Elements[1], Is.SameAs(_fakeElement1.Object));

      Assert.That(dropScriptResult.Elements.Count, Is.EqualTo(2));
      Assert.That(((ScriptStatement)dropScriptResult.Elements[0]).Statement, Is.EqualTo("-- Drop all synonyms"));
      Assert.That(dropScriptResult.Elements[1], Is.SameAs(_fakeElement2.Object));
    }

    [Test]
    public void GetCreateScript_GetDropScript_SeveralEmptyViewDefinitionsAdded ()
    {
      _emptyViewElementFactoryStub.Setup(stub => stub.GetCreateElement(_emptyViewDefinition1, _synonym1)).Returns(_fakeElement1.Object);
      _emptyViewElementFactoryStub.Setup(stub => stub.GetDropElement(_emptyViewDefinition1, _synonym1)).Returns(_fakeElement3.Object);
      _emptyViewElementFactoryStub.Setup(stub => stub.GetCreateElement(_emptyViewDefinition2, _synonym2)).Returns(_fakeElement2.Object);
      _emptyViewElementFactoryStub.Setup(stub => stub.GetDropElement(_emptyViewDefinition2, _synonym2)).Returns(_fakeElement2.Object);
      _emptyViewElementFactoryStub.Setup(stub => stub.GetCreateElement(_emptyViewDefinition2, _synonym3)).Returns(_fakeElement3.Object);
      _emptyViewElementFactoryStub.Setup(stub => stub.GetDropElement(_emptyViewDefinition2, _synonym3)).Returns(_fakeElement1.Object);

      _builder.AddEntityDefinition(_emptyViewDefinition1);
      _builder.AddEntityDefinition(_emptyViewDefinition2);

      var createScriptResult = (ScriptElementCollection)_builder.GetCreateScript();
      var dropScriptResult = (ScriptElementCollection)_builder.GetDropScript();

      Assert.That(createScriptResult.Elements.Count, Is.EqualTo(4));
      Assert.That(((ScriptStatement)createScriptResult.Elements[0]).Statement, Is.EqualTo("-- Create synonyms for tables that were created above"));
      Assert.That(createScriptResult.Elements[1], Is.SameAs(_fakeElement1.Object));
      Assert.That(createScriptResult.Elements[2], Is.SameAs(_fakeElement2.Object));
      Assert.That(createScriptResult.Elements[3], Is.SameAs(_fakeElement3.Object));

      Assert.That(dropScriptResult.Elements.Count, Is.EqualTo(4));
      Assert.That(((ScriptStatement)dropScriptResult.Elements[0]).Statement, Is.EqualTo("-- Drop all synonyms"));
      Assert.That(dropScriptResult.Elements[1], Is.SameAs(_fakeElement3.Object));
      Assert.That(dropScriptResult.Elements[2], Is.SameAs(_fakeElement2.Object));
      Assert.That(dropScriptResult.Elements[3], Is.SameAs(_fakeElement1.Object));
    }
  }
}
