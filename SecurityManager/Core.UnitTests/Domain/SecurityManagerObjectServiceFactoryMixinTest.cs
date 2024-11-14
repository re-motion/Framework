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
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Mixins;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.SearchInfrastructure.Metadata;
using Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain
{
  [TestFixture]
  public class SecurityManagerObjectServiceFactoryMixinTest
  {
    private interface IStubService : IBusinessObjectService
    {
    }

    private IBusinessObjectServiceFactory _serviceFactory;
    private SecurityManagerObjectServiceFactoryMixin _serviceMixin;
    private Mock<IBusinessObjectProviderWithIdentity> _bindableDomainObjectProvider;
    private Mock<IBusinessObjectProviderWithIdentity> _bindableObjectProvider;

    [SetUp]
    public void SetUp ()
    {
      _serviceFactory = BindableObjectServiceFactory.Create();
      _serviceMixin = Mixin.Get<SecurityManagerObjectServiceFactoryMixin>(_serviceFactory);
      _bindableDomainObjectProvider = new Mock<IBusinessObjectProviderWithIdentity>();
      _bindableObjectProvider = new Mock<IBusinessObjectProviderWithIdentity>();
      _bindableDomainObjectProvider.Setup(_ => _.ProviderAttribute).Returns(new BindableDomainObjectProviderAttribute()).Verifiable();
      _bindableObjectProvider.Setup(_ => _.ProviderAttribute).Returns(new BindableObjectProviderAttribute()).Verifiable();
    }

    [Test]
    public void Initialize ()
    {
      Assert.That(_serviceMixin, Is.Not.Null);
      Assert.That(_serviceMixin, Is.InstanceOf(typeof(IBusinessObjectServiceFactory)));
      Assert.That(Mixin.Get<BindableDomainObjectServiceFactoryMixin>(_serviceFactory), Is.Not.Null);
    }

    [Test]
    public void GetService_FromSubstitutionPropertiesSearchService ()
    {
      Assert.That(
          _serviceFactory.CreateService(_bindableObjectProvider.Object, typeof(SubstitutionPropertiesSearchService)),
          Is.InstanceOf(typeof(SubstitutionPropertiesSearchService)));
    }

    [Test]
    public void GetService_FromRolePropertiesSearchService ()
    {
      Assert.That(
          _serviceFactory.CreateService(_bindableObjectProvider.Object, typeof(RolePropertiesSearchService)),
          Is.InstanceOf(typeof(RolePropertiesSearchService)));
    }

    [Test]
    public void GetService_FromTenantPropertyTypeSearchService ()
    {
      Assert.That(
          _serviceFactory.CreateService(_bindableObjectProvider.Object, typeof(TenantPropertyTypeSearchService)),
          Is.InstanceOf(typeof(TenantPropertyTypeSearchService)));
    }

    [Test]
    public void GetService_FromGroupPropertyTypeSearchService ()
    {
      Assert.That(
          _serviceFactory.CreateService(_bindableObjectProvider.Object, typeof(GroupPropertyTypeSearchService)),
          Is.InstanceOf(typeof(GroupPropertyTypeSearchService)));
    }

    [Test]
    public void GetService_FromUserPropertyTypeSearchService ()
    {
      Assert.That(
          _serviceFactory.CreateService(_bindableObjectProvider.Object, typeof(UserPropertyTypeSearchService)),
          Is.InstanceOf(typeof(UserPropertyTypeSearchService)));
    }

    [Test]
    public void GetService_FromPositionPropertyTypeSearchService ()
    {
      Assert.That(
          _serviceFactory.CreateService(_bindableObjectProvider.Object, typeof(PositionPropertyTypeSearchService)),
          Is.InstanceOf(typeof(PositionPropertyTypeSearchService)));
    }

    [Test]
    public void GetService_FromGroupTypePropertyTypeSearchService ()
    {
      Assert.That(
          _serviceFactory.CreateService(_bindableObjectProvider.Object, typeof(GroupTypePropertyTypeSearchService)),
          Is.InstanceOf(typeof(GroupTypePropertyTypeSearchService)));
    }

    [Test]
    public void GetService_FromAbstractRoleDefinitionPropertyTypeSearchService ()
    {
      Assert.That(
          _serviceFactory.CreateService(_bindableObjectProvider.Object, typeof(AbstractRoleDefinitionPropertyTypeSearchService)),
          Is.InstanceOf(typeof(AbstractRoleDefinitionPropertyTypeSearchService)));
    }

    [Test]
    public void GetService_FromIGetObjectService ()
    {
      Assert.That(
          _serviceFactory.CreateService(_bindableDomainObjectProvider.Object, typeof(IGetObjectService)),
          Is.InstanceOf(typeof(BindableDomainObjectGetObjectService)));
    }

    [Test]
    public void GetService_FromUnknownService ()
    {
      Assert.That(
          _serviceFactory.CreateService(_bindableObjectProvider.Object, typeof(IStubService)),
          Is.Null);
    }
  }
}
