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
using Remotion.Development.UnitTesting.NUnit;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class ConstantEnumerationValueFilterTest : EnumerationTestBase
  {

    public override void SetUp ()
    {
      base.SetUp();
    }

    [Test]
    public void Initialize ()
    {
      Enum[] expected = new Enum[] {TestEnum.Value1, TestEnum.Value2};
      ConstantEnumerationValueFilter filter = new ConstantEnumerationValueFilter(expected);

      Assert.That(filter.DisabledEnumValues, Is.EqualTo(expected));
    }

    [Test]
    public void Initialize_WithMismatchedEnumValues ()
    {
      Assert.That(
          () => new ConstantEnumerationValueFilter(new Enum[] { TestEnum.Value1, EnumWithUndefinedValue.Value2 }),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Item 1 of parameter 'disabledValues' has the type 'Remotion.ObjectBinding.UnitTests.TestDomain.EnumWithUndefinedValue' "
                  + "instead of 'Remotion.ObjectBinding.UnitTests.TestDomain.TestEnum'.",
                  "disabledValues"));
    }

    [Test]
    public void IsEnabled_WithFalse ()
    {
      var mockBusinessObject  =new Mock<IBusinessObject>(MockBehavior.Strict);
      var mockProperty = new Mock<IBusinessObjectEnumerationProperty>(MockBehavior.Strict);

      IEnumerationValueFilter filter = new ConstantEnumerationValueFilter(new Enum[] { TestEnum.Value1, TestEnum.Value4 });

      bool actual = filter.IsEnabled(new EnumerationValueInfo(TestEnum.Value1, "Value1", string.Empty, true), mockBusinessObject.Object, mockProperty.Object);

      mockBusinessObject.Verify();
      mockProperty.Verify();
      Assert.That(actual, Is.False);
    }

    [Test]
    public void IsEnabled_WithTrue ()
    {
      var mockBusinessObject = new Mock<IBusinessObject>(MockBehavior.Strict);
      var mockProperty = new Mock<IBusinessObjectEnumerationProperty>(MockBehavior.Strict);

      IEnumerationValueFilter filter = new ConstantEnumerationValueFilter(new Enum[] { TestEnum.Value1, TestEnum.Value4 });

      bool actual = filter.IsEnabled(new EnumerationValueInfo(TestEnum.Value2, "Value2", string.Empty, true), mockBusinessObject.Object, mockProperty.Object);

      mockBusinessObject.Verify();
      mockProperty.Verify();
      Assert.That(actual, Is.True);
    }
  }
}
