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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests
{
  [TestFixture]
  public class DomainObjectAssociatedTransactionHierarchyTest : StandardMappingTest
  {
    private ClientTransaction _rootTransaction;

    public override void SetUp ()
    {
      base.SetUp ();

      _rootTransaction = ClientTransaction.CreateRootTransaction();
    }

    [Test]
    public void NewObject_InRootTransaction_AssociatesThatRootTransaction ()
    {
      var order = _rootTransaction.ExecuteInScope (() => Order.NewObject());

      Assert.That (order.RootTransaction, Is.SameAs (_rootTransaction));
    }

    [Test]
    public void NewObject_InSubTransaction_AssociatesRootTransaction ()
    {
      var subTransaction = _rootTransaction.CreateSubTransaction();

      var order = subTransaction.ExecuteInScope (() => Order.NewObject ());

      Assert.That (order.RootTransaction, Is.SameAs (_rootTransaction));
      Assert.That (order.TransactionContext[_rootTransaction].State, Is.EqualTo (StateType.Invalid));
    }

    [Test]
    public void GetObjectReference_InRootTransaction_AssociatesThatRootTransaction ()
    {
      var order = DomainObjectIDs.Order1.GetObjectReference<Order> (_rootTransaction);

      Assert.That (order.RootTransaction, Is.SameAs (_rootTransaction));
    }

    [Test]
    public void GetObjectReference_InSubTransaction_AssociatesRootTransaction ()
    {
      var subTransaction = _rootTransaction.CreateSubTransaction ();

      var order = DomainObjectIDs.Order1.GetObjectReference<Order> (subTransaction);

      Assert.That (order.RootTransaction, Is.SameAs (_rootTransaction));
    }

    [Test]
    public void AssociatedRootTransaction_InTheContextOfSubtransactions_StaysTheSame ()
    {
      var order = _rootTransaction.ExecuteInScope (() => Order.NewObject ());

      using (_rootTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That (order.RootTransaction, Is.SameAs (_rootTransaction));
      }
    }
  }
}