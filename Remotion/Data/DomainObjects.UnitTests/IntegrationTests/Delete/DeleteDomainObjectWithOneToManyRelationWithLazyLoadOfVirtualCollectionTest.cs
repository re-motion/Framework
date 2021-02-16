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
  public class DeleteDomainObjectWithOneToManyRelationWithLazyLoadOfVirtualCollectionTest : ClientTransactionBaseTest
  {
    private Product _product;
    private ProductReview _productReview1;
    private ProductReview _productReview2;
    private ProductReview _productReview3;
    private SequenceEventReceiver _eventReceiver;

    public override void SetUp ()
    {
      base.SetUp();

      _product = DomainObjectIDs.Product1.GetObject<Product>();
      _productReview1 = DomainObjectIDs.ProductReview1.GetObject<ProductReview>();
      _productReview2 = DomainObjectIDs.ProductReview2.GetObject<ProductReview>();
      _productReview3 = DomainObjectIDs.ProductReview3.GetObject<ProductReview>();

      _eventReceiver = CreateEventReceiver();
      Assert.That (_productReview1.Product, Is.SameAs (_product));
      Assert.That (_productReview2.Product, Is.SameAs (_product));
      Assert.That (_productReview3.Product, Is.SameAs (_product));
      Assert.That (_product.Reviews.IsDataComplete, Is.False);
    }

    [Test]
    public void DeleteProduct ()
    {
      _product.Delete();

      ChangeState[] expectedStates =
      {
          new ObjectDeletionState (_product, "1. Deleting of product"),
          new RelationChangeState (_productReview1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ProductReview.Product", _product, null, "2. Relation changing of productReview1"),
          new RelationChangeState (_productReview2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ProductReview.Product", _product, null, "3. Relation changing of productReview2"),
          new RelationChangeState (_productReview3, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ProductReview.Product", _product, null, "4. Relation changing of productReview2"),
          new RelationChangeState (_productReview3, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ProductReview.Product", null, null, "5. Relation changed of productReview2"),
          new RelationChangeState (_productReview2, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ProductReview.Product", null, null, "6. Relation changed of productReview2"),
          new RelationChangeState (_productReview1, "Remotion.Data.DomainObjects.UnitTests.TestDomain.ProductReview.Product", null, null, "7. Relation changed of productReview1"),
          new ObjectDeletionState (_product, "8. Deleted of product")
      };

      _eventReceiver.Check (expectedStates);
    }

    [Test]
    public void ProductCancelsDeleteEvent ()
    {
      _eventReceiver.CancelEventNumber = 1;

      Assert.That (() => _product.Delete(), Throws.TypeOf<EventReceiverCancelException>());
      ChangeState[] expectedStates = { new ObjectDeletionState (_product, "1. Deleting of product") };

      _eventReceiver.Check (expectedStates);
    }

    [Test]
    public void ProductReviewCancelsDeleteEvent ()
    {
      _eventReceiver.CancelEventNumber = 2;

      Assert.That (() => _product.Delete(), Throws.TypeOf<EventReceiverCancelException>());
      ChangeState[] expectedStates =
      {
          new ObjectDeletionState (_product, "1. Deleting of product"),
          new RelationChangeState (
              _productReview1,
              "Remotion.Data.DomainObjects.UnitTests.TestDomain.ProductReview.Product",
              _product,
              null,
              "2. Relation changing of productReview1")
      };

      _eventReceiver.Check (expectedStates);
    }

    [Test]
    public void Relations ()
    {
      _product.Delete();

      Assert.That (_product.Reviews.Count, Is.EqualTo (0));
      Assert.That (_productReview1.Product, Is.Null);
      Assert.That (_productReview2.Product, Is.Null);
      Assert.That (_productReview1.Properties[typeof (ProductReview), "Product"].GetRelatedObjectID (), Is.Null);
      Assert.That (_productReview2.Properties[typeof (ProductReview), "Product"].GetRelatedObjectID (), Is.Null);
      Assert.That (_productReview1.InternalDataContainer.State.IsChanged, Is.True);
      Assert.That (_productReview2.InternalDataContainer.State.IsChanged, Is.True);
    }

    [Test]
    public void ChangePropertyBeforeDeletion ()
    {
      _productReview1.Product = null;
      _productReview2.Product = null;
      _productReview3.Product = null;
      _eventReceiver = CreateEventReceiver();

      _product.Delete();

      ChangeState[] expectedStates =
      {
          new ObjectDeletionState (_product, "1. Deleting of product"),
          new ObjectDeletionState (_product, "2. Deleted of product"),
      };

      _eventReceiver.Check (expectedStates);
    }

    [Test]
    public void GetOriginalRelatedObjects ()
    {
      _product.Delete();
      var originalReviews =
          _product.GetOriginalRelatedObjectsAsVirtualCollection ("Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews");

      Assert.That (originalReviews, Is.Not.Null);
      Assert.That (
          originalReviews.Select (o => o.ID),
          Is.EquivalentTo (new[] { DomainObjectIDs.ProductReview1, DomainObjectIDs.ProductReview2, DomainObjectIDs.ProductReview3 }));
    }

    [Test]
    public void ReassignDeletedObject ()
    {
      _product.Delete();
      Assert.That (
          () => _productReview1.Product = _product,
          Throws.InstanceOf<ObjectDeletedException>());
    }

    private SequenceEventReceiver CreateEventReceiver ()
    {
      return new SequenceEventReceiver (
          new DomainObject[] { _product, _productReview1, _productReview2, _productReview3 },
          new DomainObjectCollection[0]);
    }
  }
}
