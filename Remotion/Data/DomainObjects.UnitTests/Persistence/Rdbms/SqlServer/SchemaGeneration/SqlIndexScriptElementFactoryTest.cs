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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.DomainObjects.UnitTests.Factories;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  [TestFixture]
  public class SqlIndexScriptElementFactoryTest : SchemaGenerationTestBase
  {
    private Mock<ISqlIndexDefinitionScriptElementFactory<SqlIndexDefinition>> _indexDefinitionElementFactoryMock;
    private Mock<ISqlIndexDefinitionScriptElementFactory<SqlPrimaryXmlIndexDefinition>> _primaryIndexDefinitionElementFactoryMock;
    private Mock<ISqlIndexDefinitionScriptElementFactory<SqlSecondaryXmlIndexDefinition>> _secondaryIndexDefinitionElementFactoryMock;
    private SqlIndexDefinition _indexDefinition;
    private SqlPrimaryXmlIndexDefinition _primaryIndexDefinition;
    private SqlSecondaryXmlIndexDefinition _secondaryIndexDefinition;
    private SqlIndexScriptElementFactory _factory;
    private Mock<IScriptElement> _fakeScriptElement;
    private EntityNameDefinition _entityNameDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _indexDefinitionElementFactoryMock = new Mock<ISqlIndexDefinitionScriptElementFactory<SqlIndexDefinition>>();
      _primaryIndexDefinitionElementFactoryMock = new Mock<ISqlIndexDefinitionScriptElementFactory<SqlPrimaryXmlIndexDefinition>>();
      _secondaryIndexDefinitionElementFactoryMock = new Mock<ISqlIndexDefinitionScriptElementFactory<SqlSecondaryXmlIndexDefinition>>();

      _factory = new SqlIndexScriptElementFactory(
          _indexDefinitionElementFactoryMock.Object,
          _primaryIndexDefinitionElementFactoryMock.Object,
          _secondaryIndexDefinitionElementFactoryMock.Object);

      var simpleColumn = ColumnDefinitionObjectMother.CreateColumn("Column");
      var indexedColumn = new SqlIndexedColumnDefinition(simpleColumn, IndexOrder.Desc);

      _entityNameDefinition = new EntityNameDefinition(null, "Table");
      _indexDefinition = new SqlIndexDefinition("Index1", new[] { indexedColumn });
      _primaryIndexDefinition = new SqlPrimaryXmlIndexDefinition("Index2", simpleColumn);
      _secondaryIndexDefinition = new SqlSecondaryXmlIndexDefinition(
          "Index3", simpleColumn, "PrimaryIndexName", SqlSecondaryXmlIndexKind.Property);

      _fakeScriptElement = new Mock<IScriptElement>();
    }

    [Test]
    public void GetCreateElement_IndexDefinition ()
    {
      _indexDefinitionElementFactoryMock
          .Setup(stub => stub.GetCreateElement(_indexDefinition, _entityNameDefinition))
          .Returns(_fakeScriptElement.Object);

      var result = _factory.GetCreateElement(_indexDefinition, _entityNameDefinition);

      Assert.That(result, Is.SameAs(_fakeScriptElement.Object));
    }

    [Test]
    public void GetCreateElement_PrimaryIndexDefinition ()
    {
      _primaryIndexDefinitionElementFactoryMock
          .Setup(stub => stub.GetCreateElement(_primaryIndexDefinition, _entityNameDefinition))
          .Returns(_fakeScriptElement.Object);

      var result = _factory.GetCreateElement(_primaryIndexDefinition, _entityNameDefinition);

      Assert.That(result, Is.SameAs(_fakeScriptElement.Object));
    }

    [Test]
    public void GetCreateElement_SecondaryIndexDefinition ()
    {
      _secondaryIndexDefinitionElementFactoryMock
          .Setup(stub => stub.GetCreateElement(_secondaryIndexDefinition, _entityNameDefinition))
          .Returns(_fakeScriptElement.Object);

      var result = _factory.GetCreateElement(_secondaryIndexDefinition, _entityNameDefinition);

      Assert.That(result, Is.SameAs(_fakeScriptElement.Object));
    }

    [Test]
    public void GetDropElement_IndexDefinition ()
    {
      _indexDefinitionElementFactoryMock
          .Setup(stub => stub.GetDropElement(_indexDefinition, _entityNameDefinition))
          .Returns(_fakeScriptElement.Object);

      var result = _factory.GetDropElement(_indexDefinition, _entityNameDefinition);

      Assert.That(result, Is.SameAs(_fakeScriptElement.Object));
    }

    [Test]
    public void GetDropElement_PrimaryIndexDefinition ()
    {
      _primaryIndexDefinitionElementFactoryMock
          .Setup(stub => stub.GetDropElement(_primaryIndexDefinition, _entityNameDefinition))
          .Returns(_fakeScriptElement.Object);

      var result = _factory.GetDropElement(_primaryIndexDefinition, _entityNameDefinition);

      Assert.That(result, Is.SameAs(_fakeScriptElement.Object));
    }

    [Test]
    public void GetDropElement_SecondaryIndexDefinition ()
    {
      _secondaryIndexDefinitionElementFactoryMock
          .Setup(stub => stub.GetDropElement(_secondaryIndexDefinition, _entityNameDefinition))
          .Returns(_fakeScriptElement.Object);

      var result = _factory.GetDropElement(_secondaryIndexDefinition, _entityNameDefinition);

      Assert.That(result, Is.SameAs(_fakeScriptElement.Object));
    }
  }
}
