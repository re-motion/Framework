namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.InterfaceMapping
{
  [DBTable]
  [TestDomain]
  public class ComplexOrder : DomainObject, IOrder
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

    public string ComplexOrderName
    {
      get => Properties[typeof(ComplexOrder), nameof(ComplexOrderName)].GetValue<string>();
      set => Properties[typeof(ComplexOrder), nameof(ComplexOrderName)].SetValue(value);
    }
  }
}
