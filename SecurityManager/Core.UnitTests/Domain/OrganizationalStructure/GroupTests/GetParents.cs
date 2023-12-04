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
  public class GetParents : GroupTestBase
  {
    [Test]
    public void Test_NoParents ()
    {
      Tenant tenant = TestHelper.CreateTenant("Tenant", "UID: Tenant");
      Group root = TestHelper.CreateGroup("Root", "UID: Root", null, tenant);

      var groups = root.GetParents().ToArray();

      Assert.That(groups, Is.Empty);
    }

    [Test]
    public void Test_NoGrandParents ()
    {
      Tenant tenant = TestHelper.CreateTenant("Tenant", "UID: Tenant");
      Group parent = TestHelper.CreateGroup("parent1", "UID: parent", null, tenant);
      Group root = TestHelper.CreateGroup("Root", "UID: Root", parent, tenant);

      var groups = root.GetParents().ToArray();

      Assert.That(groups, Is.EquivalentTo(new[] { parent }));
    }

    [Test]
    public void Test_WithGrandParents ()
    {
      Tenant tenant = TestHelper.CreateTenant("Tenant", "UID: Tenant");
      Group grandParent = TestHelper.CreateGroup("Grandparent1", "UID: Grandparent1", null, tenant);
      Group parent = TestHelper.CreateGroup("parent1", "UID: parent", grandParent, tenant);
      Group root = TestHelper.CreateGroup("Root", "UID: Root", parent, tenant);

      var groups = root.GetParents().ToArray();

      Assert.That(groups, Is.EquivalentTo(new[] { parent, grandParent }));
    }

    [Test]
    public void Test_WithCircularHierarchy_ThrowsInvalidOperationException ()
    {
      Tenant tenant = TestHelper.CreateTenant("Tenant", "UID: Tenant");
      Group grandParent2 = TestHelper.CreateGroup("Grandparent2", "UID: Grandparent2", null, tenant);
      Group grandParent1 = TestHelper.CreateGroup("Grandparent1", "UID: Grandparent1", grandParent2, tenant);
      Group parent = TestHelper.CreateGroup("parent1", "UID: parent", grandParent1, tenant);
      Group root = TestHelper.CreateGroup("Root", "UID: Root", parent, tenant);
      grandParent2.Parent = root;

      Assert.That(
          () => root.GetParents().Take(10).ToArray(),
          Throws.InvalidOperationException.With.Message.EqualTo(
              string.Format("The parent hierarchy for group '{0}' cannot be resolved because a circular reference exists.", root.ID)));
    }

    [Test]
    public void Test_WithCircularHierarchy_GroupIsOwnParent_ThrowsInvalidOperationException ()
    {
      Tenant tenant = TestHelper.CreateTenant("Tenant", "UID: Tenant");
      Group root = TestHelper.CreateGroup("Root", "UID: Root", null, tenant);
      root.Parent = root;

      Assert.That(
          () => root.GetParents().Take(10).ToArray(),
          Throws.InvalidOperationException.With.Message.EqualTo(
              string.Format("The parent hierarchy for group '{0}' cannot be resolved because a circular reference exists.", root.ID)));
    }

    [Test]
    public void Test_WithCircularHierarchyAboveTheRoot_ThrowsInvalidOperationException ()
    {
      Tenant tenant = TestHelper.CreateTenant("Tenant", "UID: Tenant");
      Group grandParent3 = TestHelper.CreateGroup("Grandparent2", "UID: Grandparent3", null, tenant);
      Group grandParent2 = TestHelper.CreateGroup("Grandparent3", "UID: Grandparent2", grandParent3, tenant);
      Group grandParent1 = TestHelper.CreateGroup("Grandparent1", "UID: Grandparent1", grandParent2, tenant);
      Group parent = TestHelper.CreateGroup("parent1", "UID: parent", grandParent1, tenant);
      Group root = TestHelper.CreateGroup("Root", "UID: Root", parent, tenant);
      grandParent3.Parent = grandParent1;

      Assert.That(
          () => root.GetParents().Take(10).ToArray(),
          Throws.InvalidOperationException.With.Message.EqualTo(
              string.Format("The parent hierarchy for group '{0}' cannot be resolved because a circular reference exists.", root.ID)));
    }

    [Test]
    public void Test_WithCircularHierarchyAboveTheRoot_ParentIsOwnParent_ThrowsInvalidOperationException ()
    {
      Tenant tenant = TestHelper.CreateTenant("Tenant", "UID: Tenant");
      Group grandParent = TestHelper.CreateGroup("Grandparent", "UID: Grandparent", null, tenant);
      Group parent = TestHelper.CreateGroup("Parent", "UID: Parent", grandParent, tenant);
      Group root = TestHelper.CreateGroup("Root", "UID: Root", parent, tenant);
      grandParent.Parent = grandParent;

      Assert.That(
          () => root.GetParents().Take(10).ToArray(),
          Throws.InvalidOperationException.With.Message.EqualTo(
              string.Format("The parent hierarchy for group '{0}' cannot be resolved because a circular reference exists.", root.ID)));
    }

    [Test]
    public void Test_WithSecurity_PermissionDeniedOnParent ()
    {
      Tenant tenant = TestHelper.CreateTenant("Tenant", "UID: Tenant");
      Group grandParent2 = TestHelper.CreateGroup("Grandparent1", "UID: Grandparent2", null, tenant);
      Group grandParent1 = TestHelper.CreateGroup("Grandparent1", "UID: Grandparent1", grandParent2, tenant);
      Group parent = TestHelper.CreateGroup("parent1", "UID: parent", grandParent1, tenant);
      Group root = TestHelper.CreateGroup("Root", "UID: Root", parent, tenant);

      SimulateExistingObjectForSecurityTest(tenant);
      SimulateExistingObjectForSecurityTest(grandParent2);
      SimulateExistingObjectForSecurityTest(grandParent1);
      SimulateExistingObjectForSecurityTest(parent);
      SimulateExistingObjectForSecurityTest(root);

      var securityProviderStub = new Mock<ISecurityProvider>();

      var grandParent1SecurityContext = ((ISecurityContextFactory)grandParent1).CreateSecurityContext();
      securityProviderStub
          .Setup(stub => stub.GetAccess(It.Is<ISecurityContext>(_ => !object.Equals(_, grandParent1SecurityContext)), It.IsAny<ISecurityPrincipal>()))
          .Returns(new[] { AccessType.Get(GeneralAccessTypes.Read) });
      securityProviderStub
          .Setup(stub => stub.GetAccess(grandParent1SecurityContext, It.IsAny<ISecurityPrincipal>()))
          .Returns(new AccessType[0]);

      var serviceLocator = DefaultServiceLocator.CreateWithBootstrappedServices();
      serviceLocator.RegisterSingle(() => securityProviderStub.Object);
      serviceLocator.RegisterSingle<IPrincipalProvider>(() => new NullPrincipalProvider());
      using (new ServiceLocatorScope(serviceLocator))
      {
        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          var groups = root.GetParents().ToArray();

          Assert.That(groups, Is.EquivalentTo(new[] { parent }));
        }
      }
    }

    [Test]
    public void Test_WithSecurity_PermissionDeniedOnRoot ()
    {
      Tenant tenant = TestHelper.CreateTenant("Tenant", "UID: Tenant");
      Group root = TestHelper.CreateGroup("Root", "UID: Root", null, tenant);

      var securityProviderStub = new Mock<ISecurityProvider>();
      securityProviderStub
          .Setup(stub => stub.GetAccess(It.IsAny<SecurityContext>(), It.IsAny<ISecurityPrincipal>()))
          .Returns(new AccessType[0]);

      var serviceLocator = DefaultServiceLocator.CreateWithBootstrappedServices();
      serviceLocator.RegisterSingle(() => securityProviderStub.Object);
      serviceLocator.RegisterSingle<IPrincipalProvider>(() => new NullPrincipalProvider());
      using (new ServiceLocatorScope(serviceLocator))
      {
        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          Assert.That(root.GetParents(), Is.Empty);
        }
      }
    }
  }
}
