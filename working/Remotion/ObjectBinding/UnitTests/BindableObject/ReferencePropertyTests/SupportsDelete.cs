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
#pragma warning disable 612,618
  [TestFixture]
  public class SupportsDelete : TestBase
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
      BusinessObjectProvider.SetProvider<BindableObjectProviderForDeleteObjectServiceAttribute> (_bindableObjectProviderForPropertyType);
    }

    [Test]
    public void DeleteObjectServiceFromPropertyType ()
    {
      IDeleteObjectServiceOnType serviceMock = _mockRepository.StrictMock<IDeleteObjectServiceOnType> ();
      IBusinessObjectReferenceProperty property = DeleteProperty ("DeleteObjectServiceFromPropertyType");

      Expect.Call (serviceMock.SupportsProperty (property)).Return (true);
      _mockRepository.ReplayAll();

      _bindableObjectProviderForPropertyType.AddService (serviceMock);
      bool actual = property.SupportsDelete;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    public void DeleteObjectServiceFromPropertyDeclaration ()
    {
      var serviceMock = _mockRepository.StrictMock<IDeleteObjectServiceOnProperty>();
      var createObjectServiceOnTypeStub = _mockRepository.StrictMock<IDeleteObjectServiceOnType>();
      IBusinessObjectReferenceProperty property = DeleteProperty ("DeleteObjectServiceFromPropertyDeclaration");

      Expect.Call (serviceMock.SupportsProperty (property)).Return (true);
      _mockRepository.ReplayAll();

      _bindableObjectProviderForPropertyType.AddService (createObjectServiceOnTypeStub);
      _bindableObjectProviderForDeclaringType.AddService (serviceMock);
      bool actual = property.SupportsDelete;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    public void UnknownDeleteObjectService ()
    {
      IBusinessObjectReferenceProperty property = DeleteProperty ("DeleteObjectServiceFromPropertyType");

      Assert.That (property.SupportsDelete, Is.False);
    }

    [Test]
    public void WithoutDeleteObjectServiceAttribute_AndDefaultDeleteObjectService_FromPropertyDeclaration ()
    {
      var deleteObjectServiceMock = _mockRepository.StrictMock<IDeleteObjectService> ();
      IBusinessObjectReferenceProperty property = DeletePropertyWithoutMixing ("NoDeleteObjectService");

      Expect.Call (deleteObjectServiceMock.SupportsProperty (property)).Return (true);
      _mockRepository.ReplayAll();

      _bindableObjectProviderForDeclaringType.AddService (deleteObjectServiceMock);
      bool actual = property.SupportsDelete;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    [Ignore ("TODO RM-4144: Extend fallback behavior to include property type.")]
    public void WithoutDeleteObjectServiceAttribute_AndDefaultDeleteObjectService_FromPropertyType ()
    {
      var deleteObjectServiceMock = _mockRepository.StrictMock<IDeleteObjectService> ();
      var businessObjectClassServiceMock = _mockRepository.StrictMock<IBusinessObjectClassService> ();
      var businessObjectProviderMock = _mockRepository.StrictMock<IBusinessObjectProvider> ();
      var businessObjectClassWithIdentityMock = _mockRepository.StrictMock<IBusinessObjectClassWithIdentity> ();
      IBusinessObjectReferenceProperty property = DeletePropertyWithoutMixing ("NoDeleteObjectService");

      Expect.Call (businessObjectClassWithIdentityMock.BusinessObjectProvider).Return (businessObjectProviderMock).Repeat.Any ();
      Expect.Call (businessObjectProviderMock.GetService (typeof (IDeleteObjectService))).Return (deleteObjectServiceMock);
      Expect.Call (businessObjectClassServiceMock.GetBusinessObjectClass (typeof (ClassFromOtherBusinessObjectImplementation)))
          .Return (businessObjectClassWithIdentityMock);
      Expect.Call (deleteObjectServiceMock.SupportsProperty (property)).Return (true);
      _mockRepository.ReplayAll ();

      _bindableObjectProviderForDeclaringType.AddService (businessObjectClassServiceMock);
      bool actual = property.SupportsDelete;

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.True);
    }

    [Test]
    public void WithoutDeleteObjectServiceAttribute_AndNoDefaultDeleteObjectService_FromPropertyDeclaration ()
    {
      IBusinessObjectReferenceProperty property = DeletePropertyWithoutMixing ("NoDeleteObjectService");

      _mockRepository.ReplayAll ();

      bool actual = property.SupportsDelete;

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.False);
    }

    [Test]
    [Ignore ("TODO RM-4144: Extend fallback behavior to include property type.")]
    public void WithoutDeleteObjectServiceAttribute_AndNoDefaultDeleteObjectService_FromPropertyType ()
    {
      var businessObjectClassServiceMock = _mockRepository.StrictMock<IBusinessObjectClassService> ();
      var businessObjectProviderMock = _mockRepository.StrictMock<IBusinessObjectProvider> ();
      var businessObjectClassWithIdentityMock = _mockRepository.StrictMock<IBusinessObjectClassWithIdentity> ();
      IBusinessObjectReferenceProperty property = DeletePropertyWithoutMixing ("NoDeleteObjectService");

      Expect.Call (businessObjectClassWithIdentityMock.BusinessObjectProvider).Return (businessObjectProviderMock).Repeat.Any ();
      Expect.Call (businessObjectProviderMock.GetService (typeof (IDeleteObjectService))).Return (null);
      Expect.Call (businessObjectClassServiceMock.GetBusinessObjectClass (typeof (ClassFromOtherBusinessObjectImplementation)))
          .Return (businessObjectClassWithIdentityMock);
      _mockRepository.ReplayAll ();
      
      _bindableObjectProviderForDeclaringType.AddService (businessObjectClassServiceMock);

      _mockRepository.VerifyAll ();
      Assert.That (property.SupportsDelete, Is.False);
    }

    private ReferenceProperty DeleteProperty (string propertyName)
    {
      PropertyBase.Parameters propertyParameters = GetPropertyParameters (propertyName);
      return new ReferenceProperty (propertyParameters);
    }

    private ReferenceProperty DeletePropertyWithoutMixing (string propertyName)
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
#pragma warning restore 612,618
}
