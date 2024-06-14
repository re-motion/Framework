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
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.SearchInfrastructure;
using Remotion.SecurityManager.Domain.SearchInfrastructure.Metadata;
using Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.SearchInfrastructure.Metadata.AbstractRoleDefinitionPropertyTypeSearchServiceTests
{
  [TestFixture]
  public class SearchAbstractRoleDefinition : DomainTest
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
      IBusinessObjectClass aceClass = BindableObjectProviderTestHelper.GetBindableObjectClass(typeof(AccessControlEntry));
      _property = (IBusinessObjectReferenceProperty)aceClass.GetPropertyDefinition("SpecificAbstractRole");
      Assert.That(_property, Is.Not.Null);
    }

    [Test]
    public void SupportsProperty ()
    {
      Assert.That(_searchService.SupportsProperty(_property), Is.True);
    }

    [Test]
    public void Search ()
    {
      var expected = AbstractRoleDefinition.FindAll().ToArray();
      Assert.That(expected, Is.Not.Empty);

      IBusinessObject[] actual = _searchService.Search(null, _property, CreateSecurityManagerSearchArguments(null));

      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void Search_WithDisplayNameConstraint_FindDisplayNameContainingPrefix ()
    {
      var expected = AbstractRoleDefinition.FindAll().AsEnumerable().Where(r => r.DisplayName.Contains("QualityManager")).ToArray();
      Assert.That(expected.Length, Is.EqualTo(1));

      var actual = _searchService.Search(null, _property, CreateSecurityManagerSearchArguments("Manager|"));

      Assert.That(actual, Is.EquivalentTo(expected));
    }

    [Test]
    public void Search_WithDisplayNameConstraint_FindDisplayNameCaseInsensitive ()
    {
      var expected = AbstractRoleDefinition.FindAll().AsEnumerable().Where(r => r.DisplayName.Contains("QualityManager")).ToArray();
      Assert.That(expected.Length, Is.EqualTo(1));

      var actual = _searchService.Search(null, _property, CreateSecurityManagerSearchArguments("qualitymanager"));

      Assert.That(actual, Is.EquivalentTo(expected));
    }

    [Test]
    public void Search_WithDisplayNameConstraint_DontMatchOtherValue ()
    {
      var expected = AbstractRoleDefinition.FindAll().AsEnumerable().ToArray();
      Assert.That(expected.Length, Is.EqualTo(2));

      var actual = _searchService.Search(null, _property, CreateSecurityManagerSearchArguments("QualityManager"));

      Assert.That(actual.Length, Is.EqualTo(1));
    }

    private SecurityManagerSearchArguments CreateSecurityManagerSearchArguments (string displayName)
    {
      return new SecurityManagerSearchArguments(
          null,
          null,
          !string.IsNullOrEmpty(displayName) ? new DisplayNameConstraint(displayName) : null);
    }
  }
}
