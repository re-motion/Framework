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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.DbCommandBuilders.Specifications
{
  [TestFixture]
  public class SelectedColumnsSpecificationTest : StandardMappingTest
  {
    private ColumnDefinition _column1;
    private ColumnDefinition _column2;
    private ColumnDefinition _column3;
    private SelectedColumnsSpecification _specification;
    private ISqlDialect _sqlDialectStub;

    public override void SetUp ()
    {
      base.SetUp();

      _column1 = ColumnDefinitionObjectMother.CreateColumn ("Column1");
      _column2 = ColumnDefinitionObjectMother.CreateColumn ("Column2");
      _column3 = ColumnDefinitionObjectMother.CreateColumn ("Column3");
      _specification = new SelectedColumnsSpecification (new[] { _column1, _column2, _column3 });
      _sqlDialectStub = MockRepository.GenerateStub<ISqlDialect>();
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("Column1")).Return ("[delimited Column1]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("Column2")).Return ("[delimited Column2]");
      _sqlDialectStub.Stub (stub => stub.DelimitIdentifier ("Column3")).Return ("[delimited Column3]");
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_specification.SelectedColumns, Is.EqualTo (new[] { _column1, _column2, _column3 }));
    }

    [Test]
    public void AppendProjection ()
    {
      var sb = new StringBuilder ("xx");

      _specification.AppendProjection (sb, _sqlDialectStub);

      Assert.That (sb.ToString(), Is.EqualTo ("xx[delimited Column1], [delimited Column2], [delimited Column3]"));
    }

    [Test]
    public void AppendProjection_WithNulls ()
    {
      var sb = new StringBuilder();

      var specification = new SelectedColumnsSpecification (new[] { null, _column2, null });
      specification.AppendProjection (sb, _sqlDialectStub);

      Assert.That (sb.ToString(), Is.EqualTo ("NULL, [delimited Column2], NULL"));
    }

    [Test]
    public void Union ()
    {
      var column4 = ColumnDefinitionObjectMother.CreateColumn ("Column4");
      var column5 = ColumnDefinitionObjectMother.CreateColumn ("Column5");

      var result = (SelectedColumnsSpecification) _specification.Union (new[] { column4, column5 });

      Assert.That (result.SelectedColumns, Is.EqualTo (new[] { _column1, _column2, _column3, column4, column5 }));
    }

    [Test]
    public void Union_DuplicatedColumns ()
    {
      var column4 = ColumnDefinitionObjectMother.CreateColumn ("Column4");

      var result = (SelectedColumnsSpecification) _specification.Union (new[] { column4, column4 });

      Assert.That (result.SelectedColumns, Is.EqualTo (new[] { _column1, _column2, _column3, column4 }));
    }

    [Test]
    public void AdjustForTable ()
    {
      var table = TableDefinitionObjectMother.Create (
          TestDomainStorageProviderDefinition,
          new[] { new SimpleStoragePropertyDefinition (typeof (int), _column1) });

      var result = _specification.AdjustForTable (table);

      Assert.That (result, Is.TypeOf<SelectedColumnsSpecification>());
      Assert.That (((SelectedColumnsSpecification) result).SelectedColumns, Is.EqualTo (new[] { _column1, null, null }));
    }
  }
}