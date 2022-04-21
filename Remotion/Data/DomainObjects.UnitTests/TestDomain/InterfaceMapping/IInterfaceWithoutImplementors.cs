namespace Remotion.Data.DomainObjects.UnitTests.TestDomain.InterfaceMapping
{
  [TestDomain]
  public interface IInterfaceWithoutImplementors : IDomainObject
  {
    int InterfaceProperty { get; set; }
  }
}
