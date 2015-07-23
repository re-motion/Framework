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
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.BindableObject.ReferencePropertyTests
{
  [TestFixture]
  public class SupportsSearchAvailableObjects : TestBase
  {
    private MockRepository _mockRepository;
    private BindableObjectProvider _bindableObjectProviderForDeclaringType;
    private BindableObjectProvider _bindableObjectProviderForPropertyType;

    public override void SetUp ()
    {
      base.SetUp();

      _mockRepository = new MockRepository();
      _bindableObjectProviderForDeclaringType = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory ();
      _bindableObjectProviderForPropertyType = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory ();

      BusinessObjectProvider.SetProvider<BindableObjectProviderAttribute> (_bindableObjectProviderForDeclaringType);
      BusinessObjectProvider.SetProvider<BindableObjectWithIdentityProviderAttribute> (_bindableObjectProviderForPropertyType);
    }

    [Test]
    public void SearchServiceFromPropertyType ()
    {
      var serviceMock = _mockRepository.StrictMock<ISearchServiceOnType> ();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromPropertyType");

      Expect.Call (serviceMock.SupportsProperty (property)).Return (true);
      _mockRepository.ReplayAll();

      _bindableObjectProviderForPropertyType.AddService (serviceMock);
      bool actual = property.SupportsSearchAvailableObjects;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    public void SearchServiceFromPropertyDeclaration ()
    {
      var serviceMock = _mockRepository.StrictMock<ISearchServiceOnProperty>();
      var stubSearchServiceOnType = _mockRepository.StrictMock<ISearchServiceOnType>();
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromPropertyDeclaration");

      Expect.Call (serviceMock.SupportsProperty (property)).Return (true);
      _mockRepository.ReplayAll();

      _bindableObjectProviderForPropertyType.AddService (stubSearchServiceOnType);
      _bindableObjectProviderForDeclaringType.AddService (serviceMock);
      bool actual = property.SupportsSearchAvailableObjects;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    public void UnknownSearchService ()
    {
      IBusinessObjectReferenceProperty property = CreateProperty ("SearchServiceFromPropertyType");

      Assert.That (property.SupportsSearchAvailableObjects, Is.False);
    }

    [Test]
    public void WithoutSearchServiceAttribute_AndDefaultSearchService_FromPropertyDeclaration ()
    {
      var searchAvailableObjectsServiceMock = _mockRepository.StrictMock<ISearchAvailableObjectsService>();
      IBusinessObjectReferenceProperty property = CreatePropertyWithoutMixing ("NoSearchService");

      Expect.Call (searchAvailableObjectsServiceMock.SupportsProperty (property)).Return (true);
      _mockRepository.ReplayAll();

      _bindableObjectProviderForDeclaringType.AddService (searchAvailableObjectsServiceMock);
      bool actual = property.SupportsSearchAvailableObjects;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    [Ignore ("TODO RM-4105: Extend fallback behavior to include property type.")]
    public void WithoutSearchServiceAttribute_AndDefaultSearchService_FromPropertyType ()
    {
      var searchAvailableObjectsServiceMock = _mockRepository.StrictMock<ISearchAvailableObjectsService> ();
      var businessObjectClassServiceMock = _mockRepository.StrictMock<IBusinessObjectClassService> ();
      var businessObjectProviderMock = _mockRepository.StrictMock<IBusinessObjectProvider> ();
      var businessObjectClassWithIdentityMock = _mockRepository.StrictMock<IBusinessObjectClassWithIdentity> ();
      IBusinessObjectReferenceProperty property = CreatePropertyWithoutMixing ("NoSearchService");

      Expect.Call (businessObjectClassWithIdentityMock.BusinessObjectProvider).Return (businessObjectProviderMock).Repeat.Any ();
      Expect.Call (businessObjectProviderMock.GetService (typeof (ISearchAvailableObjectsService))).Return (searchAvailableObjectsServiceMock);
      Expect.Call (businessObjectClassServiceMock.GetBusinessObjectClass (typeof (ClassFromOtherBusinessObjectImplementation)))
          .Return (businessObjectClassWithIdentityMock);
      Expect.Call (searchAvailableObjectsServiceMock.SupportsProperty (property)).Return (true);
      _mockRepository.ReplayAll ();

      _bindableObjectProviderForDeclaringType.AddService (businessObjectClassServiceMock);
      bool actual = property.SupportsSearchAvailableObjects;

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.True);
    }

    [Test]
    public void WithoutSearchServiceAttribute_AndNoDefaultSearchService_FromPropertyDeclaration ()
    {
      IBusinessObjectReferenceProperty property = CreatePropertyWithoutMixing ("NoSearchService");

      _mockRepository.ReplayAll ();

      bool actual = property.SupportsSearchAvailableObjects;

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.False);
    }

    [Test]
    [Ignore ("TODO RM-4105: Extend fallback behavior to include property type.")]
    public void WithoutSearchServiceAttribute_AndNoDefaultSearchService_FromPropertyType ()
    {
      IBusinessObjectClassService businessObjectClassServiceMock = _mockRepository.StrictMock<IBusinessObjectClassService> ();
      IBusinessObjectProvider businessObjectProviderMock = _mockRepository.StrictMock<IBusinessObjectProvider> ();
      IBusinessObjectClassWithIdentity businessObjectClassWithIdentityMock = _mockRepository.StrictMock<IBusinessObjectClassWithIdentity> ();
      IBusinessObjectReferenceProperty property = CreatePropertyWithoutMixing ("NoSearchService");

      Expect.Call (businessObjectClassWithIdentityMock.BusinessObjectProvider).Return (businessObjectProviderMock).Repeat.Any ();
      Expect.Call (businessObjectProviderMock.GetService (typeof (ISearchAvailableObjectsService))).Return (null);
      Expect.Call (businessObjectClassServiceMock.GetBusinessObjectClass (typeof (ClassFromOtherBusinessObjectImplementation)))
          .Return (businessObjectClassWithIdentityMock);
      _mockRepository.ReplayAll ();
      
      _bindableObjectProviderForDeclaringType.AddService (businessObjectClassServiceMock);

      _mockRepository.VerifyAll ();
      Assert.That (property.SupportsSearchAvailableObjects, Is.False);
    }

    private ReferenceProperty CreateProperty (string propertyName)
    {
      PropertyBase.Parameters propertyParameters = GetPropertyParameters (propertyName);
      return new ReferenceProperty (propertyParameters);
    }

    private ReferenceProperty CreatePropertyWithoutMixing (string propertyName)
    {
      PropertyBase.Parameters propertyParameters;
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        propertyParameters = GetPropertyParameters (propertyName);
      }
      return new ReferenceProperty (propertyParameters);
    }

    private PropertyBase.Parameters GetPropertyParameters (string propertyName)
    {
      return GetPropertyParameters (GetPropertyInfo (typeof (ClassWithBusinessObjectProperties), propertyName), _bindableObjectProviderForDeclaringType);
    }
  }
}
