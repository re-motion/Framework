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
  public class SupportsDefaultValue : TestBase
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
      BusinessObjectProvider.SetProvider<BindableObjectProviderForDefaultValueServiceAttribute> (_bindableObjectProviderForPropertyType);
    }

    [Test]
    public void DefaultValueServiceFromPropertyType ()
    {
      IDefaultValueServiceOnType serviceMock = _mockRepository.StrictMock<IDefaultValueServiceOnType> ();
      IBusinessObjectReferenceProperty property = CreateProperty ("DefaultValueServiceFromPropertyType");

      Expect.Call (serviceMock.SupportsProperty (property)).Return (true);
      _mockRepository.ReplayAll();

      _bindableObjectProviderForPropertyType.AddService (serviceMock);
      bool actual = property.SupportsDefaultValue;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    public void DefaultValueServiceFromPropertyDeclaration ()
    {
      var serviceMock = _mockRepository.StrictMock<IDefaultValueServiceOnProperty>();
      var createObjectServiceOnTypeStub = _mockRepository.StrictMock<IDefaultValueServiceOnType>();
      IBusinessObjectReferenceProperty property = CreateProperty ("DefaultValueServiceFromPropertyDeclaration");

      Expect.Call (serviceMock.SupportsProperty (property)).Return (true);
      _mockRepository.ReplayAll();

      _bindableObjectProviderForPropertyType.AddService (createObjectServiceOnTypeStub);
      _bindableObjectProviderForDeclaringType.AddService (serviceMock);
      bool actual = property.SupportsDefaultValue;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    public void UnknownDefaultValueService ()
    {
      IBusinessObjectReferenceProperty property = CreateProperty ("DefaultValueServiceFromPropertyType");

      Assert.That (property.SupportsDefaultValue, Is.False);
    }

    [Test]
    public void WithoutDefaultValueServiceAttribute_AndDefaultDefaultValueService_FromPropertyDeclaration ()
    {
      var createObjectServiceMock = _mockRepository.StrictMock<IDefaultValueService> ();
      IBusinessObjectReferenceProperty property = CreatePropertyWithoutMixing ("NoDefaultValueService");

      Expect.Call (createObjectServiceMock.SupportsProperty (property)).Return (true);
      _mockRepository.ReplayAll();

      _bindableObjectProviderForDeclaringType.AddService (createObjectServiceMock);
      bool actual = property.SupportsDefaultValue;

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.True);
    }

    [Test]
    [Ignore ("TODO RM-4106: Extend fallback behavior to include property type.")]
    public void WithoutDefaultValueServiceAttribute_AndDefaultDefaultValueService_FromPropertyType ()
    {
      var createObjectServiceMock = _mockRepository.StrictMock<IDefaultValueService> ();
      var businessObjectClassServiceMock = _mockRepository.StrictMock<IBusinessObjectClassService> ();
      var businessObjectProviderMock = _mockRepository.StrictMock<IBusinessObjectProvider> ();
      var businessObjectClassWithIdentityMock = _mockRepository.StrictMock<IBusinessObjectClassWithIdentity> ();
      IBusinessObjectReferenceProperty property = CreatePropertyWithoutMixing ("NoDefaultValueService");

      Expect.Call (businessObjectClassWithIdentityMock.BusinessObjectProvider).Return (businessObjectProviderMock).Repeat.Any ();
      Expect.Call (businessObjectProviderMock.GetService (typeof (IDefaultValueService))).Return (createObjectServiceMock);
      Expect.Call (businessObjectClassServiceMock.GetBusinessObjectClass (typeof (ClassFromOtherBusinessObjectImplementation)))
          .Return (businessObjectClassWithIdentityMock);
      Expect.Call (createObjectServiceMock.SupportsProperty (property)).Return (true);
      _mockRepository.ReplayAll ();

      _bindableObjectProviderForDeclaringType.AddService (businessObjectClassServiceMock);
      bool actual = property.SupportsDefaultValue;

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.True);
    }

    [Test]
    public void WithoutDefaultValueServiceAttribute_AndNoDefaultDefaultValueService_FromPropertyDeclaration ()
    {
      IBusinessObjectReferenceProperty property = CreatePropertyWithoutMixing ("NoDefaultValueService");

      _mockRepository.ReplayAll ();

      bool actual = property.SupportsDefaultValue;

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.False);
    }

    [Test]
    [Ignore ("TODO RM-4106: Extend fallback behavior to include property type.")]
    public void WithoutDefaultValueServiceAttribute_AndNoDefaultDefaultValueService_FromPropertyType ()
    {
      var businessObjectClassServiceMock = _mockRepository.StrictMock<IBusinessObjectClassService> ();
      var businessObjectProviderMock = _mockRepository.StrictMock<IBusinessObjectProvider> ();
      var businessObjectClassWithIdentityMock = _mockRepository.StrictMock<IBusinessObjectClassWithIdentity> ();
      IBusinessObjectReferenceProperty property = CreatePropertyWithoutMixing ("NoDefaultValueService");

      Expect.Call (businessObjectClassWithIdentityMock.BusinessObjectProvider).Return (businessObjectProviderMock).Repeat.Any ();
      Expect.Call (businessObjectProviderMock.GetService (typeof (IDefaultValueService))).Return (null);
      Expect.Call (businessObjectClassServiceMock.GetBusinessObjectClass (typeof (ClassFromOtherBusinessObjectImplementation)))
          .Return (businessObjectClassWithIdentityMock);
      _mockRepository.ReplayAll ();
      
      _bindableObjectProviderForDeclaringType.AddService (businessObjectClassServiceMock);

      _mockRepository.VerifyAll ();
      Assert.That (property.SupportsDefaultValue, Is.False);
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
#pragma warning restore 612,618
}
