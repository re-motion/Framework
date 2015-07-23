using System;
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.ServiceLocation.TestDomain
{
  public interface ITestMixedRegistrationTypes
  {
  }

  [ImplementationFor (typeof (ITestMixedRegistrationTypes), RegistrationType = RegistrationType.Single, Position = 1)]
  public class TestMixedRegistrationTypes1
  {
  }

  [ImplementationFor (typeof (ITestMixedRegistrationTypes), RegistrationType = RegistrationType.Multiple, Position = 2)]
  public class TestMixedRegistrationTypes2
  {
  }
}