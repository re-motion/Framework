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
  public class CustomQueryTest : ReadOnlyTransactionsTestBase
  {
    [Test]
    public void QueryInReadOnlyRootTransaction_IsAllowed ()
    {
      var resultSet = ExecuteInReadOnlyRootTransaction (
          () => QueryFactory.CreateLinqQuery<Order> ().Where (obj => obj.OrderNumber == 1).Select (o => new { o.OrderNumber }).ToList());

      Assert.That (resultSet, Has.Count.EqualTo (1));
      Assert.That (resultSet[0].OrderNumber, Is.EqualTo (1));
    }

    [Test]
    public void QueryInReadOnlyMiddleTransaction_IsAllowed ()
    {
      var resultSet = ExecuteInReadOnlyMiddleTransaction (
          () => QueryFactory.CreateLinqQuery<Order> ().Where (obj => obj.OrderNumber == 1).Select (o => new { o.OrderNumber }).ToList ());

      Assert.That (resultSet, Has.Count.EqualTo (1));
      Assert.That (resultSet[0].OrderNumber, Is.EqualTo (1));
    }
  }
}