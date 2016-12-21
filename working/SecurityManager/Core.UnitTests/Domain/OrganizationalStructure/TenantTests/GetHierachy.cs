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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Security;
using Remotion.Development.UnitTesting;
using Remotion.Security;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.ServiceLocation;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.TenantTests
{
  [TestFixture]
  public class GetHierachy : TenantTestBase
  {
    [Test]
    public void Test_NoChildren ()
    {
      Tenant root = TestHelper.CreateTenant ("Root", "UID: Root");

      Assert.That (root.GetHierachy().ToArray(), Is.EquivalentTo (new[] { root }));
    }

    [Test]
    public void Test_NoGrandChildren ()
    {
      Tenant root = TestHelper.CreateTenant ("Root", "UID: Root");
      Tenant child1 = TestHelper.CreateTenant ("Child1", "UID: Child1");
      child1.Parent = root;
      Tenant child2 = TestHelper.CreateTenant ("Child2", "UID: Child2");
      child2.Parent = root;

      Assert.That (root.GetHierachy().ToArray(), Is.EquivalentTo (new[] { root, child1, child2 }));
    }

    [Test]
    public void Test_WithGrandChildren ()
    {
      Tenant root = TestHelper.CreateTenant ("Root", "UID: Root");
      Tenant child1 = TestHelper.CreateTenant ("Child1", "UID: Child1");
      child1.Parent = root;
      Tenant child2 = TestHelper.CreateTenant ("Child2", "UID: Child2");
      child2.Parent = root;
      Tenant grandChild1 = TestHelper.CreateTenant ("GrandChild1", "UID: GrandChild1");
      grandChild1.Parent = child1;

      Assert.That (root.GetHierachy().ToArray(), Is.EquivalentTo (new[] { root, child1, child2, grandChild1 }));
    }

    [Test]
    public void Test_WithCircularHierarchy_ThrowsInvalidOperationException ()
    {
      Tenant root = TestHelper.CreateTenant ("Root", "UID: Root");
      Tenant child1 = TestHelper.CreateTenant ("Child1", "UID: Child1");
      child1.Parent = root;
      Tenant grandChild1 = TestHelper.CreateTenant ("GrandChild1", "UID: GrandChild1");
      grandChild1.Parent = child1;
      Tenant grandChild2 = TestHelper.CreateTenant ("GrandChild2", "UID: GrandChild2");
      grandChild2.Parent = grandChild1;
      root.Parent = grandChild2;

      Assert.That (
          () => grandChild1.GetHierachy().ToArray(),
          Throws.InvalidOperationException
                .With.Message.EqualTo ("The hierarchy for tenant '" + grandChild1 + "' cannot be resolved because a circular reference exists."));
    }

    [Test]
    public void Test_WithSecurity_PermissionDeniedOnChild ()
    {
      Tenant root = TestHelper.CreateTenant ("Root", "UID: Root");
      Tenant child1 = TestHelper.CreateTenant ("Child1", "UID: Child1");
      child1.Parent = root;

      Tenant child2 = TestHelper.CreateTenant ("Child2", "UID: Child2");
      child2.Parent = root;

      Tenant child1_1 = TestHelper.CreateTenant ("Child1.1", "UID: Child1.1");
      child1_1.Parent = child1;
      
      Tenant child2_1 = TestHelper.CreateTenant ("Child2.1", "UID: Child2.1");
      child2_1.Parent = child2;

      ISecurityProvider securityProviderStub = MockRepository.GenerateStub<ISecurityProvider>();

      var childOfChild2SecurityContext = ((ISecurityContextFactory) child2_1).CreateSecurityContext ();
      securityProviderStub.Stub (
          stub => stub.GetAccess (
                      Arg<ISecurityContext>.Is.NotEqual (childOfChild2SecurityContext),
                      Arg<ISecurityPrincipal>.Is.Anything)).Return (new[] { AccessType.Get (GeneralAccessTypes.Read) });
      securityProviderStub.Stub (
          stub => stub.GetAccess (
                      Arg.Is (childOfChild2SecurityContext),
                      Arg<ISecurityPrincipal>.Is.Anything)).Return (new AccessType[0]);

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle (() => securityProviderStub);
      serviceLocator.RegisterSingle<IPrincipalProvider> (() => new NullPrincipalProvider());
      using (new ServiceLocatorScope (serviceLocator))
      {
        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          ClientTransaction.Current.Extensions.Add (new SecurityClientTransactionExtension());

          Assert.That (root.GetHierachy(), Is.EquivalentTo (new[] { root, child1, child2, child1_1 }));
        }
      }
    }

    [Test]
    public void Test_WithSecurity_PermissionDeniedOnRoot ()
    {
      Tenant root = TestHelper.CreateTenant ("Root", "UID: Root");

      ISecurityProvider securityProviderStub = MockRepository.GenerateStub<ISecurityProvider>();

      securityProviderStub.Stub (
          stub => stub.GetAccess (Arg<SecurityContext>.Is.Anything, Arg<ISecurityPrincipal>.Is.Anything)).Return (new AccessType[0]);

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle (() => securityProviderStub);
      serviceLocator.RegisterSingle<IPrincipalProvider> (() => new NullPrincipalProvider());
      using (new ServiceLocatorScope (serviceLocator))
      {
        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          ClientTransaction.Current.Extensions.Add (new SecurityClientTransactionExtension());

          Assert.That (root.GetHierachy(), Is.Empty);
        }
      }
    }
  }
}