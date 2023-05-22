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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure
{
  [TestFixture]
  public class DomainObjectCheckUtilityTest : ClientTransactionBaseTest
  {
    [Test]
    public void EnsureNotInvalid_Valid ()
    {
      var order = Order.NewObject();

      DomainObjectCheckUtility.EnsureNotInvalid(order, ClientTransaction.Current);
    }

    [Test]
    public void EnsureNotInvalid_Discarded ()
    {
      var order = Order.NewObject();
      order.Delete();
      Assert.That(
          () => DomainObjectCheckUtility.EnsureNotInvalid(order, ClientTransaction.Current),
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void CheckIfRightTransaction_Works_ForSameRootTransaction ()
    {
      var order = Order.NewObject();

      DomainObjectCheckUtility.CheckIfRightTransaction(order, ClientTransaction.Current);
    }

    [Test]
    public void CheckIfRightTransaction_Works_ForLeftSubTransaction ()
    {
      var order = TestableClientTransaction.CreateSubTransaction().ExecuteInScope(() => Order.NewObject());

      DomainObjectCheckUtility.CheckIfRightTransaction(order, TestableClientTransaction);
    }

    [Test]
    public void CheckIfRightTransaction_Works_ForRightSubTransaction ()
    {
      var order = Order.NewObject();

      var subTransaction = TestableClientTransaction.CreateSubTransaction();
      DomainObjectCheckUtility.CheckIfRightTransaction(order, subTransaction);
    }

    [Test]
    public void CheckIfRightTransaction_Fails ()
    {
      var order = Order.NewObject();
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        Assert.That(
            () => DomainObjectCheckUtility.CheckIfRightTransaction(order, ClientTransaction.Current),
            Throws.InstanceOf<ClientTransactionsDifferException>()
                .With.Message.Matches(
                    "Domain object 'Order|.*|System.Guid' cannot be used in the "
                    + "given transaction as it was loaded or created in another transaction. Enter a scope for the transaction, or call EnlistInTransaction to "
                    + "enlist the object in the transaction. (If no transaction was explicitly given, ClientTransaction.Current was used.)"));
      }
    }

    [Test]
    public void DebugCheckIfRightTransaction_Works_ForSameRootTransaction ()
    {
      var order = Order.NewObject();

      DomainObjectCheckUtility.DebugCheckIfRightTransaction(order, ClientTransaction.Current);
    }

    [Test]
    public void DebugCheckIfRightTransaction_Works_ForLeftSubTransaction ()
    {
      var order = TestableClientTransaction.CreateSubTransaction().ExecuteInScope(() => Order.NewObject());

      DomainObjectCheckUtility.DebugCheckIfRightTransaction(order, TestableClientTransaction);
    }

    [Test]
    public void DebugCheckIfRightTransaction_Works_ForRightSubTransaction ()
    {
      var order = Order.NewObject();

      var subTransaction = TestableClientTransaction.CreateSubTransaction();
      DomainObjectCheckUtility.DebugCheckIfRightTransaction(order, subTransaction);
    }

#if !DEBUG
    [Ignore("Skipped unless DEBUG build")]
#endif
    [Test]
    public void DebugCheckIfRightTransaction_Fails ()
    {
      var order = Order.NewObject();
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        Assert.That(
            () => DomainObjectCheckUtility.DebugCheckIfRightTransaction(order, ClientTransaction.Current),
            Throws.InstanceOf<ClientTransactionsDifferException>()
                .With.Message.Matches(
                    "Domain object 'Order|.*|System.Guid' cannot be used in the "
                    + "given transaction as it was loaded or created in another transaction. Enter a scope for the transaction, or call EnlistInTransaction to "
                    + "enlist the object in the transaction. (If no transaction was explicitly given, ClientTransaction.Current was used.)"));
      }
    }

    [Test]
    public void EnsureNotDeleted_NotDeleted ()
    {
      var relatedObject = DomainObjectIDs.OrderItem1.GetObject<OrderItem>();

      DomainObjectCheckUtility.EnsureNotDeleted(relatedObject, TestableClientTransaction);
    }

    [Test]
    public void EnsureNotDeleted_Deleted ()
    {
      var relatedObject = DomainObjectIDs.OrderItem1.GetObject<OrderItem>();
      relatedObject.Delete();
      Assert.That(
          () => DomainObjectCheckUtility.EnsureNotDeleted(relatedObject, TestableClientTransaction),
          Throws.InstanceOf<ObjectDeletedException>());
    }
  }
}
