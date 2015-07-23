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
using Remotion.Data.DomainObjects.UnitTests.Factories;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model
{
  [TestFixture]
  public class ColumnValueTableTest
  {
    [Test]
    public void Row_Concat ()
    {
      var row1 = new ColumnValueTable.Row (new[] { "a", "b" });
      var row2 = new ColumnValueTable.Row (new[] { "c", "d" });

      var result = row1.Concat (row2);

      Assert.That (result.Values, Is.EqualTo (new[] { "a", "b", "c", "d" }));
    }

    [Test]
    public void Combine_Two ()
    {
      var column1 = ColumnDefinitionObjectMother.CreateColumn("1");
      var column2 = ColumnDefinitionObjectMother.CreateColumn("2");
      var column3 = ColumnDefinitionObjectMother.CreateColumn("3");
      var column4 = ColumnDefinitionObjectMother.CreateColumn("4");

      var one = new ColumnValueTable (
          new[] { column1, column2 },
          new[]
          {
              new ColumnValueTable.Row (new[] { "a", "b" }), 
              new ColumnValueTable.Row (new[] { "c", "d" })
          });
      var two = new ColumnValueTable (
        new[] { column3, column4 },
        new[]
          {
              new ColumnValueTable.Row (new[] { "e", "f" }), 
              new ColumnValueTable.Row (new[] { "g", "h" })
          });

      var result = ColumnValueTable.Combine (one, two);

      var expected = new ColumnValueTable(
        new[] { column1, column2, column3, column4 },
          new[]
          {
              new ColumnValueTable.Row (new[] { "a", "b", "e", "f" }), 
              new ColumnValueTable.Row (new[] { "c", "d", "g", "h" })
          });
      ColumnValueTableTestHelper.CheckTable (expected, result);
    }

    [Test]
    public void Combine_Many ()
    {
      var column1 = ColumnDefinitionObjectMother.CreateColumn ("1");
      var column2 = ColumnDefinitionObjectMother.CreateColumn ("2");
      var column3 = ColumnDefinitionObjectMother.CreateColumn ("3");
      var column4 = ColumnDefinitionObjectMother.CreateColumn ("4");
      var column5 = ColumnDefinitionObjectMother.CreateColumn ("5");
      var column6 = ColumnDefinitionObjectMother.CreateColumn ("6");

      var one = new ColumnValueTable (
          new[] { column1, column2 },
          new[]
          {
              new ColumnValueTable.Row (new[] { "a", "b" }), 
              new ColumnValueTable.Row (new[] { "c", "d" })
          });
      var two = new ColumnValueTable (
        new[] { column3, column4 },
        new[]
          {
              new ColumnValueTable.Row (new[] { "e", "f" }), 
              new ColumnValueTable.Row (new[] { "g", "h" })
          });

      var three = new ColumnValueTable (
        new[] { column5, column6 },
        new[]
          {
              new ColumnValueTable.Row (new[] { "i", "j" }), 
              new ColumnValueTable.Row (new[] { "k", "l" })
          });

      var result = ColumnValueTable.Combine (new[] { one, two, three });

      var expected = new ColumnValueTable (
        new[] { column1, column2, column3, column4, column5, column6 },
          new[]
          {
              new ColumnValueTable.Row (new[] { "a", "b", "e", "f", "i", "j" }), 
              new ColumnValueTable.Row (new[] { "c", "d", "g", "h", "k", "l" })
          });
      ColumnValueTableTestHelper.CheckTable (expected, result);
    }
  }
}