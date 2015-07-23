using System;
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.ServiceLocation.TestDomain
{
  public interface ITestRegistrationTypeSingle
  {
  }

  [ImplementationFor (typeof (ITestRegistrationTypeSingle), Position = 1, RegistrationType = RegistrationType.Single)]
  public class TestRegistrationTypeSingle1 : ITestRegistrationTypeSingle
  {
  }

  [ImplementationFor (typeof (ITestRegistrationTypeSingle), Position = 2, RegistrationType = RegistrationType.Single)]
  public class TestRegistrationTypeSingle2 : ITestRegistrationTypeSingle
  {
  }
}