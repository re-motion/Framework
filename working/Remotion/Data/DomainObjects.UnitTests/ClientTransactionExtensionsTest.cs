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
using Remotion.Development.Data.UnitTesting.DomainObjects;

namespace Remotion.Data.DomainObjects.UnitTests
{
  [TestFixture]
  public class ClientTransactionExtensionsTest : StandardMappingTest
  {
    private ClientTransaction _transaction;

    public override void SetUp ()
    {
      base.SetUp ();

      _transaction = ClientTransaction.CreateRootTransaction ();
    }

    [Test]
    public void ExecuteInScope_Action_RunsDelegate ()
    {
      bool delegateRun = false;
      Action action = () => delegateRun = true;

      _transaction.ExecuteInScope (action);

      Assert.That (delegateRun, Is.True);
    }

    [Test]
    public void ExecuteInScope_Action_SetsCurrentTx ()
    {
      Assert.That (ClientTransaction.Current, Is.Null);
      
      ClientTransaction currentInDelegate = null;
      Action action = () => currentInDelegate = ClientTransaction.Current;

      _transaction.ExecuteInScope (action);

      Assert.That (currentInDelegate, Is.SameAs (_transaction));
      Assert.That (ClientTransaction.Current, Is.Null);
    }

    [Test]
    public void ExecuteInScope_Action_ReusesScopeIfPossible ()
    {
      using (var scope = _transaction.EnterNonDiscardingScope ())
      {
        Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (scope));

        ClientTransactionScope scopeInDelegate = null;
        Action action = () => scopeInDelegate = ClientTransactionScope.ActiveScope;

        _transaction.ExecuteInScope (action);

        Assert.That (scopeInDelegate, Is.SameAs (scope));
        Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (scope));
      }
    }

    [Test]
    public void ExecuteInScope_Action_ActivatesTransaction ()
    {
      using (ClientTransactionTestHelper.MakeInactive (_transaction))
      {
        var delegateRun = false;
        Action action = () =>
        {
          Assert.That (_transaction.ActiveTransaction, Is.SameAs (_transaction));
          Assert.That (ClientTransaction.Current, Is.SameAs (_transaction));
          delegateRun = true;
        };

        _transaction.ExecuteInScope (action);

        Assert.That (delegateRun, Is.True);
      }
    }

    [Test]
    public void ExecuteInScope_Action_ActivatesTransaction_EvenWhenAlreadyCurrent ()
    {
      using (_transaction.EnterNonDiscardingScope())
      {
        using (ClientTransactionTestHelper.MakeInactive (_transaction))
        {
          var delegateRun = false;
          Action action = () =>
          {
            Assert.That (_transaction.ActiveTransaction, Is.SameAs (_transaction));
            Assert.That (ClientTransaction.Current, Is.SameAs (_transaction));
            delegateRun = true;
          };

          _transaction.ExecuteInScope (action);

          Assert.That (delegateRun, Is.True);
        }
      }
    }

    [Test]
    public void ExecuteInScope_Func_RunsDelegate ()
    {
      Func<int> func = () => 17;
      var result = _transaction.ExecuteInScope (func);

      Assert.That (result, Is.EqualTo (17));
    }

    [Test]
    public void ExecuteInScope_Func_SetsCurrentTx ()
    {
      Assert.That (ClientTransaction.Current, Is.Null);

      ClientTransaction currentInDelegate = null;
      Func<int> func = () =>
      {
        currentInDelegate = ClientTransaction.Current;
        return 4;
      };

      _transaction.ExecuteInScope (func);

      Assert.That (currentInDelegate, Is.SameAs (_transaction));
      Assert.That (ClientTransaction.Current, Is.Null);
    }

    [Test]
    public void ExecuteInScope_Func_ReusesScopeIfPossible ()
    {
      using (var scope = _transaction.EnterNonDiscardingScope ())
      {
        Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (scope));

        ClientTransactionScope scopeInDelegate = null;
        Func<int> func = () =>
        {
          scopeInDelegate = ClientTransactionScope.ActiveScope;
          return 4;
        };

        _transaction.ExecuteInScope (func);

        Assert.That (scopeInDelegate, Is.SameAs (scope));
        Assert.That (ClientTransactionScope.ActiveScope, Is.SameAs (scope));
      }
    }

    [Test]
    public void ExecuteInScope_Func_ActivatesTransaction ()
    {
      using (ClientTransactionTestHelper.MakeInactive (_transaction))
      {
        var delegateRun = false;
        Func<int> func = () =>
        {
          Assert.That (_transaction.ActiveTransaction, Is.SameAs (_transaction));
          Assert.That (ClientTransaction.Current, Is.SameAs (_transaction));
          delegateRun = true;
          return 7;
        };

        var result = _transaction.ExecuteInScope (func);

        Assert.That (delegateRun, Is.True);
        Assert.That (result, Is.EqualTo (7));
      }
    }

    [Test]
    public void ExecuteInScope_Func_ActivatesTransaction_EvenWhenAlreadyCurrent ()
    {
      using (_transaction.EnterNonDiscardingScope ())
      {
        using (ClientTransactionTestHelper.MakeInactive (_transaction))
        {
          var delegateRun = false;
          Func<int> func = () =>
          {
            Assert.That (_transaction.ActiveTransaction, Is.SameAs (_transaction));
            Assert.That (ClientTransaction.Current, Is.SameAs (_transaction));
            delegateRun = true;
            return 7;
          };

          var result = _transaction.ExecuteInScope (func);

          Assert.That (delegateRun, Is.True);
          Assert.That (result, Is.EqualTo (7));
        }
      }
    }
  }
}