using System;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class ProductReview : TestDomainBase
  {
    protected ProductReview ()
    {
    }

    [Mandatory]
    [DBBidirectionalRelation ("ProductReviews")]
    public abstract Product Product { get; set; }

    [Mandatory]
    [DBBidirectionalRelation ("ProductReviews")]
    public abstract Person Reviewer { get; set; }

    public abstract  DateTime CreatedAt { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 1000)]
    public abstract string Comment { get; set; }
  }
}