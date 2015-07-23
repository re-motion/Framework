using System;
using System.Collections.Generic;
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.ServiceLocation.TestDomain
{
  public interface ITestCompoundMixedRegistrationTypes
  {
  }

  [ImplementationFor (typeof (ITestCompoundMixedRegistrationTypes), RegistrationType = RegistrationType.Multiple, Position = 1)]
  public class TestCompoundMixedRegistrationImplementation1 : ITestCompoundMixedRegistrationTypes
  {
  }

  [ImplementationFor (typeof (ITestCompoundMixedRegistrationTypes), RegistrationType = RegistrationType.Single, Position = 2)]
  public class TestCompoundMixedRegistrationImplementation2 : ITestCompoundMixedRegistrationTypes
  {
  }

  [ImplementationFor (typeof (ITestCompoundMixedRegistrationTypes), RegistrationType = RegistrationType.Compound)]
  public class TestCompoundMixedRegistrationTypes : ITestCompoundMixedRegistrationTypes
  {
    private readonly IEnumerable<ITestCompoundRegistration> _compoundRegistrations;

    public TestCompoundMixedRegistrationTypes (IEnumerable<ITestCompoundRegistration> compoundRegistrations)
    {
      _compoundRegistrations = compoundRegistrations;
    }

    public IEnumerable<ITestCompoundRegistration> CompoundRegistrations
    {
      get { return _compoundRegistrations; }
    }
  }
}