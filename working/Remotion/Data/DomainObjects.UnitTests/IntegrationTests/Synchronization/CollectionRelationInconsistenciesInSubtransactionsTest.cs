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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Synchronization
{
  [TestFixture]
  public class CollectionRelationInconsistenciesInSubtransactionsTest : RelationInconsistenciesTestBase
  {
    [Test]
    public void VirtualEndPointQuery_OneMany_Consistent_ObjectLoadedFirst ()
    {
      OrderItem orderItem1;
      Order order1;

      // Sub-transaction loading OrderItem.Order before Order.OrderItems
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        orderItem1 = DomainObjectIDs.OrderItem1.GetObject<OrderItem>();
        order1 = DomainObjectIDs.Order1.GetObject<Order> ();
        order1.OrderItems.EnsureDataComplete();

        Assert.That (orderItem1.Order, Is.SameAs (order1));
        Assert.That (order1.OrderItems, Has.Member(orderItem1));

        CheckSyncState (orderItem1, oi => oi.Order, true);
        CheckSyncState (orderItem1.Order, o => o.OrderItems, true);

        Assert.That (orderItem1.Order, Is.SameAs (order1));
        Assert.That (order1.OrderItems, Has.Member(orderItem1));

        CheckSyncState (orderItem1, oi => oi.Order, true);
        CheckSyncState (orderItem1.Order, o => o.OrderItems, true);

        // these do nothing
        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Resolve (orderItem1, oi => oi.Order));
        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Resolve (orderItem1.Order, o => o.OrderItems));

        CheckSyncState (orderItem1, oi => oi.Order, true);
        CheckSyncState (orderItem1.Order, o => o.OrderItems, true);
      }

      CheckSyncState (orderItem1, oi => oi.Order, true);
      CheckSyncState (orderItem1.Order, o => o.OrderItems, true);
    }

    [Test]
    public void VirtualEndPointQuery_OneMany_Consistent_CollectionLoadedFirst ()
    {
      Order order1;
      OrderItem orderItem1;

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        order1 = DomainObjectIDs.Order1.GetObject<Order> ();
        order1.OrderItems.EnsureDataComplete();
        orderItem1 = DomainObjectIDs.OrderItem1.GetObject<OrderItem>();

        Assert.That (orderItem1.Order, Is.SameAs (order1));
        Assert.That (order1.OrderItems, Has.Member(orderItem1));

        CheckSyncState (orderItem1, oi => oi.Order, true);
        CheckSyncState (orderItem1.Order, o => o.OrderItems, true);

        Assert.That (order1.OrderItems, Has.Member(orderItem1));
        Assert.That (orderItem1.Order, Is.SameAs (order1));

        CheckSyncState (orderItem1, oi => oi.Order, true);
        CheckSyncState (orderItem1.Order, o => o.OrderItems, true);

        // these do nothing
        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Resolve (orderItem1, oi => oi.Order));
        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Resolve (orderItem1.Order, o => o.OrderItems));

        CheckSyncState (orderItem1, oi => oi.Order, true);
        CheckSyncState (orderItem1.Order, o => o.OrderItems, true);
      }

      CheckSyncState (orderItem1, oi => oi.Order, true);
      CheckSyncState (orderItem1.Order, o => o.OrderItems, true);
    }

    [Test]
    public void VirtualEndPointQuery_OneMany_ObjectIncluded_ThatLocallyPointsToSomewhereElse ()
    {
      Company company;
      IndustrialSector industrialSector; // virtual end point not yet resolved

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        PrepareInconsistentState_OneMany_ObjectIncluded (out company, out industrialSector);

        CheckSyncState (company, c => c.IndustrialSector, true);
        CheckSyncState (industrialSector, s => s.Companies, false);

        var otherCompany = industrialSector.Companies.FirstOrDefault (c => c != company);
        CheckSyncState (otherCompany, c => c.IndustrialSector, true);

        CheckActionWorks (company.Delete);
        ClientTransaction.Current.Rollback(); // required so that the remaining actions can be tried below

        // sync states not changed by Rollback
        CheckSyncState (company, c => c.IndustrialSector, true);
        CheckSyncState (industrialSector, s => s.Companies, false);

        CheckActionWorks (() => industrialSector.Companies.Remove (otherCompany));
        CheckActionWorks (() => industrialSector.Companies.Add (Company.NewObject()));

        var companyIndex = industrialSector.Companies.IndexOf (company);
        CheckActionThrows<InvalidOperationException> (
            () => industrialSector.Companies.Remove (company), "out of sync with the opposite object property");
        CheckActionThrows<InvalidOperationException> (
            () => industrialSector.Companies[companyIndex] = Company.NewObject(), "out of sync with the opposite object property");
        CheckActionThrows<InvalidOperationException> (
            () => industrialSector.Companies = new ObjectList<Company>(), "out of sync with the opposite object property");
        CheckActionThrows<InvalidOperationException> (industrialSector.Delete, "out of sync with the opposite object property");

        CheckActionWorks (() => company.IndustrialSector = IndustrialSector.NewObject());

        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Resolve (industrialSector, s => s.Companies));

        CheckSyncState (industrialSector, s => s.Companies, true);
        Assert.That (industrialSector.Companies, Has.No.Member(company));

        CheckActionWorks (() => industrialSector.Companies.Add (company));
      }

      CheckSyncState (company, c => c.IndustrialSector, true);
      CheckSyncState (industrialSector, s => s.Companies, true);

      Assert.That (company.IndustrialSector, Is.Null);
      Assert.That (industrialSector.Companies, Has.No.Member(company));
    }

    [Test]
    public void VirtualEndPointQuery_OneMany_ObjectIncluded_ThatLocallyPointsToSomewhereElse_SolvableViaReload ()
    {
      Company company;
      IndustrialSector industrialSector; // virtual end point not yet resolved

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        PrepareInconsistentState_OneMany_ObjectIncluded (out company, out industrialSector);

        Assert.That (company.IndustrialSector, Is.Null);
        Assert.That (industrialSector.Companies, Has.Member(company));
        CheckSyncState (industrialSector, s => s.Companies, false);

        UnloadService.UnloadData (ClientTransaction.Current, company.ID);
        company.EnsureDataAvailable();

        CheckSyncState (industrialSector, s => s.Companies, true);
        Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
        Assert.That (industrialSector.Companies, Has.Member(company));

        CheckActionWorks (() => industrialSector.Companies.Remove (company));
      }

      CheckSyncState (industrialSector, s => s.Companies, true);
      Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
      Assert.That (industrialSector.Companies, Has.Member(company));
    }

    [Test]
    public void VirtualEndPointQuery_OneMany_ObjectNotIncluded_ThatLocallyPointsToHere ()
    {
      Company company;
      IndustrialSector industrialSector;

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        PrepareInconsistentState_OneMany_ObjectNotIncluded (out company, out industrialSector);

        Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
        Assert.That (industrialSector.Companies, Has.No.Member(company));

        CheckSyncState (company, c => c.IndustrialSector, false);
        CheckSyncState (industrialSector, s => s.Companies, true);
        CheckSyncState (industrialSector.Companies[0], c => c.IndustrialSector, true);

        CheckActionThrows<InvalidOperationException> (company.Delete, "out of sync with the opposite property");
        CheckActionThrows<InvalidOperationException> (industrialSector.Delete, "out of sync with the collection property");

        CheckActionWorks (() => industrialSector.Companies.RemoveAt (0));
        CheckActionWorks (() => industrialSector.Companies.Add (Company.NewObject()));

        CheckActionThrows<InvalidOperationException> (() => industrialSector.Companies.Add (company), "out of sync with the collection property");
        CheckActionThrows<InvalidOperationException> (() => company.IndustrialSector = null, "out of sync with the opposite property ");

        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Resolve (company, c => c.IndustrialSector));

        CheckSyncState (company, c => c.IndustrialSector, true);
        Assert.That (industrialSector.Companies, Has.Member(company));
        CheckActionWorks (() => company.IndustrialSector = null);
        CheckActionWorks (() => industrialSector.Companies.Add (company));
      }

      CheckSyncState (company, c => c.IndustrialSector, true);
      CheckSyncState (industrialSector, s => s.Companies, true);

      Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
      Assert.That (industrialSector.Companies, Has.Member(company));
    }

    [Test]
    public void VirtualEndPointQuery_OneMany_ObjectIncludedInTwoCollections ()
    {
      SetDatabaseModifyable();

      Company company;
      IndustrialSector industrialSector1;
      IndustrialSector industrialSector2;
      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        var companyID = CreateCompanyAndSetIndustrialSectorInOtherTransaction (DomainObjectIDs.IndustrialSector1);
        company = companyID.GetObject<Company> ();

        industrialSector1 = DomainObjectIDs.IndustrialSector1.GetObject<IndustrialSector> ();
        industrialSector1.Companies.EnsureDataComplete();

        SetIndustrialSectorInOtherTransaction (company.ID, DomainObjectIDs.IndustrialSector2);

        industrialSector2 = DomainObjectIDs.IndustrialSector2.GetObject<IndustrialSector> ();
        industrialSector2.Companies.EnsureDataComplete();

        Assert.That (company.IndustrialSector, Is.SameAs (industrialSector1));
        Assert.That (industrialSector1.Companies, Has.Member(company));
        Assert.That (industrialSector2.Companies, Has.Member (company));

        CheckSyncState (company, c => c.IndustrialSector, true);
        CheckSyncState (industrialSector1, s => s.Companies, true);
        CheckSyncState (industrialSector2, s => s.Companies, false);

        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, RelationEndPointID.Resolve (industrialSector2, s => s.Companies));

        Assert.That (company.IndustrialSector, Is.SameAs (industrialSector1));
        Assert.That (industrialSector1.Companies, Has.Member (company));
        Assert.That (industrialSector2.Companies, Has.No.Member(company));

        CheckSyncState (company, c => c.IndustrialSector, true);
        CheckSyncState (industrialSector1, s => s.Companies, true);
        CheckSyncState (industrialSector2, s => s.Companies, true);
      }

      CheckSyncState (company, c => c.IndustrialSector, true);
      CheckSyncState (industrialSector1, s => s.Companies, true);
      CheckSyncState (industrialSector2, s => s.Companies, true);

      Assert.That (company.IndustrialSector, Is.SameAs (industrialSector1));
      Assert.That (industrialSector1.Companies, Has.Member (company));
      Assert.That (industrialSector2.Companies, Has.No.Member(company));
    }

    [Test]
    public void ConsistentState_GuaranteedInSubTransaction_OneMany ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order> ();
      order.OrderItems.EnsureDataComplete();

      var orderItem = DomainObjectIDs.OrderItem1.GetObject<OrderItem>();

      Assert.That (order.OrderItems, Has.Member (orderItem));
      Assert.That (orderItem.Order, Is.SameAs (order));
      CheckSyncState (order, o => o.OrderItems, true);
      CheckSyncState (orderItem, oi => oi.Order, true);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That (order.OrderItems, Has.Member (orderItem));
        Assert.That (orderItem.Order, Is.SameAs (order));
        CheckSyncState (order, o => o.OrderItems, true);
        CheckSyncState (orderItem, oi => oi.Order, true);
      }

      order.OrderItems.Remove (orderItem);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That (order.OrderItems, Has.No.Member(orderItem));
        Assert.That (orderItem.Order, Is.Null);
        CheckSyncState (order, o => o.OrderItems, true);
        CheckSyncState (orderItem, oi => oi.Order, true);
      }
      ClientTransaction.Current.Rollback();

      orderItem.Order = null;

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That (order.OrderItems, Has.No.Member(orderItem));
        Assert.That (orderItem.Order, Is.Null);
        CheckSyncState (order, o => o.OrderItems, true);
        CheckSyncState (orderItem, oi => oi.Order, true);
      }
    }

    [Test]
    public void InconsistentState_GuaranteedInSubTransaction_OneMany_ObjectIncluded ()
    {
      Company company;
      IndustrialSector industrialSector;
      PrepareInconsistentState_OneMany_ObjectIncluded (out company, out industrialSector);

      Assert.That (company.IndustrialSector, Is.Null);
      Assert.That (industrialSector.Companies, Has.Member (company));

      CheckSyncState (company, c => c.IndustrialSector, true);
      CheckSyncState (industrialSector, s => s.Companies, false);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That (company.IndustrialSector, Is.Null);
        Assert.That (industrialSector.Companies, Has.Member (company));
        CheckSyncState (company, c => c.IndustrialSector, true);
        CheckSyncState (industrialSector, s => s.Companies, false);
      }

      CheckActionThrows<InvalidOperationException> (() => industrialSector.Companies.Remove (company), "out of sync");

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That (company.IndustrialSector, Is.Null);
        Assert.That (industrialSector.Companies, Has.Member (company));
        CheckSyncState (company, c => c.IndustrialSector, true);
        CheckSyncState (industrialSector, s => s.Companies, false);
      }

      CheckActionThrows<InvalidOperationException> (() => company.IndustrialSector = industrialSector, "out of sync");

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.That (company.IndustrialSector, Is.Null);
        Assert.That (industrialSector.Companies, Has.Member (company));
        CheckSyncState (company, c => c.IndustrialSector, true);
        CheckSyncState (industrialSector, s => s.Companies, false);
      }
    }

    [Test]
    public void InconsistentState_GuaranteedInSubTransaction_OneMany_ObjectNotIncluded ()
    {
      Company company;
      IndustrialSector industrialSector;
      PrepareInconsistentState_OneMany_ObjectNotIncluded (out company, out industrialSector);

      Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
      Assert.That (industrialSector.Companies, Has.No.Member(company));
      CheckSyncState (company, c => c.IndustrialSector, false);
      CheckSyncState (industrialSector, s => s.Companies, true);

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
        Assert.That (industrialSector.Companies, Has.No.Member(company));
        CheckSyncState (company, c => c.IndustrialSector, false);
        CheckSyncState (industrialSector, s => s.Companies, true);
      }

      CheckActionThrows<InvalidOperationException> (() => industrialSector.Companies.Add (company), "out of sync");

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
        Assert.That (industrialSector.Companies, Has.No.Member(company));
        CheckSyncState (company, c => c.IndustrialSector, false);
        CheckSyncState (industrialSector, s => s.Companies, true);
      }

      CheckActionThrows<InvalidOperationException> (() => company.IndustrialSector = null, "out of sync");

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
        Assert.That (industrialSector.Companies, Has.No.Member(company));
        CheckSyncState (company, c => c.IndustrialSector, false);
        CheckSyncState (industrialSector, s => s.Companies, true);
      }
    }

    [Test]
    public void Commit_InSubTransaction_InconsistentState_OneMany_ObjectIncluded ()
    {
      Company company;
      IndustrialSector industrialSector;
      PrepareInconsistentState_OneMany_ObjectIncluded (out company, out industrialSector);

      Assert.That (company.IndustrialSector, Is.Null);
      Assert.That (industrialSector.Companies, Has.Member (company));

      CheckSyncState (company, c => c.IndustrialSector, true);
      CheckSyncState (industrialSector, s => s.Companies, false);

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (company.IndustrialSector, Is.Null);
        Assert.That (industrialSector.Companies.Count, Is.EqualTo (6));

        CheckActionThrows<InvalidOperationException> (() => company.IndustrialSector = industrialSector, "out of sync");
        company.IndustrialSector = DomainObjectIDs.IndustrialSector2.GetObject<IndustrialSector> ();
        industrialSector.Companies.Insert (0, DomainObjectIDs.Company2.GetObject<Company> ());
        CheckActionThrows<InvalidOperationException> (() => industrialSector.Companies.Remove (company), "out of sync");

        Assert.That (company.IndustrialSector, Is.SameAs (DomainObjectIDs.IndustrialSector2.GetObject<IndustrialSector> ()));
        Assert.That (industrialSector.Companies.Count, Is.EqualTo (7));

        ClientTransaction.Current.Commit();

        CheckSyncState (company, c => c.IndustrialSector, true);
        CheckSyncState (industrialSector, s => s.Companies, false);

        Assert.That (company.IndustrialSector, Is.SameAs (DomainObjectIDs.IndustrialSector2.GetObject<IndustrialSector> ()));
        Assert.That (industrialSector.Companies.Count, Is.EqualTo (7));
      }

      Assert.That (company.IndustrialSector, Is.SameAs (DomainObjectIDs.IndustrialSector2.GetObject<IndustrialSector> ()));
      Assert.That (industrialSector.Companies.Count, Is.EqualTo (7));

      CheckSyncState (company, c => c.IndustrialSector, true);
      CheckSyncState (industrialSector, s => s.Companies, false);
    }

    [Test]
    public void Commit_InSubTransaction_InconsistentState_OneMany_ObjectNotIncluded ()
    {
      Company company;
      IndustrialSector industrialSector;
      PrepareInconsistentState_OneMany_ObjectNotIncluded(out company, out industrialSector);

      Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
      Assert.That (industrialSector.Companies, Has.No.Member(company));

      CheckSyncState (company, c => c.IndustrialSector, false);
      CheckSyncState (industrialSector, s => s.Companies, true);

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
        Assert.That (industrialSector.Companies.Count, Is.EqualTo (5));

        CheckActionThrows<InvalidOperationException> (() => company.IndustrialSector = null, "out of sync");
        industrialSector.Companies.Add (DomainObjectIDs.Company1.GetObject<Company> ());
        CheckActionThrows<InvalidOperationException> (() => industrialSector.Companies.Add (company), "out of sync");

        Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
        Assert.That (industrialSector.Companies.Count, Is.EqualTo (6));

        ClientTransaction.Current.Commit();

        Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
        Assert.That (industrialSector.Companies.Count, Is.EqualTo (6));

        CheckSyncState (company, c => c.IndustrialSector, false);
        CheckSyncState (industrialSector, s => s.Companies, true);
      }

      Assert.That (company.IndustrialSector, Is.SameAs (industrialSector));
      Assert.That (industrialSector.Companies.Count, Is.EqualTo (6));

      CheckSyncState (company, c => c.IndustrialSector, false);
      CheckSyncState (industrialSector, s => s.Companies, true);
    }

    [Test]
    public void Synchronize_WithSubtransactions_LoadedInRootOnly_SyncedInRoot ()
    {
      Company company;
      IndustrialSector industrialSector;
      PrepareInconsistentState_OneMany_ObjectIncluded (out company, out industrialSector);

      CheckSyncState (company, c => c.IndustrialSector, true);
      CheckSyncState (industrialSector, s => s.Companies, false);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        var relationEndPointID = RelationEndPointID.Resolve (industrialSector, s => s.Companies);
        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current.ParentTransaction, relationEndPointID);

        CheckSyncState (industrialSector, s => s.Companies, true);

        var dataManager = ClientTransactionTestHelper.GetDataManager (ClientTransaction.Current);
        Assert.That (dataManager.GetRelationEndPointWithoutLoading (relationEndPointID), Is.Null);

        var endPoint = dataManager.GetRelationEndPointWithLazyLoad (relationEndPointID);
        Assert.That (endPoint.IsSynchronized, Is.Null);
        endPoint.EnsureDataComplete();
        Assert.That (endPoint.IsSynchronized, Is.True);
        CheckSyncState (industrialSector, s => s.Companies, true);
      }

      CheckSyncState (industrialSector, s => s.Companies, true);
    }

    [Test]
    public void Synchronize_WithSubtransactions_LoadedInRootOnly_SyncedInSub ()
    {
      Company company;
      IndustrialSector industrialSector;
      PrepareInconsistentState_OneMany_ObjectIncluded (out company, out industrialSector);

      CheckSyncState (company, c => c.IndustrialSector, true);
      CheckSyncState (industrialSector, s => s.Companies, false);

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        var relationEndPointID = RelationEndPointID.Resolve (industrialSector, s => s.Companies);
        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, relationEndPointID);

        CheckSyncState (company, c => c.IndustrialSector, true);

        var dataManager = ClientTransactionTestHelper.GetDataManager (ClientTransaction.Current);
        Assert.That (dataManager.GetRelationEndPointWithoutLoading (relationEndPointID), Is.Null);

        var endPoint = dataManager.GetRelationEndPointWithLazyLoad (relationEndPointID);
        Assert.That (endPoint.IsSynchronized, Is.Null);
        endPoint.EnsureDataComplete();
        Assert.That (endPoint.IsSynchronized, Is.True);
        CheckSyncState (company, c => c.IndustrialSector, true);
      }

      CheckSyncState (company, c => c.IndustrialSector, true);
    }

    [Test]
    public void Synchronize_WithSubtransactions_LoadedInRootAndSub_SyncedInRoot ()
    {
      Company company;
      IndustrialSector industrialSector;
      PrepareInconsistentState_OneMany_ObjectIncluded (out company, out industrialSector);

      CheckSyncState (company, c => c.IndustrialSector, true);
      CheckSyncState (industrialSector, s => s.Companies, false);

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        var relationEndPointID = RelationEndPointID.Resolve (industrialSector, s => s.Companies);

        var dataManager = ClientTransactionTestHelper.GetDataManager (ClientTransaction.Current);
        var endPoint = dataManager.GetRelationEndPointWithLazyLoad (relationEndPointID);
        Assert.That (endPoint.IsSynchronized, Is.Null);

        endPoint.EnsureDataComplete ();
        Assert.That (endPoint.IsSynchronized, Is.False);

        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current.ParentTransaction, relationEndPointID);

        CheckSyncState (industrialSector, s => s.Companies, true);
        Assert.That (endPoint.IsSynchronized, Is.True);
      }

      CheckSyncState (industrialSector, s => s.Companies, true);
    }

    [Test]
    public void Synchronize_WithSubtransactions_LoadedInRootAndSub_SyncedInSub ()
    {
      Company company;
      IndustrialSector industrialSector;
      PrepareInconsistentState_OneMany_ObjectIncluded (out company, out industrialSector);

      CheckSyncState (company, c => c.IndustrialSector, true);
      CheckSyncState (industrialSector, s => s.Companies, false);

      using (ClientTransaction.Current.CreateSubTransaction ().EnterDiscardingScope ())
      {
        var relationEndPointID = RelationEndPointID.Resolve (industrialSector, s => s.Companies);

        var dataManager = ClientTransactionTestHelper.GetDataManager (ClientTransaction.Current);
        var endPoint = dataManager.GetRelationEndPointWithLazyLoad (relationEndPointID);
        Assert.That (endPoint.IsSynchronized, Is.Null);

        endPoint.EnsureDataComplete();
        Assert.That (endPoint.IsSynchronized, Is.False);

        BidirectionalRelationSyncService.Synchronize (ClientTransaction.Current, relationEndPointID);

        CheckSyncState (industrialSector, s => s.Companies, true);
        Assert.That (endPoint.IsSynchronized, Is.True);
      }

      CheckSyncState (industrialSector, s => s.Companies, true);
    }
  }
}