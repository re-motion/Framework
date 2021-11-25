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
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.SubstitutionTests
{
  [TestFixture]
  public class BindableObjectImplementation : SubstitutionTestBase
  {
    public override void TearDown ()
    {
      base.TearDown();
      BusinessObjectProvider.SetProvider(typeof(BindableDomainObjectProviderAttribute), null);
    }

    [Test]
    public void SearchSubstitutingUsers ()
    {
      var searchServiceStub = new Mock<ISearchAvailableObjectsService>();
      var args = new Mock<ISearchAvailableObjectsArguments>();

      BusinessObjectProvider.SetProvider(typeof(BindableDomainObjectProviderAttribute), null);
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>().AddService(
          typeof(UserPropertyTypeSearchService), searchServiceStub.Object);
      var substitutionClass = BindableObjectProviderTestHelper.GetBindableObjectClass(typeof(Substitution));
      var substitutingUserProperty = (IBusinessObjectReferenceProperty) substitutionClass.GetPropertyDefinition("SubstitutingUser");
      Assert.That(substitutingUserProperty, Is.Not.Null);

      Substitution substitution = Substitution.NewObject();
      var expected = new[] { new Mock<IBusinessObject>().Object };

      searchServiceStub.Setup(stub => stub.SupportsProperty(substitutingUserProperty)).Returns(true);
      searchServiceStub.Setup(stub => stub.Search(substitution, substitutingUserProperty, args.Object)).Returns(expected);

      Assert.That(substitutingUserProperty.SupportsSearchAvailableObjects, Is.True);

      var actual = substitutingUserProperty.SearchAvailableObjects(substitution, args.Object);
      Assert.That(actual, Is.SameAs(expected));
    }

    [Test]
    public void SearchSubstitutedRoles ()
    {
      var searchServiceStub = new Mock<ISearchAvailableObjectsService>();
      var args = new Mock<ISearchAvailableObjectsArguments>();

      BusinessObjectProvider.SetProvider(typeof(BindableDomainObjectProviderAttribute), null);
      BusinessObjectProvider.GetProvider<BindableDomainObjectProviderAttribute>().AddService(
          typeof(SubstitutionPropertiesSearchService), searchServiceStub.Object);
      var substitutionClass = BindableObjectProviderTestHelper.GetBindableObjectClass(typeof(Substitution));
      var substitutedRoleProperty = (IBusinessObjectReferenceProperty) substitutionClass.GetPropertyDefinition("SubstitutedRole");
      Assert.That(substitutedRoleProperty, Is.Not.Null);

      Substitution substitution = Substitution.NewObject();
      var expected = new[] { new Mock<IBusinessObject>().Object };

      searchServiceStub.Setup(stub => stub.SupportsProperty(substitutedRoleProperty)).Returns(true);
      searchServiceStub.Setup(stub => stub.Search(substitution, substitutedRoleProperty, args.Object)).Returns(expected);

      Assert.That(substitutedRoleProperty.SupportsSearchAvailableObjects, Is.True);

      var actual = substitutedRoleProperty.SearchAvailableObjects(substitution, args.Object);
      Assert.That(actual, Is.SameAs(expected));
    }

    [Test]
    public void GetDisplayName_WithSubstitutedUser ()
    {
      User user = TestHelper.CreateUser("user", "Firstname", "Lastname", "Title", null, null);
      Substitution substitution = Substitution.NewObject();
      substitution.SubstitutedUser = user;

      Assert.That(substitution.DisplayName, Is.EqualTo("Lastname Firstname, Title"));
    }

    [Test]
    public void GetDisplayName_WithSubstitutedUserAndSubstitutedRole ()
    {
      Group roleGroup = TestHelper.CreateGroup("RoleGroup", Guid.NewGuid().ToString(), null, null);
      User user = TestHelper.CreateUser("user", "Firstname", "Lastname", "Title", null, null);
      Position position = TestHelper.CreatePosition("Position");
      Role role = TestHelper.CreateRole(user, roleGroup, position);
      Substitution substitution = Substitution.NewObject();
      substitution.SubstitutedUser = user;
      substitution.SubstitutedRole = role;

      Assert.That(substitution.DisplayName, Is.EqualTo("Lastname Firstname, Title (Position / RoleGroup)"));
    }

    [Test]
    public void GetDisplayName_WithoutSubstitutedUser ()
    {
      Substitution substitution = Substitution.NewObject();
      substitution.SubstitutedUser = null;

      Assert.That(substitution.DisplayName, Is.EqualTo("?"));
    }

    [Test]
    public void GetDisplayName_WithoutSubstitutedUserAndWithSubstitutedRole ()
    {
      Group roleGroup = TestHelper.CreateGroup("RoleGroup", Guid.NewGuid().ToString(), null, null);
      User user = TestHelper.CreateUser("user", "Firstname", "Lastname", "Title", null, null);
      Position position = TestHelper.CreatePosition("Position");
      Role role = TestHelper.CreateRole(user, roleGroup, position);
      Substitution substitution = Substitution.NewObject();
      substitution.SubstitutedRole = role;

      Assert.That(substitution.DisplayName, Is.EqualTo("? (Position / RoleGroup)"));
    }
  }
}
