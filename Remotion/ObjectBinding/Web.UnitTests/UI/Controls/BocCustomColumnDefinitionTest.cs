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
using Remotion.ObjectBinding.BusinessObjectPropertyPaths;
using Remotion.ObjectBinding.Validation;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Sorting;
using Remotion.ObjectBinding.Web.UnitTests.Domain;
using Remotion.Web;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls
{
  [TestFixture]
  public class BocCustomColumnDefinitionTest
  {
    [Test]
    public void GetComparer_WithPropertyPathSet ()
    {
      var propertyPath = new Mock<IBusinessObjectPropertyPath>();

      var column = new BocCustomColumnDefinition();
      column.SetPropertyPath(propertyPath.Object);
      column.CustomCell = new Mock<BocCustomColumnDefinitionCell>() { CallBase = true }.Object;
      column.OwnerControl = Mock.Of<IBocList>();

      var comparer = ((IBocSortableColumnDefinition)column).CreateCellValueComparer();
      Assert.That(comparer, Is.InstanceOf<BusinessObjectPropertyPathBasedComparer>());
      Assert.That(((BusinessObjectPropertyPathBasedComparer)comparer).PropertyPath, Is.SameAs(propertyPath.Object));
    }

    [Test]
    public void GetComparer_WithPropertyPathNull ()
    {
      var column = new BocCustomColumnDefinition();
      column.SetPropertyPath(null);
      column.CustomCell = new Mock<BocCustomColumnDefinitionCell>() { CallBase = true }.Object;
      column.OwnerControl = Mock.Of<IBocList>();

      var comparer = ((IBocSortableColumnDefinition)column).CreateCellValueComparer();
      Assert.That(comparer, Is.InstanceOf<BusinessObjectPropertyPathBasedComparer>());
      Assert.That(
          ((BusinessObjectPropertyPathBasedComparer)comparer).PropertyPath,
          Is.InstanceOf<NullBusinessObjectPropertyPath>());
    }

    [Test]
    public void GetValidationFailureMatcher_ReturnsInstanceOfPropertyPathValidationFailureMatcher ()
    {
      var column = new BocCustomColumnDefinition
                   {
                     CustomCell = new Mock<BocCustomColumnDefinitionCell>() { CallBase = true }.Object,
                     OwnerControl = Mock.Of<IBocList>()
                   };

      var validationFailureMatcher = column.GetValidationFailureMatcher();

      Assert.That(validationFailureMatcher, Is.InstanceOf<BusinessObjectPropertyPathValidationFailureMatcher>());
      Assert.That(
          ((BusinessObjectPropertyPathValidationFailureMatcher)validationFailureMatcher).PropertyPath,
          Is.EqualTo(column.GetPropertyPath()));
    }

    [Test]
    public void ColumnTitleDisplayValue_EmptyColumnTitle_NullPropertyPath_ReturnsEmptyWebString ()
    {
      var column = new BocCustomColumnDefinition();
      Assert.That(column.PropertyPathIdentifier, Is.Null);
      Assert.That(column.IsDynamic, Is.False);

      var result = column.ColumnTitleDisplayValue;
      Assert.That(result, Is.EqualTo(WebString.Empty));
    }

    [Test]
    public void ColumnTitleDisplayValue_EmptyColumnTitle_StaticPropertyPath_ReturnsDisplayNameOfLastProperty ()
    {
      var rootBusinessObjectClass = BindableObjectProviderTestHelper.GetBindableObjectClass(typeof(TypeWithAllDataTypes));
      var hostClassOfLastProperty = BindableObjectProviderTestHelper.GetBindableObjectClass(typeof(TypeWithString));
      var lastProperty = hostClassOfLastProperty.GetPropertyDefinition(nameof(TypeWithString.StringValue));
      Assert.That(lastProperty, Is.Not.Null);

      var column = new BocCustomColumnDefinition();
      column.SetPropertyPath(StaticBusinessObjectPropertyPath.Parse("BusinessObject.StringValue", rootBusinessObjectClass));
      Assert.That(column.IsDynamic, Is.False);

      var result = column.ColumnTitleDisplayValue;
      Assert.That(result.GetValue(), Is.EqualTo(lastProperty.DisplayName));
    }

    [Test]
    public void ColumnTitleDisplayValue_EmptyColumnTitle_DynamicPropertyPath_ReturnsEmptyWebString ()
    {
      var column = new BocCustomColumnDefinition();
      column.SetPropertyPath(DynamicBusinessObjectPropertyPath.Create("BusinessObject.StringValue"));
      Assert.That(column.IsDynamic, Is.True);

      var result = column.ColumnTitleDisplayValue;
      Assert.That(result.IsEmpty, Is.True);
    }

    [Test]
    public void ColumnTitleDisplayValue_WithColumnTitle_ReturnsColumnTitle ()
    {
      var column = new BocCustomColumnDefinition
                   {
                       ColumnTitle = WebString.CreateFromText("A quite distinct column title")
                   };
      column.SetPropertyPath(DynamicBusinessObjectPropertyPath.Create("BusinessObject.StringValue"));
      Assert.That(column.IsDynamic, Is.True);

      var result = column.ColumnTitleDisplayValue;
      Assert.That(result.GetValue(), Is.EqualTo("A quite distinct column title"));
    }
  }
}
