// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.ServiceLocation.TestDomain
{
  public interface ITestCompoundMixedRegistrationTypes
  {
  }

  [ImplementationFor (typeof(ITestCompoundMixedRegistrationTypes), RegistrationType = RegistrationType.Multiple, Position = 1)]
  public class TestCompoundMixedRegistrationImplementation1 : ITestCompoundMixedRegistrationTypes
  {
  }

  [ImplementationFor (typeof(ITestCompoundMixedRegistrationTypes), RegistrationType = RegistrationType.Single, Position = 2)]
  public class TestCompoundMixedRegistrationImplementation2 : ITestCompoundMixedRegistrationTypes
  {
  }

  [ImplementationFor (typeof(ITestCompoundMixedRegistrationTypes), RegistrationType = RegistrationType.Compound)]
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