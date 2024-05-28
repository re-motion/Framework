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
using Moq;
using NUnit.Framework;
using Remotion.ObjectBinding.BusinessObjectPropertyPaths.Results;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Sorting;
using Remotion.ObjectBinding.Web.UnitTests.Domain;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Sorting
{
  [TestFixture]
  public class BusinessObjectPropertyPathBasedComparerTest
  {
    [Test]
    public void Compare_StringValues ()
    {
      var valueA = TypeWithAllDataTypes.Create();
      valueA.String = "A";

      var valueB = TypeWithAllDataTypes.Create();
      valueB.String = "B";

      var valueNull = TypeWithAllDataTypes.Create();
      valueNull.String = null;

      var bindableObjectClass = BindableObjectProviderTestHelper.GetBindableObjectClass(typeof(TypeWithAllDataTypes));

      var propertyPath = BusinessObjectPropertyPath.CreateStatic(bindableObjectClass, "String");
      var comparer = new BusinessObjectPropertyPathBasedComparer(propertyPath);

      AssertCompare(comparer, valueA, valueB, valueNull);
    }

    [Test]
    public void Compare_ReferenceValues ()
    {
      var valueA = TypeWithReference.Create();
      valueA.ReferenceValue = TypeWithReference.Create("A");

      var valueB = TypeWithReference.Create();
      valueB.ReferenceValue = TypeWithReference.Create("B");

      var valueNull = TypeWithReference.Create();
      valueNull.ReferenceValue = null;

      var bindableObjectClass = BindableObjectProviderTestHelper.GetBindableObjectClass(typeof(TypeWithReference));

      var propertyPath = BusinessObjectPropertyPath.CreateStatic(bindableObjectClass, "ReferenceValue");
      var comparer = new BusinessObjectPropertyPathBasedComparer(propertyPath);

      AssertCompare(comparer, valueA, valueB, valueNull);
    }

    [Test]
    public void Compare_List ()
    {
      var valueAA = TypeWithString.Create();
      valueAA.StringArray = new[] { "A", "A" };

      var valueBA = TypeWithString.Create();
      valueBA.StringArray = new[] { "B", "A" };

      var valueAB = TypeWithString.Create();
      valueAB.StringArray = new[] { "A", "B" };

      var valueNull = TypeWithString.Create();
      valueNull.StringArray = null;

      var valueEmpty = TypeWithString.Create();
      valueEmpty.StringArray = new string[0];

      var bindableObjectClass = BindableObjectProviderTestHelper.GetBindableObjectClass(typeof(TypeWithString));

      var propertyPath = BusinessObjectPropertyPath.CreateStatic(bindableObjectClass, "StringArray");
      var comparer = new BusinessObjectPropertyPathBasedComparer(propertyPath);

      CompareEqualValues(comparer, (IBusinessObject)valueAA, (IBusinessObject)valueAA);
      CompareEqualValues(comparer, (IBusinessObject)valueAA, (IBusinessObject)valueAB);
      CompareEqualValues(comparer, (IBusinessObject)valueNull, (IBusinessObject)valueNull);
      CompareEqualValues(comparer, (IBusinessObject)valueEmpty, (IBusinessObject)valueEmpty);

      CompareAscendingValues(comparer, (IBusinessObject)valueAA, (IBusinessObject)valueBA);
      CompareAscendingValues(comparer, (IBusinessObject)valueNull, (IBusinessObject)valueAA);
      CompareAscendingValues(comparer, (IBusinessObject)valueEmpty, (IBusinessObject)valueAA);
      CompareAscendingValues(comparer, (IBusinessObject)valueNull, (IBusinessObject)valueEmpty);

      CompareDescendingValues(comparer, (IBusinessObject)valueBA, (IBusinessObject)valueAA);
      CompareDescendingValues(comparer, (IBusinessObject)valueAA, (IBusinessObject)valueNull);
      CompareDescendingValues(comparer, (IBusinessObject)valueAA, (IBusinessObject)valueEmpty);
      CompareDescendingValues(comparer, (IBusinessObject)valueEmpty, (IBusinessObject)valueNull);
    }

    [Test]
    public void Compare_ExceptionDuringGetResult_SwallowsException ()
    {
      var valueA = TypeWithAllDataTypes.Create();
      var valueThrows = TypeWithAllDataTypes.Create();

      var resultA = new Mock<IBusinessObjectPropertyPathResult>();
      resultA.Setup(_ => _.GetValue()).Returns(new object());

      var propertyPathStub = new Mock<IBusinessObjectPropertyPath>();

      propertyPathStub
          .Setup(
              _ =>
              _.GetResult(
                  (IBusinessObject)valueA,
                  BusinessObjectPropertyPath.UnreachableValueBehavior.ReturnNullForUnreachableValue,
                  BusinessObjectPropertyPath.ListValueBehavior.GetResultForFirstListEntry))
          .Returns(resultA.Object);

      propertyPathStub
          .Setup(
              _ =>
              _.GetResult(
                  (IBusinessObject)valueThrows,
                  BusinessObjectPropertyPath.UnreachableValueBehavior.ReturnNullForUnreachableValue,
                  BusinessObjectPropertyPath.ListValueBehavior.GetResultForFirstListEntry))
          .Throws(new Exception());

      var comparer = new BusinessObjectPropertyPathBasedComparer(propertyPathStub.Object);

      CompareEqualValues(comparer, (IBusinessObject)valueThrows, (IBusinessObject)valueThrows);
      CompareAscendingValues(comparer, (IBusinessObject)valueThrows, (IBusinessObject)valueA);
      CompareDescendingValues(comparer, (IBusinessObject)valueA, (IBusinessObject)valueThrows);
    }

    [Test]
    public void Compare_ExceptionDuringGetValue_SwallowsException ()
    {
      var valueA = TypeWithAllDataTypes.Create();
      var valueThrows = TypeWithAllDataTypes.Create();

      var resultA = new Mock<IBusinessObjectPropertyPathResult>();
      resultA.Setup(_ => _.GetValue()).Returns(new object());

      var resultThrows = new Mock<IBusinessObjectPropertyPathResult>();
      resultThrows.Setup(_ => _.GetValue()).Throws(new Exception());

      var propertyPathStub = new Mock<IBusinessObjectPropertyPath>();

      propertyPathStub
          .Setup(
              _ =>
              _.GetResult(
                  (IBusinessObject)valueA,
                  BusinessObjectPropertyPath.UnreachableValueBehavior.ReturnNullForUnreachableValue,
                  BusinessObjectPropertyPath.ListValueBehavior.GetResultForFirstListEntry))
          .Returns(resultA.Object);

      propertyPathStub
          .Setup(
              _ =>
              _.GetResult(
                  (IBusinessObject)valueThrows,
                  BusinessObjectPropertyPath.UnreachableValueBehavior.ReturnNullForUnreachableValue,
                  BusinessObjectPropertyPath.ListValueBehavior.GetResultForFirstListEntry))
          .Returns(resultThrows.Object);

      var comparer = new BusinessObjectPropertyPathBasedComparer(propertyPathStub.Object);

      CompareEqualValues(comparer, (IBusinessObject)valueThrows, (IBusinessObject)valueThrows);
      CompareAscendingValues(comparer, (IBusinessObject)valueThrows, (IBusinessObject)valueA);
      CompareDescendingValues(comparer, (IBusinessObject)valueA, (IBusinessObject)valueThrows);
    }

    [Test]
    public void Compare_ExceptionDuringGetString_SwallowsException ()
    {
      var valueA = TypeWithAllDataTypes.Create();
      var valueThrows = TypeWithAllDataTypes.Create();

      var resultA = new Mock<IBusinessObjectPropertyPathResult>();
      resultA.Setup(_ => _.GetValue()).Returns(new object());
      resultA.Setup(_ => _.GetString(null)).Returns("A");

      var resultThrows = new Mock<IBusinessObjectPropertyPathResult>();
      resultThrows.Setup(_ => _.GetValue()).Returns(new object());
      resultThrows.Setup(_ => _.GetString(null)).Throws(new Exception());

      var propertyPathStub = new Mock<IBusinessObjectPropertyPath>();

      propertyPathStub
          .Setup(
              _ =>
              _.GetResult(
                  (IBusinessObject)valueA,
                  BusinessObjectPropertyPath.UnreachableValueBehavior.ReturnNullForUnreachableValue,
                  BusinessObjectPropertyPath.ListValueBehavior.GetResultForFirstListEntry))
          .Returns(resultA.Object);

      propertyPathStub
          .Setup(
              _ =>
              _.GetResult(
                  (IBusinessObject)valueThrows,
                  BusinessObjectPropertyPath.UnreachableValueBehavior.ReturnNullForUnreachableValue,
                  BusinessObjectPropertyPath.ListValueBehavior.GetResultForFirstListEntry))
          .Returns(resultThrows.Object);

      var comparer = new BusinessObjectPropertyPathBasedComparer(propertyPathStub.Object);

      CompareEqualValues(comparer, (IBusinessObject)valueThrows, (IBusinessObject)valueThrows);
      CompareAscendingValues(comparer, (IBusinessObject)valueThrows, (IBusinessObject)valueA);
      CompareDescendingValues(comparer, (IBusinessObject)valueA, (IBusinessObject)valueThrows);
    }

    private void AssertCompare (BusinessObjectPropertyPathBasedComparer comparer, object valueA, object valueB, object valueNull)
    {
      CompareEqualValues(comparer, (IBusinessObject)valueA, (IBusinessObject)valueA);
      CompareEqualValues(comparer, (IBusinessObject)valueNull, (IBusinessObject)valueNull);

      CompareAscendingValues(comparer, (IBusinessObject)valueA, (IBusinessObject)valueB);
      CompareAscendingValues(comparer, (IBusinessObject)valueNull, (IBusinessObject)valueA);

      CompareDescendingValues(comparer, (IBusinessObject)valueB, (IBusinessObject)valueA);
      CompareDescendingValues(comparer, (IBusinessObject)valueA, (IBusinessObject)valueNull);
    }

    private void CompareEqualValues (IComparer<BocListRow> comparer, IBusinessObject left, IBusinessObject right)
    {
      var rowLeft = new BocListRow(0, left);
      var rowRight = new BocListRow(0, right);

      int compareResultLeftRight = comparer.Compare(rowLeft, rowRight);
      int compareResultRightLeft = comparer.Compare(rowRight, rowLeft);

      Assert.That(compareResultLeftRight == 0, Is.True, "Left - Right != zero");
      Assert.That(compareResultRightLeft == 0, Is.True, "Right - Left != zero");
    }

    private void CompareAscendingValues (IComparer<BocListRow> comparer, IBusinessObject left, IBusinessObject right)
    {
      var rowLeft = new BocListRow(0, left);
      var rowRight = new BocListRow(0, right);

      int compareResultLeftRight = comparer.Compare(rowLeft, rowRight);
      int compareResultRightLeft = comparer.Compare(rowRight, rowLeft);

      Assert.That(compareResultLeftRight < 0, Is.True, "Left - Right <= zero.");
      Assert.That(compareResultRightLeft > 0, Is.True, "Right - Left >= zero.");
    }

    private void CompareDescendingValues (IComparer<BocListRow> comparer, IBusinessObject left, IBusinessObject right)
    {
      var rowLeft = new BocListRow(0, left);
      var rowRight = new BocListRow(0, right);

      int compareResultLeftRight = comparer.Compare(rowLeft, rowRight);
      int compareResultRightLeft = comparer.Compare(rowRight, rowLeft);

      Assert.That(compareResultLeftRight > 0, Is.True, "Right - Left >= zero.");
      Assert.That(compareResultRightLeft < 0, Is.True, "Left - Right <= zero.");
    }
  }
}
