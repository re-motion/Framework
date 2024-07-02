using System;
using System.Linq;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
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

    [StringProperty(IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    public abstract decimal Price { get; set; }

    [DBBidirectionalRelation("Product", SortExpression = "CreatedAt ASC")]
    public abstract IObjectList<ProductReview> Reviews { get; set; }

    public void DeleteWithProductReviews ()
    {
      foreach (var productReview in Reviews.ToArray())
        productReview.Delete();

      this.Delete();
    }
  }
}
