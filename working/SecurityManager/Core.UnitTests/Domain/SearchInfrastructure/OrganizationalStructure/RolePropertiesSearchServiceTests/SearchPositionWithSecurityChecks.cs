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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Security;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding;
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure;
using Remotion.ServiceLocation;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain.SearchInfrastructure.OrganizationalStructure.RolePropertiesSearchServiceTests
{
  [TestFixture]
  public class SearchPositionWithSecurityChecks : SearchServiceTestBase
  {
    private IDomainObjectHandle<Group> _expectedRootGroupHandle;
    private IDomainObjectHandle<Group> _expectedParentGroup0Handle;
    private MockRepository _mocks;
    private ISecurityProvider _mockSecurityProvider;
    private IPrincipalProvider _mockPrincipalProvider;
    private IFunctionalSecurityStrategy _stubFunctionalSecurityStrategy;
    private RolePropertiesSearchService _searchService;
    private IBusinessObjectReferenceProperty _positionProperty;
    private ServiceLocatorScope _serviceLocatorScope;

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

      _mocks = new MockRepository();
      _mockSecurityProvider = (ISecurityProvider) _mocks.StrictMock (typeof (ISecurityProvider));
      SetupResult.For (_mockSecurityProvider.IsNull).Return (false);
      _mockPrincipalProvider = (IPrincipalProvider) _mocks.StrictMock (typeof (IPrincipalProvider));
      _stubFunctionalSecurityStrategy = _mocks.StrictMock<IFunctionalSecurityStrategy>();

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle (() => _stubFunctionalSecurityStrategy);
      serviceLocator.RegisterSingle (() => _mockSecurityProvider);
      serviceLocator.RegisterSingle (() => _mockPrincipalProvider);
      _serviceLocatorScope = new ServiceLocatorScope (serviceLocator);

      _searchService = new RolePropertiesSearchService();

      IBusinessObjectClass roleClass = BindableObjectProviderTestHelper.GetBindableObjectClass (typeof (Role));
      _positionProperty = (IBusinessObjectReferenceProperty) roleClass.GetPropertyDefinition ("Position");
      Assert.That (_positionProperty, Is.Not.Null);
    }

    public override void TearDown ()
    {
      base.TearDown();

      _serviceLocatorScope.Dispose();
    }

    [Test]
    public void Search_WithoutGroupType_AndWithSecurityProvider ()
    {
      var principalStub = _mocks.Stub<ISecurityPrincipal> ();
      SetupResult.For (principalStub.User).Return ("group0/user1");
      SetupResult.For (_mockPrincipalProvider.GetPrincipal ()).Return (principalStub);
      SetupResultSecurityProviderGetAccessForPosition (Delegation.Enabled, principalStub, SecurityManagerAccessTypes.AssignRole);
      SetupResultSecurityProviderGetAccessForPosition (Delegation.Disabled, principalStub);
      Group rootGroup = _expectedRootGroupHandle.GetObject();
      _mocks.ReplayAll();

      var positions = _searchService.Search (null, _positionProperty, CreateSearchArguments (rootGroup));

      _mocks.VerifyAll();
      Assert.That (positions.Length, Is.EqualTo (2));
      foreach (string positionName in new[] { "Official", "Global" })
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
    public void Search_WithGroupType_AndWithSecurityProvider ()
    {
      var principalStub = _mocks.Stub<ISecurityPrincipal> ();
      SetupResult.For (principalStub.User).Return ("group0/user1");
      SetupResult.For (_mockPrincipalProvider.GetPrincipal ()).Return (principalStub);
      SetupResultSecurityProviderGetAccessForPosition (Delegation.Enabled, principalStub, SecurityManagerAccessTypes.AssignRole);
      SetupResultSecurityProviderGetAccessForPosition (Delegation.Disabled, principalStub);
      Group parentGroup = _expectedParentGroup0Handle.GetObject();
      _mocks.ReplayAll();

      var positions = _searchService.Search (null, _positionProperty, CreateSearchArguments (parentGroup));

      _mocks.VerifyAll();
      Assert.That (positions.Length, Is.EqualTo (1));
      Assert.That (((Position) positions[0]).Name, Is.EqualTo ("Official"));
    }

    [Test]
    public void Search_UsesSecurityFreeSectionToRetrieveGroupType ()
    {
      ClientTransaction.Current.Extensions.Add (new SecurityClientTransactionExtension());
      Group parentGroup = _expectedParentGroup0Handle.GetObject();

      var principalStub = _mocks.Stub<ISecurityPrincipal> ();
      SetupResult.For (principalStub.User).Return ("group0/user1");
      SetupResult.For (_mockPrincipalProvider.GetPrincipal()).Return (principalStub);
      SetupResultSecurityProviderGetAccessForGroup (parentGroup, principalStub);
      SetupResultSecurityProviderGetAccessForPosition (Delegation.Enabled, principalStub, SecurityManagerAccessTypes.AssignRole, GeneralAccessTypes.Find);
      SetupResultSecurityProviderGetAccessForPosition (Delegation.Disabled, principalStub, GeneralAccessTypes.Find);
      _mocks.ReplayAll();

      var positions = _searchService.Search (null, _positionProperty, CreateSearchArguments (parentGroup));

      _mocks.VerifyAll();

      ClientTransaction.Current.Extensions.Remove (new SecurityClientTransactionExtension().Key);

      Assert.That (positions.Length, Is.EqualTo (1));
      Assert.That (((Position) positions[0]).Name, Is.EqualTo ("Official"));
    }

    private void SetupResultSecurityProviderGetAccessForPosition (Delegation delegation, ISecurityPrincipal principal, params Enum[] returnedAccessTypeEnums)
    {
      Type classType = typeof (Position);
      string owner = string.Empty;
      string owningGroup = string.Empty;
      string owningTenant = string.Empty;
      Dictionary<string, Enum> states = new Dictionary<string, Enum>();
      states.Add ("Delegation", delegation);
      List<Enum> abstractRoles = new List<Enum>();
      SecurityContext securityContext = SecurityContext.Create (classType, owner, owningGroup, owningTenant, states, abstractRoles);

      AccessType[] returnedAccessTypes = Array.ConvertAll (returnedAccessTypeEnums, AccessType.Get);

      SetupResult.For (_mockSecurityProvider.GetAccess (securityContext, principal)).Return (returnedAccessTypes);
    }

    private void SetupResultSecurityProviderGetAccessForGroup (Group group, ISecurityPrincipal principal, params Enum[] returnedAccessTypeEnums)
    {
      ISecurityContext securityContext = ((IDomainObjectSecurityContextFactory) group).CreateSecurityContext();

      AccessType[] returnedAccessTypes = Array.ConvertAll (returnedAccessTypeEnums, AccessType.Get);

      SetupResult.For (_mockSecurityProvider.GetAccess (securityContext, principal)).Return (returnedAccessTypes);
    }

    private ISearchAvailableObjectsArguments CreateSearchArguments (Group group)
    {
      if (group == null)
        return null;
      return new RolePropertiesSearchArguments (group.GetHandle());
    }
  }
}
