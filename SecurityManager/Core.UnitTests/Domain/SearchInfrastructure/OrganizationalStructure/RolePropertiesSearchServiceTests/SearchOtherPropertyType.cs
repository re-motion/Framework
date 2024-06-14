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
using NUnit.Framework;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.SearchInfrastructure.OrganizationalStructure.RolePropertiesSearchServiceTests
{
  [TestFixture]
  public class SearchOtherProperty : SearchServiceTestBase
  {
    private ISearchAvailableObjectsService _searchService;
    private IBusinessObjectReferenceProperty _tenantProperty;

    public override void SetUp ()
    {
      base.SetUp();

      _searchService = new RolePropertiesSearchService();
      IBusinessObjectClass userClass = BindableObjectProviderTestHelper.GetBindableObjectClass(typeof(User));
      _tenantProperty = (IBusinessObjectReferenceProperty)userClass.GetPropertyDefinition("Tenant");
      Assert.That(_tenantProperty, Is.Not.Null);
    }

    [Test]
    public void SupportsProperty_WithInvalidProperty ()
    {
      Assert.That(_searchService.SupportsProperty(_tenantProperty), Is.False);
    }

    [Test]
    public void Search_WithInvalidProperty ()
    {
      Assert.That(
          () => _searchService.Search(null, _tenantProperty, null),
          Throws.ArgumentException
              .With.Message.Contains(
                  "The property 'Tenant' is not supported by the "
                  + "'Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure.RolePropertiesSearchService' type."));
    }
  }
}
