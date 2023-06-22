// // This file is part of the re-motion Core Framework (www.re-motion.org)
// // Copyright (c) rubicon IT GmbH, www.rubicon.eu
// //
// // The re-motion Core Framework is free software; you can redistribute it
// // and/or modify it under the terms of the GNU Lesser General Public License
// // as published by the Free Software Foundation; either version 2.1 of the
// // License, or (at your option) any later version.
// //
// // re-motion is distributed in the hope that it will be useful,
// // but WITHOUT ANY WARRANTY; without even the implied warranty of
// // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// // GNU Lesser General Public License for more details.
// //
// // You should have received a copy of the GNU Lesser General Public License
// // along with re-motion; if not, see http://www.gnu.org/licenses.
// //
using System;
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocDataCellRenderArgumentsTest
  {
    private BocListDataRowRenderEventArgs _bocListDataRowRenderEventArgs;

    [SetUp]
    public void SetUp ()
    {
      _bocListDataRowRenderEventArgs = new BocListDataRowRenderEventArgs(
          13,
          Mock.Of<IBusinessObject>(),
          BooleanObjectMother.GetRandomBoolean(),
          BooleanObjectMother.GetRandomBoolean());
    }

    [Test]
    public void Initialize ()
    {
      var showIcon = BooleanObjectMother.GetRandomBoolean();
      var arguments = new BocDataCellRenderArguments(
          _bocListDataRowRenderEventArgs,
          rowIndex: 17,
          showIcon: showIcon,
          cellID: "TheCellID",
          columnsWithValidationFailures: Array.Empty<bool>(),
          headerIDs: new[] { "X" });

      Assert.That(arguments.AdditionalCssClassForDataRow, Is.EqualTo(_bocListDataRowRenderEventArgs.AdditionalCssClassForDataRow));
      Assert.That(arguments.BusinessObject, Is.SameAs(_bocListDataRowRenderEventArgs.BusinessObject));
      Assert.That(arguments.IsEditableRow, Is.EqualTo(_bocListDataRowRenderEventArgs.IsEditableRow));
      Assert.That(arguments.IsOddRow, Is.EqualTo(_bocListDataRowRenderEventArgs.IsOddRow));
      Assert.That(arguments.ListIndex, Is.EqualTo(_bocListDataRowRenderEventArgs.ListIndex));
      Assert.That(arguments.RowIndex, Is.EqualTo(17));
      Assert.That(arguments.CellID, Is.EqualTo("TheCellID"));
      Assert.That(arguments.HeaderIDs, Is.EqualTo(new[] { "X" }));
      Assert.That(arguments.ShowIcon, Is.EqualTo(showIcon));
    }

    [Test]
    public void IsRowHeader_WithCellIDNotNull_ReturnsTrue ()
    {
      var arguments = new BocDataCellRenderArguments(
          _bocListDataRowRenderEventArgs,
          -1,
          false,
          cellID: "TheCellID",
          headerIDs: Array.Empty<string>(),
          columnsWithValidationFailures: Array.Empty<bool>());

      Assert.That(arguments.IsRowHeader, Is.True);
    }
    [Test]
    public void IsRowHeader_WithCellIDNull_ReturnsFalse ()
    {
      var arguments = new BocDataCellRenderArguments(
          _bocListDataRowRenderEventArgs,
          -1,
          false,
          cellID: null,
          headerIDs: Array.Empty<string>(),
          columnsWithValidationFailures: Array.Empty<bool>());

      Assert.That(arguments.IsRowHeader, Is.False);
    }
  }
}
