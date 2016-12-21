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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction.ReadOnlyTransactions.AllowedOperations
{
  [TestFixture]
  public class VirtualObjectRelationPropertyReadTest : ReadOnlyTransactionsTestBase
  {
    private Order _order1;
    private RelationEndPointID _relationEndPointID;
    private RelationEndPointID _oppositeRelationEndPointID;

    public override void SetUp ()
    {
      base.SetUp ();

      _order1 = (Order) LifetimeService.GetObjectReference (WriteableSubTransaction, DomainObjectIDs.Order1);
      _relationEndPointID = RelationEndPointID.Resolve (_order1, o => o.OrderTicket);
      _oppositeRelationEndPointID = RelationEndPointID.Create (DomainObjectIDs.OrderTicket1, _relationEndPointID.Definition.GetOppositeEndPointDefinition ());
    }

    [Test]
    public void RelationReadInReadOnlyRootTransaction_IsAllowed_NoLoading ()
    {
      WriteableSubTransaction.EnsureDataAvailable (DomainObjectIDs.Order1);
      WriteableSubTransaction.EnsureDataComplete (_relationEndPointID);
      WriteableSubTransaction.EnsureDataAvailable (DomainObjectIDs.OrderTicket1);

      var orderTicket = ExecuteInReadOnlyRootTransaction (() => _order1.OrderTicket);

      Assert.That (orderTicket.ID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
    }

    [Test]
    public void RelationReadInReadOnlyMiddleTransaction_IsAllowed_NoLoading ()
    {
      WriteableSubTransaction.EnsureDataAvailable (DomainObjectIDs.Order1);
      WriteableSubTransaction.EnsureDataComplete (_relationEndPointID);
      WriteableSubTransaction.EnsureDataAvailable (DomainObjectIDs.OrderTicket1);

      var orderTicket = ExecuteInReadOnlyMiddleTransaction (() => _order1.OrderTicket);

      Assert.That (orderTicket.ID, Is.EqualTo (DomainObjectIDs.OrderTicket1));
    }

    [Test]
    public void RelationReadInReadOnlyRootTransaction_IsAllowed_WithLoading ()
    {
      CheckDataNotLoaded (ReadOnlyRootTransaction, DomainObjectIDs.Order1);
      CheckDataNotLoaded (ReadOnlyMiddleTransaction, DomainObjectIDs.Order1);
      CheckDataNotLoaded (WriteableSubTransaction, DomainObjectIDs.Order1);

      CheckDataNotLoaded (ReadOnlyRootTransaction, DomainObjectIDs.OrderTicket1);
      CheckDataNotLoaded (ReadOnlyMiddleTransaction, DomainObjectIDs.OrderTicket1);
      CheckDataNotLoaded (WriteableSubTransaction, DomainObjectIDs.OrderTicket1);

      CheckEndPointNull (ReadOnlyRootTransaction, _relationEndPointID);
      CheckEndPointNull (ReadOnlyMiddleTransaction, _relationEndPointID);
      CheckEndPointNull (WriteableSubTransaction, _relationEndPointID);

      CheckEndPointNull (ReadOnlyRootTransaction, _oppositeRelationEndPointID);
      CheckEndPointNull (ReadOnlyMiddleTransaction, _oppositeRelationEndPointID);
      CheckEndPointNull (WriteableSubTransaction, _oppositeRelationEndPointID);

      var orderTicket = ExecuteInReadOnlyRootTransaction (() => _order1.OrderTicket);
      
      Assert.That (orderTicket.ID, Is.EqualTo (DomainObjectIDs.OrderTicket1));

      CheckDataLoaded (ReadOnlyRootTransaction, _order1);
      CheckDataNotLoaded (ReadOnlyMiddleTransaction, _order1);
      CheckDataNotLoaded (WriteableSubTransaction, _order1);

      CheckDataLoaded (ReadOnlyRootTransaction, orderTicket);
      CheckDataNotLoaded (ReadOnlyMiddleTransaction, orderTicket);
      CheckDataNotLoaded (WriteableSubTransaction, orderTicket);

      CheckEndPointComplete (ReadOnlyRootTransaction, _relationEndPointID);
      CheckEndPointNull (ReadOnlyMiddleTransaction, _relationEndPointID);
      CheckEndPointNull (WriteableSubTransaction, _relationEndPointID);

      CheckEndPointComplete (ReadOnlyRootTransaction, _oppositeRelationEndPointID);
      CheckEndPointNull (ReadOnlyMiddleTransaction, _oppositeRelationEndPointID);
      CheckEndPointNull (WriteableSubTransaction, _oppositeRelationEndPointID);
    }

    [Test]
    public void RelationReadInReadOnlyMiddleTransaction_IsAllowed_WithLoading ()
    {
      CheckDataNotLoaded (ReadOnlyRootTransaction, DomainObjectIDs.Order1);
      CheckDataNotLoaded (ReadOnlyMiddleTransaction, DomainObjectIDs.Order1);
      CheckDataNotLoaded (WriteableSubTransaction, DomainObjectIDs.Order1);

      CheckDataNotLoaded (ReadOnlyRootTransaction, DomainObjectIDs.OrderTicket1);
      CheckDataNotLoaded (ReadOnlyMiddleTransaction, DomainObjectIDs.OrderTicket1);
      CheckDataNotLoaded (WriteableSubTransaction, DomainObjectIDs.OrderTicket1);

      CheckEndPointNull (ReadOnlyRootTransaction, _relationEndPointID);
      CheckEndPointNull (ReadOnlyMiddleTransaction, _relationEndPointID);
      CheckEndPointNull (WriteableSubTransaction, _relationEndPointID);

      CheckEndPointNull (ReadOnlyRootTransaction, _oppositeRelationEndPointID);
      CheckEndPointNull (ReadOnlyMiddleTransaction, _oppositeRelationEndPointID);
      CheckEndPointNull (WriteableSubTransaction, _oppositeRelationEndPointID);

      var orderTicket = ExecuteInReadOnlyMiddleTransaction (() => _order1.OrderTicket);

      Assert.That (orderTicket.ID, Is.EqualTo (DomainObjectIDs.OrderTicket1));

      CheckDataLoaded (ReadOnlyRootTransaction, _order1);
      CheckDataLoaded (ReadOnlyMiddleTransaction, _order1);
      CheckDataNotLoaded (WriteableSubTransaction, _order1);

      CheckDataLoaded (ReadOnlyRootTransaction, orderTicket);
      CheckDataLoaded (ReadOnlyMiddleTransaction, orderTicket);
      CheckDataNotLoaded (WriteableSubTransaction, orderTicket);

      CheckEndPointComplete (ReadOnlyRootTransaction, _relationEndPointID);
      CheckEndPointComplete (ReadOnlyMiddleTransaction, _relationEndPointID);
      CheckEndPointNull (WriteableSubTransaction, _relationEndPointID);

      CheckEndPointComplete (ReadOnlyRootTransaction, _oppositeRelationEndPointID);
      CheckEndPointComplete (ReadOnlyMiddleTransaction, _oppositeRelationEndPointID);
      CheckEndPointNull (WriteableSubTransaction, _oppositeRelationEndPointID);
    }
  }
}