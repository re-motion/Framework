namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.InterfaceMapping
{
  [TestDomain]
  public interface IOrderGroup : IDomainObject
  {
    string GroupName { get; set; }

    [Mandatory]
    [DBBidirectionalRelation("Group", SortExpression = "OrderNumber ASC")]
    ObjectList<IOrder> Orders { get; set; }
  }
}
