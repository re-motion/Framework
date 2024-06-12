using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
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
    [DBBidirectionalRelation("Reviews")]
    public abstract Product Product { get; set; }

    [Mandatory]
    [DBBidirectionalRelation("Reviews")]
    public abstract Person Reviewer { get; set; }

    public abstract  DateTime CreatedAt { get; set; }

    [StringProperty(IsNullable = false, MaximumLength = 1000)]
    public abstract string Comment { get; set; }
  }
}
