namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.InterfaceMapping
{
  [TestDomain]
  public interface IOrder : IDomainObject
  {
    int OrderNumber { get; set; }

    [Mandatory]
    [DBBidirectionalRelation("Order", SortExpression = "Position ASC")]
    IObjectList<IOrderItem> OrderItems { get; set; }
  }
}
