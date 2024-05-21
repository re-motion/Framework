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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;

[TestFixture]
public class TableTypeDefinitionTest
{
  [Test]
  public void Initialize ()
  {
    var typeName = new EntityNameDefinition("test", "MyTableType");
    var properties = new List<IRdbmsStoragePropertyDefinition> { Mock.Of<IRdbmsStoragePropertyDefinition>(), Mock.Of<IRdbmsStoragePropertyDefinition>() };
    var constraints = new List<ITableConstraintDefinition>{ Mock.Of<ITableConstraintDefinition>() };

    var tableTypeDefinition = new TableTypeDefinition(typeName, properties, constraints);

    Assert.That(tableTypeDefinition.TypeName, Is.EqualTo(typeName));
    Assert.That(tableTypeDefinition.Properties, Is.EqualTo(properties));
    Assert.That(tableTypeDefinition.Constraints, Is.EqualTo(constraints));
  }

  [Test]
  public void GetAllColumns_ReturnsColumnsOfAllProperties ()
  {
    var property1 = new Mock<IRdbmsStoragePropertyDefinition>();
    property1.Setup(_ => _.GetColumns()).Returns(
    [
        new ColumnDefinition("Column1", Mock.Of<IStorageTypeInformation>(), false),
        new ColumnDefinition("Column2", Mock.Of<IStorageTypeInformation>(), false)
    ]);

    var property2 = new Mock<IRdbmsStoragePropertyDefinition>();
    property2.Setup(_ => _.GetColumns()).Returns(
    [
        new ColumnDefinition("Column3", Mock.Of<IStorageTypeInformation>(), false),
        new ColumnDefinition("Column4", Mock.Of<IStorageTypeInformation>(), false)
    ]);

    var typeName = new EntityNameDefinition("test", "MyTableType");
    var properties = new List<IRdbmsStoragePropertyDefinition> { property1.Object, property2.Object };
    var constraints = Array.Empty<ITableConstraintDefinition>();

    var tableTypeDefinition = new TableTypeDefinition(typeName, properties, constraints);
    var result = tableTypeDefinition.GetAllColumns().ToArray();

    Assert.That(result.Length, Is.EqualTo(4));
    Assert.That(result.Select(o => o.Name), Is.EqualTo(new[] { "Column1", "Column2", "Column3", "Column4" }));
  }

  [Test]
  public void Accept_Calls_VisitTableTypeDefinition ()
  {
    var typeName = new EntityNameDefinition("test", "MyTableType");
    var properties = new List<IRdbmsStoragePropertyDefinition> { Mock.Of<IRdbmsStoragePropertyDefinition>() };
    var constraints = Array.Empty<ITableConstraintDefinition>();

    var tableTypeDefinition = new TableTypeDefinition(typeName, properties, constraints);

    var visitorMock = new Mock<IRdbmsStructuredTypeDefinitionVisitor>();
    visitorMock.Setup(_=>_.VisitTableTypeDefinition(tableTypeDefinition)).Verifiable();

    tableTypeDefinition.Accept(visitorMock.Object);
    visitorMock.Verify();
  }
}
