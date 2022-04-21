using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.InterfaceMapping
{
  [DBTable]
  [TestDomain]
  public class OnlyImplementor : DomainObject, IInterfaceWithOneImplementor
  {
    // TODO R2I revert to auto property once the it works again
    public int InterfaceProperty
    {
      get => Properties[typeof(IInterfaceWithOneImplementor), nameof(InterfaceProperty)].GetValue<int>();
      set => Properties[typeof(IInterfaceWithOneImplementor), nameof(InterfaceProperty)].SetValue(value);
    }

    public virtual int OnlyImplementorProperty
    {
      get => Properties[typeof(OnlyImplementor), nameof(OnlyImplementorProperty)].GetValue<int>();
      set => Properties[typeof(OnlyImplementor), nameof(OnlyImplementorProperty)].SetValue(value);
    }
  }
}
