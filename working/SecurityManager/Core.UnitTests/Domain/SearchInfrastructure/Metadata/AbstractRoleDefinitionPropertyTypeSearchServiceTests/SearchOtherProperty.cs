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
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.SearchInfrastructure.Metadata;
using Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.SearchInfrastructure.Metadata.AbstractRoleDefinitionPropertyTypeSearchServiceTests
{
  [TestFixture]
  public class SearchOtherProperty : DomainTest
  {
    private OrganizationalStructureTestHelper _testHelper;
    private ISearchAvailableObjectsService _searchService;
    private IBusinessObjectReferenceProperty _property;

    public override void SetUp ()
    {
      base.SetUp();

      _testHelper = new OrganizationalStructureTestHelper();
      _testHelper.Transaction.EnterNonDiscardingScope();

      _searchService = new AbstractRoleDefinitionPropertyTypeSearchService();
      IBusinessObjectClass aceClass = BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (AccessControlEntry));
      _property = (IBusinessObjectReferenceProperty) aceClass.GetPropertyDefinition ("AccessControlList");
      Assert.That (_property, Is.Not.Null);
    }

    [Test]
    public void SupportsProperty_WithInvalidProperty ()
    {
      Assert.That (_searchService.SupportsProperty (_property), Is.False);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The type of the property 'AccessControlList', declared on "
        + "'Remotion.SecurityManager.Domain.AccessControl.AccessControlEntry, Remotion.SecurityManager', is not supported by the "
        + "'Remotion.SecurityManager.Domain.SearchInfrastructure.Metadata.AbstractRoleDefinitionPropertyTypeSearchService' type.",
        MatchType = MessageMatch.Contains)]
    public void Search_WithInvalidProperty ()
    {
      _searchService.Search (null, _property, null);
    }
  }
}