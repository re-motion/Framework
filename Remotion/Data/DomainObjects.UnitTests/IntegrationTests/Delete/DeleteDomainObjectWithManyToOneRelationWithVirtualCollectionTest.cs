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
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Delete
{
  [TestFixture]
  public class DeleteDomainObjectWithManyToOneRelationWithVirtualCollectionTest : ClientTransactionBaseTest
  {
    private ProductReview _productReview;
    private Product _product;
    private SequenceEventReceiver _eventReceiver;

    public override void SetUp ()
    {
      base.SetUp();

      _productReview = DomainObjectIDs.ProductReview1.GetObject<ProductReview>();
      _product = _productReview.Product;

      _eventReceiver = CreateEventReceiver();
      _product.EnsureDataAvailable();
    }

    [Test]
    public void DeleteProductReviewEvents ()
    {
      _productReview.Delete();

      ChangeState[] expectedStates =
      {
          new ObjectDeletionState(_productReview, "1. Deleting event of productReview"),
          new RelationChangeState(_product, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews", _productReview, null, "2. Relation changing event of product"),
          new RelationChangeState(_product, "Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews", null, null, "3. Relation changed event of product"),
          new ObjectDeletionState(_productReview, "4. Deleted event of productReview"),
      };

      _eventReceiver.Check(expectedStates);
    }

    [Test]
    public void ProductReviewCancelsDeleteEvent ()
    {
      _eventReceiver.CancelEventNumber = 1;

      Assert.That(() => _productReview.Delete(), Throws.TypeOf<EventReceiverCancelException>());
      ChangeState[] expectedStates = { new ObjectDeletionState(_productReview, "1. Deleting event of productReview") };

      _eventReceiver.Check(expectedStates);
    }

    [Test]
    public void ProductCancelsRelationChangeEvent ()
    {
      _eventReceiver.CancelEventNumber = 2;

      Assert.That(() => _productReview.Delete(), Throws.TypeOf<EventReceiverCancelException>());
      ChangeState[] expectedStates =
      {
          new ObjectDeletionState(_productReview, "1. Deleting event of productReview"),
          new RelationChangeState(
              _product,
              "Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews",
              _productReview,
              null,
              "2. Relation changing event of product")
      };

      _eventReceiver.Check(expectedStates);
    }

    [Test]
    public void Relations ()
    {
      int numberOfOrderItemsBeforeDelete = _product.Reviews.Count;
      _productReview.Delete();

      Assert.That(_productReview.Product, Is.Null);
      Assert.That(_product.Reviews.Count, Is.EqualTo(numberOfOrderItemsBeforeDelete - 1));
      Assert.That(_product.Reviews.Contains(_productReview.ID), Is.False);
      Assert.That(_productReview.Properties[typeof(ProductReview), "Product"].GetRelatedObjectID(), Is.Null);
      Assert.That(_product.State.IsChanged, Is.True);
      Assert.That(_product.InternalDataContainer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void ChangePropertyBeforeDeletion ()
    {
      _productReview.Product = null;
      _eventReceiver = CreateEventReceiver();

      _productReview.Delete();

      ChangeState[] expectedStates =
      {
          new ObjectDeletionState(_productReview, "1. Deleting event of productReview"),
          new ObjectDeletionState(_productReview, "2. Deleted event of productReview"),
      };

      _eventReceiver.Check(expectedStates);
    }

    [Test]
    public void GetOriginalRelatedObjects ()
    {
      _productReview.Delete();

      var originalReviews = _product.GetOriginalRelatedObjectsAsVirtualCollection("Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews");

      Assert.That(originalReviews, Is.Not.Null);
      Assert.That(
          originalReviews.Select(o => o.ID),
          Is.EquivalentTo(new[] { DomainObjectIDs.ProductReview1, DomainObjectIDs.ProductReview2, DomainObjectIDs.ProductReview3 }));
    }

    [Test]
    public void SetRelatedObjectOfDeletedObject ()
    {
      _productReview.Delete();
      Assert.That(
          () => _productReview.Product = _product,
          Throws.InstanceOf<ObjectDeletedException>());
    }

    [Test]
    public void DeleteProductEvents ()
    {
      _product.Delete();

      ChangeState[] expectedStates = 
      {
          new ObjectDeletionState(_product, "1. Deleting event of product"),
          new RelationChangeState(_productReview, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ProductReview.Product", _product, null, "2. Relation changing event of productReview"),
          new RelationChangeState(_productReview, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ProductReview.Product", null, null, "3. Relation changed event of productReview"),
          new ObjectDeletionState(_product, "4. Deleted event of product"),
      };

      _eventReceiver.Check(expectedStates);
    }

    [Test]
    public void DeleteProductEvents_CancelAfterObjectDeletingEvent ()
    {
      _eventReceiver.CancelEventNumber = 2;

      Assert.That(() => _product.Delete(), Throws.TypeOf<EventReceiverCancelException>());
      ChangeState[] expectedStates =
      {
          new ObjectDeletionState(_product, "1. Deleting event of product"),
          new RelationChangeState(
              _productReview,
              "Remotion.Data.DomainObjects.UnitTests.TestDomain.ProductReview.Product",
              _product,
              null,
              "2. Relation changing event of productReview"),
      };

      _eventReceiver.Check(expectedStates);

      Assert.That(_product.State.IsDeleted, Is.False);
      Assert.That(_product.Reviews, Is.Not.Empty);
    }

    private SequenceEventReceiver CreateEventReceiver ()
    {
      return new SequenceEventReceiver(
          new DomainObject[] { _productReview, _product },
          new DomainObjectCollection[0]);
    }
  }
}
