using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class Product : TestDomainBase
  {
    public static Product NewObject ()
    {
      return NewObject<Product>();
    }

    protected Product ()
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string ProductName { get; set; }

    public abstract decimal ProductPrice { get; set; }

    [DBBidirectionalRelation ("Product")]
    public abstract IObjectList<ProductReview> ProductReviews { get; set; }
  }
}