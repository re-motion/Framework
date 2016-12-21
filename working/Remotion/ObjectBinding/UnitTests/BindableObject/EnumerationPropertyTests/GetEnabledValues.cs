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
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.BindableObject.EnumerationPropertyTests
{
  [TestFixture]
  public class GetEnabledValues : EnumerationTestBase
  {
    private BindableObjectProvider _businessObjectProvider;

    private MockRepository _mockRepository;

    public override void SetUp ()
    {
      base.SetUp();

      _businessObjectProvider = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory ();

      _mockRepository = new MockRepository();
      _mockRepository.StrictMock<IBusinessObject>();
    }

    [Test]
    public void DisableFromProperty ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithDisabledEnumValue), "DisabledFromProperty");
      EnumerationValueInfo[] expected = new[]
          {
              new EnumerationValueInfo (TestEnum.Value2, "Value2", "Value2", true),
              new EnumerationValueInfo (TestEnum.Value3, "Value3", "Value3", true),
              new EnumerationValueInfo (TestEnum.Value4, "Value4", "Value4", true),
              new EnumerationValueInfo (TestEnum.Value5, "Value5", "Value5", true)
          };

      CheckEnumerationValueInfos (expected, property.GetEnabledValues (null));
    }

    [Test]
    public void DisableFromPropertyType ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithDisabledEnumValue), "DisabledFromPropertyType");
      EnumerationValueInfo[] expected = new[]
          {
              new EnumerationValueInfo (TestEnum.Value1, "Value1", "Value1", true),
              new EnumerationValueInfo (TestEnum.Value2, "Value2", "Value2", true),
              new EnumerationValueInfo (TestEnum.Value3, "Value3", "Value3", true),
              new EnumerationValueInfo (TestEnum.Value4, "Value4", "Value4", true)
          };

      CheckEnumerationValueInfos (expected, property.GetEnabledValues (null));
    }

    [Test]
    public void DisableWithBusinessObject ()
    {
      IBusinessObjectEnumerationProperty property = CreateProperty (typeof (ClassWithDisabledEnumValue), "DisabledWithObject");
      EnumerationValueInfo[] expected = new[]
          {
              new EnumerationValueInfo (TestEnum.Value1, "Value1", "Value1", true),
              new EnumerationValueInfo (TestEnum.Value3, "Value3", "Value3", true),
              new EnumerationValueInfo (TestEnum.Value5, "Value5", "Value5", true),
          };

      CheckEnumerationValueInfos (expected, property.GetEnabledValues (MockRepository.GenerateStub<IBusinessObject>()));
    }

    private EnumerationProperty CreateProperty (Type type, string propertyName)
    {
      return new EnumerationProperty (
        GetPropertyParameters (GetPropertyInfo (type, propertyName), _businessObjectProvider));
    }
  }
}
