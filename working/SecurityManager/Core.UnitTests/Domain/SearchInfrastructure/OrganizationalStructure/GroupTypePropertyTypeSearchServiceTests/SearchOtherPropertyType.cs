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
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.SearchInfrastructure.OrganizationalStructure.GroupTypePropertyTypeSearchServiceTests
{
  [TestFixture]
  public class SearchOtherProperty : SearchServiceTestBase
  {
    private ISearchAvailableObjectsService _searchService;
    private IBusinessObjectReferenceProperty _tenantProperty;

    public override void SetUp ()
    {
      base.SetUp();

      _searchService = new GroupTypePropertyTypeSearchService();
      IBusinessObjectClass userClass = BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (User));
      _tenantProperty = (IBusinessObjectReferenceProperty) userClass.GetPropertyDefinition ("Tenant");
      Assert.That (_tenantProperty, Is.Not.Null);
    }

    [Test]
    public void SupportsProperty_WithInvalidProperty ()
    {
      Assert.That (_searchService.SupportsProperty (_tenantProperty), Is.False);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The type of the property 'Tenant', declared on 'Remotion.SecurityManager.Domain.OrganizationalStructure.User, Remotion.SecurityManager', "
        + "is not supported by the 'Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure.GroupTypePropertyTypeSearchService' type.",
        MatchType = MessageMatch.Contains)]
    public void Search_WithInvalidProperty ()
    {
      _searchService.Search (null, _tenantProperty, null);
    }
  }
}