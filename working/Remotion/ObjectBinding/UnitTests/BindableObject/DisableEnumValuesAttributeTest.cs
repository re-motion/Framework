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
using NUnit.Framework;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class DisableEnumValuesAttributeTest
  {
    private class StubEnumerationValueFilter:IEnumerationValueFilter
    {
      public bool IsEnabled (IEnumerationValueInfo value, IBusinessObject businessObject, IBusinessObjectEnumerationProperty property)
      {
        throw new NotImplementedException();
      }
    }

    [Test]
    public void GetEnumerationValueFilter_FromFilterTypeCtor()
    {
      DisableEnumValuesAttribute attribute = new DisableEnumValuesAttribute (typeof (StubEnumerationValueFilter));

      Assert.That (attribute.GetEnumerationValueFilter (), Is.TypeOf (typeof (StubEnumerationValueFilter)));
    }

    [Test]
    public void GetEnumerationValueFilter_FromEnumValueCtorWithArray ()
    {
      DisableEnumValuesAttribute attribute = new DisableEnumValuesAttribute (new object[] {TestEnum.Value1, TestEnum.Value3});

      CheckConstantEnumerationValueFilter (attribute.GetEnumerationValueFilter(), TestEnum.Value1, TestEnum.Value3);
    }

    [Test]
    public void GetEnumerationValueFilter_FromEnumValueCtorWithOneParameter ()
    {
      DisableEnumValuesAttribute attribute = new DisableEnumValuesAttribute (TestEnum.Value1);

      CheckConstantEnumerationValueFilter (attribute.GetEnumerationValueFilter(), TestEnum.Value1);
    }

    [Test]
    public void GetEnumerationValueFilter_FromEnumValueCtorWithTwoParameters ()
    {
      DisableEnumValuesAttribute attribute = new DisableEnumValuesAttribute (TestEnum.Value1, TestEnum.Value2);

      CheckConstantEnumerationValueFilter (attribute.GetEnumerationValueFilter (), TestEnum.Value1, TestEnum.Value2);
    }

    [Test]
    public void GetEnumerationValueFilter_FromEnumValueCtorWithThreeParameters ()
    {
      DisableEnumValuesAttribute attribute = new DisableEnumValuesAttribute (TestEnum.Value1, TestEnum.Value2, TestEnum.Value3);

      CheckConstantEnumerationValueFilter (attribute.GetEnumerationValueFilter (), TestEnum.Value1, TestEnum.Value2, TestEnum.Value3);
    }

    [Test]
    public void GetEnumerationValueFilter_FromEnumValueCtorWithFourParameters ()
    {
      DisableEnumValuesAttribute attribute = new DisableEnumValuesAttribute (TestEnum.Value1, TestEnum.Value2, TestEnum.Value3, TestEnum.Value4);

      CheckConstantEnumerationValueFilter (attribute.GetEnumerationValueFilter(), TestEnum.Value1, TestEnum.Value2, TestEnum.Value3, TestEnum.Value4);
    }

    [Test]
    public void GetEnumerationValueFilter_FromEnumValueCtorWithFiveParameters ()
    {
      DisableEnumValuesAttribute attribute =
          new DisableEnumValuesAttribute (TestEnum.Value1, TestEnum.Value2, TestEnum.Value3, TestEnum.Value4, TestEnum.Value5);

      CheckConstantEnumerationValueFilter (
          attribute.GetEnumerationValueFilter(), TestEnum.Value1, TestEnum.Value2, TestEnum.Value3, TestEnum.Value4, TestEnum.Value5);
    }

    private void CheckConstantEnumerationValueFilter (IEnumerationValueFilter filter, params Enum[] expectedDisabledEnumValues)
    {
      Assert.That (filter, Is.TypeOf (typeof (ConstantEnumerationValueFilter)));
      Assert.That (((ConstantEnumerationValueFilter) filter).DisabledEnumValues, Is.EqualTo (expectedDisabledEnumValues));
    }
  }
}
