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
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction.ReadOnlyTransactions.AllowedOperations
{
  [TestFixture]
  public class ResurrectTest : ReadOnlyTransactionsTestBase
  {
    private Order _invalidOrder;

    public override void SetUp ()
    {
      base.SetUp();

      _invalidOrder = ExecuteInWriteableSubTransaction(() => Order.NewObject());
      ExecuteInWriteableSubTransaction(() => _invalidOrder.Delete());
    }

    [Test]
    public void ResurrectInReadOnlyRootTransaction_IsAllowed ()
    {
      CheckState(ReadOnlyRootTransaction, _invalidOrder, state => state.IsInvalid);
      CheckState(ReadOnlyMiddleTransaction, _invalidOrder, state => state.IsInvalid);
      CheckState(WriteableSubTransaction, _invalidOrder, state => state.IsInvalid);

      ResurrectionService.ResurrectInvalidObject(ReadOnlyRootTransaction, _invalidOrder.ID);

      CheckState(ReadOnlyRootTransaction, _invalidOrder, state => state.IsNotLoadedYet);
      CheckState(ReadOnlyMiddleTransaction, _invalidOrder, state => state.IsNotLoadedYet);
      CheckState(WriteableSubTransaction, _invalidOrder, state => state.IsNotLoadedYet);
    }

    [Test]
    public void UnloadDataInReadOnlyMiddleTransaction_IsAllowed ()
    {
      CheckState(ReadOnlyRootTransaction, _invalidOrder, state => state.IsInvalid);
      CheckState(ReadOnlyMiddleTransaction, _invalidOrder, state => state.IsInvalid);
      CheckState(WriteableSubTransaction, _invalidOrder, state => state.IsInvalid);

      ResurrectionService.ResurrectInvalidObject(ReadOnlyMiddleTransaction, _invalidOrder.ID);

      CheckState(ReadOnlyRootTransaction, _invalidOrder, state => state.IsNotLoadedYet);
      CheckState(ReadOnlyMiddleTransaction, _invalidOrder, state => state.IsNotLoadedYet);
      CheckState(WriteableSubTransaction, _invalidOrder, state => state.IsNotLoadedYet);
    }
  }
}
