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
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.ObjectBinding.BusinessObjectPropertyPaths;
using Remotion.ObjectBinding.Validation;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Sorting;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls
{
  [TestFixture]
  public class BocCompundColumnDefinitionTest
  {
    [Test]
    public void GetComparer_WithPropertyPathSet ()
    {
      var propertyPath0 = new Mock<IBusinessObjectPropertyPath>();
      var propertyPath1 = new Mock<IBusinessObjectPropertyPath>();

      var column = new BocCompoundColumnDefinition();
      column.PropertyPathBindings.Add(new PropertyPathBinding(propertyPath0.Object));
      column.PropertyPathBindings.Add(new PropertyPathBinding(propertyPath1.Object));

      var comparer = ((IBocSortableColumnDefinition)column).CreateCellValueComparer();
      Assert.That(comparer, Is.InstanceOf<CompoundComparer<BocListRow>>());

      var comparers = ((CompoundComparer<BocListRow>)comparer).Comparers;
      Assert.That(comparers.Count, Is.EqualTo(2));
      Assert.That(((BusinessObjectPropertyPathBasedComparer)comparers[0]).PropertyPath, Is.SameAs(propertyPath0.Object));
      Assert.That(((BusinessObjectPropertyPathBasedComparer)comparers[1]).PropertyPath, Is.SameAs(propertyPath1.Object));
    }

    [Test]
    public void GetComparer_WithPropertyPathNull ()
    {
      var propertyPath1 = new Mock<IBusinessObjectPropertyPath>();

      var column = new BocCompoundColumnDefinition();
      column.PropertyPathBindings.Add(new PropertyPathBinding());
      column.PropertyPathBindings.Add(new PropertyPathBinding(propertyPath1.Object));

      var comparer = ((IBocSortableColumnDefinition)column).CreateCellValueComparer();
      Assert.That(comparer, Is.InstanceOf<CompoundComparer<BocListRow>>());

      var comparers = ((CompoundComparer<BocListRow>)comparer).Comparers;
      Assert.That(comparers.Count, Is.EqualTo(2));
      Assert.That(
          ((BusinessObjectPropertyPathBasedComparer)comparers[0]).PropertyPath,
          Is.InstanceOf<NullBusinessObjectPropertyPath>());
      Assert.That(((BusinessObjectPropertyPathBasedComparer)comparers[1]).PropertyPath, Is.SameAs(propertyPath1.Object));
    }

    [Test]
    public void GetValidationFailureMatcher_ReturnsInstanceOfCompoundValidationFailureMatcher ()
    {
      var column = new BocCompoundColumnDefinition();

      var validationFailureMatcher = ((IBocColumnDefinitionWithValidationSupport)column).GetValidationFailureMatcher();

      Assert.That(validationFailureMatcher, Is.InstanceOf<CompoundBusinessObjectPropertyPathBasedValidationFailureMatcher>());
      Assert.That(
          ((CompoundBusinessObjectPropertyPathBasedValidationFailureMatcher)validationFailureMatcher).PropertyPaths,
          Is.EqualTo(column.PropertyPathBindings.Cast<PropertyPathBinding>().Select(p => p.GetPropertyPath())));
    }
  }
}
