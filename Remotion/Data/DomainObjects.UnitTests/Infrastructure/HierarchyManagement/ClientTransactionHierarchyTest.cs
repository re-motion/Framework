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
using Remotion.Data.DomainObjects.Infrastructure.HierarchyManagement;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.HierarchyManagement
{
  [TestFixture]
  public class ClientTransactionHierarchyTest : StandardMappingTest
  {
    private ClientTransaction _rootTransaction;
    private ClientTransactionHierarchy _hierarchy;

    public override void SetUp ()
    {
      base.SetUp();

      _rootTransaction = ClientTransaction.CreateRootTransaction();
      _hierarchy = new ClientTransactionHierarchy(_rootTransaction);
    }

    [Test]
    public void Initialization_SetsEverythingToTheRootTransaction ()
    {
      Assert.That(_hierarchy.RootTransaction, Is.SameAs(_rootTransaction));
      Assert.That(_hierarchy.LeafTransaction, Is.SameAs(_rootTransaction));
      Assert.That(_hierarchy.ActiveTransaction, Is.SameAs(_rootTransaction));
    }

    [Test]
    public void AppendLeafTransaction_SetsLeafTransaction ()
    {
      var subTransaction = _rootTransaction.CreateSubTransaction();

      _hierarchy.AppendLeafTransaction(subTransaction);

      Assert.That(_hierarchy.RootTransaction, Is.SameAs(_rootTransaction));
      Assert.That(_hierarchy.LeafTransaction, Is.SameAs(subTransaction));
      Assert.That(_hierarchy.ActiveTransaction, Is.SameAs(_rootTransaction));
    }

    [Test]
    public void AppendLeafTransaction_Twice_SetsLeafTransaction ()
    {
      var subTransaction1 = _rootTransaction.CreateSubTransaction();

      _hierarchy.AppendLeafTransaction(subTransaction1);

      var subTransaction2 = subTransaction1.CreateSubTransaction();

      _hierarchy.AppendLeafTransaction(subTransaction2);

      Assert.That(_hierarchy.RootTransaction, Is.SameAs(_rootTransaction));
      Assert.That(_hierarchy.LeafTransaction, Is.SameAs(subTransaction2));
      Assert.That(_hierarchy.ActiveTransaction, Is.SameAs(_rootTransaction));
    }

    [Test]
    public void AppendLeafTransaction_WithNotASubOfCurrentLeaf_Throws ()
    {
      var unrelatedTransaction = ClientTransactionObjectMother.Create();

      Assert.That(
          () => _hierarchy.AppendLeafTransaction(unrelatedTransaction),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "The new LeafTransaction must have the previous LeafTransaction as its parent.", "leafTransaction"));

      Assert.That(_hierarchy.RootTransaction, Is.SameAs(_rootTransaction));
      Assert.That(_hierarchy.LeafTransaction, Is.SameAs(_rootTransaction));
      Assert.That(_hierarchy.ActiveTransaction, Is.SameAs(_rootTransaction));
    }

    [Test]
    public void RemoveLeafTransaction_RevertsLeafToParentTransaction ()
    {
      var subTransaction = _rootTransaction.CreateSubTransaction();
      _hierarchy.AppendLeafTransaction(subTransaction);

      _hierarchy.RemoveLeafTransaction();

      Assert.That(_hierarchy.RootTransaction, Is.SameAs(_rootTransaction));
      Assert.That(_hierarchy.LeafTransaction, Is.SameAs(_rootTransaction));
      Assert.That(_hierarchy.ActiveTransaction, Is.SameAs(_rootTransaction));
    }

    [Test]
    public void RemoveLeafTransaction_InLargerHierarchy_RevertsStepByStep ()
    {
      var subTransaction1 = _rootTransaction.CreateSubTransaction();
      var subTransaction2 = subTransaction1.CreateSubTransaction();
      _hierarchy.AppendLeafTransaction(subTransaction1);
      _hierarchy.AppendLeafTransaction(subTransaction2);

      Assert.That(_hierarchy.LeafTransaction, Is.SameAs(subTransaction2));
      Assert.That(_hierarchy.ActiveTransaction, Is.SameAs(_rootTransaction));

      _hierarchy.RemoveLeafTransaction();

      Assert.That(_hierarchy.LeafTransaction, Is.SameAs(subTransaction1));
      Assert.That(_hierarchy.ActiveTransaction, Is.SameAs(_rootTransaction));

      _hierarchy.RemoveLeafTransaction();

      Assert.That(_hierarchy.LeafTransaction, Is.SameAs(_rootTransaction));
      Assert.That(_hierarchy.ActiveTransaction, Is.SameAs(_rootTransaction));
    }

    [Test]
    public void RemoveLeafTransaction_WithRootTransaction_Throws ()
    {
      Assert.That(
          () => _hierarchy.RemoveLeafTransaction(), Throws.InvalidOperationException.With.Message.EqualTo("Cannot remove the root transaction."));

      Assert.That(_hierarchy.LeafTransaction, Is.SameAs(_rootTransaction));
      Assert.That(_hierarchy.ActiveTransaction, Is.SameAs(_rootTransaction));
    }

    [Test]
    public void ActivateTransaction_ChangesActiveTransaction ()
    {
      var subTransaction = _rootTransaction.CreateSubTransaction();
      _hierarchy.AppendLeafTransaction(subTransaction);

      Assert.That(_hierarchy.ActiveTransaction, Is.SameAs(_rootTransaction));

      _hierarchy.ActivateTransaction(subTransaction);

      Assert.That(_hierarchy.ActiveTransaction, Is.SameAs(subTransaction));
    }

    [Test]
    public void ActivateTransaction_ReturnsScopeForRevertingActiveTransaction ()
    {
      var subTransaction = _rootTransaction.CreateSubTransaction();
      _hierarchy.AppendLeafTransaction(subTransaction);

      var scope = _hierarchy.ActivateTransaction(subTransaction);

      Assert.That(scope, Is.Not.Null);
      scope.Dispose();

      Assert.That(_hierarchy.ActiveTransaction, Is.SameAs(_rootTransaction));
    }

    [Test]
    public void ActivateTransaction_MultipleTimes_ReturnsScopesRevertingStepByStep ()
    {
      var subTransaction1 = _rootTransaction.CreateSubTransaction();
      var subTransaction2 = subTransaction1.CreateSubTransaction();
      _hierarchy.AppendLeafTransaction(subTransaction1);
      _hierarchy.AppendLeafTransaction(subTransaction2);

      var scope1 = _hierarchy.ActivateTransaction(subTransaction1);
      var scope2 = _hierarchy.ActivateTransaction(subTransaction2);

      Assert.That(_hierarchy.ActiveTransaction, Is.SameAs(subTransaction2));

      scope2.Dispose();

      Assert.That(_hierarchy.ActiveTransaction, Is.SameAs(subTransaction1));

      scope1.Dispose();

      Assert.That(_hierarchy.ActiveTransaction, Is.SameAs(_rootTransaction));
    }

    [Test]
    public void ActivateTransaction_WithLeafTransactionChanges ()
    {
      var subTransaction1 = _rootTransaction.CreateSubTransaction();
      _hierarchy.AppendLeafTransaction(subTransaction1);

      _hierarchy.ActivateTransaction(subTransaction1);

      var subTransaction2 = subTransaction1.CreateSubTransaction();
      _hierarchy.AppendLeafTransaction(subTransaction2);

      Assert.That(_hierarchy.LeafTransaction, Is.SameAs(subTransaction2));
      Assert.That(_hierarchy.ActiveTransaction, Is.SameAs(subTransaction1));

      _hierarchy.RemoveLeafTransaction();

      Assert.That(_hierarchy.LeafTransaction, Is.SameAs(subTransaction1));
      Assert.That(_hierarchy.ActiveTransaction, Is.SameAs(subTransaction1));
    }

    [Test]
    public void ActivateTransaction_TransactionNotFromHierarchy_Throws ()
    {
      var unrelatedTransaction = ClientTransactionObjectMother.Create();

      Assert.That(
          () => _hierarchy.ActivateTransaction(unrelatedTransaction),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "The activated transaction must be from this ClientTransactionHierarchy.", "clientTransaction"));

      Assert.That(_hierarchy.ActiveTransaction, Is.SameAs(_rootTransaction));
    }

    [Test]
    public void ActivateTransaction_CallingScopesOutOfOrder_Throws ()
    {
      var subTransaction = _rootTransaction.CreateSubTransaction();
      _hierarchy.AppendLeafTransaction(subTransaction);

      var scope1 = _hierarchy.ActivateTransaction(subTransaction);
      _hierarchy.ActivateTransaction(_rootTransaction);

      Assert.That(
          () => scope1.Dispose(),
          Throws.InvalidOperationException.With.Message.EqualTo("The scopes returned by ActivateTransaction must be disposed inside out."));
      Assert.That(_hierarchy.ActiveTransaction, Is.SameAs(_rootTransaction));
    }
  }
}
