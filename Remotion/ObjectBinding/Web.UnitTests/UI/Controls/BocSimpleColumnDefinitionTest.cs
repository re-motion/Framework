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
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Sorting;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls
{
  [TestFixture]
  public class BocSimpleColumnDefinitionTest
  {
    [Test]
    public void GetComparer_WithPropertyPathSet ()
    {
      var propertyPath = new Mock<IBusinessObjectPropertyPath>();

      var column = new BocSimpleColumnDefinition();
      column.SetPropertyPath(propertyPath.Object);

      var comparer = ((IBocSortableColumnDefinition)column).CreateCellValueComparer();
      Assert.That(comparer, Is.InstanceOf<BusinessObjectPropertyPathBasedComparer>());
      Assert.That(((BusinessObjectPropertyPathBasedComparer)comparer).PropertyPath, Is.SameAs(propertyPath.Object));
    }

    [Test]
    public void GetComparer_WithPropertyPathNull ()
    {
      var column = new BocSimpleColumnDefinition();
      column.SetPropertyPath(null);

      var comparer = ((IBocSortableColumnDefinition)column).CreateCellValueComparer();
      Assert.That(comparer, Is.InstanceOf<BusinessObjectPropertyPathBasedComparer>());
      Assert.That(
          ((BusinessObjectPropertyPathBasedComparer)comparer).PropertyPath,
          Is.InstanceOf<NullBusinessObjectPropertyPath>());
    }

    [Test]
    public void GetValidationFailureMatcher_ReturnsInstanceOfPropertyPathValidationFailureMatcher ()
    {
      var column = new BocSimpleColumnDefinition();

      var validationFailureMatcher = ((IBocColumnDefinitionWithValidationSupport)column).GetValidationFailureMatcher();

      Assert.That(validationFailureMatcher, Is.InstanceOf<BusinessObjectPropertyPathValidationFailureMatcher>());
      Assert.That(
          ((BusinessObjectPropertyPathValidationFailureMatcher)validationFailureMatcher).PropertyPath,
          Is.EqualTo(column.GetPropertyPath()));
    }
  }
}
