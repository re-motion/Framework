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
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests
{
  [TestFixture]
  public class BindableDomainObjectServiceFactoryMixinTest
  {
    private interface IStubService : IBusinessObjectService
    {}

    private IBusinessObjectServiceFactory _serviceFactory;
    private BindableDomainObjectServiceFactoryMixin _serviceMixin;
    private Mock<IBusinessObjectProviderWithIdentity> _bindableDomainObjectProvider;
    private Mock<IBusinessObjectProviderWithIdentity> _bindableObjectProvider;

    [SetUp]
    public void SetUp ()
    {
      _serviceFactory = BindableObjectServiceFactory.Create();
      _serviceMixin = Mixin.Get<BindableDomainObjectServiceFactoryMixin>(_serviceFactory);

      _bindableDomainObjectProvider = new Mock<IBusinessObjectProviderWithIdentity>();
      _bindableObjectProvider = new Mock<IBusinessObjectProviderWithIdentity>();
      _bindableDomainObjectProvider.Setup(_ => _.ProviderAttribute).Returns(new BindableDomainObjectProviderAttribute());
      _bindableObjectProvider.Setup(_ => _.ProviderAttribute).Returns(new BindableObjectProviderAttribute());
    }

    [Test]
    public void Initialize ()
    {
      Assert.That(_serviceMixin, Is.Not.Null);
      Assert.That(_serviceMixin, Is.InstanceOf(typeof(IBusinessObjectServiceFactory)));
    }

    [Test]
    public void GetService_FromIBusinessObjectStringFormatterService ()
    {
      Assert.That(
          _serviceFactory.CreateService(_bindableDomainObjectProvider.Object, typeof(IBusinessObjectStringFormatterService)),
          Is.InstanceOf(typeof(BusinessObjectStringFormatterService)));
    }

    [Test]
    public void GetService_FromIGetObjectService ()
    {
      Assert.That(
          _serviceFactory.CreateService(_bindableDomainObjectProvider.Object, typeof(IGetObjectService)),
          Is.InstanceOf(typeof(BindableDomainObjectGetObjectService)));
    }

    [Test]
    public void GetService_FromISearchAvailableObjectsService ()
    {
      Assert.That(
          _serviceFactory.CreateService(_bindableDomainObjectProvider.Object, typeof(ISearchAvailableObjectsService)),
          Is.InstanceOf(typeof(BindableDomainObjectCompoundSearchService)));
    }

    [Test]
    public void GetService_FromIGetObjectServiceWithBindableObjectProvider ()
    {
      Assert.That(
          _serviceFactory.CreateService(_bindableObjectProvider.Object, typeof(IGetObjectService)),
          Is.Null);
    }

    [Test]
    public void GetService_FromISearchAvailableObjectsServiceWithBindableObjectProvider ()
    {
      Assert.That(
          _serviceFactory.CreateService(_bindableObjectProvider.Object, typeof(ISearchAvailableObjectsService)),
          Is.Null);
    }

    [Test]
    public void GetService_FromUnknownService ()
    {
      Assert.That(
          _serviceFactory.CreateService(_bindableDomainObjectProvider.Object, typeof(IStubService)),
          Is.Null);
    }
  }
}
