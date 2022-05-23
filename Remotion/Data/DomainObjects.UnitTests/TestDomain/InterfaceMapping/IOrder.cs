namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.InterfaceMapping
{
  [TestDomain]
  public interface IOrder : IDomainObject
  {
    int OrderNumber { get; set; }

    [DBBidirectionalRelation("Orders")]
    IOrderGroup Group { get; set; }

    [DBBidirectionalRelation("Order", SortExpression = "Position ASC")]
    IObjectList<IOrderItem> OrderItems { get; set; }
  }
}
