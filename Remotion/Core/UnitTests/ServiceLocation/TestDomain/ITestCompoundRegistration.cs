using System;
using System.Collections.Generic;
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.ServiceLocation.TestDomain
{
  public interface ITestCompoundRegistration
  {
  }

  [ImplementationFor (typeof (ITestCompoundRegistration), RegistrationType = RegistrationType.Multiple, Position = 1)]
  public class TestCompoundImplementation1 : ITestCompoundRegistration
  {
  }

  [ImplementationFor (typeof (ITestCompoundRegistration), RegistrationType = RegistrationType.Multiple, Position = 2)]
  public class TestCompoundImplementation2 : ITestCompoundRegistration
  {
  }

  [ImplementationFor (typeof (ITestCompoundRegistration), RegistrationType = RegistrationType.Compound)]
  public class TestCompoundRegistration : ITestCompoundRegistration
  {
    private readonly IEnumerable<ITestCompoundRegistration> _compoundRegistrations;

    public TestCompoundRegistration (IEnumerable<ITestCompoundRegistration> compoundRegistrations)
    {
      _compoundRegistrations = compoundRegistrations;
    }

    public IEnumerable<ITestCompoundRegistration> CompoundRegistrations
    {
      get { return _compoundRegistrations; }
    }
  }
  
  [ImplementationFor (typeof (ITestCompoundRegistration), RegistrationType = RegistrationType.Compound, Position = 2)]
  public class TestCompoundRegistration2 : ITestCompoundRegistration
  {
    private readonly IEnumerable<ITestCompoundRegistration> _compoundRegistrations;

    public TestCompoundRegistration2 (IEnumerable<ITestCompoundRegistration> compoundRegistrations)
    {
      _compoundRegistrations = compoundRegistrations;
    }

    public IEnumerable<ITestCompoundRegistration> CompoundRegistrations
    {
      get { return _compoundRegistrations; }
    }
  }
}