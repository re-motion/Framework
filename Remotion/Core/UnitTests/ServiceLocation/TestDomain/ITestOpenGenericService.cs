using Remotion.ServiceLocation;

namespace Remotion.UnitTests.ServiceLocation.TestDomain
{
  public interface ITestOpenGenericService<T>
  {
     
  }

  [ImplementationFor (typeof (ITestOpenGenericService<int>))]
  public class TestOpenGenericIntImplementation : ITestOpenGenericService<int>
  {
  }

  [ImplementationFor (typeof (ITestOpenGenericService<string>))]
  public class TestOpenGenericStringImplementation : ITestOpenGenericService<string>
  {
  }


}