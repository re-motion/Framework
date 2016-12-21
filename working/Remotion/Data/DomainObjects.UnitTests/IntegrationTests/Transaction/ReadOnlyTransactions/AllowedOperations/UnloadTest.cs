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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction.ReadOnlyTransactions.AllowedOperations
{
  [TestFixture]
  public class UnloadTest : ReadOnlyTransactionsTestBase
  {
    private Order _order1;
    private RelationEndPointID _endPointID;

    public override void SetUp ()
    {
      base.SetUp ();

      _order1 = ExecuteInWriteableSubTransaction (() => DomainObjectIDs.Order1.GetObject<Order> ());
      ExecuteInWriteableSubTransaction (() => _order1.OrderItems.EnsureDataComplete());
      _endPointID = ExecuteInWriteableSubTransaction (() => _order1.OrderItems.AssociatedEndPointID);
    }

    [Test]
    public void UnloadDataInReadOnlyRootTransaction_IsAllowed ()
    {
      CheckDataLoaded (ReadOnlyRootTransaction, _order1);
      CheckDataLoaded (ReadOnlyMiddleTransaction, _order1);
      CheckDataLoaded (WriteableSubTransaction, _order1);

      UnloadService.UnloadData (ReadOnlyRootTransaction, DomainObjectIDs.Order1);

      CheckDataNotLoaded (ReadOnlyRootTransaction, _order1);
      CheckDataNotLoaded (ReadOnlyMiddleTransaction, _order1);
      CheckDataNotLoaded (WriteableSubTransaction, _order1);
    }

    [Test]
    public void UnloadDataInReadOnlyMiddleTransaction_IsAllowed ()
    {
      CheckDataLoaded (ReadOnlyRootTransaction, _order1);
      CheckDataLoaded (ReadOnlyMiddleTransaction, _order1);
      CheckDataLoaded (WriteableSubTransaction, _order1);

      UnloadService.UnloadData (ReadOnlyMiddleTransaction, DomainObjectIDs.Order1);

      CheckDataNotLoaded (ReadOnlyRootTransaction, _order1);
      CheckDataNotLoaded (ReadOnlyMiddleTransaction, _order1);
      CheckDataNotLoaded (WriteableSubTransaction, _order1);
    }

    [Test]
    public void UnloadVirtualEndPointInReadOnlyRootTransaction_IsAllowed ()
    {
      CheckEndPointComplete (ReadOnlyRootTransaction, _endPointID);
      CheckEndPointComplete (ReadOnlyMiddleTransaction, _endPointID);
      CheckEndPointComplete (WriteableSubTransaction, _endPointID);

      UnloadService.UnloadVirtualEndPoint (ReadOnlyRootTransaction, _endPointID);

      CheckEndPointUnloaded (ReadOnlyRootTransaction, _endPointID);
      CheckEndPointUnloaded (ReadOnlyMiddleTransaction, _endPointID);
      CheckEndPointUnloaded (WriteableSubTransaction, _endPointID);
    }

    [Test]
    public void UnloadVirtualRelationEndPointInReadOnlyMiddleTransaction_IsAllowed ()
    {
      CheckEndPointComplete (ReadOnlyRootTransaction, _endPointID);
      CheckEndPointComplete (ReadOnlyMiddleTransaction, _endPointID);
      CheckEndPointComplete (WriteableSubTransaction, _endPointID);

      UnloadService.UnloadVirtualEndPoint (ReadOnlyMiddleTransaction, _endPointID);

      CheckEndPointUnloaded (ReadOnlyRootTransaction, _endPointID);
      CheckEndPointUnloaded (ReadOnlyMiddleTransaction, _endPointID);
      CheckEndPointUnloaded (WriteableSubTransaction, _endPointID);
    }

    protected void CheckEndPointUnloaded (ClientTransaction clientTransaction, RelationEndPointID relationEndPointID)
    {
      CheckEndPointIncomplete(clientTransaction, relationEndPointID);
    }
  }
}