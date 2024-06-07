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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.Domain.SearchInfrastructure;
using Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.SearchInfrastructure.OrganizationalStructure.UserPropertyTypeSearchServiceTests
{
  [TestFixture]
  public class SearchUser : SearchServiceTestBase
  {
    private ISearchAvailableObjectsService _searchService;
    private IBusinessObjectReferenceProperty _property;
    private TenantConstraint _tenantConstraint;

    public override void SetUp ()
    {
      base.SetUp();

      _searchService = new UserPropertyTypeSearchService();
      IBusinessObjectClass roleClass = BindableObjectProviderTestHelper.GetBindableObjectClass(typeof(Role));
      _property = (IBusinessObjectReferenceProperty)roleClass.GetPropertyDefinition("User");
      Assert.That(_property, Is.Not.Null);

      var user = User.FindByUserName("group0/user1");
      Assert.That(user, Is.Not.Null);

      _tenantConstraint = new TenantConstraint(user.Tenant.GetHandle());
    }

    [Test]
    public void SupportsProperty ()
    {
      Assert.That(_searchService.SupportsProperty(_property), Is.True);
    }

    [Test]
    public void Search ()
    {
      var expected = User.FindByTenant(_tenantConstraint.Value);
      Assert.That(expected, Is.Not.Empty);

      IBusinessObject[] actual = _searchService.Search(null, _property, CreateSecurityManagerSearchArguments(null));

      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Search_WithDisplayNameConstraint_FindLastNameContainingPrefix ()
    {
      var expected = User.FindByTenant(_tenantConstraint.Value).Where(u => u.LastName.Contains("user")).ToArray();
      Assert.That(expected.Length, Is.GreaterThan(1));

      var actual = _searchService.Search(null, _property, CreateSecurityManagerSearchArguments("user"));

      Assert.That(actual, Is.EquivalentTo(expected));
    }

    [Test]
    public void Search_WithDisplayNameConstraint_FindFirstNameContainingPrefix ()
    {
      var usersWithFirstNameNull = User.FindByTenant(_tenantConstraint.Value).AsEnumerable().Where(u => u.FirstName == null).ToArray();
      Assert.That(usersWithFirstNameNull, Is.Not.Empty);

      var expected = User.FindByTenant(_tenantConstraint.Value).Where(u => u.FirstName.Contains("est")).ToArray();
      Assert.That(expected, Is.Not.Empty);

      var actual = _searchService.Search(null, _property, CreateSecurityManagerSearchArguments("est"));

      Assert.That(actual, Is.EquivalentTo(expected));
    }

    [Test]
    public void Search_WithoutTenantConstraint_FindsNoUsers ()
    {
      IBusinessObject[] actual = _searchService.Search(
          null,
          _property,
          new SecurityManagerSearchArguments(null, null, new DisplayNameConstraint("user")));

      Assert.That(actual, Is.Empty);
    }

    private SecurityManagerSearchArguments CreateSecurityManagerSearchArguments (string displayName)
    {
      return new SecurityManagerSearchArguments(
          _tenantConstraint,
          null,
          !string.IsNullOrEmpty(displayName) ? new DisplayNameConstraint(displayName) : null);
    }
  }
}
