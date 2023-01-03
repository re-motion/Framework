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
#pragma warning disable 612,618
  [TestFixture]
  public class SupportsDefaultValue : TestBase
  {
    private BindableObjectProvider _bindableObjectProviderForDeclaringType;
    private BindableObjectProvider _bindableObjectProviderForPropertyType;

    public override void SetUp ()
    {
      base.SetUp();

      _bindableObjectProviderForDeclaringType = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory();
      _bindableObjectProviderForPropertyType = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory();

      BusinessObjectProvider.SetProvider<BindableObjectProviderAttribute>(_bindableObjectProviderForDeclaringType);
      BusinessObjectProvider.SetProvider<BindableObjectProviderForDefaultValueServiceAttribute>(_bindableObjectProviderForPropertyType);
    }

    [Test]
    public void DefaultValueServiceFromPropertyType ()
    {
      var serviceMock = new Mock<IDefaultValueServiceOnType>(MockBehavior.Strict);
      IBusinessObjectReferenceProperty property = CreateProperty("DefaultValueServiceFromPropertyType");

      serviceMock.Setup(_ => _.SupportsProperty(property)).Returns(true).Verifiable();

      _bindableObjectProviderForPropertyType.AddService(serviceMock.Object);
      bool actual = property.SupportsDefaultValue;

      serviceMock.Verify();
      Assert.That(actual, Is.True);
    }

    [Test]
    public void DefaultValueServiceFromPropertyDeclaration ()
    {
      var serviceMock = new Mock<IDefaultValueServiceOnProperty>(MockBehavior.Strict);
      var createObjectServiceOnTypeStub = new Mock<IDefaultValueServiceOnType>(MockBehavior.Strict);
      IBusinessObjectReferenceProperty property = CreateProperty("DefaultValueServiceFromPropertyDeclaration");

      serviceMock.Setup(_ => _.SupportsProperty(property)).Returns(true).Verifiable();

      _bindableObjectProviderForPropertyType.AddService(createObjectServiceOnTypeStub.Object);
      _bindableObjectProviderForDeclaringType.AddService(serviceMock.Object);
      bool actual = property.SupportsDefaultValue;

      serviceMock.Verify();
      createObjectServiceOnTypeStub.Verify();
      Assert.That(actual, Is.True);
    }

    [Test]
    public void UnknownDefaultValueService ()
    {
      IBusinessObjectReferenceProperty property = CreateProperty("DefaultValueServiceFromPropertyType");

      Assert.That(property.SupportsDefaultValue, Is.False);
    }

    [Test]
    public void WithoutDefaultValueServiceAttribute_AndDefaultDefaultValueService_FromPropertyDeclaration ()
    {
      var createObjectServiceMock = new Mock<IDefaultValueService>(MockBehavior.Strict);
      IBusinessObjectReferenceProperty property = CreatePropertyWithoutMixing("NoDefaultValueService");

      createObjectServiceMock.Setup(_ => _.SupportsProperty(property)).Returns(true).Verifiable();

      _bindableObjectProviderForDeclaringType.AddService(createObjectServiceMock.Object);
      bool actual = property.SupportsDefaultValue;

      createObjectServiceMock.Verify();
      Assert.That(actual, Is.True);
    }

    [Test]
    [Ignore("TODO RM-4106: Extend fallback behavior to include property type.")]
    public void WithoutDefaultValueServiceAttribute_AndDefaultDefaultValueService_FromPropertyType ()
    {
      var createObjectServiceMock = new Mock<IDefaultValueService>(MockBehavior.Strict);
      var businessObjectClassServiceMock = new Mock<IBusinessObjectClassService>(MockBehavior.Strict);
      var businessObjectProviderMock = new Mock<IBusinessObjectProvider>(MockBehavior.Strict);
      var businessObjectClassWithIdentityMock = new Mock<IBusinessObjectClassWithIdentity>(MockBehavior.Strict);
      IBusinessObjectReferenceProperty property = CreatePropertyWithoutMixing("NoDefaultValueService");

      businessObjectClassWithIdentityMock.Setup(_ => _.BusinessObjectProvider).Returns(businessObjectProviderMock.Object).Verifiable();
      businessObjectProviderMock.Setup(_ => _.GetService(typeof(IDefaultValueService))).Returns(createObjectServiceMock.Object).Verifiable();
      businessObjectClassServiceMock
          .Setup(_ => _.GetBusinessObjectClass(typeof(ClassFromOtherBusinessObjectImplementation)))
          .Returns(businessObjectClassWithIdentityMock.Object)
          .Verifiable();
      createObjectServiceMock.Setup(_ => _.SupportsProperty(property)).Returns(true).Verifiable();

      _bindableObjectProviderForDeclaringType.AddService(businessObjectClassServiceMock.Object);
      bool actual = property.SupportsDefaultValue;

      createObjectServiceMock.Verify();
      businessObjectClassServiceMock.Verify();
      businessObjectProviderMock.Verify();
      businessObjectClassWithIdentityMock.Verify();
      Assert.That(actual, Is.True);
    }

    [Test]
    public void WithoutDefaultValueServiceAttribute_AndNoDefaultDefaultValueService_FromPropertyDeclaration ()
    {
      IBusinessObjectReferenceProperty property = CreatePropertyWithoutMixing("NoDefaultValueService");

      bool actual = property.SupportsDefaultValue;
      Assert.That(actual, Is.False);
    }

    [Test]
    [Ignore("TODO RM-4106: Extend fallback behavior to include property type.")]
    public void WithoutDefaultValueServiceAttribute_AndNoDefaultDefaultValueService_FromPropertyType ()
    {
      var businessObjectClassServiceMock = new Mock<IBusinessObjectClassService>(MockBehavior.Strict);
      var businessObjectProviderMock = new Mock<IBusinessObjectProvider>(MockBehavior.Strict);
      var businessObjectClassWithIdentityMock = new Mock<IBusinessObjectClassWithIdentity>(MockBehavior.Strict);
      IBusinessObjectReferenceProperty property = CreatePropertyWithoutMixing("NoDefaultValueService");

      businessObjectClassWithIdentityMock.Setup(_ => _.BusinessObjectProvider).Returns(businessObjectProviderMock.Object).Verifiable();
      businessObjectProviderMock.Setup(_ => _.GetService(typeof(IDefaultValueService))).Returns((IBusinessObjectService)null).Verifiable();
      businessObjectClassServiceMock
          .Setup(_ => _.GetBusinessObjectClass(typeof(ClassFromOtherBusinessObjectImplementation)))
          .Returns(businessObjectClassWithIdentityMock.Object)
          .Verifiable();

      _bindableObjectProviderForDeclaringType.AddService(businessObjectClassServiceMock.Object);

      businessObjectClassServiceMock.Verify();
      businessObjectProviderMock.Verify();
      businessObjectClassWithIdentityMock.Verify();
      Assert.That(property.SupportsDefaultValue, Is.False);
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
#pragma warning restore 612,618
}
