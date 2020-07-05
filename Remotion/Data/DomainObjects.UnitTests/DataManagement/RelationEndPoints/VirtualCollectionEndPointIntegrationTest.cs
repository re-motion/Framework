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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class VirtualCollectionEndPointIntegrationTest : ClientTransactionBaseTest
  {
    private ProductReview _productReview1; // belongs to customer1
    private ProductReview _productReview2; // belongs to customer1
    private ProductReview _productReview3; // belongs to customer3
    private Product _product1;

    private VirtualCollectionEndPoint _productEndPoint;

    public override void SetUp ()
    {
      base.SetUp();

      _productReview1 = DomainObjectIDs.ProductReview1.GetObject<ProductReview>();
      _productReview2 = DomainObjectIDs.ProductReview2.GetObject<ProductReview>();
      _productReview3 = DomainObjectIDs.ProductReview3.GetObject<ProductReview>();
      _product1 = DomainObjectIDs.Product1.GetObject<Product>();

      var stateUpdateRaisingEndPointDecorator = (StateUpdateRaisingVirtualCollectionEndPointDecorator)
          TestableClientTransaction.DataManager.GetRelationEndPointWithLazyLoad (
              RelationEndPointID.Create (
                  DomainObjectIDs.Product1,
                  typeof (Product),
                  "Reviews"));
      _productEndPoint = (VirtualCollectionEndPoint) stateUpdateRaisingEndPointDecorator.InnerEndPoint;
    }

    [Test]
    public void Add_WithCompleteCollectionEndPoint_InvalidatesCollectionState ()
    {
      var newProductReview = ProductReview.NewObject();
      newProductReview.CreatedAt = DateTime.Now;
      Assert.That (
          _productEndPoint.Collection,
          Is.EqualTo (new[] { _productReview1, _productReview2, _productReview3 }));
      Assert.That (_productEndPoint.IsDataComplete, Is.True);
      Assert.That (_productEndPoint.HasBeenTouched, Is.False);

      newProductReview.Product = _product1;

      Assert.That (_productEndPoint.IsDataComplete, Is.True);
      Assert.That (_productEndPoint.HasBeenTouched, Is.True);
      Assert.That (
          _productEndPoint.Collection,
          Is.EqualTo (new[] { _productReview1, _productReview2, _productReview3, newProductReview }),
          "changes go down to actual data store");
      Assert.That (newProductReview.Product, Is.SameAs (_productEndPoint.GetDomainObject()), "bidirectional modification");
    }

    [Test]
    public void Add_WithLazyLoadedCollectionEndPoint ()
    {
      var newProductReview = ProductReview.NewObject();
      newProductReview.CreatedAt = DateTime.Now;
      Assert.That (_productEndPoint.IsDataComplete, Is.False);
      Assert.That (_productEndPoint.HasBeenTouched, Is.False);

      newProductReview.Product = _product1;

      Assert.That (_productEndPoint.IsDataComplete, Is.True);
      Assert.That (_productEndPoint.HasBeenTouched, Is.True);
      Assert.That (
          _productEndPoint.Collection,
          Is.EqualTo (new[] { _productReview1, _productReview2, _productReview3, newProductReview }),
          "changes go down to actual data store");
      Assert.That (newProductReview.Product, Is.SameAs (_productEndPoint.GetDomainObject()), "bidirectional modification");
      Assert.Ignore ("RM-7294: IsDataComplete after update should be false."); //TODO: RM-7294
    }

    [Test]
    public void Remove_WithCompleteCollectionEndPoint_InvalidatesCollectionState ()
    {
      _productEndPoint.EnsureDataComplete();
      Assert.That (
          _productEndPoint.Collection,
          Is.EqualTo (new[] { _productReview1, _productReview2, _productReview3 }));
      Assert.That (_productEndPoint.IsDataComplete, Is.True);
      Assert.That (_productEndPoint.HasBeenTouched, Is.False);

      _productReview1.Product = null;

      Assert.That (_productEndPoint.IsDataComplete, Is.True);
      Assert.That (_productEndPoint.HasBeenTouched, Is.True);
      Assert.That (_productEndPoint.Collection, Is.EqualTo (new[] { _productReview2, _productReview3 }), "changes go down to actual data store");
      Assert.That (_productReview1.Product, Is.Null, "bidirectional modification");
    }

    [Test]
    public void Remove_WithLazyLoadedCollectionEndPoint ()
    {
      Assert.That (_productEndPoint.IsDataComplete, Is.False);
      Assert.That (_productEndPoint.HasBeenTouched, Is.False);

      _productReview1.Product = null;

      Assert.That (_productEndPoint.IsDataComplete, Is.True);
      Assert.That (_productEndPoint.HasBeenTouched, Is.True);
      Assert.That (_productEndPoint.Collection, Is.EqualTo (new[] { _productReview2, _productReview3 }), "changes go down to actual data store");
      Assert.That (_productReview1.Product, Is.Null, "bidirectional modification");
      Assert.Ignore ("RM-7294: IsDataComplete after update should be false."); //TODO: RM-7294
    }

    [Test]
    public void GetCollection_DoesNotLoadData ()
    {
      _productEndPoint.MarkDataIncomplete();
      Assert.That (_productEndPoint.IsDataComplete, Is.False);

      Dev.Null = _productEndPoint.Collection;

      Assert.That (_productEndPoint.IsDataComplete, Is.False);
    }

    [Test]
    public void GetCollectionWithOriginalData_LoadsData ()
    {
      _productEndPoint.MarkDataIncomplete();
      Assert.That (_productEndPoint.IsDataComplete, Is.False);

      Dev.Null = _productEndPoint.GetCollectionWithOriginalData();

      Assert.That (_productEndPoint.IsDataComplete, Is.True);
    }

    [Test]
    public void Rollback_WithCompleteCollectionEndPoint_InvalidatesCollectionState ()
    {
      var newProductReview = ProductReview.NewObject();
      newProductReview.CreatedAt = DateTime.Now;
      newProductReview.InternalDataContainer.CommitState();

      Assert.That (_productEndPoint.HasBeenTouched, Is.False);
      Assert.That (_productEndPoint.Collection, Is.EqualTo (new[] { _productReview1, _productReview2, _productReview3 }));
      Assert.That (_productEndPoint.GetCollectionWithOriginalData(), Is.EqualTo (new[] { _productReview1, _productReview2, _productReview3 }));

      newProductReview.Product = _product1;

      Assert.That (_productEndPoint.IsDataComplete, Is.True);
      Assert.That (_productEndPoint.HasBeenTouched, Is.True);
      Assert.That (_productEndPoint.Collection, Is.EqualTo (new[] { _productReview1, _productReview2, _productReview3, newProductReview }));
      Assert.That (_productEndPoint.GetCollectionWithOriginalData(), Is.EqualTo (new[] { _productReview1, _productReview2, _productReview3 }));

      _productEndPoint.Rollback();
      newProductReview.InternalDataContainer.RollbackState();

      Assert.That (_productEndPoint.IsDataComplete, Is.True);
      Assert.That (_productEndPoint.HasBeenTouched, Is.False);
      Assert.That (_productEndPoint.Collection, Is.EqualTo (new[] { _productReview1, _productReview2, _productReview3 }));
      Assert.That (_productEndPoint.GetCollectionWithOriginalData(), Is.EqualTo (new[] { _productReview1, _productReview2, _productReview3 }));
      Assert.That (_productEndPoint.GetCollectionWithOriginalData(), Is.EqualTo (_productEndPoint.Collection));
    }

    [Test]
    public void Rollback_WithLazyLoadedCollectionEndPoint ()
    {
      var newProductReview = ProductReview.NewObject();
      newProductReview.CreatedAt = DateTime.Now;
      newProductReview.InternalDataContainer.CommitState();

      Assert.That (_productEndPoint.IsDataComplete, Is.False);
      Assert.That (_productEndPoint.HasBeenTouched, Is.False);

      newProductReview.Product = _product1;

      Assert.That (_productEndPoint.IsDataComplete, Is.True);
      Assert.That (_productEndPoint.HasBeenTouched, Is.True);

      _productEndPoint.Rollback();
      newProductReview.InternalDataContainer.RollbackState();

      Assert.That (_productEndPoint.IsDataComplete, Is.True);
      Assert.That (_productEndPoint.HasBeenTouched, Is.False);
      Assert.That (_productEndPoint.Collection, Is.EqualTo (new[] { _productReview1, _productReview2, _productReview3 }));
      Assert.That (_productEndPoint.GetCollectionWithOriginalData(), Is.EqualTo (new[] { _productReview1, _productReview2, _productReview3 }));
      Assert.That (_productEndPoint.GetCollectionWithOriginalData(), Is.EqualTo (_productEndPoint.Collection));
      Assert.Ignore ("RM-7294: IsDataComplete after update should be false."); //TODO: RM-7294
    }

    [Test]
    public void Rollback_UnchangedUnloaded_DoesNotLoadData ()
    {
      _productEndPoint.MarkDataIncomplete();
      Assert.That (_productEndPoint.HasChanged, Is.False);
      Assert.That (_productEndPoint.IsDataComplete, Is.False);

      _productEndPoint.Rollback();

      Assert.That (_productEndPoint.IsDataComplete, Is.False);
    }

    [Test]
    public void Commit_WithCompleteCollectionEndPoint_InvalidatesCollectionState ()
    {
      var newProductReview = ProductReview.NewObject();
      newProductReview.CreatedAt = DateTime.Now;

      Assert.That (_productEndPoint.HasBeenTouched, Is.False);
      Assert.That (_productEndPoint.Collection, Is.EqualTo (new[] { _productReview1, _productReview2, _productReview3 }));
      Assert.That (_productEndPoint.GetCollectionWithOriginalData(), Is.EqualTo (new[] { _productReview1, _productReview2, _productReview3 }));

      newProductReview.Product = _product1;

      Assert.That (_productEndPoint.IsDataComplete, Is.True);
      Assert.That (_productEndPoint.HasBeenTouched, Is.True);
      Assert.That (_productEndPoint.Collection, Is.EqualTo (new[] { _productReview1, _productReview2, _productReview3, newProductReview }));
      Assert.That (_productEndPoint.GetCollectionWithOriginalData(), Is.EqualTo (new[] { _productReview1, _productReview2, _productReview3 }));

      _productEndPoint.Commit();
      newProductReview.InternalDataContainer.CommitState();

      Assert.That (_productEndPoint.IsDataComplete, Is.True);
      Assert.That (_productEndPoint.HasBeenTouched, Is.False);
      Assert.That (_productEndPoint.Collection, Is.EqualTo (new[] { _productReview1, _productReview2, _productReview3, newProductReview }));
      Assert.That (_productEndPoint.GetCollectionWithOriginalData(), Is.EqualTo (new[] { _productReview1, _productReview2, _productReview3, newProductReview }));
      Assert.That (_productEndPoint.GetCollectionWithOriginalData(), Is.EqualTo (_productEndPoint.Collection));
      Assert.That (_productEndPoint.HasBeenTouched, Is.False);
    }

    [Test]
    public void Commit_WithLazyLoadedCollectionEndPoint ()
    {
      var newProductReview = ProductReview.NewObject();
      newProductReview.CreatedAt = DateTime.Now;

      Assert.That (_productEndPoint.IsDataComplete, Is.False);
      Assert.That (_productEndPoint.HasBeenTouched, Is.False);

      newProductReview.Product = _product1;

      Assert.That (_productEndPoint.IsDataComplete, Is.True);
      Assert.That (_productEndPoint.HasBeenTouched, Is.True);

      _productEndPoint.Commit();
      newProductReview.InternalDataContainer.CommitState();

      Assert.That (_productEndPoint.IsDataComplete, Is.True);
      Assert.That (_productEndPoint.HasBeenTouched, Is.False);
      Assert.That (_productEndPoint.Collection, Is.EqualTo (new[] { _productReview1, _productReview2, _productReview3, newProductReview }));
      Assert.That (_productEndPoint.GetCollectionWithOriginalData(), Is.EqualTo (new[] { _productReview1, _productReview2, _productReview3, newProductReview }));
      Assert.That (_productEndPoint.GetCollectionWithOriginalData(), Is.EqualTo (_productEndPoint.Collection));
      Assert.Ignore ("RM-7294: IsDataComplete after update should be false."); //TODO: RM-7294
    }

    [Test]
    public void ChangesToDataState_CauseTransactionListenerNotifications ()
    {
      var listener = ClientTransactionTestHelperWithMocks.CreateAndAddListenerMock (_productEndPoint.ClientTransaction);

      var newProductReview = ProductReview.NewObject();
      newProductReview.CreatedAt = DateTime.Now;

      newProductReview.Product = _product1;

      listener.AssertWasCalled (mock => mock.VirtualRelationEndPointStateUpdated (_productEndPoint.ClientTransaction, _productEndPoint.ID, true));
    }

    [Test]
    public void HasBeenTouched_Add_WithCompleteCollectionEndPoint ()
    {
      var newProductReview = ProductReview.NewObject();
      _productEndPoint.EnsureDataComplete();
      Assert.That (_productEndPoint.HasBeenTouched, Is.False);

      newProductReview.Product = _product1;

      Assert.That (_productEndPoint.IsDataComplete, Is.True);
      Assert.That (_productEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void HasBeenTouched_Add_WithLazyLoadedCollectionEndPoint ()
    {
      var newProductReview = ProductReview.NewObject();
      Assert.That (_productEndPoint.IsDataComplete, Is.False);
      Assert.That (_productEndPoint.HasBeenTouched, Is.False);

      newProductReview.Product = _product1;

      Assert.That (_productEndPoint.IsDataComplete, Is.True);
      Assert.That (_productEndPoint.HasBeenTouched, Is.True);
      Assert.Ignore ("RM-7294: IsDataComplete after update should be false."); //TODO: RM-7294
    }

    [Test]
    public void HasBeenTouched_Remove_WithCompleteCollectionEndPoint ()
    {
      _productEndPoint.EnsureDataComplete();
      Assert.That (_productEndPoint.HasBeenTouched, Is.False);

      _productReview1.Product = null;

      Assert.That (_productEndPoint.IsDataComplete, Is.True);
      Assert.That (_productEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void HasBeenTouched_Remove_WithLazyLoadedCollectionEndPoint ()
    {
      Assert.That (_productEndPoint.IsDataComplete, Is.False);
      Assert.That (_productEndPoint.HasBeenTouched, Is.False);

      _productReview1.Product = null;

      Assert.That (_productEndPoint.IsDataComplete, Is.True);
      Assert.That (_productEndPoint.HasBeenTouched, Is.True);
      Assert.Ignore ("RM-7294: IsDataComplete after update should be false."); //TODO: RM-7294
    }

    [Test]
    public void HasBeenTouched_AddAndRemove_LeavingSameElements ()
    {
      var newProductReview = ProductReview.NewObject();

      Assert.That (_productEndPoint.HasBeenTouched, Is.False);
      newProductReview.Product = _product1;
      Assert.That (_productEndPoint.HasBeenTouched, Is.True);
      newProductReview.Product = null;
      Assert.That (_productEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void HasBeenTouched_DoesNotLoadData ()
    {
      _productEndPoint.Touch();
      _productEndPoint.MarkDataIncomplete();

      var result = _productEndPoint.HasBeenTouched;

      Assert.That (_productEndPoint.IsDataComplete, Is.False);
      Assert.That (result, Is.True);
    }

    [Test]
    public void Touch_DoesNotLoadData ()
    {
      _productEndPoint.MarkDataIncomplete();

      _productEndPoint.Touch();

      Assert.That (_productEndPoint.IsDataComplete, Is.False);
      Assert.That (_productEndPoint.HasBeenTouched, Is.True);
    }
  }
}