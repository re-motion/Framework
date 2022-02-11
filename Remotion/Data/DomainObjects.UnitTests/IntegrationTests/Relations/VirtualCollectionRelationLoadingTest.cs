using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.Relations
{
  [TestFixture]
  public class VirtualCollectionRelationLoadingTest : ClientTransactionBaseTest
  {
    private Product _product;

    public override void SetUp ()
    {
      base.SetUp();

      _product = DomainObjectIDs.Product1.GetObjectReference<Product>();
    }

    [Test]
    public void AccessingRelatedVirtualCollection_ReturnsCollectionWithIncompleteContents_AndAlsoLoadsOriginatingObject ()
    {
      Assert.That(_product.State.IsNotLoadedYet, Is.True);

      var orderItems = _product.Reviews;

      Assert.That(orderItems.AssociatedEndPointID, Is.EqualTo(RelationEndPointID.Resolve(_product, o => o.Reviews)));
      Assert.That(orderItems.IsDataComplete, Is.False);
      Assert.That(_product.State.IsUnchanged, Is.True);
    }

    [Test]
    public void AccessingRelatedVirtualCollection_ReturnsCollectionWithIncompleteContents_ContentsIsLoadedWhenNeeded ()
    {
      Assert.That(_product.Reviews.IsDataComplete, Is.False);

      Assert.That(_product.Reviews.Count, Is.EqualTo(3));

      Assert.That(_product.Reviews.IsDataComplete, Is.True);
    }

    [Test]
    [Ignore("TODO: RM-7294: test mandatory collection")]
    public void AccessingRelatedVirtualCollection_ExceptionOnLoading_IsTriggeredOnDemand ()
    {
      //var orderWithoutReviews = DomainObjectIDs.OrderWithoutReviews.GetObject<Order>();

      //Assert.That (orderWithoutReviews.Reviews.IsDataComplete, Is.False);

      //Assert.That (() => orderWithoutReviews.Reviews.Count, Throws.TypeOf<PersistenceException>());
    }

    [Test]
    public void AccessingRelatedVirtualCollection_ReturnsAlreadyLoadedCollection_IfAlreadyLoaded ()
    {
      TestableClientTransaction.EnsureDataComplete(RelationEndPointID.Resolve(_product, o => o.Reviews));

      Assert.That(_product.Reviews.IsDataComplete, Is.True);
    }

    [Test]
    public void AccessingOriginalRelatedVirtualCollection_LoadsContentsForBothOriginalAndCurrentCollection ()
    {
      Assert.That(_product.Properties[typeof(Product), "Reviews"].GetOriginalValue<IObjectList<ProductReview>>().IsDataComplete, Is.True);
      // Since the data had to be loaded for the original contents, it has also been loaded into the actual collection.
      Assert.That(_product.Reviews.IsDataComplete, Is.True);
    }

    [Test]
    public void CommittingNestedSubTransactions_PropagatesItemsAddedToCollectionOnEveryHop ()
    {
      var product = DomainObjectIDs.Product1.GetObject<Product>();
      ProductReview productReview;

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          productReview = ProductReview.NewObject();
          productReview.Reviewer = DomainObjectIDs.Person1.GetObject<Person>();
          productReview.Comment = "Test Comment";
          productReview.Product = product;
          Assert.That(product.Reviews.Count, Is.EqualTo(4));
          Assert.That(product.Reviews, Has.Member(productReview));
          ClientTransaction.Current.Commit();
        }

        Assert.That(product.Reviews.Count, Is.EqualTo(4));
        Assert.That(product.Reviews, Has.Member(productReview));
        ClientTransaction.Current.Commit();
      }

      Assert.That(product.Reviews.Count, Is.EqualTo(4));
      Assert.That(product.Reviews, Has.Member(productReview));
    }

    [Test]
    public void CommittingNestedSubTransactions_PropagatesItemsRemovedFromCollectionOnEveryHop ()
    {
      var product = DomainObjectIDs.Product1.GetObject<Product>();
      ProductReview productReview = DomainObjectIDs.ProductReview1.GetObject<ProductReview>();

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          productReview.Delete();
          Assert.That(product.Reviews.Count, Is.EqualTo(2));
          Assert.That(product.Reviews, Has.No.Member(productReview));
          ClientTransaction.Current.Commit();
        }

        Assert.That(product.Reviews.Count, Is.EqualTo(2));
        Assert.That(product.Reviews, Has.No.Member(productReview));
        ClientTransaction.Current.Commit();
      }

      Assert.That(product.Reviews.Count, Is.EqualTo(2));
      Assert.That(product.Reviews, Has.No.Member(productReview));
    }
  }
}
