using System;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.InterfaceMapping
{
  [DBTable]
  [TestDomain]
  public class Implementor2 : DomainObject, IInterfaceWithMultipleImplementors
  {
    public virtual int InterfaceProperty
    {
      get => Properties[typeof(IInterfaceWithMultipleImplementors), nameof(InterfaceProperty)].GetValue<int>();
      set => Properties[typeof(IInterfaceWithMultipleImplementors), nameof(InterfaceProperty)].SetValue(value);
    }

    public virtual int OnlyImplementor2Property { get; set; }
  }
}
