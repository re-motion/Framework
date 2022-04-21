namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.InterfaceMapping
{
  [DBTable]
  [TestDomain]
  public class SimpleOrder : DomainObject, IOrder
  {
    /// <inheritdoc />
    public virtual int OrderNumber
    {
      get => Properties[typeof(IOrder), nameof(IOrder.OrderNumber)].GetValue<int>();
      set => Properties[typeof(IOrder), nameof(IOrder.OrderNumber)].SetValue(value);
    }

    /// <inheritdoc />
    public virtual IObjectList<IOrderItem> OrderItems
    {
      get => Properties[typeof(IOrder), nameof(IOrder.OrderItems)].GetValue<IObjectList<IOrderItem>>();
      set => Properties[typeof(IOrder), nameof(IOrder.OrderItems)].SetValue(value);
    }

    public string SimpleOrderName
    {
      get => Properties[typeof(SimpleOrder), nameof(SimpleOrderName)].GetValue<string>();
      set => Properties[typeof(SimpleOrder), nameof(SimpleOrderName)].SetValue(value);
    }
  }
}
