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

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction.ReadOnlyTransactions.AllowedOperations
{
  [TestFixture]
  public class GetObjectTest : ReadOnlyTransactionsTestBase
  {
    [Test]
    public void LoadInReadOnlyRootTransaction_IsAllowed ()
    {
      CheckDataNotLoaded (ReadOnlyRootTransaction, DomainObjectIDs.Order1);
      CheckDataNotLoaded (ReadOnlyMiddleTransaction, DomainObjectIDs.Order1);
      CheckDataNotLoaded (WriteableSubTransaction, DomainObjectIDs.Order1);

      var order1 = ExecuteInReadOnlyRootTransaction (() => DomainObjectIDs.Order1.GetObject<Order> ());

      Assert.That (order1.ID, Is.EqualTo (DomainObjectIDs.Order1));

      CheckDataLoaded (ReadOnlyRootTransaction, order1);
      CheckDataNotLoaded (ReadOnlyMiddleTransaction, order1);
      CheckDataNotLoaded (WriteableSubTransaction, order1);
    }

    [Test]
    public void LoadInReadOnlyMiddleTransaction_IsAllowed ()
    {
      CheckDataNotLoaded (ReadOnlyRootTransaction, DomainObjectIDs.Order1);
      CheckDataNotLoaded (ReadOnlyMiddleTransaction, DomainObjectIDs.Order1);
      CheckDataNotLoaded (WriteableSubTransaction, DomainObjectIDs.Order1);

      var order1 = ExecuteInReadOnlyMiddleTransaction (() => DomainObjectIDs.Order1.GetObject<Order>());

      Assert.That (order1.ID, Is.EqualTo (DomainObjectIDs.Order1));

      CheckDataLoaded (ReadOnlyRootTransaction, order1);
      CheckDataLoaded (ReadOnlyMiddleTransaction, order1);
      CheckDataNotLoaded (WriteableSubTransaction, order1);
    }
  }
}