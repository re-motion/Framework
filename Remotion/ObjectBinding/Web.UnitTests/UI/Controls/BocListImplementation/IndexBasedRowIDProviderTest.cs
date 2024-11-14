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
  public class IndexBasedRowIDProviderTest
  {
    [Test]
    public void GetControlRowID ()
    {
      var rowIDProvider = new IndexBasedRowIDProvider(new IBusinessObject[6]);
      Assert.That(rowIDProvider.GetControlRowID(new BocListRow(5, CreateObject())), Is.EqualTo("5"));
    }

    [Test]
    public void GetControlRowID_BeyondEndOfList ()
    {
      var rowIDProvider = new IndexBasedRowIDProvider(new IBusinessObject[6]);

      Assert.That(
          () => rowIDProvider.GetControlRowID(new BocListRow(6, CreateObject())),
          Throws.InvalidOperationException
              .With.Message.StartsWith("Tried to retrieve the ID for the row at index 6 but the current length of the row collection is 6."));
    }

    [Test]
    public void GetItemRowID ()
    {
      var rowIDProvider = new IndexBasedRowIDProvider(new IBusinessObject[6]);
      Assert.That(rowIDProvider.GetItemRowID(new BocListRow(3, CreateObject())), Is.EqualTo("3"));
    }

    [Test]
    public void GetItemRowID_BeyondEndOfList ()
    {
      var rowIDProvider = new IndexBasedRowIDProvider(new IBusinessObject[6]);

      Assert.That(
          () => rowIDProvider.GetItemRowID(new BocListRow(6, CreateObject())),
          Throws.InvalidOperationException
              .With.Message.StartsWith("Tried to retrieve the ID for the row at index 6 but the current length of the row collection is 6."));
    }

    [Test]
    public void GetRowFromItemRowID ()
    {
      var values = new[]
                   {
                       CreateObject(),
                       CreateObject(),
                       CreateObject()
                   };
      var rowIDProvider = new IndexBasedRowIDProvider(values);

      var row = rowIDProvider.GetRowFromItemRowID(values, "1");

      Assert.That(row.Index, Is.EqualTo(1));
      Assert.That(row.BusinessObject, Is.SameAs(values[1]));
    }

    [Test]
    public void GetRowFromItemRowID_IndexGreaterThanValueLength ()
    {
      var values = new[]
                   {
                       CreateObject()
                   };
      var rowIDProvider = new IndexBasedRowIDProvider(values);

      var row = rowIDProvider.GetRowFromItemRowID(values, "1");

      Assert.That(row, Is.Null);
    }

    [Test]
    public void GetRowFromItemRowID_ValueLengthDiffersFromInitialization ()
    {
      var rowIDProvider = new IndexBasedRowIDProvider(new IBusinessObject[2]);
      var values = new[]
                   {
                       CreateObject(),
                       CreateObject(),
                       CreateObject()
                   };

      var row = rowIDProvider.GetRowFromItemRowID(values, "1");

      Assert.That(row, Is.Null);
    }

    [Test]
    public void GetRowFromItemRowID_InvalidRowIDFormat_ThrowsFormatException ()
    {
      var rowIDProvider = new IndexBasedRowIDProvider(new IBusinessObject[0]);

      Assert.That(
          () => rowIDProvider.GetRowFromItemRowID(new IBusinessObject[0], "x"),
          Throws.TypeOf<FormatException>().With.Message.EqualTo("RowID 'x' could not be parsed as an integer."));
    }

    [Test]
    public void AddRow_AtMiddleOfList ()
    {
      var rowIDProvider = new IndexBasedRowIDProvider(new IBusinessObject[6]);

      rowIDProvider.AddRow(new BocListRow(3, CreateObject()));

      Assert.That(rowIDProvider.GetItemRowID(new BocListRow(2, CreateObject())), Is.EqualTo("2"));
      Assert.That(rowIDProvider.GetItemRowID(new BocListRow(3, CreateObject())), Is.EqualTo("6"));
      Assert.That(rowIDProvider.GetItemRowID(new BocListRow(4, CreateObject())), Is.EqualTo("3"));
      Assert.That(rowIDProvider.GetItemRowID(new BocListRow(5, CreateObject())), Is.EqualTo("4"));
      Assert.That(rowIDProvider.GetItemRowID(new BocListRow(6, CreateObject())), Is.EqualTo("5"));
    }

    [Test]
    public void AddRow_AtEndOfList ()
    {
      var rowIDProvider = new IndexBasedRowIDProvider(new IBusinessObject[6]);

      rowIDProvider.AddRow(new BocListRow(6, CreateObject()));

      Assert.That(rowIDProvider.GetItemRowID(new BocListRow(6, CreateObject())), Is.EqualTo("6"));
    }

    [Test]
    public void AddRow_BeyondEndOfList ()
    {
      var rowIDProvider = new IndexBasedRowIDProvider(new IBusinessObject[6]);

      Assert.That(
          () => rowIDProvider.AddRow(new BocListRow(7, CreateObject())),
          Throws.InvalidOperationException.With.Message.StartsWith("Tried to add row at index 7 but the current length of the row collection is 6."));
    }

    [Test]
    public void RemoveRow ()
    {
      var rowIDProvider = new IndexBasedRowIDProvider(new IBusinessObject[6]);

      rowIDProvider.RemoveRow(new BocListRow(3, CreateObject()));

      Assert.That(rowIDProvider.GetItemRowID(new BocListRow(2, CreateObject())), Is.EqualTo("2"));
      Assert.That(rowIDProvider.GetItemRowID(new BocListRow(3, CreateObject())), Is.EqualTo("4"));
      Assert.That(rowIDProvider.GetItemRowID(new BocListRow(4, CreateObject())), Is.EqualTo("5"));
    }

    [Test]
    public void RemoveRow_BeyondEndOfList ()
    {
      var rowIDProvider = new IndexBasedRowIDProvider(new IBusinessObject[6]);

      Assert.That(
          () => rowIDProvider.RemoveRow(new BocListRow(7, CreateObject())),
          Throws.InvalidOperationException.With.Message.StartsWith("Tried to remove row at index 7 but the current length of the row collection is 6."));
    }

    [Test]
    public void RemoveRow_AddRow ()
    {
      var rowIDProvider = new IndexBasedRowIDProvider(new IBusinessObject[6]);

      rowIDProvider.RemoveRow(new BocListRow(3, CreateObject()));
      rowIDProvider.AddRow(new BocListRow(3, CreateObject()));

      Assert.That(rowIDProvider.GetItemRowID(new BocListRow(2, CreateObject())), Is.EqualTo("2"));
      Assert.That(rowIDProvider.GetItemRowID(new BocListRow(3, CreateObject())), Is.EqualTo("6"));
      Assert.That(rowIDProvider.GetItemRowID(new BocListRow(4, CreateObject())), Is.EqualTo("4"));
    }

    [Test]
    public void GetItemRowID_GetRowFromItemRowID ()
    {
      var rowIDProvider = new IndexBasedRowIDProvider(new IBusinessObject[3]);
      var rowID = rowIDProvider.GetItemRowID(new BocListRow(1, CreateObject()));

      var values = new[]
                   {
                       CreateObject(),
                       CreateObject(),
                       CreateObject()
                   };

      var row = rowIDProvider.GetRowFromItemRowID(values, rowID);
      Assert.That(row.Index, Is.EqualTo(1));
      Assert.That(row.BusinessObject, Is.SameAs(values[1]));
    }

    private IBusinessObject CreateObject ()
    {
      return new Mock<IBusinessObject>().Object;
    }
  }
}
