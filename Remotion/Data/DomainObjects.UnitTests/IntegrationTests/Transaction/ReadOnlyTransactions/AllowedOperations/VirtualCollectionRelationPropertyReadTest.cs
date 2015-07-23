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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Transaction.ReadOnlyTransactions.AllowedOperations
{
  [TestFixture]
  public class VirtualCollectionRelationPropertyReadTest : ReadOnlyTransactionsTestBase
  {
    private Order _order1;
    private RelationEndPointID _relationEndPointID;
    private RelationEndPointID _oppositeRelationEndPointID;

    public override void SetUp ()
    {
      base.SetUp ();

      _order1 = (Order) LifetimeService.GetObjectReference (WriteableSubTransaction, DomainObjectIDs.Order1);
      _relationEndPointID = RelationEndPointID.Resolve (_order1, o => o.OrderItems);
      _oppositeRelationEndPointID = RelationEndPointID.Create (DomainObjectIDs.OrderItem1, _relationEndPointID.Definition.GetOppositeEndPointDefinition ());
    }

    [Test]
    public void RelationReadInReadOnlyRootTransaction_WithoutLoading_IsAllowed ()
    {
      WriteableSubTransaction.EnsureDataAvailable (DomainObjectIDs.Order1);
      WriteableSubTransaction.EnsureDataComplete (_relationEndPointID);
      WriteableSubTransaction.EnsureDataAvailable (DomainObjectIDs.OrderItem1);
      WriteableSubTransaction.EnsureDataAvailable (DomainObjectIDs.OrderItem2);

      var orderItems = ExecuteInReadOnlyRootTransaction (() => _order1.OrderItems.ToList ());

      Assert.That (orderItems.Select (oi => oi.ID), Is.EquivalentTo (new[] { DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2 }));
    }

    [Test]
    public void RelationReadInReadOnlyMiddleTransaction_WithoutLoading_IsAllowed ()
    {
      WriteableSubTransaction.EnsureDataAvailable (DomainObjectIDs.Order1);
      WriteableSubTransaction.EnsureDataComplete (_relationEndPointID);
      WriteableSubTransaction.EnsureDataAvailable (DomainObjectIDs.OrderItem1);
      WriteableSubTransaction.EnsureDataAvailable (DomainObjectIDs.OrderItem2);

      var orderItems = ExecuteInReadOnlyMiddleTransaction (() => _order1.OrderItems.ToList ());

      Assert.That (orderItems.Select (oi => oi.ID), Is.EquivalentTo (new[] { DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2 }));
    }

    [Test]
    public void RelationReadInReadOnlyRootTransaction_WithLoading_IsAllowed ()
    {
      CheckDataNotLoaded (ReadOnlyRootTransaction, DomainObjectIDs.Order1);
      CheckDataNotLoaded (ReadOnlyMiddleTransaction, DomainObjectIDs.Order1);
      CheckDataNotLoaded (WriteableSubTransaction, DomainObjectIDs.Order1);

      CheckDataNotLoaded (ReadOnlyRootTransaction, DomainObjectIDs.OrderItem1);
      CheckDataNotLoaded (ReadOnlyMiddleTransaction, DomainObjectIDs.OrderItem1);
      CheckDataNotLoaded (WriteableSubTransaction, DomainObjectIDs.OrderItem1);

      CheckEndPointNull (ReadOnlyRootTransaction, _relationEndPointID);
      CheckEndPointNull (ReadOnlyMiddleTransaction, _relationEndPointID);
      CheckEndPointNull (WriteableSubTransaction, _relationEndPointID);

      CheckEndPointNull (ReadOnlyRootTransaction, _oppositeRelationEndPointID);
      CheckEndPointNull (ReadOnlyMiddleTransaction, _oppositeRelationEndPointID);
      CheckEndPointNull (WriteableSubTransaction, _oppositeRelationEndPointID);

      var orderItems = ExecuteInReadOnlyRootTransaction (() => _order1.OrderItems.ToList ());

      Assert.That (orderItems.Select (oi => oi.ID), Is.EquivalentTo (new[] { DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2 }));

      CheckDataLoaded (ReadOnlyRootTransaction, _order1);
      CheckDataNotLoaded (ReadOnlyMiddleTransaction, _order1);
      CheckDataNotLoaded (WriteableSubTransaction, _order1);

      CheckDataLoaded (ReadOnlyRootTransaction, orderItems[0]);
      CheckDataNotLoaded (ReadOnlyMiddleTransaction, orderItems[0]);
      CheckDataNotLoaded (WriteableSubTransaction, orderItems[0]);

      CheckEndPointComplete (ReadOnlyRootTransaction, _relationEndPointID);
      CheckEndPointNull (ReadOnlyMiddleTransaction, _relationEndPointID);
      CheckEndPointNull (WriteableSubTransaction, _relationEndPointID);

      CheckEndPointComplete (ReadOnlyRootTransaction, _oppositeRelationEndPointID);
      CheckEndPointNull (ReadOnlyMiddleTransaction, _oppositeRelationEndPointID);
      CheckEndPointNull (WriteableSubTransaction, _oppositeRelationEndPointID);
    }

    [Test]
    public void RelationReadInReadOnlyMiddleTransaction_WithLoading_IsAllowed ()
    {
      CheckDataNotLoaded (ReadOnlyRootTransaction, DomainObjectIDs.Order1);
      CheckDataNotLoaded (ReadOnlyMiddleTransaction, DomainObjectIDs.Order1);
      CheckDataNotLoaded (WriteableSubTransaction, DomainObjectIDs.Order1);

      CheckDataNotLoaded (ReadOnlyRootTransaction, DomainObjectIDs.OrderItem1);
      CheckDataNotLoaded (ReadOnlyMiddleTransaction, DomainObjectIDs.OrderItem1);
      CheckDataNotLoaded (WriteableSubTransaction, DomainObjectIDs.OrderItem1);

      CheckEndPointNull (ReadOnlyRootTransaction, _relationEndPointID);
      CheckEndPointNull (ReadOnlyMiddleTransaction, _relationEndPointID);
      CheckEndPointNull (WriteableSubTransaction, _relationEndPointID);

      CheckEndPointNull (ReadOnlyRootTransaction, _oppositeRelationEndPointID);
      CheckEndPointNull (ReadOnlyMiddleTransaction, _oppositeRelationEndPointID);
      CheckEndPointNull (WriteableSubTransaction, _oppositeRelationEndPointID);

      var orderItems = ExecuteInReadOnlyMiddleTransaction (() => _order1.OrderItems.ToList ());

      Assert.That (orderItems.Select (oi => oi.ID), Is.EquivalentTo (new[] { DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2 }));

      CheckDataLoaded (ReadOnlyRootTransaction, _order1);
      CheckDataLoaded (ReadOnlyMiddleTransaction, _order1);
      CheckDataNotLoaded (WriteableSubTransaction, _order1);

      CheckDataLoaded (ReadOnlyRootTransaction, orderItems[0]);
      CheckDataLoaded (ReadOnlyMiddleTransaction, orderItems[0]);
      CheckDataNotLoaded (WriteableSubTransaction, orderItems[0]);

      CheckEndPointComplete (ReadOnlyRootTransaction, _relationEndPointID);
      CheckEndPointComplete (ReadOnlyMiddleTransaction, _relationEndPointID);
      CheckEndPointNull (WriteableSubTransaction, _relationEndPointID);

      CheckEndPointComplete (ReadOnlyRootTransaction, _oppositeRelationEndPointID);
      CheckEndPointComplete (ReadOnlyMiddleTransaction, _oppositeRelationEndPointID);
      CheckEndPointNull (WriteableSubTransaction, _oppositeRelationEndPointID);
    }
  }
}