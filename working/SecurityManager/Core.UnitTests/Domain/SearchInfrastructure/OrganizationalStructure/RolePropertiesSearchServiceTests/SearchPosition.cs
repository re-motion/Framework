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
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.SearchInfrastructure.OrganizationalStructure.RolePropertiesSearchServiceTests
{
  [TestFixture]
  public class SearchPosition : SearchServiceTestBase
  {
    private IDomainObjectHandle<Group> _expectedRootGroupHandle;
    private IDomainObjectHandle<Group> _expectedParentGroup0Handle;
    private RolePropertiesSearchService _searchService;
    private IBusinessObjectReferenceProperty _positionProperty;

    public override void TestFixtureSetUp ()
    {
      base.TestFixtureSetUp();

      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        Tenant tenant = dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants (ClientTransactionScope.CurrentTransaction);
        var expectedTenantHandle = tenant.GetHandle();

        var groups = Group.FindByTenant (expectedTenantHandle);
        foreach (Group group in groups)
        {
          if (group.UniqueIdentifier == "UID: rootGroup")
            _expectedRootGroupHandle = group.GetHandle();
          else if (group.UniqueIdentifier == "UID: parentGroup0")
            _expectedParentGroup0Handle = group.GetHandle();
        }
      }
    }

    public override void SetUp ()
    {
      base.SetUp();

      _searchService = new RolePropertiesSearchService();

      IBusinessObjectClass roleClass = BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (Role));
      _positionProperty = (IBusinessObjectReferenceProperty) roleClass.GetPropertyDefinition ("Position");
      Assert.That (_positionProperty, Is.Not.Null);
    }

    [Test]
    public void Search_WithoutGroup ()
    {
      var positions = _searchService.Search (null, _positionProperty, CreateSearchArguments (null));

      Assert.That (positions.Length, Is.EqualTo (3));
    }

    [Test]
    public void Search_WithGroupType ()
    {
      Group parentGroup = _expectedParentGroup0Handle.GetObject();

       var positions = _searchService.Search (null, _positionProperty, CreateSearchArguments (parentGroup));

      Assert.That (positions.Length, Is.EqualTo (2));
      foreach (string positionName in new[] { "Official", "Manager" })
      {
        Assert.IsTrue (
// ReSharper disable AccessToModifiedClosure
            Array.Exists (positions, current => positionName == ((Position) current).Name),
// ReSharper restore AccessToModifiedClosure
            "Position '{0}' was not found.",
            positionName);
      }
    }

    [Test]
    public void Search_WithoutGroupType ()
    {
      Group rootGroup = _expectedRootGroupHandle.GetObject();

       var positions = _searchService.Search (null, _positionProperty, CreateSearchArguments (rootGroup));

      Assert.That (positions.Length, Is.EqualTo (3));
    }

    private ISearchAvailableObjectsArguments CreateSearchArguments (Group group)
    {
      if (group == null)
        return null;
      return new RolePropertiesSearchArguments (group.GetHandle());
    }
  }
}
