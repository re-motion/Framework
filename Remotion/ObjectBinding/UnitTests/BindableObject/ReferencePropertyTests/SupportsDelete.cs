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
  public class SupportsDelete : TestBase
  {
    private BindableObjectProvider _bindableObjectProviderForDeclaringType;
    private BindableObjectProvider _bindableObjectProviderForPropertyType;

    public override void SetUp ()
    {
      base.SetUp();

      _bindableObjectProviderForDeclaringType = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory();
      _bindableObjectProviderForPropertyType = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory();

      BusinessObjectProvider.SetProvider<BindableObjectProviderAttribute>(_bindableObjectProviderForDeclaringType);
      BusinessObjectProvider.SetProvider<BindableObjectProviderForDeleteObjectServiceAttribute>(_bindableObjectProviderForPropertyType);
    }

    [Test]
    public void DeleteObjectServiceFromPropertyType ()
    {
      var serviceMock = new Mock<IDeleteObjectServiceOnType>(MockBehavior.Strict);
      IBusinessObjectReferenceProperty property = DeleteProperty("DeleteObjectServiceFromPropertyType");

      serviceMock.Setup(_ => _.SupportsProperty(property)).Returns(true).Verifiable();

      _bindableObjectProviderForPropertyType.AddService(serviceMock.Object);
      bool actual = property.SupportsDelete;

      serviceMock.Verify();
      Assert.That(actual, Is.True);
    }

    [Test]
    public void DeleteObjectServiceFromPropertyDeclaration ()
    {
      var serviceMock = new Mock<IDeleteObjectServiceOnProperty>(MockBehavior.Strict);
      var createObjectServiceOnTypeStub = new Mock<IDeleteObjectServiceOnType>(MockBehavior.Strict);
      IBusinessObjectReferenceProperty property = DeleteProperty("DeleteObjectServiceFromPropertyDeclaration");

      serviceMock.Setup(_ => _.SupportsProperty(property)).Returns(true).Verifiable();

      _bindableObjectProviderForPropertyType.AddService(createObjectServiceOnTypeStub.Object);
      _bindableObjectProviderForDeclaringType.AddService(serviceMock.Object);
      bool actual = property.SupportsDelete;

      serviceMock.Verify();
      createObjectServiceOnTypeStub.Verify();
      Assert.That(actual, Is.True);
    }

    [Test]
    public void UnknownDeleteObjectService ()
    {
      IBusinessObjectReferenceProperty property = DeleteProperty("DeleteObjectServiceFromPropertyType");

      Assert.That(property.SupportsDelete, Is.False);
    }

    [Test]
    public void WithoutDeleteObjectServiceAttribute_AndDefaultDeleteObjectService_FromPropertyDeclaration ()
    {
      var deleteObjectServiceMock = new Mock<IDeleteObjectService>(MockBehavior.Strict);
      IBusinessObjectReferenceProperty property = DeletePropertyWithoutMixing("NoDeleteObjectService");

      deleteObjectServiceMock.Setup(_ => _.SupportsProperty(property)).Returns(true).Verifiable();

      _bindableObjectProviderForDeclaringType.AddService(deleteObjectServiceMock.Object);
      bool actual = property.SupportsDelete;

      deleteObjectServiceMock.Verify();
      Assert.That(actual, Is.True);
    }

    [Test]
    [Ignore ("TODO RM-4144: Extend fallback behavior to include property type.")]
    public void WithoutDeleteObjectServiceAttribute_AndDefaultDeleteObjectService_FromPropertyType ()
    {
      var deleteObjectServiceMock = new Mock<IDeleteObjectService>(MockBehavior.Strict);
      var businessObjectClassServiceMock = new Mock<IBusinessObjectClassService>(MockBehavior.Strict);
      var businessObjectProviderMock = new Mock<IBusinessObjectProvider>(MockBehavior.Strict);
      var businessObjectClassWithIdentityMock = new Mock<IBusinessObjectClassWithIdentity>(MockBehavior.Strict);
      IBusinessObjectReferenceProperty property = DeletePropertyWithoutMixing("NoDeleteObjectService");

      businessObjectClassWithIdentityMock.Setup(_ => _.BusinessObjectProvider).Returns(businessObjectProviderMock.Object).Verifiable();
      businessObjectProviderMock.Setup(_ => _.GetService(typeof(IDeleteObjectService))).Returns(deleteObjectServiceMock.Object).Verifiable();
      businessObjectClassServiceMock.Setup(_ => _.GetBusinessObjectClass(typeof(ClassFromOtherBusinessObjectImplementation)))
          .Returns(businessObjectClassWithIdentityMock.Object)
          .Verifiable();
      deleteObjectServiceMock.Setup(_ => _.SupportsProperty(property)).Returns(true).Verifiable();

      _bindableObjectProviderForDeclaringType.AddService(businessObjectClassServiceMock.Object);
      bool actual = property.SupportsDelete;

      deleteObjectServiceMock.Verify();
      businessObjectClassServiceMock.Verify();
      businessObjectProviderMock.Verify();
      businessObjectClassWithIdentityMock.Verify();
      Assert.That(actual, Is.True);
    }

    [Test]
    public void WithoutDeleteObjectServiceAttribute_AndNoDefaultDeleteObjectService_FromPropertyDeclaration ()
    {
      IBusinessObjectReferenceProperty property = DeletePropertyWithoutMixing("NoDeleteObjectService");

      bool actual = property.SupportsDelete;

      Assert.That(actual, Is.False);
    }

    [Test]
    [Ignore ("TODO RM-4144: Extend fallback behavior to include property type.")]
    public void WithoutDeleteObjectServiceAttribute_AndNoDefaultDeleteObjectService_FromPropertyType ()
    {
      var businessObjectClassServiceMock = new Mock<IBusinessObjectClassService>(MockBehavior.Strict);
      var businessObjectProviderMock = new Mock<IBusinessObjectProvider>(MockBehavior.Strict);
      var businessObjectClassWithIdentityMock = new Mock<IBusinessObjectClassWithIdentity>(MockBehavior.Strict);
      IBusinessObjectReferenceProperty property = DeletePropertyWithoutMixing("NoDeleteObjectService");

      businessObjectClassWithIdentityMock.Setup(_ => _.BusinessObjectProvider).Returns(businessObjectProviderMock.Object).Verifiable();
      businessObjectProviderMock.Setup(_ => _.GetService(typeof(IDeleteObjectService))).Returns((IBusinessObjectService)null).Verifiable();
      businessObjectClassServiceMock.Setup(_ => _.GetBusinessObjectClass(typeof(ClassFromOtherBusinessObjectImplementation)))
          .Returns(businessObjectClassWithIdentityMock.Object)
          .Verifiable();

      _bindableObjectProviderForDeclaringType.AddService(businessObjectClassServiceMock.Object);

      businessObjectClassServiceMock.Verify();
      businessObjectProviderMock.Verify();
      businessObjectClassWithIdentityMock.Verify();
      Assert.That(property.SupportsDelete, Is.False);
    }

    private ReferenceProperty DeleteProperty (string propertyName)
    {
      PropertyBase.Parameters propertyParameters = GetPropertyParameters(propertyName);
      return new ReferenceProperty(propertyParameters);
    }

    private ReferenceProperty DeletePropertyWithoutMixing (string propertyName)
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
