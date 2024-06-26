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
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGeneration
{
  [TestFixture]
  public class TableTypeScriptBuilderTest : SchemaGenerationTestBase
  {
    private Mock<IStructuredTypeScriptElementFactory> _tableTypeScriptFactoryStub;
    private TableTypeScriptBuilder _builder;
    private TableTypeDefinition _tableTypeDefinition1;
    private TableTypeDefinition _tableTypeDefinition2;
    private TableTypeDefinition _tableTypeDefinition3;
    private Mock<IScriptElement> _fakeElement1;
    private Mock<IScriptElement> _fakeElement2;
    private Mock<IScriptElement> _fakeElement3;

    public override void SetUp ()
    {
      base.SetUp();

      var commentScriptElementFactoryStub = new Mock<ICommentScriptElementFactory>();
      commentScriptElementFactoryStub
          .Setup(_ => _.GetCommentElement(It.IsAny<string>()))
          .Returns((Func<string, ScriptStatement>)(comment => new ScriptStatement("[comment] " + comment)));

      _tableTypeScriptFactoryStub = new Mock<IStructuredTypeScriptElementFactory>();
      _builder = new TableTypeScriptBuilder(_tableTypeScriptFactoryStub.Object, commentScriptElementFactoryStub.Object);

      _tableTypeDefinition1 = TableTypeDefinitionObjectMother.Create();
      _tableTypeDefinition2 = TableTypeDefinitionObjectMother.Create();
      _tableTypeDefinition3 = TableTypeDefinitionObjectMother.Create();

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

      Assert.That(() => _builder.AddEntityDefinition(Mock.Of<IRdbmsStorageEntityDefinition>()), Throws.Nothing);

      Assert.That(elements.Count, Is.EqualTo(1));
      Assert.That(scriptStatement.Statement, Is.EqualTo(before));
    }

    [Test]
    public void GetCreateScript_GetDropScript_NoEntitiesAdded ()
    {
      var createScriptResult = _builder.GetCreateScript();
      var dropScriptResult = _builder.GetDropScript();

      Assert.That(((ScriptElementCollection)createScriptResult).Elements.Count, Is.EqualTo(1));
      Assert.That(((ScriptStatement)((ScriptElementCollection)createScriptResult).Elements[0]).Statement, Is.EqualTo("[comment] Create all structured types"));
      Assert.That(((ScriptElementCollection)dropScriptResult).Elements.Count, Is.EqualTo(1));
      Assert.That(((ScriptStatement)((ScriptElementCollection)dropScriptResult).Elements[0]).Statement, Is.EqualTo("[comment] Drop all structured types"));
    }

    [Test]
    public void GetCreateScript_GetDropScript_OneTableDefinitionAdded ()
    {
      _tableTypeScriptFactoryStub.Setup(stub => stub.GetCreateElement(_tableTypeDefinition1)).Returns(_fakeElement1.Object);
      _tableTypeScriptFactoryStub.Setup(stub => stub.GetDropElement(_tableTypeDefinition1)).Returns(_fakeElement2.Object);

      _builder.AddStructuredTypeDefinition(_tableTypeDefinition1);

      var createScriptResult = _builder.GetCreateScript();
      var dropScriptResult = _builder.GetDropScript();

      Assert.That(((ScriptElementCollection)createScriptResult).Elements.Count, Is.EqualTo(2));
      Assert.That(((ScriptStatement)((ScriptElementCollection)createScriptResult).Elements[0]).Statement, Is.EqualTo("[comment] Create all structured types"));
      Assert.That(((ScriptElementCollection)createScriptResult).Elements[1], Is.SameAs(_fakeElement1.Object));

      Assert.That(((ScriptElementCollection)dropScriptResult).Elements.Count, Is.EqualTo(2));
      Assert.That(((ScriptStatement)((ScriptElementCollection)dropScriptResult).Elements[0]).Statement, Is.EqualTo("[comment] Drop all structured types"));
      Assert.That(((ScriptElementCollection)dropScriptResult).Elements[1], Is.SameAs(_fakeElement2.Object));
    }

    [Test]
    public void GetCreateScript_GetDropScript_SeveralTableDefinitionsAdded ()
    {
      _tableTypeScriptFactoryStub.Setup(stub => stub.GetCreateElement(_tableTypeDefinition1)).Returns(_fakeElement1.Object);
      _tableTypeScriptFactoryStub.Setup(stub => stub.GetDropElement(_tableTypeDefinition1)).Returns(_fakeElement3.Object);
      _tableTypeScriptFactoryStub.Setup(stub => stub.GetCreateElement(_tableTypeDefinition2)).Returns(_fakeElement2.Object);
      _tableTypeScriptFactoryStub.Setup(stub => stub.GetDropElement(_tableTypeDefinition2)).Returns(_fakeElement2.Object);
      _tableTypeScriptFactoryStub.Setup(stub => stub.GetCreateElement(_tableTypeDefinition3)).Returns(_fakeElement3.Object);
      _tableTypeScriptFactoryStub.Setup(stub => stub.GetDropElement(_tableTypeDefinition3)).Returns(_fakeElement1.Object);

      _builder.AddStructuredTypeDefinition(_tableTypeDefinition1);
      _builder.AddStructuredTypeDefinition(_tableTypeDefinition2);
      _builder.AddStructuredTypeDefinition(_tableTypeDefinition3);

      var createScriptResult = _builder.GetCreateScript();
      var dropScriptResult = _builder.GetDropScript();

      Assert.That(((ScriptElementCollection)createScriptResult).Elements.Count, Is.EqualTo(4));
      Assert.That(((ScriptStatement)((ScriptElementCollection)createScriptResult).Elements[0]).Statement, Is.EqualTo("[comment] Create all structured types"));
      Assert.That(((ScriptElementCollection)createScriptResult).Elements[1], Is.SameAs(_fakeElement1.Object));
      Assert.That(((ScriptElementCollection)createScriptResult).Elements[2], Is.SameAs(_fakeElement2.Object));
      Assert.That(((ScriptElementCollection)createScriptResult).Elements[3], Is.SameAs(_fakeElement3.Object));

      Assert.That(((ScriptElementCollection)dropScriptResult).Elements.Count, Is.EqualTo(4));
      Assert.That(((ScriptStatement)((ScriptElementCollection)dropScriptResult).Elements[0]).Statement, Is.EqualTo("[comment] Drop all structured types"));
      Assert.That(((ScriptElementCollection)dropScriptResult).Elements[1], Is.SameAs(_fakeElement3.Object));
      Assert.That(((ScriptElementCollection)dropScriptResult).Elements[2], Is.SameAs(_fakeElement2.Object));
      Assert.That(((ScriptElementCollection)dropScriptResult).Elements[3], Is.SameAs(_fakeElement1.Object));
    }
  }
}
