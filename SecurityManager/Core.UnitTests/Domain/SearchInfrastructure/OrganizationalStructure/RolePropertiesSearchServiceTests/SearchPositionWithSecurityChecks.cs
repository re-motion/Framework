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
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Security;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding;
using Remotion.Security;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure;
using Remotion.ServiceLocation;

namespace Remotion.SecurityManager.UnitTests.Domain.SearchInfrastructure.OrganizationalStructure.RolePropertiesSearchServiceTests
{
  [TestFixture]
  public class SearchPositionWithSecurityChecks : SearchServiceTestBase
  {
    private IDomainObjectHandle<Group> _expectedRootGroupHandle;
    private IDomainObjectHandle<Group> _expectedParentGroup0Handle;
    private Mock<ISecurityProvider> _mockSecurityProvider;
    private Mock<IPrincipalProvider> _mockPrincipalProvider;
    private Mock<IFunctionalSecurityStrategy> _stubFunctionalSecurityStrategy;
    private RolePropertiesSearchService _searchService;
    private IBusinessObjectReferenceProperty _positionProperty;
    private ServiceLocatorScope _serviceLocatorScope;

    public override void OneTimeSetUp ()
    {
      base.OneTimeSetUp();

      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        Tenant tenant = dbFixtures.CreateAndCommitOrganizationalStructureWithTwoTenants(ClientTransactionScope.CurrentTransaction);
        var expectedTenantHandle = tenant.GetHandle();

        var groups = Group.FindByTenant(expectedTenantHandle);
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

      _mockSecurityProvider = new Mock<ISecurityProvider>(MockBehavior.Strict);
      _mockSecurityProvider.Setup(_ => _.IsNull).Returns(false).Verifiable();
      _mockPrincipalProvider = new Mock<IPrincipalProvider>(MockBehavior.Strict);
      _stubFunctionalSecurityStrategy = new Mock<IFunctionalSecurityStrategy>(MockBehavior.Strict);

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle(() => _stubFunctionalSecurityStrategy.Object);
      serviceLocator.RegisterSingle(() => _mockSecurityProvider.Object);
      serviceLocator.RegisterSingle(() => _mockPrincipalProvider.Object);
      _serviceLocatorScope = new ServiceLocatorScope(serviceLocator);

      _searchService = new RolePropertiesSearchService();

      IBusinessObjectClass roleClass = BindableObjectProviderTestHelper.GetBindableObjectClass(typeof(Role));
      _positionProperty = (IBusinessObjectReferenceProperty)roleClass.GetPropertyDefinition("Position");
      Assert.That(_positionProperty, Is.Not.Null);
    }

    public override void TearDown ()
    {
      base.TearDown();

      _serviceLocatorScope.Dispose();
    }

    [Test]
    public void Search_WithoutGroupType_AndWithSecurityProvider ()
    {
      var principalStub = new Mock<ISecurityPrincipal>();
      principalStub.Setup(_ => _.User).Returns("group0/user1");
      _mockPrincipalProvider.Setup(_ => _.GetPrincipal()).Returns(principalStub.Object).Verifiable();
      SetupResultSecurityProviderGetAccessForPosition(Delegation.Enabled, principalStub.Object, SecurityManagerAccessTypes.AssignRole);
      SetupResultSecurityProviderGetAccessForPosition(Delegation.Disabled, principalStub.Object);
      Group rootGroup = _expectedRootGroupHandle.GetObject();

      var positions = _searchService.Search(null, _positionProperty, CreateSearchArguments(rootGroup));

      _stubFunctionalSecurityStrategy.Verify();
      _mockPrincipalProvider.Verify();
      Assert.That(positions.Length, Is.EqualTo(2));
      foreach (string positionName in new[] { "Official", "Global" })
      {
        Assert.That(
// ReSharper disable AccessToModifiedClosure
            Array.Exists(positions, current => positionName == ((Position)current).Name),
// ReSharper restore AccessToModifiedClosure
            Is.True,
            $"Position '{positionName}' was not found.");
      }
    }

