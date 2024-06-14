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
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.OrganizationalStructure;

namespace Remotion.SecurityManager.UnitTests.Domain.OrganizationalStructure.GroupTests
{
  [TestFixture]
  public class OnCommitting : GroupTestBase
  {
    [Test]
    public void OnCommitting_WithCircularParentHierarchy_ThrowsInvalidOperationException ()
    {
      Tenant tenant = TestHelper.CreateTenant(ClientTransactionScope.CurrentTransaction, "Tenant", Guid.NewGuid().ToString());
      Group grandParent = TestHelper.CreateGroup(ClientTransactionScope.CurrentTransaction, "GrandParent", Guid.NewGuid().ToString(), null, tenant);
      Group parent = TestHelper.CreateGroup(ClientTransactionScope.CurrentTransaction, "Parent", Guid.NewGuid().ToString(), grandParent, tenant);
      Group child = TestHelper.CreateGroup(ClientTransactionScope.CurrentTransaction, "Child", Guid.NewGuid().ToString(), parent, tenant);

      ClientTransactionScope.CurrentTransaction.Commit();

      grandParent.Parent = child;

      Assert.That(
          () => ClientTransactionScope.CurrentTransaction.Commit(),
          Throws.InvalidOperationException
              .And.Message.EqualTo("Group '" + grandParent.ID + "' cannot be committed because it would result in a circular parent hierarchy."));
    }

    [Test]
    public void OnCommitting_WithCircularParentHierarchy_GroupIsOwnParent_ThrowsInvalidOperationException ()
    {
      Tenant tenant = TestHelper.CreateTenant(ClientTransactionScope.CurrentTransaction, "Tenant", Guid.NewGuid().ToString());
      Group root = TestHelper.CreateGroup(ClientTransactionScope.CurrentTransaction, "Root", Guid.NewGuid().ToString(), null, tenant);

      ClientTransactionScope.CurrentTransaction.Commit();

      root.Parent = root;

      Assert.That(
          () => ClientTransactionScope.CurrentTransaction.Commit(),
          Throws.InvalidOperationException
              .And.Message.EqualTo("Group '" + root.ID + "' cannot be committed because it would result in a circular parent hierarchy."));
    }

    [Test]
    public void OnCommitting_WithCircularParentHierarchy_ChecksOnlyIfParentHasChanged_ThrowsInvalidOperationException ()
    {
      Tenant tenant = TestHelper.CreateTenant(ClientTransactionScope.CurrentTransaction, "Tenant", Guid.NewGuid().ToString());
      Group grandParent = TestHelper.CreateGroup(ClientTransactionScope.CurrentTransaction, "GrandParent", Guid.NewGuid().ToString(), null, tenant);
      Group parent = TestHelper.CreateGroup(ClientTransactionScope.CurrentTransaction, "Parent", Guid.NewGuid().ToString(), grandParent, tenant);
      Group child = TestHelper.CreateGroup(ClientTransactionScope.CurrentTransaction, "Child", Guid.NewGuid().ToString(), null, tenant);
      grandParent.Parent = child;

      ClientTransactionScope.CurrentTransaction.Commit();

      parent.Name = "NewName";
      child.Parent = parent;

      // Order of DomainObjects is stable and equal to order in which the objects have been added to DataManager
      // Should this ever change, call OnCommitting manually in order to ensure that the parent check is skipped and the child causes the exception
      Assert.That(
          () => ClientTransactionScope.CurrentTransaction.Commit(),
          Throws.InvalidOperationException
              .And.Message.EqualTo("Group '" + child.ID + "' cannot be committed because it would result in a circular parent hierarchy."));
    }

    [Test]
    public void OnCommitting_WithCircularParentHierarchy_ChangesHappensInDifferentTransactions_ThrowsInvalidOperationException ()
    {
      Tenant tenant = TestHelper.CreateTenant(ClientTransactionScope.CurrentTransaction, "Tenant", Guid.NewGuid().ToString());
      Group grandParent = TestHelper.CreateGroup(ClientTransactionScope.CurrentTransaction, "GrandParent", Guid.NewGuid().ToString(), null, tenant);
      Group parent = TestHelper.CreateGroup(ClientTransactionScope.CurrentTransaction, "Parent", Guid.NewGuid().ToString(), null, tenant);

      ClientTransactionScope.CurrentTransaction.Commit();

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        Group parentInOtherThread = parent.ID.GetObject<Group>();
        parentInOtherThread.Parent = grandParent.ID.GetObject<Group>();
        ClientTransaction.Current.Commit();
      }

      Group child = TestHelper.CreateGroup(ClientTransactionScope.CurrentTransaction, "Child", Guid.NewGuid().ToString(), parent, tenant);
      grandParent.Parent = child;

      Assert.That(
          () => ClientTransactionScope.CurrentTransaction.Commit(),
          Throws.InvalidOperationException
              .And.Message.EqualTo("Group '" + grandParent.ID + "' cannot be committed because it would result in a circular parent hierarchy."));
    }

    [Test]
    public void OnCommitting_ReloadsUnchangedParentGroupsDuringOnCommittingToPreventConcurrencyExceptions ()
    {
      Tenant tenant = TestHelper.CreateTenant(ClientTransactionScope.CurrentTransaction, "Tenant", Guid.NewGuid().ToString());
      Group grandParent = TestHelper.CreateGroup(ClientTransactionScope.CurrentTransaction, "GrandParent", Guid.NewGuid().ToString(), null, tenant);
      Group parent = TestHelper.CreateGroup(ClientTransactionScope.CurrentTransaction, "Parent", Guid.NewGuid().ToString(), grandParent, tenant);
      Group child = TestHelper.CreateGroup(ClientTransactionScope.CurrentTransaction, "Child", Guid.NewGuid().ToString(), parent, tenant);

      ClientTransactionScope.CurrentTransaction.Commit();

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        Group parentInOtherThread = parent.ID.GetObject<Group>();
        parentInOtherThread.Name = "NewName";
        ClientTransaction.Current.Commit();
      }

      TestHelper.CreateGroup(ClientTransactionScope.CurrentTransaction, "GrandChild", Guid.NewGuid().ToString(), child, tenant);

      ClientTransactionScope.CurrentTransaction.Commit();
    }

    [Test]
    public void OnCommitting_ReloadsParentGroupsHavingAChangedVirtualEndPointDuringOnCommittingToPreventConcurrencyExceptions ()
    {
      Tenant tenant = TestHelper.CreateTenant(ClientTransactionScope.CurrentTransaction, "Tenant", Guid.NewGuid().ToString());
      Group grandParent = TestHelper.CreateGroup(ClientTransactionScope.CurrentTransaction, "GrandParent", Guid.NewGuid().ToString(), null, tenant);
      Group parent = TestHelper.CreateGroup(ClientTransactionScope.CurrentTransaction, "Parent", Guid.NewGuid().ToString(), grandParent, tenant);

      ClientTransactionScope.CurrentTransaction.Commit();

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        Group parentInOtherThread = parent.ID.GetObject<Group>();
        parentInOtherThread.Name = "NewName";
        ClientTransaction.Current.Commit();
      }

      TestHelper.CreateGroup(ClientTransactionScope.CurrentTransaction, "Child", Guid.NewGuid().ToString(), parent, tenant);

      ClientTransactionScope.CurrentTransaction.Commit();
    }
  }
}
