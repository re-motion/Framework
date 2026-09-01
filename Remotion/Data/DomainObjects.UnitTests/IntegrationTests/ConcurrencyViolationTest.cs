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
using System.Transactions;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests
{
  [TestFixture]
  public class ConcurrencyViolationTest : ClientTransactionBaseTest
  {
    [Test]
    public void SystemTransaction_Committed_PersistsChangesFromTwoSequentialClientTransactions ()
    {
      string originalSerialNumber;
      int originalOrderNumber;
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        originalSerialNumber = DomainObjectIDs.Computer1.GetObject<Computer>().SerialNumber;
        originalOrderNumber = DomainObjectIDs.Order1.GetObject<Order>().OrderNumber;
      }

      // RequiresNew suspends the ambient TransactionScope opened by DatabaseTest.SetUp for the whole test,
      // which is never completed and would otherwise doom this transaction too, hiding whether Complete() worked.
      using (var systemTransaction = new TransactionScope(TransactionScopeOption.RequiresNew))
      {
        using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
        {
          var computer = DomainObjectIDs.Computer1.GetObject<Computer>();
          computer.SerialNumber = originalSerialNumber + "-changed";
          ClientTransaction.Current.Commit();
        }

        using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
        {
          var order = DomainObjectIDs.Order1.GetObject<Order>();
          order.OrderNumber = originalOrderNumber + 1;
          ClientTransaction.Current.Commit();
        }

        systemTransaction.Complete();
      }

      using (new TransactionScope(TransactionScopeOption.Suppress))
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        Assert.That(DomainObjectIDs.Computer1.GetObject<Computer>().SerialNumber, Is.EqualTo(originalSerialNumber + "-changed"));
        Assert.That(DomainObjectIDs.Order1.GetObject<Order>().OrderNumber, Is.EqualTo(originalOrderNumber + 1));
      }
    }

    [Test]
    public void SystemTransaction_NotCompleted_RevertsChangesFromTwoSequentialClientTransactions ()
    {
      string originalSerialNumber;
      int originalOrderNumber;
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        originalSerialNumber = DomainObjectIDs.Computer1.GetObject<Computer>().SerialNumber;
        originalOrderNumber = DomainObjectIDs.Order1.GetObject<Order>().OrderNumber;
      }

      using (var systemTransaction = new TransactionScope(TransactionScopeOption.RequiresNew))
      {
        using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
        {
          var computer = DomainObjectIDs.Computer1.GetObject<Computer>();
          computer.SerialNumber = originalSerialNumber + "-changed";
          ClientTransaction.Current.Commit();
        }

        using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
        {
          var order = DomainObjectIDs.Order1.GetObject<Order>();
          order.OrderNumber = originalOrderNumber + 1;
          ClientTransaction.Current.Commit();
        }

        // systemTransaction.Complete() is intentionally not called, so Dispose() rolls back both commits.
      }

      using (new TransactionScope(TransactionScopeOption.Suppress))
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        Assert.That(DomainObjectIDs.Computer1.GetObject<Computer>().SerialNumber, Is.EqualTo(originalSerialNumber));
        Assert.That(DomainObjectIDs.Order1.GetObject<Order>().OrderNumber, Is.EqualTo(originalOrderNumber));
      }
    }

    [Test]
    public void ConcurrencyViolationException_WhenSomebodyElseModifiesData ()
    {
      var computer = DomainObjectIDs.Computer1.GetObject<Computer>();
      computer.SerialNumber = "100";

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var computerInOtherTransaction = DomainObjectIDs.Computer1.GetObject<Computer>();
        computerInOtherTransaction.SerialNumber = "200";
        ClientTransaction.Current.Commit();
      }

      try
      {
        TestableClientTransaction.Commit();
        Assert.Fail("Expected ConcurrencyViolationException");
      }
      catch (ConcurrencyViolationException)
      {
        // succeed
      }
    }

    [Test]
    public void ConcurrencyViolationException_WhenSomebodyElseRegistersForCommit ()
    {
      var computer = DomainObjectIDs.Computer1.GetObject<Computer>();
      computer.RegisterForCommit();

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var computerInOtherTransaction = DomainObjectIDs.Computer1.GetObject<Computer>();
        computerInOtherTransaction.RegisterForCommit();
        ClientTransaction.Current.Commit();
      }

      try
      {
        TestableClientTransaction.Commit();
        Assert.Fail("Expected ConcurrencyViolationException");
      }
      catch (ConcurrencyViolationException)
      {
        // succeed
      }
    }
  }
}
