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

namespace Remotion.SecurityManager.UnitTests.Domain.SearchInfrastructure.OrganizationalStructure.SubstitutionPropertiesSearchServiceTests
{
  [TestFixture]
  public class SearchOtherProperty : SearchServiceTestBase
  {
    private ISearchAvailableObjectsService _searchService;
    private IBusinessObjectReferenceProperty _otherProperty;

    public override void SetUp ()
    {
      base.SetUp();

      _searchService = new SubstitutionPropertiesSearchService();
      IBusinessObjectClass substitutionClass = BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (Substitution));
      _otherProperty = (IBusinessObjectReferenceProperty) substitutionClass.GetPropertyDefinition ("SubstitutedUser");
      Assert.That (_otherProperty, Is.Not.Null);
    }

    [Test]
    public void SupportsProperty_WithInvalidProperty ()
    {
      Assert.That (_searchService.SupportsProperty (_otherProperty), Is.False);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The property 'SubstitutedUser' is not supported by the "
        + "'Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure.SubstitutionPropertiesSearchService' type.",
        MatchType = MessageMatch.Contains)]
    public void Search_WithInvalidProperty ()
    {
      _searchService.Search (null, _otherProperty, null);
    }
  }
}