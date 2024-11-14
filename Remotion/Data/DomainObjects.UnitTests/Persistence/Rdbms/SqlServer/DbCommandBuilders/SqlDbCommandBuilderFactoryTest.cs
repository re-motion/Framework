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
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.DbCommandBuilders
{
  [TestFixture]
  public class SqlDbCommandBuilderFactoryTest : StandardMappingTest
  {
    private Mock<ISqlDialect> _sqlDialectStub;
    private SqlDbCommandBuilderFactory _factory;
    private TableDefinition _tableDefinition;
    private ColumnDefinition _column1;
    private ColumnDefinition _column2;
    private ColumnValue _columnValue1;
    private ColumnValue _columnValue2;
    private OrderedColumn _orderColumn1;
    private OrderedColumn _orderColumn2;
    private Mock<ISingleScalarStructuredTypeDefinitionProvider> _singleScalarStructuredTypeDefinitionProviderStub;

    public override void SetUp ()
    {
      base.SetUp();

      _sqlDialectStub = new Mock<ISqlDialect>();

      _tableDefinition = TableDefinitionObjectMother.Create(TestDomainStorageProviderDefinition, new EntityNameDefinition(null, "Table"));

      _column1 = ColumnDefinitionObjectMother.CreateColumn("Column1");
      _column2 = ColumnDefinitionObjectMother.CreateColumn("Column2");
      _columnValue1 = new ColumnValue(_column1, new object());
      _columnValue2 = new ColumnValue(_column2, new object());

      _orderColumn1 = new OrderedColumn(_column1, SortOrder.Ascending);
      _orderColumn2 = new OrderedColumn(_column2, SortOrder.Descending);

      _singleScalarStructuredTypeDefinitionProviderStub = new Mock<ISingleScalarStructuredTypeDefinitionProvider>();
      _factory = new SqlDbCommandBuilderFactory(_singleScalarStructuredTypeDefinitionProviderStub.Object, _sqlDialectStub.Object);
    }

    [Test]
    public void CreateForSelect_Table ()
    {
      var result = _factory.CreateForSelect(
          _tableDefinition,
          new[] { _column1, _column2 },
          new[] { _columnValue1, _columnValue2 },
          new[] { _orderColumn1, _orderColumn2 });

      Assert.That(result, Is.TypeOf(typeof(SelectDbCommandBuilder)));
      var dbCommandBuilder = (SelectDbCommandBuilder)result;
      Assert.That(dbCommandBuilder.Table, Is.SameAs(_tableDefinition));
      Assert.That(((SelectedColumnsSpecification)dbCommandBuilder.SelectedColumns).SelectedColumns, Is.EqualTo(new[] { _column1, _column2 }));
      Assert.That(
          ((ComparedColumnsSpecification)dbCommandBuilder.ComparedColumns).ComparedColumnValues,
          Is.EqualTo(new[] { _columnValue1, _columnValue2 }));
      Assert.That(((OrderedColumnsSpecification)dbCommandBuilder.OrderedColumns).Columns, Is.EqualTo(new[] { _orderColumn1, _orderColumn2 }));
    }

    [Test]
    public void CreateForSelect_Table_MultiValueLookup ()
    {
      var columnValueTable = new ColumnValueTable(
          new[] { _column1 },
          new[]
          {
            new ColumnValueTable.Row(new object[] { 12 }),
            new ColumnValueTable.Row(new object[] { 13 }),
          }
          );

      var dotNetType = _column1.StorageTypeInfo.DotNetType;

      _singleScalarStructuredTypeDefinitionProviderStub.Setup(_ => _.GetStructuredTypeDefinition(dotNetType, false))
          .Returns(
              new TableTypeDefinition(
                  new EntityNameDefinition(null, "Test"),
                  [new SimpleStoragePropertyDefinition(dotNetType, _column1)],
                  Array.Empty<ITableConstraintDefinition>()));

      var result = _factory.CreateForSelect(_tableDefinition, new[] { _column1, _column2 }, columnValueTable, new[] { _orderColumn1, _orderColumn2 });

      Assert.That(result, Is.TypeOf(typeof(SelectDbCommandBuilder)));
      var dbCommandBuilder = (SelectDbCommandBuilder)result;
      Assert.That(dbCommandBuilder.Table, Is.SameAs(_tableDefinition));
      Assert.That(((SelectedColumnsSpecification)dbCommandBuilder.SelectedColumns).SelectedColumns, Is.EqualTo(new[] { _column1, _column2 }));
      Assert.That(((SqlTableValuedParameterComparedColumnSpecification)dbCommandBuilder.ComparedColumns).ComparedColumnDefinition, Is.SameAs(_column1));
      Assert.That(((SqlTableValuedParameterComparedColumnSpecification)dbCommandBuilder.ComparedColumns).ObjectValues, Is.EqualTo(new[] { 12, 13 }));
      Assert.That(((OrderedColumnsSpecification)dbCommandBuilder.OrderedColumns).Columns, Is.EqualTo(new[] { _orderColumn1, _orderColumn2 }));
    }

    [Test]
    public void CreateForSelect_Table_MultiValueLookup_MoreThanOneColumn ()
    {
      var columnValueTable = new ColumnValueTable(
          new[] { _column1, _column2 },
          new[]
          {
            new ColumnValueTable.Row(new object[] { 12, 14 }),
            new ColumnValueTable.Row(new object[] { 13, 15 }),
          }
          );

      Assert.That(
          () => _factory.CreateForSelect(_tableDefinition, new[] { _column1, _column2 }, columnValueTable, new[] { _orderColumn1, _orderColumn2 }),
          Throws.TypeOf<NotSupportedException>()
              .With.Message.EqualTo("The SQL provider can only handle multi-value comparisons with a single ColumnDefinition.")
              .And.InnerException.TypeOf<InvalidOperationException>());
    }

    [Test]
    public void CreateForSelect_UnionView ()
    {
      var unionViewDefinition = UnionViewDefinitionObjectMother.Create(
          TestDomainStorageProviderDefinition,
          new EntityNameDefinition(null, "UnionEntity"),
          _tableDefinition);

      var result = _factory.CreateForSelect(
          unionViewDefinition,
          new[] { _column1, _column2 },
          new[] { _columnValue1, _columnValue2 },
          new[] { _orderColumn1, _orderColumn2 });

      Assert.That(result, Is.TypeOf(typeof(UnionSelectDbCommandBuilder)));
      var dbCommandBuilder = (UnionSelectDbCommandBuilder)result;
      Assert.That(dbCommandBuilder.UnionViewDefinition, Is.SameAs(unionViewDefinition));
      Assert.That(((SelectedColumnsSpecification)dbCommandBuilder.SelectedColumns).SelectedColumns, Is.EqualTo(new[] { _column1, _column2 }));
      Assert.That(
          ((ComparedColumnsSpecification)dbCommandBuilder.ComparedColumns).ComparedColumnValues,
          Is.EqualTo(new[] { _columnValue1, _columnValue2 }));
      Assert.That(((OrderedColumnsSpecification)dbCommandBuilder.OrderedColumns).Columns, Is.EqualTo(new[] { _orderColumn1, _orderColumn2 }));
    }

    [Test]
    public void CreateForQuery ()
    {
      var result = _factory.CreateForQuery("statement", new QueryParameterWithDataParameterDefinition[0]);

      Assert.That(result, Is.TypeOf(typeof(QueryDbCommandBuilder)));
      Assert.That(((QueryDbCommandBuilder)result).SqlDialect, Is.SameAs(_sqlDialectStub.Object));
    }

    [Test]
    public void CreateForInsert ()
    {
      var result = _factory.CreateForInsert(_tableDefinition, new[] { _columnValue1, _columnValue2 });

      Assert.That(result, Is.TypeOf(typeof(InsertDbCommandBuilder)));
      Assert.That(((InsertDbCommandBuilder)result).SqlDialect, Is.SameAs(_sqlDialectStub.Object));
      Assert.That(((InsertDbCommandBuilder)result).TableDefinition, Is.SameAs(_tableDefinition));
      Assert.That(
          ((InsertedColumnsSpecification)((InsertDbCommandBuilder)result).InsertedColumnsSpecification).ColumnValues,
          Is.EqualTo(new[] { _columnValue1, _columnValue2 }));
    }

    [Test]
    public void CreateForUpdate ()
    {
      var result = _factory.CreateForUpdate(_tableDefinition, new[] { _columnValue1, _columnValue2 }, new[] { _columnValue2, _columnValue1 });

      Assert.That(result, Is.TypeOf(typeof(UpdateDbCommandBuilder)));
      Assert.That(((UpdateDbCommandBuilder)result).SqlDialect, Is.SameAs(_sqlDialectStub.Object));
      Assert.That(((UpdateDbCommandBuilder)result).TableDefinition, Is.SameAs(_tableDefinition));
      Assert.That(
          ((UpdatedColumnsSpecification)((UpdateDbCommandBuilder)result).UpdatedColumnsSpecification).ColumnValues,
          Is.EqualTo(new[] { _columnValue1, _columnValue2 }));
      Assert.That(
          ((ComparedColumnsSpecification)((UpdateDbCommandBuilder)result).ComparedColumnsSpecification).ComparedColumnValues,
          Is.EqualTo(new[] { _columnValue2, _columnValue1 }));
    }

    [Test]
    public void CreateForDelete ()
    {
      var result = _factory.CreateForDelete(_tableDefinition, new[] { _columnValue1, _columnValue2 });

      Assert.That(result, Is.TypeOf(typeof(DeleteDbCommandBuilder)));
      Assert.That(((DeleteDbCommandBuilder)result).SqlDialect, Is.SameAs(_sqlDialectStub.Object));
      Assert.That(((DeleteDbCommandBuilder)result).TableDefinition, Is.SameAs(_tableDefinition));
      Assert.That(
          ((ComparedColumnsSpecification)((DeleteDbCommandBuilder)result).ComparedColumnsSpecification).ComparedColumnValues,
          Is.EqualTo(new[] { _columnValue1, _columnValue2 }));
    }
  }
}
