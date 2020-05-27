using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class ProductReview : TestDomainBase
  {
    public static ProductReview NewObject ()
    {
      return NewObject<ProductReview>();
    }

    protected ProductReview ()
    {
    }

    [Mandatory]
    [DBBidirectionalRelation ("ProductReviews")]
    public abstract Product Product { get; set; }

    [Mandatory]
    //[DBBidirectionalRelation ("ProductReviews")] //TODO: RM-7294
    public abstract Person Reviewer { get; set; }

    public abstract  DateTime CreatedAt { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 1000)]
    public abstract string Comment { get; set; }
  }
}