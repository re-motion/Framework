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
using NUnit.Framework;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Rendering
{
  [TestFixture]
  public class BocTitleCellRenderArgumentsTest
  {
    [Test]
    public void Initialize ()
    {
      var isRowHeader = BooleanObjectMother.GetRandomBoolean();
      var arguments = new BocTitleCellRenderArguments(
          SortingDirection.Ascending,
          cellID: "TheCellID",
          orderIndex: 11,
          isRowHeader: isRowHeader);

      Assert.That(arguments.OrderIndex, Is.EqualTo(11));
      Assert.That(arguments.SortingDirection, Is.EqualTo(SortingDirection.Ascending));
      Assert.That(arguments.CellID, Is.EqualTo("TheCellID"));
      Assert.That(arguments.IsRowHeader, Is.EqualTo(isRowHeader));
    }
  }
}
