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
using Remotion.Development.Data.UnitTesting.DomainObjects;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction.ReadOnlyTransactions.ForbiddenOperations
{
  [TestFixture]
  public class NewObjectTest : ReadOnlyTransactionsTestBase
  {
    [Test]
    public void NewObjectInReadOnlyRootTransaction_IsForbidden ()
    {
      CheckTransactionIsEmpty (ReadOnlyRootTransaction);
      CheckTransactionIsEmpty (ReadOnlyMiddleTransaction);
      CheckTransactionIsEmpty (WriteableSubTransaction);

      CheckForbidden (() => ExecuteInReadOnlyRootTransaction (() => Order.NewObject()), "NewObjectCreating");

      CheckTransactionIsEmpty (ReadOnlyRootTransaction);
      CheckTransactionIsEmpty (ReadOnlyMiddleTransaction);
      CheckTransactionIsEmpty (WriteableSubTransaction);
    }

    [Test]
    public void NewObjectInReadOnlyMiddleTransaction_IsForbidden ()
    {
      CheckTransactionIsEmpty (ReadOnlyRootTransaction);
      CheckTransactionIsEmpty (ReadOnlyMiddleTransaction);
      CheckTransactionIsEmpty (WriteableSubTransaction);

      CheckForbidden (() => ExecuteInReadOnlyMiddleTransaction (() => Order.NewObject()), "NewObjectCreating");

      CheckTransactionIsEmpty (ReadOnlyRootTransaction);
      CheckTransactionIsEmpty (ReadOnlyMiddleTransaction);
      CheckTransactionIsEmpty (WriteableSubTransaction);
    }

    private void CheckTransactionIsEmpty (ClientTransaction clientTransaction)
    {
      var dataManager = ClientTransactionTestHelper.GetIDataManager (clientTransaction);
      Assert.That (dataManager.RelationEndPoints, Is.Empty);
      Assert.That (dataManager.DataContainers, Is.Empty);
    }
  }
}