using System;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class Product : TestDomainBase
  {
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