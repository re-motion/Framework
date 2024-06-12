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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation
{
  [TestFixture]
  public class UniqueIdentifierBasedRowIDProviderTest
  {
    [Test]
    public void GetControlRowID ()
    {
      var rowIDProvider = new UniqueIdentifierBasedRowIDProvider();
      Assert.That(rowIDProvider.GetControlRowID(new BocListRow(5, CreateObject("a"))), Is.EqualTo("a"));
    }

    [Test]
    public void GetControlRowID_EscapesInvalidIDCharacters ()
    {
      var rowIDProvider = new UniqueIdentifierBasedRowIDProvider();
      Assert.That(rowIDProvider.GetControlRowID(new BocListRow(5, CreateObject("a.1|"))), Is.EqualTo("a_1_"));
    }

    [Test]
    public void GetItemRowID ()
    {
      var rowIDProvider = new UniqueIdentifierBasedRowIDProvider();
      Assert.That(rowIDProvider.GetItemRowID(new BocListRow(3, CreateObject("a"))), Is.EqualTo("3|a"));
    }

    [Test]
    public void GetItemRowID_EscapesInvalidIDCharacters ()
    {
      var rowIDProvider = new UniqueIdentifierBasedRowIDProvider();
      Assert.That(rowIDProvider.GetItemRowID(new BocListRow(3, CreateObject("a.1|"))), Is.EqualTo("3|a.1|"));
    }

    [Test]
    public void GetRowFromItemRowID_IndexMatches ()
    {
      var values = new[]
                   {
                       CreateObject("a"),
                       CreateObject("b.1|"),
                       CreateObject("c")
                   };
      var rowIDProvider = new UniqueIdentifierBasedRowIDProvider();

      var row = rowIDProvider.GetRowFromItemRowID(values, "1|b.1|");

      Assert.That(row.Index, Is.EqualTo(1));
      Assert.That(row.BusinessObject, Is.SameAs(values[1]));
    }

    [Test]
    public void GetRowFromItemRowID_IndexDoesNotMatch_IndexTooBig ()
    {
      var values = new[]
                   {
                       CreateObject("a"),
                       CreateObject("b.1|"),
                       CreateObject("c"),
                       CreateObject("d"),
                       CreateObject("e"),
                       CreateObject("f"),
                       CreateObject("g"),
                       CreateObject("h")
                   };
      var rowIDProvider = new UniqueIdentifierBasedRowIDProvider();

      var row = rowIDProvider.GetRowFromItemRowID(values, "5|b.1|");

      Assert.That(row.Index, Is.EqualTo(1));
      Assert.That(row.BusinessObject, Is.SameAs(values[1]));
    }

    [Test]
    public void GetRowFromItemRowID_IndexDoesNotMatch_IndexTooSmall ()
    {
      var values = new[]
                   {
                       CreateObject("a"),
                       CreateObject("b"),
                       CreateObject("c"),
                       CreateObject("d"),
                       CreateObject("e"),
                       CreateObject("f"),
                       CreateObject("g.1|"),
                       CreateObject("h")
                   };
      var rowIDProvider = new UniqueIdentifierBasedRowIDProvider();

      var row = rowIDProvider.GetRowFromItemRowID(values, "2|g.1|");

      Assert.That(row.Index, Is.EqualTo(6));
      Assert.That(row.BusinessObject, Is.SameAs(values[6]));
    }

    [Test]
    public void GetRowFromItemRowID_ItemInValueList_IndexGreaterThanValueLength ()
    {
      var values = new[]
                   {
                       CreateObject("a"),
                       CreateObject("b.1|")
                   };
      var rowIDProvider = new UniqueIdentifierBasedRowIDProvider();

      var row = rowIDProvider.GetRowFromItemRowID(values, "2|b.1|");

      Assert.That(row.Index, Is.EqualTo(1));
      Assert.That(row.BusinessObject, Is.SameAs(values[1]));
    }

    [Test]
    public void GetRowFromItemRowID_ItemNotInValueList ()
    {
      var rowIDProvider = new UniqueIdentifierBasedRowIDProvider();
      var values = new[]
                   {
                       CreateObject("a"),
                       CreateObject("c"),
                       CreateObject("b")
                   };

      var row = rowIDProvider.GetRowFromItemRowID(values, "1|d");

      Assert.That(row, Is.Null);
    }

    [Test]
    public void GetRowFromItemRowID_ItemNotInValueList_IndexTooBig ()
    {
      var rowIDProvider = new UniqueIdentifierBasedRowIDProvider();
      var values = new[]
                   {
                       CreateObject("a"),
                       CreateObject("c"),
                       CreateObject("b")
                   };

      var row = rowIDProvider.GetRowFromItemRowID(values, "4|d");

      Assert.That(row, Is.Null);
    }

    [Test]
    public void GetRowFromItemRowID_InvalidRowIDFormat_ThrowsFormatException ()
    {
      var rowIDProvider = new UniqueIdentifierBasedRowIDProvider();

      Assert.That(
          () => rowIDProvider.GetRowFromItemRowID(new IBusinessObject[0], "x"),
          Throws.TypeOf<FormatException>().With.Message.EqualTo("RowID 'x' could not be parsed. Expected format: '<rowIndex>|<unqiueIdentifier>'"));
    }

    [Test]
    public void GetRowFromItemRowID_InvalidRowIndexFormat_ThrowsFormatException ()
    {
      var rowIDProvider = new UniqueIdentifierBasedRowIDProvider();

      Assert.That(
          () => rowIDProvider.GetRowFromItemRowID(new IBusinessObject[0], "a|x"),
          Throws.TypeOf<FormatException>().With.Message.EqualTo("RowID 'a|x' could not be parsed. Expected format: '<rowIndex>|<unqiueIdentifier>'"));
    }

    [Test]
    public void AddRow_HasNoEffect ()
    {
      var rowIDProvider = new UniqueIdentifierBasedRowIDProvider();

      rowIDProvider.AddRow(new BocListRow(3, CreateObject("a")));

      Assert.That(rowIDProvider.GetItemRowID(new BocListRow(3, CreateObject("a"))), Is.EqualTo("3|a"));
      Assert.That(rowIDProvider.GetItemRowID(new BocListRow(4, CreateObject("a"))), Is.EqualTo("4|a"));
      Assert.That(rowIDProvider.GetItemRowID(new BocListRow(3, CreateObject("b"))), Is.EqualTo("3|b"));
    }

    [Test]
    public void RemoveRow_HasNoEffect ()
    {
      var rowIDProvider = new UniqueIdentifierBasedRowIDProvider();

      rowIDProvider.RemoveRow(new BocListRow(3, CreateObject("a")));

      Assert.That(rowIDProvider.GetItemRowID(new BocListRow(3, CreateObject("a"))), Is.EqualTo("3|a"));
    }

    [Test]
    public void GetItemRowID_GetRowFromItemRowID ()
    {
      var rowIDProvider = new UniqueIdentifierBasedRowIDProvider();
      var rowID = rowIDProvider.GetItemRowID(new BocListRow(1, CreateObject("b.1|")));

      var values = new[]
                   {
                       CreateObject("a.1|"),
                       CreateObject("b.1|"),
                       CreateObject("c.1|")
                   };

      var row = rowIDProvider.GetRowFromItemRowID(values, rowID);
      Assert.That(row.Index, Is.EqualTo(1));
      Assert.That(row.BusinessObject, Is.SameAs(values[1]));
    }

    private IBusinessObject CreateObject (string id)
    {
      var obj = new Mock<IBusinessObjectWithIdentity>();
      obj.Setup(_ => _.UniqueIdentifier).Returns(id);

      return obj.Object;
    }
  }
}
