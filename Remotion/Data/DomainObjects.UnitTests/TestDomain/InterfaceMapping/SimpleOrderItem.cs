namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.InterfaceMapping
{
  [DBTable]
  [TestDomain]
  public class SimpleOrderItem : DomainObject, IOrderItem
  {
    /// <inheritdoc />
    public int Position
    {
      get => Properties[typeof(IOrderItem), nameof(IOrderItem.Position)].GetValue<int>();
      set => Properties[typeof(IOrderItem), nameof(IOrderItem.Position)].SetValue(value);
    }

    /// <inheritdoc />
    public string Product
    {
      get => Properties[typeof(IOrderItem), nameof(IOrderItem.Product)].GetValue<string>();
      set => Properties[typeof(IOrderItem), nameof(IOrderItem.Product)].SetValue(value);
    }

    /// <inheritdoc />
    public virtual IOrder Order
    {
      get => Properties[typeof(IOrderItem), nameof(IOrderItem.Order)].GetValue<IOrder>();
      set => Properties[typeof(IOrderItem), nameof(IOrderItem.Order)].SetValue(value);
    }

    public string SimpleOrderItemName
    {
      get => Properties[typeof(SimpleOrderItem), nameof(SimpleOrderItemName)].GetValue<string>();
      set => Properties[typeof(SimpleOrderItem), nameof(SimpleOrderItemName)].SetValue(value);
    }
  }
}
