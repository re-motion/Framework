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
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class RemoveFromOneToManyRelationWithVirtualCollectionTest : ClientTransactionBaseTest
  {
    private Product _product;
    private ProductReview _productReview;

    private DomainObjectEventReceiver _productEventReceiver;
    private DomainObjectEventReceiver _productReviewEventReceiver;

    public override void SetUp ()
    {
      base.SetUp();

      _product = DomainObjectIDs.Product1.GetObject<Product>();
      _productReview = DomainObjectIDs.ProductReview1.GetObject<ProductReview>();

      _productEventReceiver = new DomainObjectEventReceiver(_product);
      _productReviewEventReceiver = new DomainObjectEventReceiver(_productReview);

      _product.EnsureDataAvailable();
      _productReview.EnsureDataAvailable();
      _product.Reviews.EnsureDataComplete();
    }

    [Test]
    public void ChangeEvents ()
    {
      _productReviewEventReceiver.Cancel = false;
      _productEventReceiver.Cancel = false;

      _productReview.Product = null;

      Assert.That(_productReviewEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo(true));
      Assert.That(_productReviewEventReceiver.HasRelationChangedEventBeenCalled, Is.EqualTo(true));
      Assert.That(
          _productReviewEventReceiver.ChangingRelationPropertyName,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.TestDomain.ProductReview.Product"));
      Assert.That(
          _productReviewEventReceiver.ChangedRelationPropertyName,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.TestDomain.ProductReview.Product"));
      Assert.That(_productReviewEventReceiver.ChangingOldRelatedObject, Is.SameAs(_product));
      Assert.That(_productReviewEventReceiver.ChangingNewRelatedObject, Is.Null);
      Assert.That(_productReviewEventReceiver.ChangedOldRelatedObject, Is.SameAs(_product));
      Assert.That(_productReviewEventReceiver.ChangedNewRelatedObject, Is.Null);

      Assert.That(_productEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo(true));
      Assert.That(_productEventReceiver.HasRelationChangedEventBeenCalled, Is.EqualTo(true));
      Assert.That(
          _productEventReceiver.ChangingRelationPropertyName,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews"));
      Assert.That(
          _productEventReceiver.ChangedRelationPropertyName,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews"));
      Assert.That(_productEventReceiver.ChangingOldRelatedObject, Is.SameAs(_productReview));
      Assert.That(_productEventReceiver.ChangingNewRelatedObject, Is.Null);
      Assert.That(_productEventReceiver.ChangedOldRelatedObject, Is.SameAs(_productReview));
      Assert.That(_productEventReceiver.ChangedNewRelatedObject, Is.Null);

      Assert.That(_productReview.State.IsChanged, Is.True);
      Assert.That(_product.State.IsChanged, Is.True); // TODO: RM-7294: IsRelationChanged
      Assert.That(_product.Reviews.GetObject(_productReview.ID), Is.Null);
      Assert.That(_productReview.Product, Is.Null);
    }

    [Test]
    public void ChildCancelsChangeEvent ()
    {
      _productReviewEventReceiver.Cancel = true;
      _productEventReceiver.Cancel = false;

      Assert.That(() => _productReview.Product = null, Throws.TypeOf<EventReceiverCancelException>());

      Assert.That(_productReviewEventReceiver.HasRelationChangingEventBeenCalled, Is.True);
      Assert.That(_productReviewEventReceiver.HasRelationChangedEventBeenCalled, Is.False);
      Assert.That(
          _productReviewEventReceiver.ChangingRelationPropertyName,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.TestDomain.ProductReview.Product"));
      Assert.That(_productReviewEventReceiver.ChangedRelationPropertyName, Is.Null);
      Assert.That(_productReviewEventReceiver.ChangingOldRelatedObject, Is.SameAs(_product));
      Assert.That(_productReviewEventReceiver.ChangingNewRelatedObject, Is.Null);

      Assert.That(_productEventReceiver.HasRelationChangingEventBeenCalled, Is.False);
      Assert.That(_productEventReceiver.HasRelationChangedEventBeenCalled, Is.False);
      Assert.That(_productEventReceiver.ChangingRelationPropertyName, Is.Null);
      Assert.That(_productEventReceiver.ChangedRelationPropertyName, Is.Null);
      Assert.That(_productEventReceiver.ChangingOldRelatedObject, Is.Null);
      Assert.That(_productEventReceiver.ChangingNewRelatedObject, Is.Null);

      Assert.That(_productReview.State.IsUnchanged, Is.True);
      Assert.That(_product.State.IsUnchanged, Is.True); // TODO: RM-7294: IsRelationChanged
      Assert.That(_product.Reviews.GetObject(_productReview.ID), Is.EqualTo(_productReview));
      Assert.That(_productReview.Product, Is.SameAs(_product));
    }

    [Test]
    public void ParentCancelsChangeEvent ()
    {
      _productReviewEventReceiver.Cancel = false;
      _productEventReceiver.Cancel = true;

      Assert.That(() => _productReview.Product = null, Throws.TypeOf<EventReceiverCancelException>());

      Assert.That(_productReviewEventReceiver.HasRelationChangingEventBeenCalled, Is.True);
      Assert.That(_productReviewEventReceiver.HasRelationChangedEventBeenCalled, Is.False);
      Assert.That(
          _productReviewEventReceiver.ChangingRelationPropertyName,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.TestDomain.ProductReview.Product"));
      Assert.That(_productReviewEventReceiver.ChangedRelationPropertyName, Is.Null);
      Assert.That(_productReviewEventReceiver.ChangingOldRelatedObject, Is.SameAs(_product));
      Assert.That(_productReviewEventReceiver.ChangingNewRelatedObject, Is.Null);

      Assert.That(_productEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo(true));
      Assert.That(_productEventReceiver.HasRelationChangedEventBeenCalled, Is.False);
      Assert.That(
          _productEventReceiver.ChangingRelationPropertyName,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews"));
      Assert.That(_productEventReceiver.ChangedRelationPropertyName, Is.Null);
      Assert.That(_productEventReceiver.ChangingOldRelatedObject, Is.SameAs(_productReview));
      Assert.That(_productEventReceiver.ChangingNewRelatedObject, Is.Null);

      Assert.That(_productReview.State.IsUnchanged, Is.True);
      Assert.That(_product.State.IsUnchanged, Is.True); // TODO: RM-7294: IsRelationChanged
      Assert.That(_product.Reviews.GetObject(_productReview.ID), Is.EqualTo(_productReview));
      Assert.That(_productReview.Product, Is.SameAs(_product));
    }

    [Test]
    public void StateTracking ()
    {
      Assert.That(_product.State.IsUnchanged, Is.True);
      Assert.That(_productReview.State.IsUnchanged, Is.True);

      _productReview.Product = null;

      Assert.That(_product.State.IsChanged, Is.True); // TODO: RM-7294: IsRelationChanged
      Assert.That(_productReview.State.IsChanged, Is.True);
    }

    [Test]
    public void RelationEndPointMap ()
    {
      _productReview.Product = null;

      Assert.That(_productReview.Product, Is.Null);
    }

    [Test]
    public void SetPropertyValue ()
    {
      _productReview.Product = null;

      Assert.That(_productReview.Properties[typeof(ProductReview), "Product"].GetRelatedObjectID(), Is.Null);
    }

    [Test]
    public void SetParentNull ()
    {
      _productReview.Product = null;

      Assert.That(_productReview.Product, Is.Null);
      Assert.That(_product.Reviews.Count, Is.EqualTo(2));
      Assert.That(_product.Reviews.GetObject(_productReview.ID), Is.Null);
    }
  }
}
