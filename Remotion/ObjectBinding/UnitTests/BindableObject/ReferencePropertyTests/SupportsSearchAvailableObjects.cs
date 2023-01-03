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
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.BindableObject.ReferencePropertyTests.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.BindableObject.ReferencePropertyTests
{
  [TestFixture]
  public class SupportsSearchAvailableObjects : TestBase
  {
    private BindableObjectProvider _bindableObjectProviderForDeclaringType;
    private BindableObjectProvider _bindableObjectProviderForPropertyType;

    public override void SetUp ()
    {
      base.SetUp();

      _bindableObjectProviderForDeclaringType = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory();
      _bindableObjectProviderForPropertyType = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory();

      BusinessObjectProvider.SetProvider<BindableObjectProviderAttribute>(_bindableObjectProviderForDeclaringType);
      BusinessObjectProvider.SetProvider<BindableObjectWithIdentityProviderAttribute>(_bindableObjectProviderForPropertyType);
    }

    [Test]
    public void SearchServiceFromPropertyType ()
    {
      var serviceMock = new Mock<ISearchServiceOnType>(MockBehavior.Strict);
      IBusinessObjectReferenceProperty property = CreateProperty("SearchServiceFromPropertyType");

      serviceMock.Setup(_ => _.SupportsProperty(property)).Returns(true).Verifiable();

      _bindableObjectProviderForPropertyType.AddService(serviceMock.Object);
      bool actual = property.SupportsSearchAvailableObjects;

      serviceMock.Verify();
      Assert.That(actual, Is.True);
    }

    [Test]
    public void SearchServiceFromPropertyDeclaration ()
    {
      var serviceMock = new Mock<ISearchServiceOnProperty>(MockBehavior.Strict);
      var stubSearchServiceOnType = new Mock<ISearchServiceOnType>(MockBehavior.Strict);
      IBusinessObjectReferenceProperty property = CreateProperty("SearchServiceFromPropertyDeclaration");

      serviceMock.Setup(_ => _.SupportsProperty(property)).Returns(true).Verifiable();

      _bindableObjectProviderForPropertyType.AddService(stubSearchServiceOnType.Object);
      _bindableObjectProviderForDeclaringType.AddService(serviceMock.Object);
      bool actual = property.SupportsSearchAvailableObjects;

      serviceMock.Verify();
      stubSearchServiceOnType.Verify();
      Assert.That(actual, Is.True);
    }

    [Test]
    public void UnknownSearchService ()
    {
      IBusinessObjectReferenceProperty property = CreateProperty("SearchServiceFromPropertyType");

      Assert.That(property.SupportsSearchAvailableObjects, Is.False);
    }

    [Test]
    public void WithoutSearchServiceAttribute_AndDefaultSearchService_FromPropertyDeclaration ()
    {
      var searchAvailableObjectsServiceMock = new Mock<ISearchAvailableObjectsService>(MockBehavior.Strict);
      IBusinessObjectReferenceProperty property = CreatePropertyWithoutMixing("NoSearchService");

      searchAvailableObjectsServiceMock.Setup(_ => _.SupportsProperty(property)).Returns(true).Verifiable();

      _bindableObjectProviderForDeclaringType.AddService(searchAvailableObjectsServiceMock.Object);
      bool actual = property.SupportsSearchAvailableObjects;

      searchAvailableObjectsServiceMock.Verify();
      Assert.That(actual, Is.True);
    }

    [Test]
    [Ignore("TODO RM-4105: Extend fallback behavior to include property type.")]
    public void WithoutSearchServiceAttribute_AndDefaultSearchService_FromPropertyType ()
    {
      var searchAvailableObjectsServiceMock = new Mock<ISearchAvailableObjectsService>(MockBehavior.Strict);
      var businessObjectClassServiceMock = new Mock<IBusinessObjectClassService>(MockBehavior.Strict);
      var businessObjectProviderMock = new Mock<IBusinessObjectProvider>(MockBehavior.Strict);
      var businessObjectClassWithIdentityMock = new Mock<IBusinessObjectClassWithIdentity>(MockBehavior.Strict);
      IBusinessObjectReferenceProperty property = CreatePropertyWithoutMixing("NoSearchService");

      businessObjectClassWithIdentityMock.Setup(_ => _.BusinessObjectProvider).Returns(businessObjectProviderMock.Object).Verifiable();
      businessObjectProviderMock.Setup(_ => _.GetService(typeof(ISearchAvailableObjectsService))).Returns(searchAvailableObjectsServiceMock.Object).Verifiable();
      businessObjectClassServiceMock
          .Setup(_ => _.GetBusinessObjectClass(typeof(ClassFromOtherBusinessObjectImplementation)))
          .Returns(businessObjectClassWithIdentityMock.Object)
          .Verifiable();
      searchAvailableObjectsServiceMock.Setup(_ => _.SupportsProperty(property)).Returns(true).Verifiable();

      _bindableObjectProviderForDeclaringType.AddService(businessObjectClassServiceMock.Object);
      bool actual = property.SupportsSearchAvailableObjects;

      searchAvailableObjectsServiceMock.Verify();
      businessObjectClassServiceMock.Verify();
      businessObjectProviderMock.Verify();
      businessObjectClassWithIdentityMock.Verify();
      Assert.That(actual, Is.True);
    }

    [Test]
    public void WithoutSearchServiceAttribute_AndNoDefaultSearchService_FromPropertyDeclaration ()
    {
      IBusinessObjectReferenceProperty property = CreatePropertyWithoutMixing("NoSearchService");

      bool actual = property.SupportsSearchAvailableObjects;
      Assert.That(actual, Is.False);
    }

    [Test]
    [Ignore("TODO RM-4105: Extend fallback behavior to include property type.")]
    public void WithoutSearchServiceAttribute_AndNoDefaultSearchService_FromPropertyType ()
    {
      var businessObjectClassServiceMock = new Mock<IBusinessObjectClassService>(MockBehavior.Strict);
      var businessObjectProviderMock = new Mock<IBusinessObjectProvider>(MockBehavior.Strict);
      var businessObjectClassWithIdentityMock = new Mock<IBusinessObjectClassWithIdentity>(MockBehavior.Strict);
      IBusinessObjectReferenceProperty property = CreatePropertyWithoutMixing("NoSearchService");

      businessObjectClassWithIdentityMock.Setup(_ => _.BusinessObjectProvider).Returns(businessObjectProviderMock.Object).Verifiable();
      businessObjectProviderMock.Setup(_ => _.GetService(typeof(ISearchAvailableObjectsService))).Returns((IBusinessObjectService)null).Verifiable();
      businessObjectClassServiceMock
          .Setup(_ => _.GetBusinessObjectClass(typeof(ClassFromOtherBusinessObjectImplementation)))
          .Returns(businessObjectClassWithIdentityMock.Object)
          .Verifiable();

      _bindableObjectProviderForDeclaringType.AddService(businessObjectClassServiceMock.Object);

      businessObjectClassServiceMock.Verify();
      businessObjectProviderMock.Verify();
      businessObjectClassWithIdentityMock.Verify();
      Assert.That(property.SupportsSearchAvailableObjects, Is.False);
    }

    private ReferenceProperty CreateProperty (string propertyName)
    {
      PropertyBase.Parameters propertyParameters = GetPropertyParameters(propertyName);
      return new ReferenceProperty(propertyParameters);
    }

    private ReferenceProperty CreatePropertyWithoutMixing (string propertyName)
    {
      PropertyBase.Parameters propertyParameters;
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        propertyParameters = GetPropertyParameters(propertyName);
      }
      return new ReferenceProperty(propertyParameters);
    }

    private PropertyBase.Parameters GetPropertyParameters (string propertyName)
    {
      return GetPropertyParameters(GetPropertyInfo(typeof(ClassWithBusinessObjectProperties), propertyName), _bindableObjectProviderForDeclaringType);
    }
  }
}
