﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.BindableObject.ReferencePropertyTests.TestDomain;
using Remotion.TypePipe;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.BindableObject.ReferencePropertyTests
{
#pragma warning disable 612,618
  [TestFixture]
  public class IsDefaultValue : TestBase
  {
    private MockRepository _mockRepository;
    private BindableObjectProvider _bindableObjectProviderForDeclaringType;
    private BindableObjectProvider _bindableObjectProviderForPropertyType;

    public override void SetUp ()
    {
      base.SetUp();

      _mockRepository = new MockRepository();
      _bindableObjectProviderForDeclaringType = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory();
      _bindableObjectProviderForPropertyType = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory();

      BusinessObjectProvider.SetProvider<BindableObjectProviderAttribute> (_bindableObjectProviderForDeclaringType);
      BusinessObjectProvider.SetProvider<BindableObjectProviderForDefaultValueServiceAttribute> (_bindableObjectProviderForPropertyType);
    }

    [Test]
    public void IsDefaultValue_WithDefaultValueSupported ()
    {
      var stubBusinessObject = _mockRepository.Stub<IBusinessObject>();
      var mockService = _mockRepository.StrictMock<IDefaultValueServiceOnProperty>();
      IBusinessObjectReferenceProperty property = CreateProperty ("DefaultValueServiceFromPropertyDeclaration");
      var value = _mockRepository.Stub<IBusinessObject> ();
      var emptyProperties = new IBusinessObjectProperty[0];

      using (_mockRepository.Ordered())
      {
        Expect.Call (mockService.SupportsProperty (property)).Return (true);
        Expect.Call (mockService.IsDefaultValue (stubBusinessObject, property, value, emptyProperties)).Return (true);
      }
      _mockRepository.ReplayAll();

      _bindableObjectProviderForDeclaringType.AddService (mockService);
      bool actual = property.IsDefaultValue (stubBusinessObject, value, emptyProperties);

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    public void IsDefaultValue_WithDefaultValueSupportedAndReferencingObjectNull ()
    {
      var mockService = _mockRepository.StrictMock<IDefaultValueServiceOnType>();
      var property = CreateProperty ("DefaultValueServiceFromPropertyType");
      var value = _mockRepository.Stub<IBusinessObject> ();
      var emptyProperties = new IBusinessObjectProperty[0];

      using (_mockRepository.Ordered())
      {
        Expect.Call (mockService.SupportsProperty (property)).Return (true);
        Expect.Call (mockService.IsDefaultValue (null, property, value, emptyProperties)).Return (true);
      }
      _mockRepository.ReplayAll();

      _bindableObjectProviderForPropertyType.AddService (mockService);
      bool actual = property.IsDefaultValue (null, value, emptyProperties);

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage =
        "Checking for a value's default is not supported for reference property 'DefaultValueServiceFromPropertyDeclaration' of business object class "
        + "'Remotion.ObjectBinding.UnitTests.BindableObject.ReferencePropertyTests.TestDomain.ClassWithBusinessObjectProperties, "
        + "Remotion.ObjectBinding.UnitTests'.")]
    public void IsDefaultValue_WithDefaultValueNotSupported ()
    {
      IBusinessObject businessObject = (IBusinessObject) ObjectFactory.Create<ClassWithBusinessObjectProperties> (ParamList.Empty);
      var mockService = _mockRepository.StrictMock<IDefaultValueServiceOnProperty>();
      IBusinessObjectReferenceProperty property = CreateProperty ("DefaultValueServiceFromPropertyDeclaration");
      var value = _mockRepository.Stub<IBusinessObject> ();
      var emptyProperties = new IBusinessObjectProperty[0];

      Expect.Call (mockService.SupportsProperty (property)).Return (false);
      _mockRepository.ReplayAll();

      _bindableObjectProviderForDeclaringType.AddService (mockService);
      try
      {
        property.IsDefaultValue (businessObject, value, emptyProperties);
      }
      finally
      {
        _mockRepository.VerifyAll();
      }
    }

    private ReferenceProperty CreateProperty (string propertyName)
    {
      PropertyBase.Parameters propertyParameters =
          GetPropertyParameters (GetPropertyInfo (typeof (ClassWithBusinessObjectProperties), propertyName), _bindableObjectProviderForDeclaringType);
      ReferenceProperty property = new ReferenceProperty (propertyParameters);
      property.SetReflectedClass (BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (ClassWithBusinessObjectProperties)));

      return property;
    }
  }
#pragma warning restore 612,618
}