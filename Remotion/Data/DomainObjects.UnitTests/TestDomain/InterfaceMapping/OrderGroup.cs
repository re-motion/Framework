namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.InterfaceMapping
{
  [DBTable]
  [TestDomain]
  public class OrderGroup : DomainObject, IOrderGroup
  {
    /// <inheritdoc />
    public virtual string GroupName
    {
      get => Properties[typeof(OrderGroup), nameof(GroupName)].GetValue<string>();
      set => Properties[typeof(OrderGroup), nameof(GroupName)].SetValue(value);
    }

    /// <inheritdoc />
    public virtual ObjectList<IOrder> Orders
    {
      get => Properties[typeof(IOrderGroup), nameof(IOrderGroup.Orders)].GetValue<ObjectList<IOrder>>();
      set => Properties[typeof(IOrderGroup), nameof(IOrderGroup.Orders)].SetValue(value);
    }
  }
}
