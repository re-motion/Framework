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
using Remotion.Data.DomainObjects;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.SearchInfrastructure.OrganizationalStructure.SubstitutionPropertiesSearchServiceTests
{
  [TestFixture]
  public class SearchSubstitutedRole : SearchServiceTestBase
  {
    private ISearchAvailableObjectsService _searchService;
    private IBusinessObjectReferenceProperty _substitutedRoleProperty;
    private User _user;

    public override void SetUp ()
    {
      base.SetUp();

      _searchService = new SubstitutionPropertiesSearchService();
      IBusinessObjectClass substitutionClass = BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (Substitution));
      _substitutedRoleProperty = (IBusinessObjectReferenceProperty) substitutionClass.GetPropertyDefinition ("SubstitutedRole");
      Assert.That (_substitutedRoleProperty, Is.Not.Null);

      _user = User.FindByUserName ("group0/user1");
      Assert.That (_user, Is.Not.Null);
    }

    [Test]
    public void SupportsProperty ()
    {
      Assert.That (_searchService.SupportsProperty (_substitutedRoleProperty), Is.True);
    }

    [Test]
    public void Search ()
    {
      DomainObjectCollection expectedRoles = _user.Roles;
      Assert.That (expectedRoles, Is.Not.Empty);
      Substitution substitution = Substitution.NewObject();
      substitution.SubstitutedUser = _user;

      IBusinessObject[] actualRoles = _searchService.Search (substitution, _substitutedRoleProperty, new DefaultSearchArguments (null));

      Assert.That (actualRoles, Is.EquivalentTo (expectedRoles));
    }

    [Test]
    public void Search_WithSubstitutionHasNoSubstitutedUser ()
    {
      Substitution substitution = Substitution.NewObject ();
      substitution.SubstitutingUser = _user;

      IBusinessObject[] actualRoles = _searchService.Search (substitution, _substitutedRoleProperty, new DefaultSearchArguments (null));

      Assert.That (actualRoles, Is.Empty);
    }
  }
}
