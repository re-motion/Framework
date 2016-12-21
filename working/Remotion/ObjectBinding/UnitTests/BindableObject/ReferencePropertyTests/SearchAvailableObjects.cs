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
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.BindableObject.ReferencePropertyTests.TestDomain;
using Remotion.TypePipe;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.BindableObject.ReferencePropertyTests
{
  [TestFixture]
  public class SearchAvailableObjects : TestBase
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
      BusinessObjectProvider.SetProvider<BindableObjectWithIdentityProviderAttribute> (_bindableObjectProviderForPropertyType);
    }

    [Test]
    public void Search_WithSearchSupported ()
    {
      IBusinessObject stubBusinessObject = _mockRepository.Stub<IBusinessObject>();
      ISearchServiceOnProperty mockService = _mockRepository.StrictMock<ISearchServiceOnProperty>();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromPropertyDeclaration");
      IBusinessObject[] expected = new IBusinessObject[0];
      ISearchAvailableObjectsArguments searchArgumentsStub = _mockRepository.Stub<ISearchAvailableObjectsArguments>();

      using (_mockRepository.Ordered())
      {
        Expect.Call (mockService.SupportsProperty (property)).Return (true);
        Expect.Call (mockService.Search (stubBusinessObject, property, searchArgumentsStub)).Return (expected);
      }
      _mockRepository.ReplayAll();

      _bindableObjectProviderForDeclaringType.AddService (mockService);
      IBusinessObject[] actual = property.SearchAvailableObjects (stubBusinessObject, searchArgumentsStub);

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.SameAs (expected));
    }

    [Test]
    public void Search_WithSearchSupportedAndReferencingObjectNull ()
    {
      ISearchServiceOnType mockService = _mockRepository.StrictMock<ISearchServiceOnType>();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromPropertyType");
      IBusinessObject[] expected = new IBusinessObject[0];
      ISearchAvailableObjectsArguments searchArgumentsStub = _mockRepository.Stub<ISearchAvailableObjectsArguments>();

      using (_mockRepository.Ordered())
      {
        Expect.Call (mockService.SupportsProperty (property)).Return (true);
        Expect.Call (mockService.Search (null, property, searchArgumentsStub)).Return (expected);
      }
      _mockRepository.ReplayAll();

      _bindableObjectProviderForPropertyType.AddService (mockService);
      IBusinessObject[] actual = property.SearchAvailableObjects (null, searchArgumentsStub);

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.SameAs (expected));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage =
        "Searching is not supported for reference property 'SearchServiceFromPropertyDeclaration' of business object class "
        + "'Remotion.ObjectBinding.UnitTests.BindableObject.ReferencePropertyTests.TestDomain.ClassWithBusinessObjectProperties, "
        + "Remotion.ObjectBinding.UnitTests'.")]
    public void Search_WithSearchNotSupported ()
    {
      IBusinessObject businessObject = (IBusinessObject) ObjectFactory.Create<ClassWithBusinessObjectProperties> (ParamList.Empty);
      ISearchServiceOnProperty mockService = _mockRepository.StrictMock<ISearchServiceOnProperty>();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromPropertyDeclaration");
      ISearchAvailableObjectsArguments searchArgumentsStubb = _mockRepository.Stub<ISearchAvailableObjectsArguments>();

      Expect.Call (mockService.SupportsProperty (property)).Return (false);
      _mockRepository.ReplayAll();

      _bindableObjectProviderForDeclaringType.AddService (mockService);
      try
      {
        property.SearchAvailableObjects (businessObject, searchArgumentsStubb);
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
}