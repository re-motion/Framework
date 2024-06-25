using System;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration
{
  [DBTable]
  [TestDomain]
  [Instantiable]
  public abstract class Product : TestDomainBase
  {
    protected Product ()
    {
    }

    [StringProperty(IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    public abstract decimal Price { get; set; }

    [DBBidirectionalRelation("Product", SortExpression = "CreatedAt DESC")]
    public abstract IObjectList<ProductReview> Reviews { get; set; }
  }
}
