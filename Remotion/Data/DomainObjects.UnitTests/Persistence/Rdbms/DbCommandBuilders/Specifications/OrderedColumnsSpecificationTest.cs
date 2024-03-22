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
using System.Text;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Factories;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.DbCommandBuilders.Specifications
{
  [TestFixture]
  public class OrderedColumnsSpecificationTest
  {
    private ColumnDefinition _column1;
    private ColumnDefinition _column2;
    private ColumnDefinition _column3;
    private OrderedColumnsSpecification _specification;
    private Mock<ISqlDialect> _sqlDialectStub;
    private OrderedColumnsSpecification _specificationWithEmptyColumns;

    [SetUp]
    public void SetUp ()
    {
      _column1 = ColumnDefinitionObjectMother.CreateColumn("Column1");
      _column2 = ColumnDefinitionObjectMother.CreateColumn("Column2");
      _column3 = ColumnDefinitionObjectMother.CreateColumn("Column3");
      _specification =
          new OrderedColumnsSpecification(
              new[]
              {
                  new OrderedColumn(_column1, SortOrder.Ascending), new OrderedColumn(_column2, SortOrder.Descending),
                  new OrderedColumn(_column3, SortOrder.Ascending)
              });
      _sqlDialectStub = new Mock<ISqlDialect>();
      _sqlDialectStub.Setup(stub => stub.DelimitIdentifier("Column1")).Returns("[delimited Column1]");
      _sqlDialectStub.Setup(stub => stub.DelimitIdentifier("Column2")).Returns("[delimited Column2]");
      _sqlDialectStub.Setup(stub => stub.DelimitIdentifier("Column3")).Returns("[delimited Column3]");

      _specificationWithEmptyColumns = new OrderedColumnsSpecification(new OrderedColumn[0]);
    }

    [Test]
    public void CreateEmpty ()
    {
      var result = OrderedColumnsSpecification.CreateEmpty();

      Assert.That(result.Columns, Is.Empty);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_specification.Columns, Is.EqualTo(new[]
              {
                  new OrderedColumn(_column1, SortOrder.Ascending), new OrderedColumn(_column2, SortOrder.Descending),
                  new OrderedColumn(_column3, SortOrder.Ascending)
              }));
    }

    [Test]
    public void IsEmpty_True ()
    {
      var result = _specificationWithEmptyColumns.IsEmpty;

      Assert.That(result, Is.True);
    }

    [Test]
    public void IsEmpty_False ()
    {
      var result = _specification.IsEmpty;

      Assert.That(result, Is.False);
    }

    [Test]
    public void AppendOrderings_StringBuilderEmpty ()
    {
      var sb = new StringBuilder();

      _specification.AppendOrderings(sb, _sqlDialectStub.Object);

      Assert.That(sb.ToString(), Is.EqualTo("[delimited Column1] ASC, [delimited Column2] DESC, [delimited Column3] ASC"));
    }

    [Test]
    public void AppendOrderings_StringBuilderNotEmpty ()
    {
      var sb = new StringBuilder("test ");

      _specification.AppendOrderings(sb, _sqlDialectStub.Object);

      Assert.That(sb.ToString(), Is.EqualTo("test [delimited Column1] ASC, [delimited Column2] DESC, [delimited Column3] ASC"));
    }

    [Test]
    public void AppendOrderings_NoColumns ()
    {
      var sb = new StringBuilder();

      _specificationWithEmptyColumns.AppendOrderings(sb, _sqlDialectStub.Object);

      Assert.That(sb.ToString(), Is.Empty);
    }

    [Test]
    public void UnionWithSelectedColumns ()
    {
      var selectedColumns = new Mock<ISelectedColumnsSpecification>(MockBehavior.Strict);

      selectedColumns
          .Setup(mock => mock.Union(new[] { _column1, _column2, _column3 }))
          .Returns(selectedColumns.Object)
          .Verifiable();

      var result = _specification.UnionWithSelectedColumns(selectedColumns.Object);

      Assert.That(result, Is.SameAs(selectedColumns.Object));
      selectedColumns.Verify();
    }

    [Test]
    public void UnionWithSelectedColumns_NoColumns ()
    {
      var selectedColumns = new Mock<ISelectedColumnsSpecification>(MockBehavior.Strict);

      var result = _specificationWithEmptyColumns.UnionWithSelectedColumns(selectedColumns.Object);

      Assert.That(result, Is.SameAs(selectedColumns.Object));
      selectedColumns.Verify();
    }
  }
}
