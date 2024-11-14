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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Development.UnitTesting;
using Remotion.Security;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.ServiceLocation;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.GroupTests
{
  [TestFixture]
  public class GetHierarchy : GroupTestBase
  {
    [Test]
    public void Test_NoChildren ()
    {
      Tenant tenant = TestHelper.CreateTenant("Tenant", "UID: Tenant");
      Group root = TestHelper.CreateGroup("Root", "UID: Root", null, tenant);

      var groups = root.GetHierachy().ToArray();

      Assert.That(groups, Is.EquivalentTo(new[] { root }));
    }

    [Test]
    public void Test_NoGrandChildren ()
    {
      Tenant tenant = TestHelper.CreateTenant("Tenant", "UID: Tenant");
      Group root = TestHelper.CreateGroup("Root", "UID: Root", null, tenant);
      Group child1 = TestHelper.CreateGroup("Child1", "UID: Child1", root, tenant);
      Group child2 = TestHelper.CreateGroup("Child2", "UID: Child2", root, tenant);

      var groups = root.GetHierachy().ToArray();

      Assert.That(groups, Is.EquivalentTo(new[] { root, child1, child2 }));
    }

    [Test]
    public void Test_WithGrandChildren ()
    {
      Tenant tenant = TestHelper.CreateTenant("Tenant", "UID: Tenant");
      Group root = TestHelper.CreateGroup("Root", "UID: Root", null, tenant);
      Group child1 = TestHelper.CreateGroup("Child1", "UID: Child1", root, tenant);
      Group child2 = TestHelper.CreateGroup("Child2", "UID: Child2", root, tenant);
      Group grandChild1 = TestHelper.CreateGroup("GrandChild1", "UID: GrandChild1", child1, tenant);

      var groups = root.GetHierachy().ToArray();

      Assert.That(groups, Is.EquivalentTo(new[] { root, child1, child2, grandChild1 }));
    }

    [Test]
    public void Test_WithCircularHierarchy_ThrowsInvalidOperationException ()
    {
      Tenant tenant = TestHelper.CreateTenant("Tenant", "UID: Tenant");
      Group root = TestHelper.CreateGroup("Root", "UID: Root", null, tenant);
      Group child1 = TestHelper.CreateGroup("Child1", "UID: Child1", root, tenant);
      Group grandChild1 = TestHelper.CreateGroup("GrandChild1", "UID: GrandChild1", child1, tenant);
      Group grandChild2 = TestHelper.CreateGroup("GrandChild2", "UID: GrandChild2", grandChild1, tenant);
      root.Parent = grandChild2;

      Assert.That(
          () => grandChild1.GetHierachy().ToArray(),
          Throws.InvalidOperationException
                .With.Message.EqualTo("The hierarchy for group '" + grandChild1.ID + "' cannot be resolved because a circular reference exists."));
    }

    [Test]
    public void Test_WithCircularHierarchy_GroupIsOwnChild_ThrowsInvalidOperationException ()
    {
      Tenant tenant = TestHelper.CreateTenant("Tenant", "UID: Tenant");
      Group root = TestHelper.CreateGroup("Root", "UID: Root", null, tenant);
      root.Parent = root;

      Assert.That(
          () => root.GetHierachy().ToArray(),
          Throws.InvalidOperationException
                .With.Message.EqualTo("The hierarchy for group '" + root.ID + "' cannot be resolved because a circular reference exists."));
    }

    [Test]
    public void Test_WithSecurity_PermissionDeniedOnChild ()
    {
      Tenant tenant = TestHelper.CreateTenant("Tenant", "UID: Tenant");
      Group root = TestHelper.CreateGroup("Root", "UID: Root", null, tenant);
      Group child1 = TestHelper.CreateGroup("Child1", "UID: Child1", root, tenant);
      Group child2 = TestHelper.CreateGroup("Child2", "UID: Child2", root, tenant);
      Group child1_1 = TestHelper.CreateGroup("GrandChild1.1", "UID: GrandChild1.1", child1, tenant);
      Group child2_1 = TestHelper.CreateGroup("GrandChild2.1", "UID: GrandChild2.1", child2, tenant);

      SimulateExistingObjectForSecurityTest(tenant);
      SimulateExistingObjectForSecurityTest(root);
      SimulateExistingObjectForSecurityTest(child1);
      SimulateExistingObjectForSecurityTest(child2);
      SimulateExistingObjectForSecurityTest(child1_1);
      SimulateExistingObjectForSecurityTest(child2_1);

      var securityProviderStub = new Mock<ISecurityProvider>();

      var childOfChild2SecurityContext = ((ISecurityContextFactory)child2_1).CreateSecurityContext();
      securityProviderStub
          .Setup(stub => stub.GetAccess(It.Is<ISecurityContext>(_ => !object.Equals(_, childOfChild2SecurityContext)), It.IsAny<ISecurityPrincipal>()))
          .Returns(new[] { AccessType.Get(GeneralAccessTypes.Read) });
      securityProviderStub
          .Setup(stub => stub.GetAccess(childOfChild2SecurityContext, It.IsAny<ISecurityPrincipal>()))
          .Returns(new AccessType[0]);

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle(() => securityProviderStub.Object);
      serviceLocator.RegisterSingle<IPrincipalProvider>(() => new NullPrincipalProvider());
      using (new ServiceLocatorScope(serviceLocator))
      {
        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          var groups = root.GetHierachy().ToArray();

          Assert.That(groups, Is.EquivalentTo(new[] { root, child1, child2, child1_1 }));
        }
      }
    }

    [Test]
    public void Test_WithSecurity_PermissionDeniedOnRoot ()
    {
      Tenant tenant = TestHelper.CreateTenant("Tenant", "UID: Tenant");
      Group root = TestHelper.CreateGroup("Root", "UID: Root", null, tenant);

      SimulateExistingObjectForSecurityTest(tenant);
      SimulateExistingObjectForSecurityTest(root);

      var securityProviderStub = new Mock<ISecurityProvider>();
      securityProviderStub
          .Setup(stub => stub.GetAccess(It.IsAny<SecurityContext>(), It.IsAny<ISecurityPrincipal>()))
          .Returns(new AccessType[0]);

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle(() => securityProviderStub.Object);
      serviceLocator.RegisterSingle<IPrincipalProvider>(() => new NullPrincipalProvider());
      using (new ServiceLocatorScope(serviceLocator))
      {
        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          Assert.That(root.GetHierachy(), Is.Empty);
        }
      }
    }
  }
}
