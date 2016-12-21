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
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction.ReadOnlyTransactions.AllowedOperations
{
  [TestFixture]
  public class CollectionQueryTest : ReadOnlyTransactionsTestBase
  {
    [Test]
    public void QueryInReadOnlyRootTransaction_WithoutLoading_IsAllowed ()
    {
      WriteableSubTransaction.EnsureDataAvailable (DomainObjectIDs.Order1);

      var resultSet = ExecuteInReadOnlyRootTransaction (() => QueryFactory.CreateLinqQuery<Order>().Where (obj => obj.OrderNumber == 1).ToList());

      Assert.That (resultSet, Has.Count.EqualTo (1));
      Assert.That (resultSet[0].ID, Is.EqualTo (DomainObjectIDs.Order1));
    }

    [Test]
    public void QueryInReadOnlyMiddleTransaction_WithoutLoading_IsAllowed ()
    {
      WriteableSubTransaction.EnsureDataAvailable (DomainObjectIDs.Order1);

      var resultSet = ExecuteInReadOnlyMiddleTransaction ( () => QueryFactory.CreateLinqQuery<Order>().Where (obj => obj.OrderNumber == 1).ToList());

      Assert.That (resultSet, Has.Count.EqualTo (1));
      Assert.That (resultSet[0].ID, Is.EqualTo (DomainObjectIDs.Order1));
    }

    [Test]
    public void QueryInReadOnlyRootTransaction_WithLoading_IsAllowed ()
    {
      CheckDataNotLoaded (ReadOnlyRootTransaction, DomainObjectIDs.Order1);
      CheckDataNotLoaded (ReadOnlyMiddleTransaction, DomainObjectIDs.Order1);
      CheckDataNotLoaded (WriteableSubTransaction, DomainObjectIDs.Order1);

      var resultSet = ExecuteInReadOnlyRootTransaction (() => QueryFactory.CreateLinqQuery<Order>().Where (obj => obj.OrderNumber == 1).ToList());

      Assert.That (resultSet, Has.Count.EqualTo (1));
      Assert.That (resultSet[0].ID, Is.EqualTo (DomainObjectIDs.Order1));

      CheckDataLoaded (ReadOnlyRootTransaction, resultSet[0]);
      CheckDataNotLoaded (ReadOnlyMiddleTransaction, resultSet[0]);
      CheckDataNotLoaded (WriteableSubTransaction, resultSet[0]);
    }

    [Test]
    public void QueryInReadOnlyMiddleTransaction_WithLoading_IsAllowed ()
    {
      CheckDataNotLoaded (ReadOnlyRootTransaction, DomainObjectIDs.Order1);
      CheckDataNotLoaded (ReadOnlyMiddleTransaction, DomainObjectIDs.Order1);
      CheckDataNotLoaded (WriteableSubTransaction, DomainObjectIDs.Order1);

      var resultSet = ExecuteInReadOnlyMiddleTransaction (() => QueryFactory.CreateLinqQuery<Order>().Where (obj => obj.OrderNumber == 1).ToList());

      Assert.That (resultSet, Has.Count.EqualTo (1));
      Assert.That (resultSet[0].ID, Is.EqualTo (DomainObjectIDs.Order1));

      CheckDataLoaded (ReadOnlyRootTransaction, resultSet[0]);
      CheckDataLoaded (ReadOnlyMiddleTransaction, resultSet[0]);
      CheckDataNotLoaded (WriteableSubTransaction, resultSet[0]);
    }
  }
}