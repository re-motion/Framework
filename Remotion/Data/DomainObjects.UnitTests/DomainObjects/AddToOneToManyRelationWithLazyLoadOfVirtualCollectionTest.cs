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
using Remotion.Data.DomainObjects.UnitTests.EventReceiver;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class AddToOneToManyRelationWithLazyLoadOfVirtualCollectionTest : ClientTransactionBaseTest
  {
    private Product _product;
    private ProductReview _productReviewWithoutProduct;

    private DomainObjectEventReceiver _productEventReceiver;
    private DomainObjectEventReceiver _productReviewEventReceiver;
    private ClientTransactionScope _clientTransactionScope;

    public override void SetUp ()
    {
      base.SetUp();

      _product = DomainObjectIDs.Product1.GetObject<Product>();
      _productReviewWithoutProduct = DomainObjectMother.CreateObjectInTransaction<ProductReview>(TestableClientTransaction);

      _productEventReceiver = new DomainObjectEventReceiver(_product);
      _productReviewEventReceiver = new DomainObjectEventReceiver(_productReviewWithoutProduct);

      _clientTransactionScope = TestableClientTransaction.CreateSubTransaction().EnterDiscardingScope();

      _product.EnsureDataAvailable();
      _productReviewWithoutProduct.EnsureDataAvailable();
      Assert.That(_product.Reviews.IsDataComplete, Is.False);
    }

    public override void TearDown ()
    {
      _clientTransactionScope.Leave();
      base.TearDown();
    }

    [Test]
    public void AddEvents ()
    {
      _productReviewEventReceiver.Cancel = false;
      _productEventReceiver.Cancel = false;

      _productReviewWithoutProduct.Product = _product;

      Assert.That(_productReviewEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo(true));
      Assert.That(_productReviewEventReceiver.HasRelationChangedEventBeenCalled, Is.EqualTo(true));
      Assert.That(
          _productReviewEventReceiver.ChangingRelationPropertyName,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.TestDomain.ProductReview.Product"));
      Assert.That(
          _productReviewEventReceiver.ChangedRelationPropertyName,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.TestDomain.ProductReview.Product"));
      Assert.That(_productReviewEventReceiver.ChangingOldRelatedObject, Is.Null);
      Assert.That(_productReviewEventReceiver.ChangingNewRelatedObject, Is.SameAs(_product));
      Assert.That(_productReviewEventReceiver.ChangedOldRelatedObject, Is.Null);
      Assert.That(_productReviewEventReceiver.ChangedNewRelatedObject, Is.SameAs(_product));

      Assert.That(_productEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo(true));
      Assert.That(_productEventReceiver.HasRelationChangedEventBeenCalled, Is.EqualTo(true));
      Assert.That(
          _productEventReceiver.ChangingRelationPropertyName,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews"));
      Assert.That(
          _productEventReceiver.ChangedRelationPropertyName,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews"));
      Assert.That(_productEventReceiver.ChangingOldRelatedObject, Is.Null);
      Assert.That(_productEventReceiver.ChangingNewRelatedObject, Is.SameAs(_productReviewWithoutProduct));
      Assert.That(_productEventReceiver.ChangedOldRelatedObject, Is.Null);
      Assert.That(_productEventReceiver.ChangedNewRelatedObject, Is.SameAs(_productReviewWithoutProduct));

      Assert.That(_productReviewWithoutProduct.State.IsChanged, Is.True);
      Assert.That(_product.State.IsChanged, Is.True); // TODO: RM-7294: IsRelationChanged
      Assert.That(_product.Reviews.GetObject(_productReviewWithoutProduct.ID), Is.EqualTo(_productReviewWithoutProduct));
      Assert.That(_productReviewWithoutProduct.Product, Is.SameAs(_product));
    }

    [Test]
    public void ChildCancelsChangeEvent ()
    {
      _productReviewEventReceiver.Cancel = true;
      _productEventReceiver.Cancel = false;

      Assert.That(() => _productReviewWithoutProduct.Product = _product, Throws.TypeOf<EventReceiverCancelException>());

      Assert.That(_productReviewEventReceiver.HasRelationChangingEventBeenCalled, Is.True);
      Assert.That(_productReviewEventReceiver.HasRelationChangedEventBeenCalled, Is.False);
      Assert.That(
          _productReviewEventReceiver.ChangingRelationPropertyName,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.TestDomain.ProductReview.Product"));
      Assert.That(_productReviewEventReceiver.ChangedRelationPropertyName, Is.Null);
      Assert.That(_productReviewEventReceiver.ChangingOldRelatedObject, Is.Null);
      Assert.That(_productReviewEventReceiver.ChangingNewRelatedObject, Is.SameAs(_product));

      Assert.That(_productEventReceiver.HasRelationChangingEventBeenCalled, Is.False);
      Assert.That(_productEventReceiver.HasRelationChangedEventBeenCalled, Is.False);
      Assert.That(_productEventReceiver.ChangingRelationPropertyName, Is.Null);
      Assert.That(_productEventReceiver.ChangedRelationPropertyName, Is.Null);
      Assert.That(_productEventReceiver.ChangingOldRelatedObject, Is.Null);
      Assert.That(_productEventReceiver.ChangingNewRelatedObject, Is.Null);

      Assert.That(_productReviewWithoutProduct.State.IsUnchanged, Is.True);
      Assert.That(_product.State.IsUnchanged, Is.True); // TODO: RM-7294: IsRelationChanged
      Assert.That(_product.Reviews.Count, Is.EqualTo(3));
      Assert.That(_productReviewWithoutProduct.Product, Is.Null);
    }

    [Test]
    public void ParentCancelsChangeEvent ()
    {
      _productReviewEventReceiver.Cancel = false;
      _productEventReceiver.Cancel = true;

      Assert.That(() => _productReviewWithoutProduct.Product = _product, Throws.TypeOf<EventReceiverCancelException>());

      Assert.That(_productReviewEventReceiver.HasRelationChangingEventBeenCalled, Is.True);
      Assert.That(_productReviewEventReceiver.HasRelationChangedEventBeenCalled, Is.False);
      Assert.That(
          _productReviewEventReceiver.ChangingRelationPropertyName,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.TestDomain.ProductReview.Product"));
      Assert.That(_productReviewEventReceiver.ChangedRelationPropertyName, Is.Null);
      Assert.That(_productReviewEventReceiver.ChangingOldRelatedObject, Is.Null);
      Assert.That(_productReviewEventReceiver.ChangingNewRelatedObject, Is.SameAs(_product));

      Assert.That(_productEventReceiver.HasRelationChangingEventBeenCalled, Is.EqualTo(true));
      Assert.That(_productEventReceiver.HasRelationChangedEventBeenCalled, Is.False);
      Assert.That(
          _productEventReceiver.ChangingRelationPropertyName,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews"));
      Assert.That(_productEventReceiver.ChangedRelationPropertyName, Is.Null);
      Assert.That(_productEventReceiver.ChangingOldRelatedObject, Is.Null);
      Assert.That(_productEventReceiver.ChangingNewRelatedObject, Is.SameAs(_productReviewWithoutProduct));

      Assert.That(_productReviewWithoutProduct.State.IsUnchanged, Is.True);
      Assert.That(_product.State.IsUnchanged, Is.True); // TODO: RM-7294: IsRelationChanged
      Assert.That(_product.Reviews.Count, Is.EqualTo(3));
      Assert.That(_productReviewWithoutProduct.Product, Is.Null);
    }

    [Test]
    public void StateTracking ()
    {
      Assert.That(_product.State.IsUnchanged, Is.True);
      Assert.That(_productReviewWithoutProduct.State.IsUnchanged, Is.True);

      _productReviewWithoutProduct.Product = _product;

      Assert.That(_product.State.IsChanged, Is.True); // TODO: RM-7294: IsRelationChanged
      Assert.That(_productReviewWithoutProduct.State.IsChanged, Is.True);
    }

    [Test]
    public void RelationEndPointMap ()
    {
      _productReviewWithoutProduct.Product = _product;

      Assert.That(_productReviewWithoutProduct.Product, Is.SameAs(_product));
    }

    [Test]
    public void SetPropertyValue ()
    {
      _productReviewWithoutProduct.Product = _product;

      Assert.That(_productReviewWithoutProduct.Properties[typeof(ProductReview), "Product"].GetRelatedObjectID(), Is.EqualTo(_product.ID));
    }

    [Test]
    public void SetParent ()
    {
      _productReviewWithoutProduct.Product = _product;

      Assert.That(_productReviewWithoutProduct.Product, Is.SameAs(_product));
      Assert.That(_product.Reviews.IsDataComplete, Is.False);
      Assert.That(_product.Reviews.Count, Is.EqualTo(4));
      Assert.That(_product.Reviews.IsDataComplete, Is.True);
      Assert.That(_product.Reviews.GetObject(_productReviewWithoutProduct.ID), Is.EqualTo(_productReviewWithoutProduct));
    }

    [Test]
    public void SetSameParent ()
    {
      var productReview = _product.Reviews.GetObject(DomainObjectIDs.ProductReview1);
      Assert.That(productReview.Product, Is.EqualTo(_product));
      productReview.Product = _product;

      Assert.That(_product.State.IsUnchanged, Is.True); // TODO: RM-7294: IsRelationChanged
      Assert.That(productReview.State.IsUnchanged, Is.True);
      Assert.That(productReview.Product, Is.EqualTo(_product));
      Assert.That(_product.Reviews.Contains(productReview.ID), Is.True);
    }

    [Test]
    public void SetParentNull ()
    {
      var productReview = _product.Reviews.First();
      productReview.Product = null;

      Assert.That(_product.State.IsChanged, Is.True);
      Assert.That(productReview.State.IsChanged, Is.True);
      Assert.That(productReview.Product, Is.Null);
      Assert.That(_product.Reviews.Contains(productReview.ID), Is.False);
    }

    [Test]
    public void SetParentWhenChildAlreadyHasParent ()
    {
      var productReview = DomainObjectIDs.ProductReview4.GetObject<ProductReview>();
      var oldProduct = DomainObjectIDs.Product2.GetObject<Product>();
      oldProduct.Reviews.EnsureDataComplete();

      var productReviewEventReceiver = new DomainObjectEventReceiver(productReview);
      productReviewEventReceiver.Cancel = false;

      var oldProductEventReceiver = new DomainObjectEventReceiver(oldProduct);
      oldProductEventReceiver.Cancel = false;

      _productEventReceiver.Cancel = false;

      productReview.Product = _product;

      Assert.That(oldProductEventReceiver.HasRelationChangingEventBeenCalled, Is.True);
      Assert.That(oldProductEventReceiver.HasRelationChangedEventBeenCalled, Is.True);
      Assert.That(
          oldProductEventReceiver.ChangingRelationPropertyName,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews"));
      Assert.That(
          oldProductEventReceiver.ChangedRelationPropertyName,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews"));

      Assert.That(productReviewEventReceiver.HasRelationChangingEventBeenCalled, Is.True);
      Assert.That(productReviewEventReceiver.HasRelationChangedEventBeenCalled, Is.True);
      Assert.That(
          productReviewEventReceiver.ChangingRelationPropertyName,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.TestDomain.ProductReview.Product"));
      Assert.That(
          productReviewEventReceiver.ChangedRelationPropertyName,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.TestDomain.ProductReview.Product"));
      Assert.That(productReviewEventReceiver.ChangingOldRelatedObject, Is.SameAs(oldProduct));
      Assert.That(productReviewEventReceiver.ChangingNewRelatedObject, Is.SameAs(_product));
      Assert.That(productReviewEventReceiver.ChangedOldRelatedObject, Is.SameAs(oldProduct));
      Assert.That(productReviewEventReceiver.ChangedNewRelatedObject, Is.SameAs(_product));

      Assert.That(_productEventReceiver.HasRelationChangingEventBeenCalled, Is.True);
      Assert.That(_productEventReceiver.HasRelationChangedEventBeenCalled, Is.True);
      Assert.That(
          _productEventReceiver.ChangingRelationPropertyName,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews"));
      Assert.That(
          _productEventReceiver.ChangedRelationPropertyName,
          Is.EqualTo("Remotion.Data.DomainObjects.UnitTests.TestDomain.Product.Reviews"));
      Assert.That(_productEventReceiver.ChangingOldRelatedObject, Is.Null);
      Assert.That(_productEventReceiver.ChangingNewRelatedObject, Is.SameAs(productReview));
      Assert.That(_productEventReceiver.ChangedOldRelatedObject, Is.Null);
      Assert.That(_productEventReceiver.ChangedNewRelatedObject, Is.SameAs(productReview));

      Assert.That(productReview.State.IsChanged, Is.True);
      Assert.That(_product.State.IsChanged, Is.True); // TODO: RM-7294: IsRelationChanged
      Assert.That(oldProduct.State.IsChanged, Is.True); // TODO: RM-7294: IsRelationChanged

      Assert.That(_product.Reviews.GetObject(productReview.ID), Is.EqualTo(productReview));
      Assert.That(oldProduct.Reviews.Contains(productReview.ID), Is.False);
      Assert.That(productReview.Product, Is.SameAs(_product));
    }

    [Test]
    public void SetParentAddsElementSorted ()
    {
      var lastProductReview = _product.Reviews.Last();
      _productReviewWithoutProduct.CreatedAt = lastProductReview.CreatedAt.Subtract(TimeSpan.FromSeconds(1));
      _productReviewWithoutProduct.Product = _product;

      Assert.That(_product.Reviews.ToList().IndexOf(_productReviewWithoutProduct), Is.EqualTo(_product.Reviews.Count - 2));
    }
  }
}
