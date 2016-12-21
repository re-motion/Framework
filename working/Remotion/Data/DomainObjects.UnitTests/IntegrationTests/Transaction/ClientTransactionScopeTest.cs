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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction
{
  [TestFixture]
  public class ClientTransactionScopeTest : StandardMappingTest
  {
    private ClientTransactionScope _outermostScope;

    public override void SetUp ()
    {
      base.SetUp();

      ClientTransactionScope.ResetActiveScope();
      _outermostScope = ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope();
    }

    public override void TearDown ()
    {
      if (ClientTransactionScope.ActiveScope != null)
      {
        Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (_outermostScope));
        _outermostScope.Leave();
      }
      base.TearDown();
    }

    [Test]
    public void ScopeSetsAndResetsCurrentTransaction ()
    {
      ClientTransaction clientTransaction = ClientTransaction.CreateRootTransaction();
      Assert.That (ClientTransactionScope.CurrentTransaction, Is.Not.SameAs (clientTransaction));
      using (clientTransaction.EnterNonDiscardingScope())
      {
        Assert.That (ClientTransactionScope.CurrentTransaction, Is.SameAs (clientTransaction));
      }
      Assert.That (ClientTransactionScope.CurrentTransaction, Is.Not.SameAs (clientTransaction));
    }

    [Test]
    public void EnterNullScopeSetsNullTransaction ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        Assert.That (ClientTransactionScope.HasCurrentTransaction, Is.True);
        using (ClientTransactionScope.EnterNullScope())
        {
          Assert.That (ClientTransactionScope.HasCurrentTransaction, Is.False);
        }
        Assert.That (ClientTransactionScope.HasCurrentTransaction, Is.True);
      }
    }

    [Test]
    public void ActiveScope ()
    {
      _outermostScope.Leave();
      Assert.That (ClientTransactionScope.ActiveScope, Is.Null);
      using (ClientTransactionScope scope = ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        Assert.That (ClientTransactionScope.ActiveScope, Is.Not.Null);
        Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (scope));
      }
    }

    [Test]
    public void NestedScopes ()
    {
      ClientTransaction clientTransaction1 = ClientTransaction.CreateRootTransaction();
      ClientTransaction clientTransaction2 = ClientTransaction.CreateRootTransaction();
      ClientTransactionScope originalScope = ClientTransactionScope.ActiveScope;
      ClientTransaction original = ClientTransactionScope.CurrentTransaction;

      Assert.That (original, Is.Not.SameAs (clientTransaction1));
      Assert.That (original, Is.Not.SameAs (clientTransaction2));
      Assert.That (original, Is.Not.Null);

      using (ClientTransactionScope scope1 = clientTransaction1.EnterNonDiscardingScope())
      {
        Assert.That (ClientTransactionScope.CurrentTransaction, Is.SameAs (clientTransaction1));
        Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (scope1));

        using (ClientTransactionScope scope2 = clientTransaction2.EnterNonDiscardingScope())
        {
          Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (scope2));
          Assert.That (ClientTransactionScope.CurrentTransaction, Is.SameAs (clientTransaction2));
        }
        Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (scope1));
        Assert.That (ClientTransactionScope.CurrentTransaction, Is.SameAs (clientTransaction1));
      }
      Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (originalScope));
      Assert.That (ClientTransactionScope.CurrentTransaction, Is.SameAs (original));
    }

    [Test]
    public void LeavesEmptyTransaction ()
    {
      using (ClientTransactionScope.EnterNullScope())
      {
        Assert.That (ClientTransactionScope.HasCurrentTransaction, Is.False);
        using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
        {
          Assert.That (ClientTransactionScope.HasCurrentTransaction, Is.True);
        }
        Assert.That (ClientTransactionScope.HasCurrentTransaction, Is.False);
      }
    }

    [Test]
    public void ScopeCreatesTransactionWithDefaultCtor ()
    {
      ClientTransaction original = ClientTransactionScope.CurrentTransaction;
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        Assert.That (ClientTransactionScope.CurrentTransaction, Is.Not.Null);
        Assert.That (ClientTransactionScope.CurrentTransaction, Is.Not.SameAs (original));
      }
      Assert.That (ClientTransactionScope.CurrentTransaction, Is.SameAs (original));
    }

    [Test]
    public void ScopeHasTransactionProperty ()
    {
      ClientTransaction outerTransaction = ClientTransaction.CreateRootTransaction();
      ClientTransaction innerTransaction = ClientTransaction.CreateRootTransaction();
      using (ClientTransactionScope outer = outerTransaction.EnterNonDiscardingScope())
      {
        using (ClientTransactionScope inner = innerTransaction.EnterNonDiscardingScope())
        {
          Assert.That (inner.ScopedTransaction, Is.SameAs (innerTransaction));
          Assert.That (outer.ScopedTransaction, Is.SameAs (outerTransaction));
        }
      }
    }

    [Test]
    public void ScopeHasTransactionProperty_FromITransactionScopeInterface ()
    {
      ClientTransaction outerTransaction = ClientTransaction.CreateRootTransaction();
      ClientTransaction innerTransaction = ClientTransaction.CreateRootTransaction();
      using (ClientTransactionScope outer = outerTransaction.EnterNonDiscardingScope())
      {
        using (ClientTransactionScope inner = innerTransaction.EnterNonDiscardingScope())
        {
          Assert.That (((ITransactionScope) inner).ScopedTransaction.To<ClientTransaction>(), Is.SameAs (innerTransaction));
          Assert.That (((ITransactionScope) outer).ScopedTransaction.To<ClientTransaction> (), Is.SameAs (outerTransaction));
        }
      }
    }

    [Test]
    public void ScopeHasAutoRollbackBehavior ()
    {
      using (ClientTransactionScope scope = ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        Assert.That (scope.AutoRollbackBehavior, Is.EqualTo (AutoRollbackBehavior.Discard));
        scope.AutoRollbackBehavior = AutoRollbackBehavior.None;
        Assert.That (scope.AutoRollbackBehavior, Is.EqualTo (AutoRollbackBehavior.None));
      }

      using (ClientTransactionScope scope = ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        Assert.That (scope.AutoRollbackBehavior, Is.EqualTo (AutoRollbackBehavior.Discard));
      }

      using (ClientTransactionScope scope = ClientTransaction.CreateRootTransaction().EnterScope (AutoRollbackBehavior.None))
      {
        Assert.That (scope.AutoRollbackBehavior, Is.EqualTo (AutoRollbackBehavior.None));
      }

      using (ClientTransactionScope scope = ClientTransaction.CreateRootTransaction().EnterScope (AutoRollbackBehavior.Rollback))
      {
        Assert.That (scope.AutoRollbackBehavior, Is.EqualTo (AutoRollbackBehavior.Rollback));
      }
    }

    private class TransactionEventCounter
    {
      public int Rollbacks = 0;
      public int Commits = 0;

      public TransactionEventCounter (ClientTransaction clientTransaction)
      {
        clientTransaction.RolledBack += ClientTransaction_RolledBack;
        clientTransaction.Committed += ClientTransaction_Committed;
      }

      private void ClientTransaction_RolledBack (object sender, ClientTransactionEventArgs args)
      {
        ++Rollbacks;
      }

      private void ClientTransaction_Committed (object sender, ClientTransactionEventArgs args)
      {
        ++Commits;
      }
    }

    [Test]
    public void NoAutoRollbackWhenNoneBehavior ()
    {
      var mock = new TestableClientTransaction();
      var eventCounter = new TransactionEventCounter (mock);

      using (mock.EnterScope (AutoRollbackBehavior.None))
      {
        Order order = new DomainObjectIDs (MappingConfiguration.Current).Order1.GetObject<Order> ();
        order.OrderNumber = 0xbadf00d;
        order.OrderTicket = OrderTicket.NewObject();
        order.OrderItems.Add (OrderItem.NewObject());
      }

      Assert.That (eventCounter.Rollbacks, Is.EqualTo (0));

      using (mock.EnterScope (AutoRollbackBehavior.None))
      {
      }

      Assert.That (eventCounter.Rollbacks, Is.EqualTo (0));

      using (ClientTransactionScope scope = mock.EnterScope (AutoRollbackBehavior.None))
      {
        Order order = new DomainObjectIDs (MappingConfiguration.Current).Order1.GetObject<Order> ();
        order.OrderNumber = 0xbadf00d;

        scope.ScopedTransaction.Rollback();
      }

      Assert.That (eventCounter.Rollbacks, Is.EqualTo (1));
    }

    [Test]
    public void AutoRollbackWhenRollbackBehavior ()
    {
      var mock = new TestableClientTransaction();
      var eventCounter = new TransactionEventCounter (mock);

      using (mock.EnterScope (AutoRollbackBehavior.Rollback))
      {
        Order order = new DomainObjectIDs (MappingConfiguration.Current).Order1.GetObject<Order> ();
        order.OrderNumber = 0xbadf00d;
      }

      Assert.That (eventCounter.Rollbacks, Is.EqualTo (1));
      eventCounter.Rollbacks = 0;

      using (mock.EnterScope (AutoRollbackBehavior.Rollback))
      {
        Order order = new DomainObjectIDs (MappingConfiguration.Current).Order1.GetObject<Order> ();
        order.OrderTicket = OrderTicket.NewObject();
      }

      Assert.That (eventCounter.Rollbacks, Is.EqualTo (1));
      eventCounter.Rollbacks = 0;

      using (mock.EnterScope (AutoRollbackBehavior.Rollback))
      {
        Order order = new DomainObjectIDs (MappingConfiguration.Current).Order1.GetObject<Order> ();
        order.OrderItems.Add (OrderItem.NewObject());
      }

      Assert.That (eventCounter.Rollbacks, Is.EqualTo (1));
      eventCounter.Rollbacks = 0;

      using (mock.EnterScope (AutoRollbackBehavior.Rollback))
      {
      }

      Assert.That (eventCounter.Rollbacks, Is.EqualTo (0));

      using (ClientTransactionScope scope = mock.EnterScope (AutoRollbackBehavior.Rollback))
      {
        Order order = new DomainObjectIDs (MappingConfiguration.Current).Order1.GetObject<Order> ();
        order.OrderNumber = 0xbadf00d;
        scope.ScopedTransaction.Rollback();
      }

      Assert.That (eventCounter.Rollbacks, Is.EqualTo (1));
      eventCounter.Rollbacks = 0;

      using (ClientTransactionScope scope = mock.EnterScope (AutoRollbackBehavior.Rollback))
      {
        Order order = new DomainObjectIDs (MappingConfiguration.Current).Order1.GetObject<Order> ();
        order.OrderNumber = 0xbadf00d;
        scope.ScopedTransaction.Rollback();

        order.OrderNumber = 0xbadf00d;
      }

      Assert.That (eventCounter.Rollbacks, Is.EqualTo (2));
    }

    [Test]
    public void CommitAndRollbackOnScope ()
    {
      ClientTransaction transaction = ClientTransaction.CreateRootTransaction();
      var eventCounter = new TransactionEventCounter (transaction);
      using (ClientTransactionScope scope = transaction.EnterNonDiscardingScope())
      {
        Assert.That (eventCounter.Commits, Is.EqualTo (0));
        Assert.That (eventCounter.Rollbacks, Is.EqualTo (0));

        scope.Commit();

        Assert.That (eventCounter.Commits, Is.EqualTo (1));
        Assert.That (eventCounter.Rollbacks, Is.EqualTo (0));

        scope.Rollback();

        Assert.That (eventCounter.Commits, Is.EqualTo (1));
        Assert.That (eventCounter.Rollbacks, Is.EqualTo (1));

        transaction.Commit();

        Assert.That (eventCounter.Commits, Is.EqualTo (2));
        Assert.That (eventCounter.Rollbacks, Is.EqualTo (1));

        transaction.Rollback();

        Assert.That (eventCounter.Commits, Is.EqualTo (2));
        Assert.That (eventCounter.Rollbacks, Is.EqualTo (2));
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The ClientTransactionScope has already been left.")]
    public void LeaveTwiceThrows ()
    {
      ClientTransactionScope scope = ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope();
      scope.Leave();
      scope.Leave();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The ClientTransactionScope has already been left.")]
    public void LeaveAndDisposeThrows ()
    {
      using (ClientTransactionScope scope = ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        scope.Leave();
      }
    }

    [Test]
    public void NoAutoEnlisting ()
    {
      Order order = new DomainObjectIDs (MappingConfiguration.Current).Order1.GetObject<Order> ();
      Assert.That (ClientTransaction.Current.IsEnlisted (order), Is.True);
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        Assert.That (ClientTransaction.Current.IsEnlisted (order), Is.False);
      }
    }

    [Test]
    public void ResetScope ()
    {
      ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope();
      Assert.That (ClientTransactionScope.ActiveScope, Is.Not.Null);
      Assert.That (ClientTransactionScope.HasCurrentTransaction, Is.True);
      ClientTransactionScope.ResetActiveScope();
      Assert.That (ClientTransactionScope.ActiveScope, Is.Null);
      Assert.That (ClientTransactionScope.HasCurrentTransaction, Is.False);
    }

    [Test]
    public void Leave_NullAttachedScope_NoExceptionThrown ()
    {
      var scopedTransaction = ClientTransactionObjectMother.Create ();

      var scope =
          (ClientTransactionScope)
          PrivateInvoke.CreateInstanceNonPublicCtor (typeof (ClientTransactionScope), scopedTransaction, AutoRollbackBehavior.None, null);

      Assert.That (() => scope.Leave (), Throws.Nothing);
    }

    [Test]
    public void Leave_ExecutesAutoDiscardBehavior ()
    {
      var transactionMock = ClientTransactionObjectMother.CreateStrictMock();

      transactionMock
          .Expect (mock => mock.EnterScope (AutoRollbackBehavior.Discard))
          .Return (
              (ClientTransactionScope)
              PrivateInvoke.CreateInstanceNonPublicCtor (typeof (ClientTransactionScope), transactionMock, AutoRollbackBehavior.Discard, null));
      transactionMock.Expect (mock => mock.Discard ());

      transactionMock.Replay();

      using (transactionMock.EnterScope (AutoRollbackBehavior.Discard))
      {
      }

      transactionMock.VerifyAllExpectations();
    }

    [Test]
    public void Leave_ExecutesAttachedScope_AfterAutoDiscardBehavior ()
    {
      var scopedTransaction = ClientTransactionObjectMother.Create ();
      var attachedScopeMock = MockRepository.GenerateStrictMock<IDisposable> ();
      attachedScopeMock.Expect (mock => mock.Dispose ()).WhenCalled (mock => Assert.That (scopedTransaction.IsDiscarded, Is.True));

      var scope = (ClientTransactionScope) PrivateInvoke.CreateInstanceNonPublicCtor (
          typeof (ClientTransactionScope), scopedTransaction, AutoRollbackBehavior.Discard, attachedScopeMock);

      scope.Leave ();

      attachedScopeMock.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "This ClientTransactionScope is not the active scope. Leave the active scope before leaving this one.")]
    public void LeaveNonActiveScopeThrows ()
    {
      try
      {
        using (ClientTransaction.CreateRootTransaction().EnterScope (AutoRollbackBehavior.Rollback))
        {
          ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope();
        }
      }
      finally
      {
        ClientTransactionScope.ResetActiveScope(); // for TearDown
      }
    }

    [Test]
    public void ITransactionScope_IsActiveScope ()
    {
      ClientTransaction clientTransaction = ClientTransaction.CreateRootTransaction();
      try
      {
        ITransactionScope outerScope = clientTransaction.EnterDiscardingScope();
        Assert.That (outerScope.IsActiveScope, Is.True);

        using (ClientTransactionScope innerScope = clientTransaction.CreateSubTransaction().EnterDiscardingScope())
        {
          Assert.That (outerScope.IsActiveScope, Is.False);
          Assert.That (((ITransactionScope) innerScope).IsActiveScope, Is.True);
        }

        Assert.That (outerScope.IsActiveScope, Is.True);
        outerScope.Leave ();
        Assert.That (outerScope.IsActiveScope, Is.False);
      }
      finally
      {
        ClientTransactionScope.ResetActiveScope(); // for TearDown
      }
    }
  }
}
