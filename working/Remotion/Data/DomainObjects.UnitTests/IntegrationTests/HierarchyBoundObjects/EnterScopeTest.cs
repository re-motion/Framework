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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.HierarchyBoundObjects
{
  [TestFixture]
  public class EnterScopeTest : HierarchyBoundObjectsTestBase
  {
    private ClientTransaction _rootTransaction;
    private Order _order1LoadedInRootTransaction;

    public override void SetUp ()
    {
      base.SetUp ();

      _rootTransaction = ClientTransaction.CreateRootTransaction();
      _order1LoadedInRootTransaction = DomainObjectIDs.Order1.GetObject<Order> (_rootTransaction);
    }

    public override void TearDown ()
    {
      base.TearDown ();

      // Cleanup scopes left open by some test.
      while (ClientTransactionScope.ActiveScope != null)
      {
        ClientTransactionScope.ActiveScope.Leave();
      }
    }

    [Test]
    public void OpeningScopeForLeafTransaction_AffectsCurrentTransaction ()
    {
      using (_rootTransaction.CreateSubTransaction ().EnterNonDiscardingScope ())
      {
        Assert.That (ClientTransaction.Current, Is.SameAs (_rootTransaction.LeafTransaction));
        ClientTransaction.Current.Discard ();
      }

      using (_rootTransaction.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (ClientTransaction.Current, Is.SameAs (_rootTransaction.LeafTransaction));
      }

      using (_rootTransaction.CreateSubTransaction ().EnterScope (AutoRollbackBehavior.Rollback))
      {
        Assert.That (ClientTransaction.Current, Is.SameAs (_rootTransaction.LeafTransaction));
      }
    }

    [Test]
    public void Scope_AffectsNewObject ()
    {
      Assert.That (
          () => Order.NewObject(),
          Throws.InvalidOperationException.With.Message.EqualTo ("No ClientTransaction has been associated with the current thread."));

      var leafTransaction = _rootTransaction.CreateSubTransaction ();
      using (leafTransaction.EnterDiscardingScope ())
      {
        var instance = Order.NewObject ();
        Assert.That (instance.RootTransaction, Is.SameAs (_rootTransaction));
        Assert.That (instance.DefaultTransactionContext.ClientTransaction, Is.SameAs (leafTransaction));
      }
    }

    [Test]
    public void Scope_AffectsGetObject ()
    {
      Assert.That (
          () => DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes> (),
          Throws.InvalidOperationException.With.Message.EqualTo ("No ClientTransaction has been associated with the current thread."));

      var clientTransaction = _rootTransaction.CreateSubTransaction ();
      using (clientTransaction.EnterNonDiscardingScope ())
      {
        var instance = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes> ();
        Assert.That (instance.RootTransaction, Is.SameAs (_rootTransaction));
        Assert.That (instance.DefaultTransactionContext.ClientTransaction, Is.SameAs (clientTransaction));
      }
    }

    [Test]
    public void Scope_AffectsQuery ()
    {
      var query = from cwadt in QueryFactory.CreateLinqQuery<ClassWithAllDataTypes> () select cwadt;
      Assert.That (
          () => query.ToArray(),
          Throws.InvalidOperationException.With.Message.EqualTo ("No ClientTransaction has been associated with the current thread."));

      var clientTransaction = _rootTransaction.CreateSubTransaction ();
      using (clientTransaction.EnterNonDiscardingScope ())
      {
        var result = query.ToArray ().First ();
        Assert.That (result.RootTransaction, Is.SameAs (_rootTransaction));
        Assert.That (result.DefaultTransactionContext.ClientTransaction, Is.SameAs (clientTransaction));
      }
    }

    [Test]
    public void ScopeForOtherHierarchy_DoesNotAffectDefaultContext ()
    {
      _order1LoadedInRootTransaction.OrderNumber = 2;

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        Assert.That (ClientTransaction.Current, Is.Not.SameAs (_rootTransaction));

        Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (_rootTransaction));
        Assert.That (_order1LoadedInRootTransaction.OrderNumber, Is.EqualTo (2));
        Assert.That (_order1LoadedInRootTransaction.State, Is.EqualTo (StateType.Changed));
      }

      using (ClientTransaction.CreateRootTransaction ().EnterNonDiscardingScope ())
      {
        Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (_rootTransaction));
        Assert.That (_order1LoadedInRootTransaction.OrderNumber, Is.EqualTo (2));
        Assert.That (_order1LoadedInRootTransaction.State, Is.EqualTo (StateType.Changed));
      }

      using (ClientTransaction.CreateRootTransaction ().EnterScope (AutoRollbackBehavior.Rollback))
      {
        Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (_rootTransaction));
        Assert.That (_order1LoadedInRootTransaction.OrderNumber, Is.EqualTo (2));
        Assert.That (_order1LoadedInRootTransaction.State, Is.EqualTo (StateType.Changed));
      }
    }

    [Test]
    public void OpeningScopeForTransaction_InfluencesDefaultContext ()
    {
      var subTransaction = _rootTransaction.CreateSubTransaction();

      Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (_rootTransaction));
      Assert.That (subTransaction.ActiveTransaction, Is.SameAs (_rootTransaction));

      Assert.That (_order1LoadedInRootTransaction.RootTransaction, Is.SameAs (_rootTransaction));
      Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (_rootTransaction));

      Assert.That (() => _order1LoadedInRootTransaction.OrderNumber = 2, Throws.TypeOf<ClientTransactionReadOnlyException> ());

      using (subTransaction.EnterNonDiscardingScope())
      {
        Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (subTransaction));
        Assert.That (subTransaction.ActiveTransaction, Is.SameAs (subTransaction));

        Assert.That (_order1LoadedInRootTransaction.RootTransaction, Is.SameAs (_rootTransaction));
        Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (subTransaction));

        Assert.That (_order1LoadedInRootTransaction.OrderNumber, Is.EqualTo (1));
        Assert.That (_order1LoadedInRootTransaction.State, Is.EqualTo (StateType.Unchanged));

        _order1LoadedInRootTransaction.OrderNumber = 3;

        Assert.That (_order1LoadedInRootTransaction.OrderNumber, Is.EqualTo (3));
        Assert.That (_order1LoadedInRootTransaction.State, Is.EqualTo (StateType.Changed));
      }

      Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (_rootTransaction));
      Assert.That (_order1LoadedInRootTransaction.OrderNumber, Is.EqualTo (1));
    }

    [Test]
    public void Scope_NestedForDifferentTransactionsInTheHierarchy ()
    {
      var middleTransaction = _rootTransaction.CreateSubTransaction ();
      var subTransaction = middleTransaction.CreateSubTransaction ();

      Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (_rootTransaction));
      Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (_rootTransaction));

      using (middleTransaction.EnterNonDiscardingScope())
      {
        Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (middleTransaction));
        Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (middleTransaction));

        using (subTransaction.EnterNonDiscardingScope())
        {
          Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (subTransaction));
          Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (subTransaction));

          using (_rootTransaction.EnterNonDiscardingScope())  
          {
            Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (_rootTransaction));
            Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (_rootTransaction));
          }

          Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (subTransaction));
          Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (subTransaction));
        }

        Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (middleTransaction));
        Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (middleTransaction));
      }

      Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (_rootTransaction));
      Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (_rootTransaction));
    }

    [Test]
    public void CreatingSubTransaction_WithinScope_DoesNotInfluenceActiveTransaction ()
    {
      Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (_rootTransaction));

      using (_rootTransaction.EnterNonDiscardingScope())
      {
        Assert.That (ClientTransaction.Current, Is.SameAs (_rootTransaction));
        Assert.That (ClientTransaction.Current.ActiveTransaction, Is.SameAs (_rootTransaction));

        _rootTransaction.CreateSubTransaction ();

        Assert.That (ClientTransaction.Current, Is.SameAs (_rootTransaction));
        Assert.That (ClientTransaction.Current.ActiveTransaction, Is.SameAs (_rootTransaction));
      }

      Assert.That (ClientTransaction.Current, Is.Null);
      Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (_rootTransaction));
    }

    [Test]
    public void LeaveOuterScope_WithoutLeavingInnerScope_MakeActiveFlags_Throws ()
    {
      var rootScope = _rootTransaction.EnterNonDiscardingScope();
        Assert.That (ClientTransaction.Current, Is.SameAs (_rootTransaction));
        Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (_rootTransaction));

      var subTransaction = _rootTransaction.CreateSubTransaction ();
      subTransaction.EnterNonDiscardingScope();

      Assert.That (ClientTransaction.Current, Is.SameAs (subTransaction));
      Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (subTransaction));

      Assert.That (
          () => rootScope.Leave(),
          Throws.TypeOf<InvalidOperationException>()
                .With.Message.EqualTo ("This ClientTransactionScope is not the active scope. Leave the active scope before leaving this one."));

      Assert.That (ClientTransaction.Current, Is.SameAs (subTransaction));
      Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (subTransaction));
    }
  }
}