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
using System.Linq.Expressions;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Development.UnitTesting.ObjectMothers;

namespace Remotion.Data.DomainObjects.UnitTests
{
  [TestFixture]
  public class ClientTransactionExtensionsTest : StandardMappingTest
  {
    [Test]
    public void ExecuteInScope_Action_RunsDelegate ()
    {
      var transaction = ClientTransaction.CreateRootTransaction();

      bool delegateRun = false;
      Action action = () => delegateRun = true;

      transaction.ExecuteInScope(action);

      Assert.That(delegateRun, Is.True);
    }

    [Test]
    public void ExecuteInScope_Action_SetsCurrentTx ()
    {
      var transaction = ClientTransaction.CreateRootTransaction();

      Assert.That(ClientTransaction.Current, Is.Null);

      ClientTransaction currentInDelegate = null;
      Action action = () => currentInDelegate = ClientTransaction.Current;

      transaction.ExecuteInScope(action);

      Assert.That(currentInDelegate, Is.SameAs(transaction));
      Assert.That(ClientTransaction.Current, Is.Null);
    }

    [Test]
    public void ExecuteInScope_Action_ReusesScopeIfPossible ()
    {
      var transaction = ClientTransaction.CreateRootTransaction();

      using (var scope = transaction.EnterNonDiscardingScope())
      {
        Assert.That(ClientTransactionScope.ActiveScope, Is.SameAs(scope));

        ClientTransactionScope scopeInDelegate = null;
        Action action = () => scopeInDelegate = ClientTransactionScope.ActiveScope;

        transaction.ExecuteInScope(action);

        Assert.That(scopeInDelegate, Is.SameAs(scope));
        Assert.That(ClientTransactionScope.ActiveScope, Is.SameAs(scope));
      }
    }

    [Test]
    public void ExecuteInScope_Action_ActivatesTransaction ()
    {
      var transaction = ClientTransaction.CreateRootTransaction();

      using (ClientTransactionTestHelper.MakeInactive(transaction))
      {
        var delegateRun = false;
        Action action = () =>
        {
          Assert.That(transaction.ActiveTransaction, Is.SameAs(transaction));
          Assert.That(ClientTransaction.Current, Is.SameAs(transaction));
          delegateRun = true;
        };

        transaction.ExecuteInScope(action);

        Assert.That(delegateRun, Is.True);
      }
    }

    [Test]
    public void ExecuteInScope_Action_ActivatesTransaction_EvenWhenAlreadyCurrent ()
    {
      var transaction = ClientTransaction.CreateRootTransaction();

      using (transaction.EnterNonDiscardingScope())
      {
        using (ClientTransactionTestHelper.MakeInactive(transaction))
        {
          var delegateRun = false;
          Action action = () =>
          {
            Assert.That(transaction.ActiveTransaction, Is.SameAs(transaction));
            Assert.That(ClientTransaction.Current, Is.SameAs(transaction));
            delegateRun = true;
          };

          transaction.ExecuteInScope(action);

          Assert.That(delegateRun, Is.True);
        }
      }
    }

    [Test]
    public void ExecuteInScope_Func_RunsDelegate ()
    {
      var transaction = ClientTransaction.CreateRootTransaction();

      Func<int> func = () => 17;
      var result = transaction.ExecuteInScope(func);

      Assert.That(result, Is.EqualTo(17));
    }

    [Test]
    public void ExecuteInScope_Func_SetsCurrentTx ()
    {
      var transaction = ClientTransaction.CreateRootTransaction();

      Assert.That(ClientTransaction.Current, Is.Null);

      ClientTransaction currentInDelegate = null;
      Func<int> func = () =>
      {
        currentInDelegate = ClientTransaction.Current;
        return 4;
      };

      transaction.ExecuteInScope(func);

      Assert.That(currentInDelegate, Is.SameAs(transaction));
      Assert.That(ClientTransaction.Current, Is.Null);
    }

    [Test]
    public void ExecuteInScope_Func_ReusesScopeIfPossible ()
    {
      var transaction = ClientTransaction.CreateRootTransaction();

      using (var scope = transaction.EnterNonDiscardingScope())
      {
        Assert.That(ClientTransactionScope.ActiveScope, Is.SameAs(scope));

        ClientTransactionScope scopeInDelegate = null;
        Func<int> func = () =>
        {
          scopeInDelegate = ClientTransactionScope.ActiveScope;
          return 4;
        };

        transaction.ExecuteInScope(func);

        Assert.That(scopeInDelegate, Is.SameAs(scope));
        Assert.That(ClientTransactionScope.ActiveScope, Is.SameAs(scope));
      }
    }

    [Test]
    public void ExecuteInScope_Func_ActivatesTransaction ()
    {
      var transaction = ClientTransaction.CreateRootTransaction();

      using (ClientTransactionTestHelper.MakeInactive(transaction))
      {
        var delegateRun = false;
        Func<int> func = () =>
        {
          Assert.That(transaction.ActiveTransaction, Is.SameAs(transaction));
          Assert.That(ClientTransaction.Current, Is.SameAs(transaction));
          delegateRun = true;
          return 7;
        };

        var result = transaction.ExecuteInScope(func);

        Assert.That(delegateRun, Is.True);
        Assert.That(result, Is.EqualTo(7));
      }
    }

    [Test]
    public void ExecuteInScope_Func_ActivatesTransaction_EvenWhenAlreadyCurrent ()
    {
      var transaction = ClientTransaction.CreateRootTransaction();

      using (transaction.EnterNonDiscardingScope())
      {
        using (ClientTransactionTestHelper.MakeInactive(transaction))
        {
          var delegateRun = false;
          Func<int> func = () =>
          {
            Assert.That(transaction.ActiveTransaction, Is.SameAs(transaction));
            Assert.That(ClientTransaction.Current, Is.SameAs(transaction));
            delegateRun = true;
            return 7;
          };

          var result = transaction.ExecuteInScope(func);

          Assert.That(delegateRun, Is.True);
          Assert.That(result, Is.EqualTo(7));
        }
      }
    }

    [Test]
    public void HasChanged ()
    {
      var commitRollbackAgentMock = new Mock<ICommitRollbackAgent>(MockBehavior.Strict);

      var transactionWithMocks = ClientTransactionObjectMother.CreateWithComponents<TestableClientTransaction>(
          commitRollbackAgent: commitRollbackAgentMock.Object);

      var expectedResult = BooleanObjectMother.GetRandomBoolean();
      Expression<Func<Predicate<DomainObjectState>, bool>> matchesHasChangedPredicate =
          predicate => predicate(new DomainObjectState.Builder().SetNew().Value)
                       && predicate(new DomainObjectState.Builder().SetChanged().Value)
                       && predicate(new DomainObjectState.Builder().SetDeleted().Value)
                       && predicate(new DomainObjectState.Builder().SetChanged().SetDeleted().Value)
                       && predicate(new DomainObjectState.Builder().SetChanged().SetNotLoadedYet().Value)
                       && !predicate(new DomainObjectState.Builder().SetUnchanged().Value)
                       && !predicate(new DomainObjectState.Builder().SetInvalid().Value)
                       && !predicate(new DomainObjectState.Builder().SetNotLoadedYet().Value);

      commitRollbackAgentMock.Setup(mock => mock.HasData(It.Is(matchesHasChangedPredicate))).Returns(expectedResult).Verifiable();

      var result = transactionWithMocks.HasChanged();

      Assert.That(result, Is.EqualTo(expectedResult));
    }
  }
}
