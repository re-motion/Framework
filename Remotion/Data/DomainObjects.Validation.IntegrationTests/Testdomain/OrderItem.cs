using System;

namespace Remotion.Data.DomainObjects.Validation.IntegrationTests.Testdomain
{
  [DBTable]
  public class OrderItem : DomainObject
  {
    public static OrderItem NewObject ()
    {
      return NewObject<OrderItem>();
    }

    [DBBidirectionalRelation("OrderItems")]
    [Mandatory]
    public virtual Order Order { get; set; }

    [DBBidirectionalRelation("OrderItem", ContainsForeignKey = true)]
    [Mandatory]
    public virtual ProductReference ProductReference { get; set; }
  }
}
