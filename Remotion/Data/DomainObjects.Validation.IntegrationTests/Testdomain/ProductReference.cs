using System;

namespace Remotion.Data.DomainObjects.Validation.IntegrationTests.Testdomain
{
  [DBTable]
  public class ProductReference : DomainObject
  {
    public static ProductReference NewObject ()
    {
      return NewObject<ProductReference>();
    }

    [DBBidirectionalRelation ("ProductReference")]
    [Mandatory]
    public virtual OrderItem OrderItem { get; set; }

    [Mandatory]
    public virtual Product Product { get; set; }
  }
}
