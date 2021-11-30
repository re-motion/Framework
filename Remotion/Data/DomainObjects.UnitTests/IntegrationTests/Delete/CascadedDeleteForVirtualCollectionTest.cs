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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Delete
{
  [TestFixture]
  public class CascadedDeleteForVirtualCollectionTest : ClientTransactionBaseTest
  {
    [Test]
    [Ignore("TODO RM-6156: Define what re-store should do here - actually, it's not allowed to modify the relations within the Deleting handler, but the exception is quite unclear.")]
    public void BidirectionalRelation_CascadeWithinDeleting ()
    {
      var product = Product.NewObject();
      var productReview = ProductReview.NewObject();
      productReview.Product = product;

      product.Deleting += delegate
      {
        var reviews = product.Reviews.ToArray();
        foreach (var review in reviews)
          review.Delete();
      };

      product.Delete();

      Assert.That(product.State.IsInvalid, Is.True);
      Assert.That(productReview.State.IsInvalid, Is.True);
      Assert.That(TestableClientTransaction.DataManager.DataContainers, Is.Empty);
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints, Is.Empty);
    }

    [Test]
    public void BidirectionalRelation_CascadeWithinDeleted ()
    {
      var product = Product.NewObject();
      var productReview = ProductReview.NewObject();

      productReview.Product = product;

      ProductReview objectToBeDeleted = null;
      product.Deleting += delegate { objectToBeDeleted = product.Reviews.Single(); };
      product.Deleted += delegate { objectToBeDeleted.Delete(); };

      product.Delete();

      Assert.That(product.State.IsInvalid, Is.True);
      Assert.That(productReview.State.IsInvalid, Is.True);
      Assert.That(TestableClientTransaction.DataManager.DataContainers, Is.Empty);
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints, Is.Empty);
    }

    [Test]
    [Ignore("TODO: Define what re-store should do here - actually, it's not allowed to modify the relations within the Deleting handler, but the exception is quite unclear.")]
    public void BidirectionalRelation_CascadeWithinDeleting_SubTransaction ()
    {
      var product = Product.NewObject();
      var productReview = ProductReview.NewObject();
      productReview.Product = product;

      product.Deleting += delegate
      {
        var reviews = product.Reviews.ToArray();
        foreach (var review in reviews)
          review.Delete();
      };

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        product.Delete();
        ClientTransaction.Current.Commit();
      }

      Assert.That(product.State.IsInvalid, Is.True);
      Assert.That(productReview.State.IsInvalid, Is.True);
      Assert.That(TestableClientTransaction.DataManager.DataContainers, Is.Empty);
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints, Is.Empty);
    }

    [Test]
    public void BidirectionalRelation_CascadeWithinDeleted_SubTransaction ()
    {
      var product = Product.NewObject();
      var productReview = ProductReview.NewObject();

      productReview.Product = product;

      ProductReview objectToBeDeleted = null;
      product.Deleting += delegate { objectToBeDeleted = product.Reviews.Single(); };
      product.Deleted += delegate { objectToBeDeleted.Delete(); };

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        product.Delete();
        ClientTransaction.Current.Commit();
      }

      Assert.That(product.State.IsInvalid, Is.True);
      Assert.That(productReview.State.IsInvalid, Is.True);
      Assert.That(TestableClientTransaction.DataManager.DataContainers, Is.Empty);
      Assert.That(TestableClientTransaction.DataManager.RelationEndPoints, Is.Empty);
    }
  }
}
