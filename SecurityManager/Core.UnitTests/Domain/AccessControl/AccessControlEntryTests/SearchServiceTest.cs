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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.SearchInfrastructure.Metadata;
using Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.AccessControl.AccessControlEntryTests
{
  [TestFixture]
  public class SearchSerivceTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;
    private IBusinessObjectClass _aceClass;
    private AccessControlEntry _ace;
    private Mock<ISearchAvailableObjectsService> _searchServiceStub;
    private Mock<ISearchAvailableObjectsArguments> _searchServiceArgsStub;

    public override void OneTimeSetUp ()
    {
      base.OneTimeSetUp();

      _searchServiceStub = new Mock<ISearchAvailableObjectsService>();
      _searchServiceArgsStub = new Mock<ISearchAvailableObjectsArguments>();
      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>().AddService (
          typeof (ISearchAvailableObjectsService), new Mock<ISearchAvailableObjectsService>().Object);
    }

    public override void SetUp ()
    {
      base.SetUp();

      _aceClass = BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (AccessControlEntry));

      _testHelper = new AccessControlTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();

      _ace = AccessControlEntry.NewObject();
    }

    public override void TestFixtureTearDown ()
    {
      base.TestFixtureTearDown();
      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
    }

    [Test]
    public void SearchSpecificTenants ()
    {
      var property = (IBusinessObjectReferenceProperty) _aceClass.GetPropertyDefinition ("SpecificTenant");
      Assert.That (property, Is.Not.Null);

      var expected = new[] { new Mock<IBusinessObject>().Object };

      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>()
          .AddService (typeof (TenantPropertyTypeSearchService), _searchServiceStub.Object);
      _searchServiceStub.Setup (stub => stub.SupportsProperty (property)).Returns (true);
      _searchServiceStub.Setup (stub => stub.Search (_ace, property, _searchServiceArgsStub.Object)).Returns (expected);

      Assert.That (property.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actual = property.SearchAvailableObjects (_ace, _searchServiceArgsStub.Object);
      Assert.That (actual, Is.EquivalentTo (expected));
    }

    [Test]
    public void SearchSpecificGroups ()
    {
      var property = (IBusinessObjectReferenceProperty) _aceClass.GetPropertyDefinition ("SpecificGroup");
      Assert.That (property, Is.Not.Null);

      var expected = new[] { new Mock<IBusinessObject>().Object };

      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>()
          .AddService (typeof (GroupPropertyTypeSearchService), _searchServiceStub.Object);
      _searchServiceStub.Setup (stub => stub.SupportsProperty (property)).Returns (true);
      _searchServiceStub.Setup (stub => stub.Search (_ace, property, _searchServiceArgsStub.Object)).Returns (expected);

      Assert.That (property.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actual = property.SearchAvailableObjects (_ace, _searchServiceArgsStub.Object);
      Assert.That (actual, Is.EquivalentTo (expected));
    }

    [Test]
    public void SearchSpecificGroupType ()
    {
      var property = (IBusinessObjectReferenceProperty) _aceClass.GetPropertyDefinition ("SpecificGroupType");
      Assert.That (property, Is.Not.Null);

      var expected = new[] { new Mock<IBusinessObject>().Object };

      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>()
          .AddService (typeof (GroupTypePropertyTypeSearchService), _searchServiceStub.Object);
      _searchServiceStub.Setup (stub => stub.SupportsProperty (property)).Returns (true);
      _searchServiceStub.Setup (stub => stub.Search (_ace, property, _searchServiceArgsStub.Object)).Returns (expected);

      Assert.That (property.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actual = property.SearchAvailableObjects (_ace, _searchServiceArgsStub.Object);
      Assert.That (actual, Is.EquivalentTo (expected));
    }

    [Test]
    public void SearchSpecificUsers ()
    {
      var property = (IBusinessObjectReferenceProperty) _aceClass.GetPropertyDefinition ("SpecificUser");
      Assert.That (property, Is.Not.Null);

      var expected = new[] { new Mock<IBusinessObject>().Object };

      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>()
          .AddService (typeof (UserPropertyTypeSearchService), _searchServiceStub.Object);
      _searchServiceStub.Setup (stub => stub.SupportsProperty (property)).Returns (true);
      _searchServiceStub.Setup (stub => stub.Search (_ace, property, _searchServiceArgsStub.Object)).Returns (expected);

      Assert.That (property.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actual = property.SearchAvailableObjects (_ace, _searchServiceArgsStub.Object);
      Assert.That (actual, Is.EquivalentTo (expected));
    }

    [Test]
    public void SearchSpecificPositions ()
    {
      var property = (IBusinessObjectReferenceProperty) _aceClass.GetPropertyDefinition ("SpecificPosition");
      Assert.That (property, Is.Not.Null);

      var expected = new[] { new Mock<IBusinessObject>().Object };

      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>()
          .AddService (typeof (PositionPropertyTypeSearchService), _searchServiceStub.Object);
      _searchServiceStub.Setup (stub => stub.SupportsProperty (property)).Returns (true);
      _searchServiceStub.Setup (stub => stub.Search (_ace, property, _searchServiceArgsStub.Object)).Returns (expected);

      Assert.That (property.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actual = property.SearchAvailableObjects (_ace, _searchServiceArgsStub.Object);
      Assert.That (actual, Is.EquivalentTo (expected));
    }

    [Test]
    public void SearchSpecificAbstractRoles ()
    {
      var property = (IBusinessObjectReferenceProperty) _aceClass.GetPropertyDefinition ("SpecificAbstractRole");
      Assert.That (property, Is.Not.Null);

      var expected = new[] { new Mock<IBusinessObject>().Object };

      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>().AddService (
          typeof (AbstractRoleDefinitionPropertyTypeSearchService), _searchServiceStub.Object);
      _searchServiceStub.Setup (stub => stub.SupportsProperty (property)).Returns (true);
      _searchServiceStub.Setup (stub => stub.Search (_ace, property, _searchServiceArgsStub.Object)).Returns (expected);

      Assert.That (property.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actual = property.SearchAvailableObjects (_ace, _searchServiceArgsStub.Object);
      Assert.That (actual, Is.EquivalentTo (expected));
    }
  }
}
