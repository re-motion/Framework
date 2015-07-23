using System;
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.ServiceLocation.TestDomain
{
  public interface ITestRegistrationTypeMultiple
  {
  }

  [ImplementationFor (typeof (ITestRegistrationTypeMultiple), Position = 1, RegistrationType = RegistrationType.Multiple)]
  public class TestRegistrationTypeMultiple1 : ITestRegistrationTypeMultiple
  {
  }

  [ImplementationFor (typeof (ITestRegistrationTypeMultiple), Position = 2, RegistrationType = RegistrationType.Multiple)]
  public class TestRegistrationTypeMultiple2 : ITestRegistrationTypeMultiple
  {
  }
}