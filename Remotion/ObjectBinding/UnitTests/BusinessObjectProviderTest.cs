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
using Remotion.ObjectBinding.UnitTests.BindableObject;
using Remotion.ObjectBinding.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests
{
  [TestFixture]
  public class BusinessObjectProviderTest : TestBase
  {
    private IBusinessObjectProvider _provider;
    private MockRepository _mockRepository;
    private IBusinessObjectServiceFactory _serviceFactoryStub;

    public override void SetUp ()
    {
      base.SetUp();

      _serviceFactoryStub = MockRepository.GenerateStub<IBusinessObjectServiceFactory>();
      _provider = new StubBusinessObjectProvider (_serviceFactoryStub);
      _mockRepository = new MockRepository();

      BusinessObjectProvider.SetProvider<StubBusinessObjectProviderAttribute> (null);
      BusinessObjectProvider.SetProvider<BindableObjectProviderAttribute> (null);
    }

    public override void TearDown ()
    {
      base.TearDown();

      BusinessObjectProvider.SetProvider<StubBusinessObjectProviderAttribute> (null);
      BusinessObjectProvider.SetProvider<BindableObjectProviderAttribute> (null);
    }

    [Test]
    public void Initialize ()
    {
      Assert.That (((BusinessObjectProvider) _provider).ServiceFactory, Is.SameAs (_serviceFactoryStub));
    }

    [Test]
    public void GetProvider ()
    {
      IBusinessObjectProvider provider = BusinessObjectProvider.GetProvider (typeof (StubBusinessObjectProviderAttribute));
      Assert.That (provider, Is.TypeOf (typeof (StubBusinessObjectProvider)));
      Assert.That (((BusinessObjectProvider)provider).ProviderAttribute, Is.TypeOf (typeof (StubBusinessObjectProviderAttribute)));
    }

    [Test]
    public void GetProvider_WithMixin ()
    {
      using (MixinConfiguration.BuildNew().ForClass (typeof (StubBusinessObjectProvider)).AddMixin<MixinStub>().EnterScope())
      {
        IBusinessObjectProvider provider = BusinessObjectProvider.GetProvider (typeof (StubBusinessObjectProviderAttribute));
        Assert.That (provider, Is.InstanceOf (typeof (IMixinTarget)));
      }
    }

    [Test]
    public void GetProvider_WithDifferentAttributesResultingInDifferentProviders ()
    {
      IBusinessObjectProvider provider = BusinessObjectProvider.GetProvider (typeof (StubBusinessObjectProviderAttribute));
      Assert.That (provider, Is.TypeOf (typeof (StubBusinessObjectProvider)));
      Assert.That (provider, Is.Not.SameAs (BusinessObjectProvider.GetProvider (typeof (Stub2BusinessObjectProviderAttribute))));
      Assert.That (provider, Is.Not.SameAs (BusinessObjectProvider.GetProvider (typeof (DerivedStubBusinessObjectProviderAttribute))));
    }

    [Test]
    public void GetProvider_FromOtherBusinessObjectImplementation ()
    {
      IBusinessObjectProvider provider = BusinessObjectProvider.GetProvider (typeof (OtherBusinessObjectImplementationProviderAttribute));
      Assert.That (provider, Is.TypeOf (typeof (OtherBusinessObjectImplementationProvider)));
    }

    [Test]
    public void GetProvider_SameTwice ()
    {
      Assert.That (
          BusinessObjectProvider.GetProvider (typeof (StubBusinessObjectProviderAttribute)),
          Is.SameAs (BusinessObjectProvider.GetProvider (typeof (StubBusinessObjectProviderAttribute))));
    }

    [Test]
    public void GetProvider_FromGeneric ()
    {
      Assert.That (
          BusinessObjectProvider.GetProvider<StubBusinessObjectProviderAttribute>(),
          Is.SameAs (BusinessObjectProvider.GetProvider (typeof (StubBusinessObjectProviderAttribute))));
    }

    [Test]
    public void SetProvider ()
    {
      BusinessObjectProvider.SetProvider (typeof (StubBusinessObjectProviderAttribute), _provider);
      Assert.That (BusinessObjectProvider.GetProvider (typeof (StubBusinessObjectProviderAttribute)), Is.SameAs (_provider));
      Assert.That (((BusinessObjectProvider) _provider).ProviderAttribute, Is.TypeOf (typeof (StubBusinessObjectProviderAttribute)));
    }

    [Test]
    public void SetProvider_FromOtherBusinessObjectImplementation ()
    {
      OtherBusinessObjectImplementationProvider provider = new OtherBusinessObjectImplementationProvider();
      BusinessObjectProvider.SetProvider (typeof (OtherBusinessObjectImplementationProviderAttribute), provider);
      Assert.That (BusinessObjectProvider.GetProvider (typeof (OtherBusinessObjectImplementationProviderAttribute)), Is.SameAs (provider));
    }

    [Test]
    public void SetProvider_WithGeneric ()
    {
      BusinessObjectProvider.SetProvider<StubBusinessObjectProviderAttribute> (_provider);
      Assert.That (BusinessObjectProvider.GetProvider (typeof (StubBusinessObjectProviderAttribute)), Is.SameAs (_provider));
    }

    [Test]
    public void SetProvider_Twice ()
    {
      BusinessObjectProvider.SetProvider (typeof (StubBusinessObjectProviderAttribute), new StubBusinessObjectProvider (_serviceFactoryStub));
      BusinessObjectProvider.SetProvider (typeof (StubBusinessObjectProviderAttribute), _provider);
      Assert.That (BusinessObjectProvider.GetProvider (typeof (StubBusinessObjectProviderAttribute)), Is.SameAs (_provider));
    }

    [Test]
    public void SetProvider_Null ()
    {
      BusinessObjectProvider.SetProvider (typeof (StubBusinessObjectProviderAttribute), _provider);
      BusinessObjectProvider.SetProvider (typeof (StubBusinessObjectProviderAttribute), null);
      Assert.That (BusinessObjectProvider.GetProvider (typeof (StubBusinessObjectProviderAttribute)), Is.Not.SameAs (_provider));
      Assert.That (BusinessObjectProvider.GetProvider (typeof (StubBusinessObjectProviderAttribute)), Is.TypeOf (typeof (StubBusinessObjectProvider)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = 
        "The provider is not compatible with the provider-type required by the businessObjectProviderAttributeType's instantiation."
        + "\r\nParameter name: provider")]
    public void SetProvider_WithMismatchedTypes ()
    {
      BusinessObjectProvider.SetProvider (typeof (BindableObjectProviderAttribute), _provider);
    }

    [Test]
    public void AddAndGetService ()
    {
      IBusinessObjectService expectedService = _mockRepository.Stub<IBusinessObjectService>();
      Assert.That (_provider.GetService (expectedService.GetType()), Is.Null);

      ((BusinessObjectProvider) _provider).AddService (expectedService.GetType(), expectedService);

      Assert.That (_provider.GetService (expectedService.GetType()), Is.SameAs (expectedService));
    }

    [Test]
    public void AddService_Twice ()
    {
      IBusinessObjectService expectedService = _mockRepository.Stub<IBusinessObjectService>();
      Assert.That (_provider.GetService (expectedService.GetType()), Is.Null);

      ((BusinessObjectProvider) _provider).AddService (expectedService.GetType(), _mockRepository.Stub<IBusinessObjectService>());
      ((BusinessObjectProvider) _provider).AddService (expectedService.GetType(), expectedService);

      Assert.That (_provider.GetService (expectedService.GetType()), Is.SameAs (expectedService));
    }

    [Test]
    public void AddService_WithGeneric ()
    {
      StubBusinessObjectService expectedService = new StubBusinessObjectService();
      Assert.That (_provider.GetService (expectedService.GetType ()), Is.Null);

      ((BusinessObjectProvider) _provider).AddService<IBusinessObjectService> (new StubBusinessObjectService());
      ((BusinessObjectProvider) _provider).AddService (expectedService);

      Assert.That (_provider.GetService (typeof (IBusinessObjectService)), Is.InstanceOf (typeof (StubBusinessObjectService)));
      Assert.That (_provider.GetService (expectedService.GetType ()), Is.SameAs (expectedService));
      Assert.That (_provider.GetService (expectedService.GetType ()), Is.Not.SameAs (_provider.GetService (typeof (IBusinessObjectService))));
    }

    [Test]
    public void GetService_FromGeneric ()
    {
      ((BusinessObjectProvider) _provider).AddService (typeof (IBusinessObjectService), _mockRepository.Stub<IBusinessObjectService>());

      Assert.That (
          ((BusinessObjectProvider) _provider).GetService<IBusinessObjectService>(),
          Is.SameAs (_provider.GetService (typeof (IBusinessObjectService))));
    }

    [Test]
    public void GetService_FromServiceFactory ()
    {
      MockRepository mockRepository = new MockRepository();
      IBusinessObjectServiceFactory serviceFactoryMock = mockRepository.StrictMock<IBusinessObjectServiceFactory>();
      IBusinessObjectStringFormatterService serviceStub = MockRepository.GenerateStub<IBusinessObjectStringFormatterService>();
      BusinessObjectProvider provider = new StubBusinessObjectProvider (serviceFactoryMock);

      Expect.Call (serviceFactoryMock.CreateService (provider, typeof (IBusinessObjectStringFormatterService))).Return (serviceStub);

      mockRepository.ReplayAll();

      IBusinessObjectService actual = provider.GetService (typeof (IBusinessObjectStringFormatterService));
      IBusinessObjectService actual2 = provider.GetService (typeof (IBusinessObjectStringFormatterService));

      mockRepository.VerifyAll();

      Assert.That (actual, Is.SameAs (serviceStub));
      Assert.That (actual, Is.SameAs (actual2));
    }

    [Test]
    public void GetService_FromExplictValue ()
    {
      MockRepository mockRepository = new MockRepository();
      IBusinessObjectServiceFactory serviceFactoryMock = mockRepository.StrictMock<IBusinessObjectServiceFactory>();
      IBusinessObjectStringFormatterService serviceStub = MockRepository.GenerateStub<IBusinessObjectStringFormatterService>();
      BusinessObjectProvider provider = new StubBusinessObjectProvider (serviceFactoryMock);

      provider.AddService (typeof (IBusinessObjectStringFormatterService), serviceStub);

      mockRepository.ReplayAll();

      IBusinessObjectService actual = provider.GetService (typeof (IBusinessObjectStringFormatterService));
      IBusinessObjectService actual2 = provider.GetService (typeof (IBusinessObjectStringFormatterService));

      mockRepository.VerifyAll();

      Assert.That (actual, Is.SameAs (serviceStub));
      Assert.That (actual, Is.SameAs (actual2));
    }
  }
}