    [Test]
    public void Search_WithGroupType_AndWithSecurityProvider ()
    {
      var principalStub = new Mock<ISecurityPrincipal>();
      principalStub.Setup(_ => _.User).Returns("group0/user1");
      _mockPrincipalProvider.Setup(_ => _.GetPrincipal()).Returns(principalStub.Object).Verifiable();
      SetupResultSecurityProviderGetAccessForPosition(Delegation.Enabled, principalStub.Object, SecurityManagerAccessTypes.AssignRole);
      SetupResultSecurityProviderGetAccessForPosition(Delegation.Disabled, principalStub.Object);
      Group parentGroup = _expectedParentGroup0Handle.GetObject();

      var positions = _searchService.Search(null, _positionProperty, CreateSearchArguments(parentGroup));

      _stubFunctionalSecurityStrategy.Verify();
      _mockPrincipalProvider.Verify();
      Assert.That(positions.Length, Is.EqualTo(1));
      Assert.That(((Position)positions[0]).Name, Is.EqualTo("Official"));
    }

    [Test]
    public void Search_UsesSecurityFreeSectionToRetrieveGroupType ()
    {
      ClientTransaction.Current.Extensions.Add(new SecurityClientTransactionExtension());
      Group parentGroup = _expectedParentGroup0Handle.GetObject();

      var principalStub = new Mock<ISecurityPrincipal>();
      principalStub.Setup(_ => _.User).Returns("group0/user1");
      _mockPrincipalProvider.Setup(_ => _.GetPrincipal()).Returns(principalStub.Object).Verifiable();
      SetupResultSecurityProviderGetAccessForGroup(parentGroup, principalStub.Object);
      SetupResultSecurityProviderGetAccessForPosition(Delegation.Enabled, principalStub.Object, SecurityManagerAccessTypes.AssignRole, GeneralAccessTypes.Find);
      SetupResultSecurityProviderGetAccessForPosition(Delegation.Disabled, principalStub.Object, GeneralAccessTypes.Find);

      var positions = _searchService.Search(null, _positionProperty, CreateSearchArguments(parentGroup));

      _stubFunctionalSecurityStrategy.Verify();
      _mockPrincipalProvider.Verify();

      ClientTransaction.Current.Extensions.Remove(new SecurityClientTransactionExtension().Key);

      Assert.That(positions.Length, Is.EqualTo(1));
      Assert.That(((Position)positions[0]).Name, Is.EqualTo("Official"));
    }

    private void SetupResultSecurityProviderGetAccessForPosition (Delegation delegation, ISecurityPrincipal principal, params Enum[] returnedAccessTypeEnums)
    {
      Type classType = typeof(Position);
      string owner = string.Empty;
      string owningGroup = string.Empty;
      string owningTenant = string.Empty;
      Dictionary<string, Enum> states = new Dictionary<string, Enum>();
      states.Add("Delegation", delegation);
      List<Enum> abstractRoles = new List<Enum>();
      SecurityContext securityContext = SecurityContext.Create(classType, owner, owningGroup, owningTenant, states, abstractRoles);

      AccessType[] returnedAccessTypes = Array.ConvertAll(returnedAccessTypeEnums, AccessType.Get);

      _mockSecurityProvider.Setup(_ => _.GetAccess(securityContext, principal)).Returns(returnedAccessTypes).Verifiable();
    }

    private void SetupResultSecurityProviderGetAccessForGroup (Group group, ISecurityPrincipal principal, params Enum[] returnedAccessTypeEnums)
    {
      ISecurityContext securityContext = ((IDomainObjectSecurityContextFactory)group).CreateSecurityContext();

      AccessType[] returnedAccessTypes = Array.ConvertAll(returnedAccessTypeEnums, AccessType.Get);

      _mockSecurityProvider.Setup(_ => _.GetAccess(securityContext, principal)).Returns(returnedAccessTypes).Verifiable();
    }

    private ISearchAvailableObjectsArguments CreateSearchArguments (Group group)
    {
      if (group == null)
        return null;
      return new RolePropertiesSearchArguments(group.GetHandle());
    }
  }
}
