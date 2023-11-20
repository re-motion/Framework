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
using Moq;
using NUnit.Framework;
using Remotion.ObjectBinding.Validation;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.Validation
{
  [TestFixture]
  public class ValidationFailureWithLocationInformationTest
  {
    [Test]
    public void CreateFailure_CreatesFailureWithoutRowAndColumnInformation ()
    {
      var bocFailure = BusinessObjectValidationFailure.Create("NO, NOT THE BEES!");

      var failure = BocListValidationFailureWithLocationInformation.CreateFailure(bocFailure);

      Assert.That(failure.Failure, Is.EqualTo(bocFailure));
      Assert.That(failure.RowObject, Is.Null);
      Assert.That(failure.ColumnDefinition, Is.Null);
    }

    [Test]
    public void CreateFailureForRow_CreatesFailureWithRowAndWithoutColumnInformation ()
    {
      var bocFailure = BusinessObjectValidationFailure.Create("NO, NOT THE BEES!");
      var rowObjectStub = new Mock<IBusinessObject>();

      var failure = BocListValidationFailureWithLocationInformation.CreateFailureForRow(bocFailure, rowObjectStub.Object);

      Assert.That(failure.Failure, Is.EqualTo(bocFailure));
      Assert.That(failure.RowObject, Is.EqualTo(rowObjectStub.Object));
      Assert.That(failure.ColumnDefinition, Is.Null);
    }

    [Test]
    public void CreateFailureForCell_CreatesFailureWithRowAndColumnInformation ()
    {
      var bocFailure = BusinessObjectValidationFailure.Create("NO, NOT THE BEES!");
      var rowObjectStub = new Mock<IBusinessObject>();
      var columnDefinitionStub = new Mock<BocColumnDefinition>();

      var failure = BocListValidationFailureWithLocationInformation.CreateFailureForCell(bocFailure, rowObjectStub.Object, columnDefinitionStub.Object);

      Assert.That(failure.Failure, Is.EqualTo(bocFailure));
      Assert.That(failure.RowObject, Is.EqualTo(rowObjectStub.Object));
      Assert.That(failure.ColumnDefinition, Is.EqualTo(columnDefinitionStub.Object));
    }
  }
}
