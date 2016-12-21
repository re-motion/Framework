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
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.RoleTests
{
  [TestFixture]
  public class BindableObjectImpelementation : RoleTestBase
  {
    public override void TearDown ()
    {
      base.TearDown ();

      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
    }

    [Test]
    public void SearchGroups ()
    {
      ISearchAvailableObjectsService searchServiceStub = MockRepository.GenerateStub<ISearchAvailableObjectsService> ();
      ISearchAvailableObjectsArguments args = MockRepository.GenerateStub<ISearchAvailableObjectsArguments> ();

      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>()
          .AddService (typeof (GroupPropertyTypeSearchService), searchServiceStub);
      IBusinessObjectClass roleClass = BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (Role));
      IBusinessObjectReferenceProperty groupProperty = (IBusinessObjectReferenceProperty) roleClass.GetPropertyDefinition ("Group");
      Assert.That (groupProperty, Is.Not.Null);

      Role role = Role.NewObject ();
      var expected = new[] { MockRepository.GenerateStub<IBusinessObject> () };

      searchServiceStub.Stub (stub => stub.SupportsProperty (groupProperty)).Return (true);
      searchServiceStub.Stub (stub => stub.Search (role, groupProperty, args)).Return (expected);

      Assert.That (groupProperty.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actual = groupProperty.SearchAvailableObjects (role, args);
      Assert.That (actual, Is.SameAs (expected));
    }

    [Test]
    public void SearchUsers ()
    {
      ISearchAvailableObjectsService searchServiceStub = MockRepository.GenerateStub<ISearchAvailableObjectsService> ();
      ISearchAvailableObjectsArguments args = MockRepository.GenerateStub<ISearchAvailableObjectsArguments> ();

      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>()
          .AddService (typeof (UserPropertyTypeSearchService), searchServiceStub);
      IBusinessObjectClass roleClass = BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (Role));
      IBusinessObjectReferenceProperty userProperty = (IBusinessObjectReferenceProperty) roleClass.GetPropertyDefinition ("User");
      Assert.That (userProperty, Is.Not.Null);

      Role role = Role.NewObject ();
      var expected = new[] { MockRepository.GenerateStub<IBusinessObject> () };

      searchServiceStub.Stub (stub => stub.SupportsProperty (userProperty)).Return (true);
      searchServiceStub.Stub (stub => stub.Search (role, userProperty, args)).Return (expected);

      Assert.That (userProperty.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actual = userProperty.SearchAvailableObjects (role, args);
      Assert.That (actual, Is.SameAs (expected));
    }

    [Test]
    public void SearchPositions ()
    {
      ISearchAvailableObjectsService searchServiceStub = MockRepository.GenerateStub<ISearchAvailableObjectsService> ();
      ISearchAvailableObjectsArguments args = MockRepository.GenerateStub<ISearchAvailableObjectsArguments> ();

      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>()
          .AddService (typeof (RolePropertiesSearchService), searchServiceStub);
      IBusinessObjectClass roleClass = BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (Role));
      IBusinessObjectReferenceProperty positionProperty = (IBusinessObjectReferenceProperty) roleClass.GetPropertyDefinition ("Position");
      Assert.That (positionProperty, Is.Not.Null);

      Role role = Role.NewObject ();
      var expected = new[] { MockRepository.GenerateStub<IBusinessObject> () };

      searchServiceStub.Stub (stub => stub.SupportsProperty (positionProperty)).Return (true);
      searchServiceStub.Stub (stub => stub.Search (role, positionProperty, args)).Return (expected);

      Assert.That (positionProperty.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actual = positionProperty.SearchAvailableObjects (role, args);
      Assert.That (actual, Is.SameAs (expected));
    }

    [Test]
    public void GetDisplayName_WithGroupAndPosition ()
    {
      Group roleGroup = TestHelper.CreateGroup ("RoleGroup", Guid.NewGuid ().ToString (), null, null);
      User user = TestHelper.CreateUser ("user", "Firstname", "Lastname", "Title", null, null);
      Position position = TestHelper.CreatePosition ("Position");
      Role role = TestHelper.CreateRole (user, roleGroup, position);

      Assert.That (role.DisplayName, Is.EqualTo ("Position / RoleGroup"));
    }

    [Test]
    public void GetDisplayName_WithGroup ()
    {
      Group roleGroup = TestHelper.CreateGroup ("RoleGroup", Guid.NewGuid ().ToString (), null, null);
      roleGroup.ShortName = "RG";
      User user = TestHelper.CreateUser ("user", "Firstname", "Lastname", "Title", null, null);
      Role role = TestHelper.CreateRole (user, roleGroup, null);

      Assert.That (role.DisplayName, Is.EqualTo ("? / RG (RoleGroup)"));
    }

    [Test]
    public void GetDisplayName_WithPosition ()
    {
      User user = TestHelper.CreateUser ("user", "Firstname", "Lastname", "Title", null, null);
      Position position = TestHelper.CreatePosition ("Position");
      Role role = TestHelper.CreateRole (user, null, position);

      Assert.That (role.DisplayName, Is.EqualTo ("Position / ?"));
    }

    [Test]
    public void GetDisplayName_WithoutGroupOrPosition ()
    {
      User user = TestHelper.CreateUser ("user", "Firstname", "Lastname", "Title", null, null);
      Role role = TestHelper.CreateRole (user, null, null);

      Assert.That (role.DisplayName, Is.EqualTo ("? / ?"));
    }
  }
}
