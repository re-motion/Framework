namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.InterfaceMapping
{
  [TestDomain]
  public interface IOrderItem : IDomainObject
  {
    int Position { get; set; }

    [StringProperty(IsNullable = false, MaximumLength = 100)]
    string Product { get; set; }

    [DBBidirectionalRelation("OrderItems")]
    [Mandatory]
    IOrder Order { get; set; }
  }
}
