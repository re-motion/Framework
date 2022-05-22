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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.Data.UnitTesting.DomainObjects;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.TypePipe;

namespace Remotion.Data.DomainObjects.UnitTests.DomainImplementation
{
  [TestFixture]
  public class UnloadServiceTest : ClientTransactionBaseTest
  {
    private RelationEndPointID _domainObjectCollectionEndPointID;
    private RelationEndPointID _virtualCollectionEndPointID;
    private RelationEndPointID _virtualObjectEndPointID;

    public override void SetUp ()
    {
      base.SetUp();

      _domainObjectCollectionEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Customer1, "Orders");
      _virtualCollectionEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Product1, "Reviews");
      _virtualObjectEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Order1, "OrderTicket");
    }

    [Test]
    public void UnloadVirtualEndPoint_DomainObjectCollectionEndPoint ()
    {
      EnsureEndPointLoadedAndComplete(_domainObjectCollectionEndPointID);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID), Is.Not.Null);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).IsDataComplete, Is.True);

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, _domainObjectCollectionEndPointID);

      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID), Is.Not.Null);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).IsDataComplete, Is.False);
    }

    [Test]
    public void UnloadVirtualEndPoint_VirtualCollectionEndPoint ()
    {
      EnsureEndPointLoadedAndComplete(_virtualCollectionEndPointID);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_virtualCollectionEndPointID), Is.Not.Null);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_virtualCollectionEndPointID).IsDataComplete, Is.True);

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, _virtualCollectionEndPointID);

      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_virtualCollectionEndPointID), Is.Null);
    }

    [Test]
    public void UnloadVirtualEndPoint_VirtualObjectEndPoint ()
    {
      EnsureEndPointLoadedAndComplete(_virtualObjectEndPointID);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_virtualObjectEndPointID), Is.Not.Null);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_virtualObjectEndPointID).IsDataComplete, Is.True);

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, _virtualObjectEndPointID);

      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_virtualObjectEndPointID), Is.Not.Null);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_virtualObjectEndPointID).IsDataComplete, Is.False);
    }

    [Test]
    public void UnloadVirtualEndPoint_DomainObjectCollectionEndPoint_NotLoadedYet ()
    {
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID), Is.Null);

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, _domainObjectCollectionEndPointID);

      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID), Is.Null);
    }

    [Test]
    public void UnloadVirtualEndPoint_VirtualCollectionEndPoint_NotLoadedYet ()
    {
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_virtualCollectionEndPointID), Is.Null);

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, _virtualCollectionEndPointID);

      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_virtualCollectionEndPointID), Is.Null);
    }

    [Test]
    public void UnloadVirtualEndPoint_VirtualObjectEndPoint_NotLoadedYet ()
    {
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_virtualObjectEndPointID), Is.Null);

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, _virtualObjectEndPointID);

      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_virtualObjectEndPointID), Is.Null);
    }

    [Test]
    public void UnloadVirtualEndPoint_DomainObjectCollectionEndPoint_LazyLoaded ()
    {
      EnsureEndPointLoaded(_domainObjectCollectionEndPointID);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID), Is.Not.Null);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).IsDataComplete, Is.False);

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, _domainObjectCollectionEndPointID);

      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID), Is.Null);
    }

    [Test]
    public void UnloadVirtualEndPoint_VirtualCollectionEndPoint_LazyLoaded ()
    {
      EnsureEndPointLoaded(_virtualCollectionEndPointID);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_virtualCollectionEndPointID), Is.Not.Null);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_virtualCollectionEndPointID).IsDataComplete, Is.False);

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, _virtualCollectionEndPointID);

      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_virtualCollectionEndPointID), Is.Null);
    }

    [Test]
    public void UnloadVirtualEndPoint_VirtualObjectEndPoint_LazyLoaded ()
    {
      EnsureEndPointLoaded(_virtualObjectEndPointID);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_virtualObjectEndPointID), Is.Not.Null);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_virtualObjectEndPointID).IsDataComplete, Is.False);

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, _virtualObjectEndPointID);

      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_virtualObjectEndPointID), Is.Null);
    }

    [Test]
    public void UnloadVirtualEndPoint_DomainObjectCollectionEndPoint_NotComplete ()
    {
      EnsureEndPointLoadedAndComplete(_domainObjectCollectionEndPointID);
      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, _domainObjectCollectionEndPointID);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID), Is.Not.Null);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).IsDataComplete, Is.False);

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, _domainObjectCollectionEndPointID);

      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID), Is.Not.Null);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).IsDataComplete, Is.False);
    }

    [Test]
    public void UnloadVirtualEndPoint_VirtualCollectionEndPoint_NotComplete ()
    {
      EnsureEndPointLoadedAndComplete(_virtualCollectionEndPointID);
      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, _virtualCollectionEndPointID);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_virtualCollectionEndPointID), Is.Null);

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, _virtualCollectionEndPointID);

      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_virtualCollectionEndPointID), Is.Null);
    }

    [Test]
    public void UnloadVirtualEndPoint_VirtualObjectEndPoint_NotComplete ()
    {
      EnsureEndPointLoadedAndComplete(_virtualObjectEndPointID);
      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, _virtualObjectEndPointID);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_virtualObjectEndPointID), Is.Not.Null);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_virtualObjectEndPointID).IsDataComplete, Is.False);

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, _virtualObjectEndPointID);

      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_virtualObjectEndPointID), Is.Not.Null);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_virtualObjectEndPointID).IsDataComplete, Is.False);
    }

    [Test]
    public void UnloadVirtualEndPoint_RealObjectEndPoint ()
    {
      var objectEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Order1, "Customer");
      EnsureEndPointLoadedAndComplete(objectEndPointID);
      Assert.That(
          () => UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, objectEndPointID),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The given end point ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/"
                  + "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' does not denote a virtual end-point.", "endPointID"));
    }

    [Test]
    public void UnloadVirtualEndPoint_DomainObjectCollectionEndPoint_Changed ()
    {
      var orders = _domainObjectCollectionEndPointID.ObjectID.GetObject<Customer>().Orders;
      orders.Clear();

      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID), Is.Not.Null);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).HasChanged, Is.True);
      Assert.That(
          () => UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, _domainObjectCollectionEndPointID),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The end point with ID 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid/"
                  + "Remotion.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders' has been changed. Changed end points cannot be unloaded."));
    }

    [Test]
    public void UnloadVirtualEndPoint_VirtualCollectionEndPoint_Changed ()
    {
      var productReviews = _virtualCollectionEndPointID.ObjectID.GetObject<Product>().Reviews;
      var productReview = productReviews.First();
      productReview.Product = null;

      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_virtualCollectionEndPointID), Is.Not.Null);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_virtualCollectionEndPointID).HasChanged, Is.True);

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, _virtualCollectionEndPointID);

      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_virtualCollectionEndPointID), Is.Null);
    }

    [Test]
    public void UnloadVirtualEndPoint_VirtualObjectEndPoint_Changed ()
    {
      var order = _virtualObjectEndPointID.ObjectID.GetObject<Order>();
      order.OrderTicket = null;

      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_virtualObjectEndPointID), Is.Not.Null);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_virtualObjectEndPointID).HasChanged, Is.True);
      Assert.That(
          () => UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, _virtualObjectEndPointID),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "The end point with ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/"
                  + "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket' has been changed. Changed end points cannot be unloaded."));
    }

    [Test]
    public void UnloadVirtualEndPoint_AppliedToSubTransaction_UnloadsFromWholeHierarchy ()
    {
      var subTransaction = TestableClientTransaction.CreateSubTransaction();

      var subDataManager = ClientTransactionTestHelper.GetDataManager(subTransaction);
      var parentDataManager = TestableClientTransaction.DataManager;

      EnsureEndPointLoadedAndComplete(subDataManager,_domainObjectCollectionEndPointID);

      Assert.That(subDataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).IsDataComplete, Is.True);
      Assert.That(parentDataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).IsDataComplete, Is.True);

      UnloadService.UnloadVirtualEndPoint(subTransaction, _domainObjectCollectionEndPointID);

      Assert.That(subDataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).IsDataComplete, Is.False);
      Assert.That(parentDataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).IsDataComplete, Is.False);
    }

    [Test]
    public void UnloadVirtualEndPoint_AppliedToParentTransaction_UnloadsFromWholeHierarchy ()
    {
      var subTransaction = TestableClientTransaction.CreateSubTransaction();

      var subDataManager = ClientTransactionTestHelper.GetDataManager(subTransaction);
      var parentDataManager = TestableClientTransaction.DataManager;

      EnsureEndPointLoadedAndComplete(subDataManager, _domainObjectCollectionEndPointID);

      Assert.That(subDataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).IsDataComplete, Is.True);
      Assert.That(parentDataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).IsDataComplete, Is.True);

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, _domainObjectCollectionEndPointID);

      Assert.That(subDataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).IsDataComplete, Is.False);
      Assert.That(parentDataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).IsDataComplete, Is.False);
    }

    [Test]
    public void TryUnloadVirtualEndPoint_DomainObjectCollectionEndPoint ()
    {
      EnsureEndPointLoadedAndComplete(_domainObjectCollectionEndPointID);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID), Is.Not.Null);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).IsDataComplete, Is.True);

      var result = UnloadService.TryUnloadVirtualEndPoint(TestableClientTransaction, _domainObjectCollectionEndPointID);

      Assert.That(result, Is.True);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID), Is.Not.Null);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).IsDataComplete, Is.False);
    }

    [Test]
    public void TryUnloadVirtualEndPoint_VirtualObject ()
    {
      EnsureEndPointLoadedAndComplete(_virtualObjectEndPointID);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_virtualObjectEndPointID), Is.Not.Null);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_virtualObjectEndPointID).IsDataComplete, Is.True);

      var result = UnloadService.TryUnloadVirtualEndPoint(TestableClientTransaction, _virtualObjectEndPointID);

      Assert.That(result, Is.True);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_virtualObjectEndPointID), Is.Not.Null);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_virtualObjectEndPointID).IsDataComplete, Is.False);
    }

    [Test]
    public void TryUnloadVirtualEndPoint_NotComplete ()
    {
      EnsureEndPointLoadedAndComplete(_domainObjectCollectionEndPointID);
      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, _domainObjectCollectionEndPointID);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID), Is.Not.Null);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).IsDataComplete, Is.False);

      var result = UnloadService.TryUnloadVirtualEndPoint(TestableClientTransaction, _domainObjectCollectionEndPointID);

      Assert.That(result, Is.True);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID), Is.Not.Null);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).IsDataComplete, Is.False);
    }

    [Test]
    public void TryUnloadVirtualEndPoint_Failure ()
    {
      var orders = _domainObjectCollectionEndPointID.ObjectID.GetObject<Customer>().Orders;
      orders.Clear();

      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID), Is.Not.Null);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).HasChanged, Is.True);

      var result = UnloadService.TryUnloadVirtualEndPoint(TestableClientTransaction, _domainObjectCollectionEndPointID);

      Assert.That(result, Is.False);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID), Is.Not.Null);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).IsDataComplete, Is.True);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).HasChanged, Is.True);
    }

    [Test]
    public void TryUnloadVirtualEndPoint_AppliedToParentTransaction_UnloadsFromWholeHierarchy ()
    {
      var subTransaction = TestableClientTransaction.CreateSubTransaction();

      var subDataManager = ClientTransactionTestHelper.GetDataManager(subTransaction);
      var parentDataManager = TestableClientTransaction.DataManager;

      EnsureEndPointLoadedAndComplete(subDataManager, _domainObjectCollectionEndPointID);

      Assert.That(subDataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).IsDataComplete, Is.True);
      Assert.That(parentDataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).IsDataComplete, Is.True);

      var result = UnloadService.TryUnloadVirtualEndPoint(TestableClientTransaction, _domainObjectCollectionEndPointID);

      Assert.That(result, Is.True);
      Assert.That(subDataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).IsDataComplete, Is.False);
      Assert.That(parentDataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).IsDataComplete, Is.False);
    }

    [Test]
    public void TryUnloadVirtualEndPoint_Failure_InHigherTransaction ()
    {
      var orders = _domainObjectCollectionEndPointID.ObjectID.GetObject<Customer>().Orders;
      orders.Clear();

      var subTransaction = TestableClientTransaction.CreateSubTransaction();
      var subDataManager = ClientTransactionTestHelper.GetDataManager(subTransaction);
      var parentDataManager = TestableClientTransaction.DataManager;

      EnsureEndPointLoadedAndComplete(subDataManager, _domainObjectCollectionEndPointID);

      Assert.That(subDataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).IsDataComplete, Is.True);
      Assert.That(subDataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).HasChanged, Is.False);

      Assert.That(parentDataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).IsDataComplete, Is.True);
      Assert.That(parentDataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).HasChanged, Is.True);

      var result = UnloadService.TryUnloadVirtualEndPoint(subTransaction, _domainObjectCollectionEndPointID);

      Assert.That(result, Is.False);

      Assert.That(subDataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID), Is.Not.Null);
      Assert.That(subDataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).IsDataComplete, Is.True);
      Assert.That(subDataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).HasChanged, Is.False);

      Assert.That(parentDataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID), Is.Not.Null);
      Assert.That(parentDataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).IsDataComplete, Is.True);
      Assert.That(parentDataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).HasChanged, Is.True);
    }

    [Test]
    public void UnloadData ()
    {
      TestableClientTransaction.EnsureDataAvailable(DomainObjectIDs.Order1);
      Assert.That(TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);

      UnloadService.UnloadData(TestableClientTransaction, DomainObjectIDs.Order1);

      Assert.That(TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);
    }

    [Test]
    public void UnloadData_ObjectIsPartOfDomainObjectCollectionRelation_LazyLoaded ()
    {
      TestableClientTransaction.EnsureDataAvailable(DomainObjectIDs.OrderItem1);
      var orderItem = DomainObjectIDs.OrderItem1.GetObject<OrderItem>();
      var orderItems = orderItem.Order.OrderItems;

      Assert.That(TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.OrderItem1], Is.Not.Null);
      Assert.That(orderItems.IsDataComplete, Is.False);

      UnloadService.UnloadData(TestableClientTransaction, DomainObjectIDs.OrderItem1);

      Assert.That(TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.OrderItem1], Is.Null);
      Assert.That(orderItems.IsDataComplete, Is.False);
    }

    [Test]
    public void UnloadData_ObjectIsPartOfVirtualCollectionRelation_LazyLoaded ()
    {
      TestableClientTransaction.EnsureDataAvailable(DomainObjectIDs.ProductReview1);
      var productReview = DomainObjectIDs.ProductReview1.GetObject<ProductReview>();
      var productReviews = productReview.Product.Reviews;

      Assert.That(TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.ProductReview1], Is.Not.Null);
      Assert.That(productReviews.IsDataComplete, Is.False);

      UnloadService.UnloadData(TestableClientTransaction, DomainObjectIDs.ProductReview1);

      Assert.That(TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.ProductReview1], Is.Null);
      Assert.That(productReviews.IsDataComplete, Is.False);
    }

    [Test]
    public void UnloadData_AppliedToSubTransaction_UnloadsFromWholeHierarchy ()
    {
      var subTransaction = TestableClientTransaction.CreateSubTransaction();
      var subDataManager = ClientTransactionTestHelper.GetDataManager(subTransaction);
      var parentDataManager = TestableClientTransaction.DataManager;

      subTransaction.EnsureDataAvailable(DomainObjectIDs.Order1);
      Assert.That(subDataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);
      Assert.That(parentDataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);

      UnloadService.UnloadData(subTransaction, DomainObjectIDs.Order1);

      Assert.That(subDataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);
      Assert.That(parentDataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);
    }

    [Test]
    public void UnloadData_AppliedToParentTransaction_UnloadsFromWholeHierarchy ()
    {
      var subTransaction = TestableClientTransaction.CreateSubTransaction();
      var subDataManager = ClientTransactionTestHelper.GetDataManager(subTransaction);
      var parentDataManager = TestableClientTransaction.DataManager;

      subTransaction.EnsureDataAvailable(DomainObjectIDs.Order1);
      Assert.That(subDataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);
      Assert.That(parentDataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);

      UnloadService.UnloadData(TestableClientTransaction, DomainObjectIDs.Order1);

      Assert.That(subDataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);
      Assert.That(parentDataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);
    }

    [Test]
    public void TryUnloadData_Success ()
    {
      TestableClientTransaction.EnsureDataAvailable(DomainObjectIDs.Order1);
      Assert.That(TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);

      var result = UnloadService.TryUnloadData(TestableClientTransaction, DomainObjectIDs.Order1);

      Assert.That(result, Is.True);
      Assert.That(TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);
    }

    [Test]
    public void TryUnloadData_FailureWithChangedData ()
    {
      TestableClientTransaction.EnsureDataAvailable(DomainObjectIDs.Order1);
      Assert.That(TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);
      TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.Order1].MarkAsChanged();
      Assert.That(TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.Order1].State.IsChanged, Is.True);

      var result = UnloadService.TryUnloadData(TestableClientTransaction, DomainObjectIDs.Order1);

      Assert.That(result, Is.False);
      Assert.That(TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);
      Assert.That(TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.Order1].State.IsChanged, Is.True);
    }

    [Test]
    public void TryUnloadData_FailureWithNewObject ()
    {
      var orderNew = LifetimeService.NewObject(TestableClientTransaction, typeof(Order), ParamList.Empty);
      Assert.That(TestableClientTransaction.DataManager.DataContainers[orderNew.ID].State.IsNew, Is.True);

      var result = UnloadService.TryUnloadData(TestableClientTransaction, orderNew.ID);

      Assert.That(result, Is.False);
      Assert.That(TestableClientTransaction.DataManager.DataContainers[orderNew.ID].State.IsNew, Is.True);
    }

    [Test]
    public void TryUnloadData_AppliedToParentTransaction_UnloadsFromWholeHierarchy ()
    {
      var subTransaction = TestableClientTransaction.CreateSubTransaction();
      var subDataManager = ClientTransactionTestHelper.GetDataManager(subTransaction);
      var parentDataManager = TestableClientTransaction.DataManager;

      subTransaction.EnsureDataAvailable(DomainObjectIDs.Order1);
      Assert.That(subDataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);
      Assert.That(parentDataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);

      var result = UnloadService.TryUnloadData(TestableClientTransaction, DomainObjectIDs.Order1);

      Assert.That(result, Is.True);
      Assert.That(subDataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);
      Assert.That(parentDataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);
    }

    [Test]
    public void TryUnloadData_FailureBecauseOfChangedData_InHigherTransaction ()
    {
      TestableClientTransaction.EnsureDataAvailable(DomainObjectIDs.Order1);
      Assert.That(TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);
      TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.Order1].MarkAsChanged();

      var subTransaction = TestableClientTransaction.CreateSubTransaction();
      var subDataManager = ClientTransactionTestHelper.GetDataManager(subTransaction);
      var parentDataManager = TestableClientTransaction.DataManager;

      subTransaction.EnsureDataAvailable(DomainObjectIDs.Order1);

      Assert.That(subDataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);
      Assert.That(parentDataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);

      Assert.That(subDataManager.DataContainers[DomainObjectIDs.Order1].State.IsUnchanged, Is.True);
      Assert.That(parentDataManager.DataContainers[DomainObjectIDs.Order1].State.IsChanged, Is.True);

      var result = UnloadService.TryUnloadData(subTransaction, DomainObjectIDs.Order1);

      Assert.That(result, Is.False);
      Assert.That(subDataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);
      Assert.That(subDataManager.DataContainers[DomainObjectIDs.Order1].State.IsUnchanged, Is.True);
      Assert.That(parentDataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);
      Assert.That(parentDataManager.DataContainers[DomainObjectIDs.Order1].State.IsChanged, Is.True);
    }

    [Test]
    public void TryUnloadData_FailureBecauseOfNewObject_InHigherTransaction ()
    {
      var orderNew = LifetimeService.NewObject(TestableClientTransaction, typeof(Order), ParamList.Empty);
      Assert.That(TestableClientTransaction.DataManager.DataContainers[orderNew.ID].State.IsNew, Is.True);

      var subTransaction = TestableClientTransaction.CreateSubTransaction();
      var subDataManager = ClientTransactionTestHelper.GetDataManager(subTransaction);
      var parentDataManager = TestableClientTransaction.DataManager;

      subTransaction.EnsureDataAvailable(orderNew.ID);

      Assert.That(subDataManager.DataContainers[orderNew.ID], Is.Not.Null);
      Assert.That(parentDataManager.DataContainers[orderNew.ID], Is.Not.Null);

      Assert.That(subDataManager.DataContainers[orderNew.ID].State.IsUnchanged, Is.True);
      Assert.That(parentDataManager.DataContainers[orderNew.ID].State.IsNew, Is.True);

      var result = UnloadService.TryUnloadData(subTransaction, orderNew.ID);

      Assert.That(result, Is.False);
      Assert.That(subDataManager.DataContainers[orderNew.ID], Is.Not.Null);
      Assert.That(subDataManager.DataContainers[orderNew.ID].State.IsUnchanged, Is.True);
      Assert.That(parentDataManager.DataContainers[orderNew.ID], Is.Not.Null);
      Assert.That(parentDataManager.DataContainers[orderNew.ID].State.IsNew, Is.True);
    }

    [Test]
    public void TryUnloadData_FailureBecauseOfNewObject_InLeafTransaction ()
    {
      var subTransaction = TestableClientTransaction.CreateSubTransaction();
      var subDataManager = ClientTransactionTestHelper.GetDataManager(subTransaction);
      var parentDataManager = TestableClientTransaction.DataManager;

      var orderNew = (Order)LifetimeService.NewObject(subTransaction, typeof(Order), ParamList.Empty);
      Assert.That(subDataManager.DataContainers[orderNew.ID].State.IsNew, Is.True);


      Assert.That(subDataManager.DataContainers[orderNew.ID], Is.Not.Null);
      Assert.That(parentDataManager.DataContainers[orderNew.ID], Is.Null);

      Assert.That(subDataManager.DataContainers[orderNew.ID].State.IsNew, Is.True);
      Assert.That(parentDataManager.GetState(orderNew.ID).IsInvalid, Is.True);

      var result = UnloadService.TryUnloadData(subTransaction, orderNew.ID);

      Assert.That(result, Is.False);
      Assert.That(subDataManager.DataContainers[orderNew.ID], Is.Not.Null);
      Assert.That(subDataManager.DataContainers[orderNew.ID].State.IsNew, Is.True);
      Assert.That(parentDataManager.DataContainers[orderNew.ID], Is.Null);
      Assert.That(parentDataManager.GetState(orderNew.ID).IsInvalid, Is.True);
    }

    [Test]
    public void UnloadVirtualEndPointAndItemData_DomainObjectCollection_UnloadsEndPointAndItems ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var orderItemsEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint(order.OrderItems);
      EnsureEndPointLoadedAndComplete(orderItemsEndPoint.ID);

      var orderItem1 = (DomainObject)orderItemsEndPoint.Collection[0];
      var orderItem2 = (DomainObject)orderItemsEndPoint.Collection[1];

      UnloadService.UnloadVirtualEndPointAndItemData(TestableClientTransaction, orderItemsEndPoint.ID);

      Assert.That(orderItemsEndPoint.IsDataComplete, Is.False);
      Assert.That(orderItem1.State.IsNotLoadedYet, Is.True);
      Assert.That(orderItem2.State.IsNotLoadedYet, Is.True);
      Assert.That(order.State.IsUnchanged, Is.True);
    }

    [Test]
    public void UnloadVirtualEndPointAndItemData_DomainObjectCollection_EmptyCollection_UnloadsEndPoint ()
    {
      var customer = DomainObjectIDs.Customer2.GetObject<Customer>();
      var ordersEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint(customer.Orders);
      EnsureEndPointLoadedAndComplete(ordersEndPoint.ID);

      Assert.That(ordersEndPoint.Collection, Is.Empty);

      UnloadService.UnloadVirtualEndPointAndItemData(TestableClientTransaction, ordersEndPoint.ID);

      Assert.That(ordersEndPoint.IsDataComplete, Is.False);
      Assert.That(customer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void UnloadVirtualEndPointAndItemData_Object_UnloadsEndPointAndItem ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var endPointID = RelationEndPointID.Resolve(order, o => o.OrderTicket);
      EnsureEndPointLoadedAndComplete(endPointID);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(endPointID), Is.Not.Null);

      var orderTicket = order.OrderTicket;

      UnloadService.UnloadVirtualEndPointAndItemData(TestableClientTransaction, endPointID);

      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(endPointID), Is.Null);
      Assert.That(orderTicket.State.IsNotLoadedYet, Is.True);
      Assert.That(order.State.IsUnchanged, Is.True);
    }

    [Test]
    public void UnloadVirtualEndPointAndItemData_Object_NullEndPoint_UnloadsEndPoint ()
    {
      var employee = DomainObjectIDs.Employee1.GetObject<Employee>();
      var endPointID = RelationEndPointID.Resolve(employee, e => e.Computer);
      EnsureEndPointLoadedAndComplete(endPointID);
      Assert.That(employee.Computer, Is.Null);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(endPointID), Is.Not.Null);

      UnloadService.UnloadVirtualEndPointAndItemData(TestableClientTransaction, endPointID);

      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(endPointID), Is.Null);
      Assert.That(employee.State.IsUnchanged, Is.True);
    }

    [Test]
    public void UnloadVirtualEndPointAndItemData_RealObjectEndPoint ()
    {
      var objectEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Order1, "Customer");
      EnsureEndPointLoadedAndComplete(objectEndPointID);
      Assert.That(
          () => UnloadService.UnloadVirtualEndPointAndItemData(TestableClientTransaction, objectEndPointID),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The given end point ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/"
                  + "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' does not denote a virtual end-point.", "endPointID"));
    }

    [Test]
    public void UnloadVirtualEndPointAndItemData_AnonymousEndPoint ()
    {
      var anonymousEndPointDefinition = GetEndPointDefinition(typeof(Location), "Client").GetOppositeEndPointDefinition();
      var endPointID = RelationEndPointID.Create(DomainObjectIDs.Client1, anonymousEndPointDefinition);
      Assert.That(
          () => UnloadService.UnloadVirtualEndPointAndItemData(TestableClientTransaction, endPointID),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The given end point ID 'Client|1627ade8-125f-4819-8e33-ce567c42b00c|System.Guid/' denotes an anonymous end-point, which cannot be unloaded.",
                 "endPointID"));
    }

    [Test]
    public void UnloadVirtualEndPointAndItemData_DoesNothing_IfEndPointNotLoaded ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Customer1, "Orders");
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(endPointID), Is.Null);

      ClientTransactionTestHelperWithMocks.EnsureTransactionThrowsOnEvents(TestableClientTransaction);

      UnloadService.UnloadVirtualEndPointAndItemData(TestableClientTransaction, endPointID);

      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(endPointID), Is.Null);
    }

    [Test]
    public void UnloadVirtualEndPointAndItemData_DoesNothing_IfDataNotComplete ()
    {
      var customer = DomainObjectIDs.Customer1.GetObject<Customer>();
      var ordersEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint(customer.Orders);

      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, ordersEndPoint.ID);

      Assert.That(ordersEndPoint.IsDataComplete, Is.False);

      ClientTransactionTestHelperWithMocks.EnsureTransactionThrowsOnEvents(TestableClientTransaction);

      UnloadService.UnloadVirtualEndPointAndItemData(TestableClientTransaction, ordersEndPoint.ID);

      Assert.That(ordersEndPoint.IsDataComplete, Is.False);
    }

    [Test]
    public void UnloadVirtualEndPointAndItemData_ThrowsAndDoesNothing_IfItemCannotBeUnloaded ()
    {
      var customer = DomainObjectIDs.Customer1.GetObject<Customer>();
      var ordersEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint(customer.Orders);

      var orderA = (Order)ordersEndPoint.Collection[0];
      var orderB = (Order)ordersEndPoint.Collection[1];

      // this will cause the orderB to be rejected for unload; orderA won't be unloaded either although it comes before orderB
      ++orderB.OrderNumber;

      Assert.That(orderA.State.IsUnchanged, Is.True);
      Assert.That(orderB.State.IsChanged, Is.True);

      Assert.That(() => UnloadService.UnloadVirtualEndPointAndItemData(TestableClientTransaction, ordersEndPoint.ID), Throws.InvalidOperationException);

      Assert.That(orderA.State.IsUnchanged, Is.True);
      Assert.That(orderB.State.IsChanged, Is.True);
      Assert.That(ordersEndPoint.IsDataComplete, Is.True);
    }

    [Test]
    public void UnloadVirtualEndPointAndItemData_AppliedToSubTransaction_UnloadsFromWholeHierarchy ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var parentOrderItemsEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint(order.OrderItems);
      EnsureEndPointLoadedAndComplete(parentOrderItemsEndPoint.ID);

      var orderItem1 = parentOrderItemsEndPoint.Collection[0];

      var subTransaction = TestableClientTransaction.CreateSubTransaction();
      var subOrderItemsEndPoint = ClientTransactionTestHelper.GetDataManager(subTransaction).GetRelationEndPointWithLazyLoad(
          parentOrderItemsEndPoint.ID);
      EnsureEndPointLoadedAndComplete(ClientTransactionTestHelper.GetDataManager(subTransaction), subOrderItemsEndPoint.ID);

      UnloadService.UnloadVirtualEndPointAndItemData(subTransaction, parentOrderItemsEndPoint.ID);

      Assert.That(subOrderItemsEndPoint.IsDataComplete, Is.False);
      Assert.That(parentOrderItemsEndPoint.IsDataComplete, Is.False);

      Assert.That(orderItem1.TransactionContext[subTransaction].State.IsNotLoadedYet, Is.True);
      Assert.That(orderItem1.TransactionContext[TestableClientTransaction].State.IsNotLoadedYet, Is.True);
    }

    [Test]
    public void UnloadVirtualEndPointAndItemData_AppliedToParentTransaction_UnloadsFromWholeHierarchy ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var parentOrderItemsEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint(order.OrderItems);
      EnsureEndPointLoadedAndComplete(parentOrderItemsEndPoint.ID);

      var orderItem1 = parentOrderItemsEndPoint.Collection[0];

      var subTransaction = TestableClientTransaction.CreateSubTransaction();
      var subOrderItemsEndPoint = ClientTransactionTestHelper.GetDataManager(subTransaction).GetRelationEndPointWithLazyLoad(
          parentOrderItemsEndPoint.ID);
      EnsureEndPointLoadedAndComplete(ClientTransactionTestHelper.GetDataManager(subTransaction), subOrderItemsEndPoint.ID);

      UnloadService.UnloadVirtualEndPointAndItemData(TestableClientTransaction, parentOrderItemsEndPoint.ID);

      Assert.That(subOrderItemsEndPoint.IsDataComplete, Is.False);
      Assert.That(parentOrderItemsEndPoint.IsDataComplete, Is.False);

      Assert.That(orderItem1.TransactionContext[subTransaction].State.IsNotLoadedYet, Is.True);
      Assert.That(orderItem1.TransactionContext[TestableClientTransaction].State.IsNotLoadedYet, Is.True);
    }

    [Test]
    public void TryUnloadVirtualEndPointAndItemData_Success_DomainObjectCollection ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var orderItemsEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint(order.OrderItems);
      EnsureEndPointLoadedAndComplete(orderItemsEndPoint.ID);

      var orderItem1 = (DomainObject)orderItemsEndPoint.Collection[0];
      var orderItem2 = (DomainObject)orderItemsEndPoint.Collection[1];

      var result = UnloadService.TryUnloadVirtualEndPointAndItemData(TestableClientTransaction, orderItemsEndPoint.ID);

      Assert.That(result, Is.True);

      Assert.That(orderItemsEndPoint.IsDataComplete, Is.False);
      Assert.That(orderItem1.State.IsNotLoadedYet, Is.True);
      Assert.That(orderItem2.State.IsNotLoadedYet, Is.True);
      Assert.That(order.State.IsUnchanged, Is.True);
    }

    [Test]
    public void TryUnloadVirtualEndPointAndItemData_Success_DomainObjectCollection_Empty ()
    {
      var customer = DomainObjectIDs.Customer2.GetObject<Customer>();
      var ordersEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint(customer.Orders);
      EnsureEndPointLoadedAndComplete(ordersEndPoint.ID);

      Assert.That(ordersEndPoint.Collection, Is.Empty);

      var result = UnloadService.TryUnloadVirtualEndPointAndItemData(TestableClientTransaction, ordersEndPoint.ID);

      Assert.That(result, Is.True);
      Assert.That(ordersEndPoint.IsDataComplete, Is.False);
      Assert.That(customer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void TryUnloadVirtualEndPointAndItemData_Success_Object ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var endPointID = RelationEndPointID.Resolve(order, o => o.OrderTicket);
      EnsureEndPointLoadedAndComplete(endPointID);
      var orderTicket = order.OrderTicket;
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(endPointID), Is.Not.Null);
      Assert.That(orderTicket, Is.Not.Null);

      var result = UnloadService.TryUnloadVirtualEndPointAndItemData(TestableClientTransaction, endPointID);

      Assert.That(result, Is.True);

      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(endPointID), Is.Null);
      Assert.That(orderTicket.State.IsNotLoadedYet, Is.True);
      Assert.That(order.State.IsUnchanged, Is.True);
    }

    [Test]
    public void TryUnloadVirtualEndPointAndItemData_Success_Object_Null ()
    {
      var employee = DomainObjectIDs.Employee1.GetObject<Employee>();
      var endPointID = RelationEndPointID.Resolve(employee, e => e.Computer);
      EnsureEndPointLoadedAndComplete(endPointID);
      Assert.That(employee.Computer, Is.Null);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(endPointID), Is.Not.Null);

      var result = UnloadService.TryUnloadVirtualEndPointAndItemData(TestableClientTransaction, endPointID);

      Assert.That(result, Is.True);

      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(endPointID), Is.Null);
      Assert.That(employee.State.IsUnchanged, Is.True);
    }

    [Test]
    public void TryUnloadVirtualEndPointAndItemData_RealObjectEndPoint ()
    {
      var objectEndPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Order1, "Customer");
      EnsureEndPointLoadedAndComplete(objectEndPointID);
      Assert.That(
          () => UnloadService.TryUnloadVirtualEndPointAndItemData(TestableClientTransaction, objectEndPointID),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The given end point ID 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid/"
                  + "Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer' does not denote a virtual end-point.", "endPointID"));
    }

    [Test]
    public void TryUnloadVirtualEndPointAndItemData_Success_IfEndPointNotLoaded ()
    {
      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID(DomainObjectIDs.Customer1, "Orders");
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(endPointID), Is.Null);

      ClientTransactionTestHelperWithMocks.EnsureTransactionThrowsOnEvents(TestableClientTransaction);

      var result = UnloadService.TryUnloadVirtualEndPointAndItemData(TestableClientTransaction, endPointID);

      Assert.That(result, Is.True);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(endPointID), Is.Null);
    }

    [Test]
    public void TryUnloadVirtualEndPointAndItemData_Success_IfDataNotComplete ()
    {
      var customer = DomainObjectIDs.Customer1.GetObject<Customer>();
      var ordersEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint(customer.Orders);
      UnloadService.UnloadVirtualEndPoint(TestableClientTransaction, ordersEndPoint.ID);
      Assert.That(ordersEndPoint.IsDataComplete, Is.False);

      ClientTransactionTestHelperWithMocks.EnsureTransactionThrowsOnEvents(TestableClientTransaction);

      var result = UnloadService.TryUnloadVirtualEndPointAndItemData(TestableClientTransaction, ordersEndPoint.ID);

      Assert.That(result, Is.True);
      Assert.That(ordersEndPoint.IsDataComplete, Is.False);
    }

    [Test]
    public void TryUnloadVirtualEndPointAndItemData_Failure_EndPointChanged_EndPointAndItemsStillLoaded ()
    {
      var customer = DomainObjectIDs.Customer1.GetObject<Customer>();

      var orderA = customer.Orders[0];
      var orderB = customer.Orders[1];
      customer.Orders.Remove(orderB);

      var ordersEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint(customer.Orders);
      Assert.That(ordersEndPoint.HasChanged, Is.True);
      Assert.That(orderA.State.IsUnchanged, Is.True);

      var result = UnloadService.TryUnloadVirtualEndPointAndItemData(TestableClientTransaction, ordersEndPoint.ID);

      Assert.That(result, Is.False);
      Assert.That(orderA.State.IsUnchanged, Is.True);
      Assert.That(ordersEndPoint.IsDataComplete, Is.True);
    }

    [Test]
    public void TryUnloadVirtualEndPointAndItemData_Failure_ItemCannotBeUnloaded_EndPointAndItemStillLoaded ()
    {
      var customer = DomainObjectIDs.Customer1.GetObject<Customer>();
      var ordersEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint(customer.Orders);

      var orderA = (Order)ordersEndPoint.Collection[0];
      var orderB = (Order)ordersEndPoint.Collection[1];

      // this will cause the orderB to be rejected for unload; orderA won't be unloaded either although it comes before orderB
      ++orderB.OrderNumber;

      Assert.That(orderA.State.IsUnchanged, Is.True);
      Assert.That(orderB.State.IsChanged, Is.True);

      var result = UnloadService.TryUnloadVirtualEndPointAndItemData(TestableClientTransaction, ordersEndPoint.ID);

      Assert.That(result, Is.False);
      Assert.That(orderA.State.IsUnchanged, Is.True);
      Assert.That(orderB.State.IsChanged, Is.True);
      Assert.That(ordersEndPoint.IsDataComplete, Is.True);
    }

    [Test]
    public void TryUnloadVirtualEndPointAndItemData_AppliedToParentTransaction_UnloadsFromWholeHierarchy ()
    {
      var order = DomainObjectIDs.Order1.GetObject<Order>();
      var parentOrderItemsEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint(order.OrderItems);
      EnsureEndPointLoadedAndComplete(parentOrderItemsEndPoint.ID);

      var orderItem1 = parentOrderItemsEndPoint.Collection[0];

      var subTransaction = TestableClientTransaction.CreateSubTransaction();
      var subOrderItemsEndPoint = ClientTransactionTestHelper.GetDataManager(subTransaction).GetRelationEndPointWithLazyLoad(
          parentOrderItemsEndPoint.ID);
      EnsureEndPointLoadedAndComplete(ClientTransactionTestHelper.GetDataManager(subTransaction), subOrderItemsEndPoint.ID);

      var result = UnloadService.TryUnloadVirtualEndPointAndItemData(TestableClientTransaction, parentOrderItemsEndPoint.ID);

      Assert.That(result, Is.True);
      Assert.That(subOrderItemsEndPoint.IsDataComplete, Is.False);
      Assert.That(parentOrderItemsEndPoint.IsDataComplete, Is.False);

      Assert.That(orderItem1.TransactionContext[subTransaction].State.IsNotLoadedYet, Is.True);
      Assert.That(orderItem1.TransactionContext[TestableClientTransaction].State.IsNotLoadedYet, Is.True);
    }

    [Test]
    public void TryUnloadVirtualEndPointAndItemData_Failure_InHigherTransaction ()
    {
      var customer = DomainObjectIDs.Customer1.GetObject<Customer>();
      var parentOrdersEndPoint = DomainObjectCollectionDataTestHelper.GetAssociatedEndPoint(customer.Orders);
      EnsureEndPointLoadedAndComplete(parentOrdersEndPoint.ID);

      customer.Orders[0].RegisterForCommit();

      Assert.That(((DomainObject)parentOrdersEndPoint.Collection[0]).State.IsChanged, Is.True);

      var subTransaction = TestableClientTransaction.CreateSubTransaction();
      var subTransactionDataManager = ClientTransactionTestHelper.GetDataManager(subTransaction);
      var subOrdersEndPoint = (IDomainObjectCollectionEndPoint)subTransactionDataManager.GetRelationEndPointWithLazyLoad(parentOrdersEndPoint.ID);
      EnsureEndPointLoadedAndComplete(subTransactionDataManager, subOrdersEndPoint.ID);

      Assert.That(subOrdersEndPoint.Collection[0].TransactionContext[subTransaction].State.IsUnchanged, Is.True);

      var result = UnloadService.TryUnloadVirtualEndPointAndItemData(subTransaction, parentOrdersEndPoint.ID);

      Assert.That(result, Is.False);
      Assert.That(subOrdersEndPoint.IsDataComplete, Is.True);
      Assert.That(parentOrdersEndPoint.IsDataComplete, Is.True);
    }

    [Test]
    public void UnloadAll ()
    {
      TestableClientTransaction.EnsureDataAvailable(DomainObjectIDs.Order1);
      EnsureEndPointLoadedAndComplete(_domainObjectCollectionEndPointID);
      Assert.That(TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID), Is.Not.Null);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID).IsDataComplete, Is.True);

      UnloadService.UnloadAll(TestableClientTransaction);

      Assert.That(TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID), Is.Null);
    }

    [Test]
    public void UnloadAll_Transactions ()
    {
      TestableClientTransaction.EnsureDataAvailable(DomainObjectIDs.Order1);
      EnsureEndPointLoadedAndComplete(_domainObjectCollectionEndPointID);
      Assert.That(TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.Order1], Is.Not.Null);
      Assert.That(TestableClientTransaction.DataManager.GetRelationEndPointWithoutLoading(_domainObjectCollectionEndPointID), Is.Not.Null);

      using (TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope())
      {
        var middleTransaction = ClientTransaction.Current;
        middleTransaction.EnsureDataAvailable(DomainObjectIDs.Order3);
        Assert.That(DataManagementService.GetDataManager(middleTransaction).DataContainers[DomainObjectIDs.Order3], Is.Not.Null);

        using (middleTransaction.CreateSubTransaction().EnterDiscardingScope())
        {
          ClientTransaction.Current.EnsureDataAvailable(DomainObjectIDs.Order4);
          Assert.That(DataManagementService.GetDataManager(ClientTransaction.Current).DataContainers[DomainObjectIDs.Order4], Is.Not.Null);

          UnloadService.UnloadAll(middleTransaction);

          Assert.That(DataManagementService.GetDataManager(ClientTransaction.Current).DataContainers[DomainObjectIDs.Order4], Is.Null);
          Assert.That(DataManagementService.GetDataManager(middleTransaction).DataContainers[DomainObjectIDs.Order3], Is.Null);
          Assert.That(TestableClientTransaction.DataManager.DataContainers[DomainObjectIDs.Order1], Is.Null);
        }
      }
    }

    private void EnsureEndPointLoadedAndComplete (RelationEndPointID endPointID)
    {
      var dataManager = TestableClientTransaction.DataManager;
      EnsureEndPointLoadedAndComplete(dataManager, endPointID);
    }

    private void EnsureEndPointLoadedAndComplete (IDataManager dataManager, RelationEndPointID endPointID)
    {
      var endPoint = dataManager.GetRelationEndPointWithLazyLoad(endPointID);
      endPoint.EnsureDataComplete();
    }

    private void EnsureEndPointLoaded (RelationEndPointID endPointID)
    {
      var dataManager = TestableClientTransaction.DataManager;
      EnsureEndPointLoaded(dataManager, endPointID);
    }

    private void EnsureEndPointLoaded (IDataManager dataManager, RelationEndPointID endPointID)
    {
      var endPoint = dataManager.GetRelationEndPointWithLazyLoad(endPointID);
      Assert.That(endPoint.IsDataComplete, Is.False);
    }
  }
}
