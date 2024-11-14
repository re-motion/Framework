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
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.RoleTests
{
  [TestFixture]
  public class BindableObjectImpelementation : RoleTestBase
  {
    public override void TearDown ()
    {
      base.TearDown();

      BusinessObjectProvider.SetProvider(typeof(BindableDomainObjectProviderAttribute), null);
    }

    [Test]
    public void SearchGroups ()
    {
      var searchServiceStub = new Mock<ISearchAvailableObjectsService>();
      var args = new Mock<ISearchAvailableObjectsArguments>();

      BusinessObjectProvider.SetProvider(typeof(BindableDomainObjectProviderAttribute), null);
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>()
          .AddService(typeof(GroupPropertyTypeSearchService), searchServiceStub.Object);
      IBusinessObjectClass roleClass = BindableObjectProviderTestHelper.GetBindableObjectClass(typeof(Role));
      IBusinessObjectReferenceProperty groupProperty = (IBusinessObjectReferenceProperty)roleClass.GetPropertyDefinition("Group");
      Assert.That(groupProperty, Is.Not.Null);

      Role role = Role.NewObject();
      var expected = new[] { new Mock<IBusinessObject>().Object };

      searchServiceStub.Setup(stub => stub.SupportsProperty(groupProperty)).Returns(true);
      searchServiceStub.Setup(stub => stub.Search(role, groupProperty, args.Object)).Returns(expected);

      Assert.That(groupProperty.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actual = groupProperty.SearchAvailableObjects(role, args.Object);
      Assert.That(actual, Is.SameAs(expected));
    }

    [Test]
    public void SearchUsers ()
    {
      var searchServiceStub = new Mock<ISearchAvailableObjectsService>();
      var args = new Mock<ISearchAvailableObjectsArguments>();

      BusinessObjectProvider.SetProvider(typeof(BindableDomainObjectProviderAttribute), null);
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>()
          .AddService(typeof(UserPropertyTypeSearchService), searchServiceStub.Object);
      IBusinessObjectClass roleClass = BindableObjectProviderTestHelper.GetBindableObjectClass(typeof(Role));
      IBusinessObjectReferenceProperty userProperty = (IBusinessObjectReferenceProperty)roleClass.GetPropertyDefinition("User");
      Assert.That(userProperty, Is.Not.Null);

      Role role = Role.NewObject();
      var expected = new[] { new Mock<IBusinessObject>().Object };

      searchServiceStub.Setup(stub => stub.SupportsProperty(userProperty)).Returns(true);
      searchServiceStub.Setup(stub => stub.Search(role, userProperty, args.Object)).Returns(expected);

      Assert.That(userProperty.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actual = userProperty.SearchAvailableObjects(role, args.Object);
      Assert.That(actual, Is.SameAs(expected));
    }

    [Test]
    public void SearchPositions ()
    {
      var searchServiceStub = new Mock<ISearchAvailableObjectsService>();
      var args = new Mock<ISearchAvailableObjectsArguments>();

      BusinessObjectProvider.SetProvider(typeof(BindableDomainObjectProviderAttribute), null);
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>()
          .AddService(typeof(RolePropertiesSearchService), searchServiceStub.Object);
      IBusinessObjectClass roleClass = BindableObjectProviderTestHelper.GetBindableObjectClass(typeof(Role));
      IBusinessObjectReferenceProperty positionProperty = (IBusinessObjectReferenceProperty)roleClass.GetPropertyDefinition("Position");
      Assert.That(positionProperty, Is.Not.Null);

      Role role = Role.NewObject();
      var expected = new[] { new Mock<IBusinessObject>().Object };

      searchServiceStub.Setup(stub => stub.SupportsProperty(positionProperty)).Returns(true);
      searchServiceStub.Setup(stub => stub.Search(role, positionProperty, args.Object)).Returns(expected);

      Assert.That(positionProperty.SupportsSearchAvailableObjects, Is.True);

      IBusinessObject[] actual = positionProperty.SearchAvailableObjects(role, args.Object);
      Assert.That(actual, Is.SameAs(expected));
    }

    [Test]
    public void GetDisplayName_WithGroupAndPosition ()
    {
      Group roleGroup = TestHelper.CreateGroup("RoleGroup", Guid.NewGuid().ToString(), null, null);
      User user = TestHelper.CreateUser("user", "Firstname", "Lastname", "Title", null, null);
      Position position = TestHelper.CreatePosition("Position");
      Role role = TestHelper.CreateRole(user, roleGroup, position);

      Assert.That(role.DisplayName, Is.EqualTo("Position / RoleGroup"));
    }

    [Test]
    public void GetDisplayName_WithGroup ()
    {
      Group roleGroup = TestHelper.CreateGroup("RoleGroup", Guid.NewGuid().ToString(), null, null);
      roleGroup.ShortName = "RG";
      User user = TestHelper.CreateUser("user", "Firstname", "Lastname", "Title", null, null);
      Role role = TestHelper.CreateRole(user, roleGroup, null);

      Assert.That(role.DisplayName, Is.EqualTo("? / RG (RoleGroup)"));
    }

    [Test]
    public void GetDisplayName_WithPosition ()
    {
      User user = TestHelper.CreateUser("user", "Firstname", "Lastname", "Title", null, null);
      Position position = TestHelper.CreatePosition("Position");
      Role role = TestHelper.CreateRole(user, null, position);

      Assert.That(role.DisplayName, Is.EqualTo("Position / ?"));
    }

    [Test]
    public void GetDisplayName_WithoutGroupOrPosition ()
    {
      User user = TestHelper.CreateUser("user", "Firstname", "Lastname", "Title", null, null);
      Role role = TestHelper.CreateRole(user, null, null);

      Assert.That(role.DisplayName, Is.EqualTo("? / ?"));
    }
  }
}
