// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Mixins;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.SearchInfrastructure.Metadata;
using Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure;
using Rhino.Mocks;

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
    private IBusinessObjectProviderWithIdentity _bindableDomainObjectProvider;
    private IBusinessObjectProviderWithIdentity _bindableObjectProvider;
    private MockRepository _mockRepository;

    [SetUp]
    public void SetUp ()
    {
      _serviceFactory = BindableObjectServiceFactory.Create();
      _serviceMixin = Mixin.Get<SecurityManagerObjectServiceFactoryMixin> (_serviceFactory);
      _mockRepository = new MockRepository();
      _bindableDomainObjectProvider = _mockRepository.Stub<IBusinessObjectProviderWithIdentity>();
      _bindableObjectProvider = _mockRepository.Stub<IBusinessObjectProviderWithIdentity>();
      SetupResult.For (_bindableDomainObjectProvider.ProviderAttribute).Return (new BindableDomainObjectProviderAttribute());
      SetupResult.For (_bindableObjectProvider.ProviderAttribute).Return (new BindableObjectProviderAttribute());
      _mockRepository.ReplayAll();
    }

    [Test]
    public void Initialize ()
    {
      Assert.That (_serviceMixin, Is.Not.Null);
      Assert.That (_serviceMixin, Is.InstanceOf (typeof (IBusinessObjectServiceFactory)));
      Assert.That (Mixin.Get<BindableDomainObjectServiceFactoryMixin> (_serviceFactory), Is.Not.Null);
    }

    [Test]
    public void GetService_FromSubstitutionPropertiesSearchService ()
    {
      Assert.That (
          _serviceFactory.CreateService (_bindableObjectProvider, typeof (SubstitutionPropertiesSearchService)),
          Is.InstanceOf (typeof (SubstitutionPropertiesSearchService)));
    }

    [Test]
    public void GetService_FromRolePropertiesSearchService ()
    {
      Assert.That (
          _serviceFactory.CreateService (_bindableObjectProvider, typeof (RolePropertiesSearchService)),
          Is.InstanceOf (typeof (RolePropertiesSearchService)));
    }

    [Test]
    public void GetService_FromTenantPropertyTypeSearchService ()
    {
      Assert.That (
          _serviceFactory.CreateService (_bindableObjectProvider, typeof (TenantPropertyTypeSearchService)),
          Is.InstanceOf (typeof (TenantPropertyTypeSearchService)));
    }

    [Test]
    public void GetService_FromGroupPropertyTypeSearchService ()
    {
      Assert.That (
          _serviceFactory.CreateService (_bindableObjectProvider, typeof (GroupPropertyTypeSearchService)),
          Is.InstanceOf (typeof (GroupPropertyTypeSearchService)));
    }

    [Test]
    public void GetService_FromUserPropertyTypeSearchService ()
    {
      Assert.That (
          _serviceFactory.CreateService (_bindableObjectProvider, typeof (UserPropertyTypeSearchService)),
          Is.InstanceOf (typeof (UserPropertyTypeSearchService)));
    }

    [Test]
    public void GetService_FromPositionPropertyTypeSearchService ()
    {
      Assert.That (
          _serviceFactory.CreateService (_bindableObjectProvider, typeof (PositionPropertyTypeSearchService)),
          Is.InstanceOf (typeof (PositionPropertyTypeSearchService)));
    }

    [Test]
    public void GetService_FromGroupTypePropertyTypeSearchService ()
    {
      Assert.That (
          _serviceFactory.CreateService (_bindableObjectProvider, typeof (GroupTypePropertyTypeSearchService)),
          Is.InstanceOf (typeof (GroupTypePropertyTypeSearchService)));
    }

    [Test]
    public void GetService_FromAbstractRoleDefinitionPropertyTypeSearchService ()
    {
      Assert.That (
          _serviceFactory.CreateService (_bindableObjectProvider, typeof (AbstractRoleDefinitionPropertyTypeSearchService)),
          Is.InstanceOf (typeof (AbstractRoleDefinitionPropertyTypeSearchService)));
    }

    [Test]
    public void GetService_FromIGetObjectService ()
    {
      Assert.That (
          _serviceFactory.CreateService (_bindableDomainObjectProvider, typeof (IGetObjectService)),
          Is.InstanceOf (typeof (BindableDomainObjectGetObjectService)));
    }

    [Test]
    public void GetService_FromUnknownService ()
    {
      Assert.That (
          _serviceFactory.CreateService (_bindableObjectProvider, typeof (IStubService)),
          Is.Null);
    }
  }
}