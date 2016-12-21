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
  public class ExecuteInScopeTest : HierarchyBoundObjectsTestBase
  {
    private ClientTransaction _rootTransaction;
    private Order _order1LoadedInRootTransaction;

    public override void SetUp ()
    {
      base.SetUp ();

      _rootTransaction = ClientTransaction.CreateRootTransaction();
      _order1LoadedInRootTransaction = DomainObjectIDs.Order1.GetObject<Order> (_rootTransaction);
    }

    [Test]
    public void ExecuteInScope_ForLeafTransaction_AffectsCurrentTransaction ()
    {
      var subTransaction = _rootTransaction.CreateSubTransaction();

      subTransaction.ExecuteInScope (() => Assert.That (ClientTransaction.Current, Is.SameAs (subTransaction)));
      subTransaction.ExecuteInScope (
          () =>
          {
            Assert.That (ClientTransaction.Current, Is.SameAs (subTransaction));
            return 7;
          });
    }

    [Test]
    public void ExecuteInScope_AffectsNewObject ()
    {
      Assert.That (
          () => Order.NewObject(),
          Throws.InvalidOperationException.With.Message.EqualTo ("No ClientTransaction has been associated with the current thread."));

      var leafTransaction = _rootTransaction.CreateSubTransaction ();
      leafTransaction.ExecuteInScope (() =>
      {
        var instance = Order.NewObject ();
        Assert.That (instance.RootTransaction, Is.SameAs (_rootTransaction));
        Assert.That (instance.DefaultTransactionContext.ClientTransaction, Is.SameAs (leafTransaction));
      });
    }

    [Test]
    public void ExecuteInScope_AffectsGetObject ()
    {
      Assert.That (
          () => DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes> (),
          Throws.InvalidOperationException.With.Message.EqualTo ("No ClientTransaction has been associated with the current thread."));

      var clientTransaction = _rootTransaction.CreateSubTransaction ();
      clientTransaction.ExecuteInScope (
          () =>
          {
            var instance = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes>();
            Assert.That (instance.RootTransaction, Is.SameAs (_rootTransaction));
            Assert.That (instance.DefaultTransactionContext.ClientTransaction, Is.SameAs (clientTransaction));
          });
    }

    [Test]
    public void ExecuteInScope_AffectsQuery ()
    {
      var query = from cwadt in QueryFactory.CreateLinqQuery<ClassWithAllDataTypes> () select cwadt;
      Assert.That (() => query.ToArray (), Throws.InvalidOperationException.With.Message.EqualTo ("No ClientTransaction has been associated with the current thread."));

      var clientTransaction = _rootTransaction.CreateSubTransaction ();
      clientTransaction.ExecuteInScope (
          () =>
          {
            var result = query.ToArray().First();
            Assert.That (result.RootTransaction, Is.SameAs (_rootTransaction));
            Assert.That (result.DefaultTransactionContext.ClientTransaction, Is.SameAs (clientTransaction));
          });
    }

    [Test]
    public void ExecuteInScope_ForOtherHierarchy_DoesNotAffectDefaultContext ()
    {
      _order1LoadedInRootTransaction.OrderNumber = 2;

      ClientTransaction.CreateRootTransaction ().ExecuteInScope (() =>
      {
        Assert.That (ClientTransaction.Current, Is.Not.SameAs (_rootTransaction));

        Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (_rootTransaction));
        Assert.That (_order1LoadedInRootTransaction.OrderNumber, Is.EqualTo (2));
        Assert.That (_order1LoadedInRootTransaction.State, Is.EqualTo (StateType.Changed));
      });
    }

    [Test]
    public void ExecuteInScope_InfluencesDefaultContext ()
    {
      var subTransaction = _rootTransaction.CreateSubTransaction();

      Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (_rootTransaction));
      Assert.That (subTransaction.ActiveTransaction, Is.SameAs (_rootTransaction));

      Assert.That (_order1LoadedInRootTransaction.RootTransaction, Is.SameAs (_rootTransaction));
      Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (_rootTransaction));

      Assert.That (() => _order1LoadedInRootTransaction.OrderNumber = 2, Throws.TypeOf<ClientTransactionReadOnlyException> ());

      subTransaction.ExecuteInScope (
          () =>
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
          });

      Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (_rootTransaction));

      Assert.That (_order1LoadedInRootTransaction.OrderNumber, Is.EqualTo (1));
    }

    [Test]
    public void ExecuteInScope_NestedForDifferentTransactionsInTheHierarchy ()
    {
      var middleTransaction = _rootTransaction.CreateSubTransaction ();
      var subTransaction = middleTransaction.CreateSubTransaction ();

      Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (_rootTransaction));
      Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (_rootTransaction));

      middleTransaction.ExecuteInScope (() =>
      {
        Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (middleTransaction));
        Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (middleTransaction));

        subTransaction.ExecuteInScope (
            () =>
            {
              Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (subTransaction));
              Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (subTransaction));

              _rootTransaction.ExecuteInScope (
                  () =>
                  {
                    Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (_rootTransaction));
                    Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (_rootTransaction));
                  });

              Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (subTransaction));
              Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (subTransaction));
            });

        Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (middleTransaction));
        Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (middleTransaction));
      });

      Assert.That (_rootTransaction.ActiveTransaction, Is.SameAs (_rootTransaction));
      Assert.That (_order1LoadedInRootTransaction.DefaultTransactionContext.ClientTransaction, Is.SameAs (_rootTransaction));
    }
  }
}