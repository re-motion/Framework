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

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.TenantTests
{
  [TestFixture]
  public class GetParents : TenantTestBase
  {
    [Test]
    public void Test_NoParents ()
    {
      Tenant root = TestHelper.CreateTenant("Root", "UID: Root");

      var groups = root.GetParents().ToArray();

      Assert.That(groups, Is.Empty);
    }

    [Test]
    public void Test_NoGrandParents ()
    {
      Tenant parent = TestHelper.CreateTenant("parent1", "UID: parent");
      Tenant root = TestHelper.CreateTenant("Root", "UID: Root");
      root.Parent = parent;

      var groups = root.GetParents().ToArray();

      Assert.That(groups, Is.EquivalentTo(new[] { parent }));
    }

    [Test]
    public void Test_WithGrandParents ()
    {
      Tenant grandParent = TestHelper.CreateTenant("Grandparent1", "UID: Grandparent");
      Tenant parent = TestHelper.CreateTenant("parent1", "UID: parent");
      parent.Parent = grandParent;
      Tenant root = TestHelper.CreateTenant("Root", "UID: Root");
      root.Parent = parent;

      var groups = root.GetParents().ToArray();

      Assert.That(groups, Is.EquivalentTo(new[] { parent, grandParent }));
    }

    [Test]
    public void Test_WithCircularHierarchy_ThrowsInvalidOperationException ()
    {
      Tenant grandParent2 = TestHelper.CreateTenant("Grandparent2", "UID: Grandparent2");
      Tenant grandParent1 = TestHelper.CreateTenant("Grandparent1", "UID: Grandparent1");
      grandParent1.Parent = grandParent2;
      Tenant parent = TestHelper.CreateTenant("parent1", "UID: parent");
      parent.Parent = grandParent1;
      Tenant root = TestHelper.CreateTenant("Root", "UID: Root");
      root.Parent = parent;
      grandParent2.Parent = root;

      Assert.That(
          () => root.GetParents().Take(10).ToArray(),
          Throws.InvalidOperationException.With.Message.EqualTo(
              string.Format("The parent hierarchy for tenant '{0}' cannot be resolved because a circular reference exists.", root.ID)));
    }

    [Test]
    public void Test_WithCircularHierarchy_ParentIsOwnParent_ThrowsInvalidOperationException ()
    {
      Tenant root = TestHelper.CreateTenant("Root", "UID: Root");
      root.Parent = root;

      Assert.That(
          () => root.GetParents().Take(10).ToArray(),
          Throws.InvalidOperationException.With.Message.EqualTo(
              string.Format("The parent hierarchy for tenant '{0}' cannot be resolved because a circular reference exists.", root.ID)));
    }

    [Test]
    public void Test_WithCircularHierarchyAboveTheRoot_ThrowsInvalidOperationException ()
    {
      Tenant grandParent3 = TestHelper.CreateTenant("Grandparent3", "UID: Grandparent3");
      Tenant grandParent2 = TestHelper.CreateTenant("Grandparent2", "UID: Grandparent2");
      grandParent2.Parent = grandParent3;
      Tenant grandParent1 = TestHelper.CreateTenant("Grandparent1", "UID: Grandparent1");
      grandParent1.Parent = grandParent2;
      Tenant parent = TestHelper.CreateTenant("parent1", "UID: parent");
      parent.Parent = grandParent1;
      Tenant root = TestHelper.CreateTenant("Root", "UID: Root");
      root.Parent = parent;
      grandParent3.Parent = grandParent1;

      Assert.That(
          () => root.GetParents().Take(10).ToArray(),
          Throws.InvalidOperationException.With.Message.EqualTo(
              string.Format("The parent hierarchy for tenant '{0}' cannot be resolved because a circular reference exists.", root.ID)));
    }

    [Test]
    public void Test_WithCircularHierarchyAboveTheRoot_ParentIsOwnParent_ThrowsInvalidOperationException ()
    {
      Tenant parent = TestHelper.CreateTenant("parent1", "UID: parent");
      Tenant root = TestHelper.CreateTenant("Root", "UID: Root");
      root.Parent = parent;
      parent.Parent = parent;

      Assert.That(
          () => root.GetParents().Take(10).ToArray(),
          Throws.InvalidOperationException.With.Message.EqualTo(
              string.Format("The parent hierarchy for tenant '{0}' cannot be resolved because a circular reference exists.", root.ID)));
    }

    [Test]
    public void Test_WithSecurity_PermissionDeniedOnParent ()
    {
      Tenant grandParent2 = TestHelper.CreateTenant("Grandparent1", "UID: Grandparent2");
      Tenant grandParent1 = TestHelper.CreateTenant("Grandparent1", "UID: Grandparent1");
      grandParent1.Parent = grandParent2;
      Tenant parent = TestHelper.CreateTenant("parent1", "UID: parent");
      parent.Parent = grandParent1;
      Tenant root = TestHelper.CreateTenant("Root", "UID: Root");
      root.Parent = parent;

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

      var serviceLocator = DefaultServiceLocator.Create();
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
      Tenant root = TestHelper.CreateTenant("Root", "UID: Root");

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
          Assert.That(root.GetParents(), Is.Empty);
        }
      }
    }
  }
}
