namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.InterfaceMapping
{
  [TestDomain]
  public interface IInterfaceWithOneImplementor : IDomainObject
  {
    int InterfaceProperty { get; set; }
  }
}
