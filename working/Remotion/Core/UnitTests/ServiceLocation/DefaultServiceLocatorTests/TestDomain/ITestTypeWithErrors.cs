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
using Remotion.Development.UnitTesting.Enumerables;
using Remotion.UnitTests.ServiceLocation.TestDomain;

namespace Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain
{
  public interface ITestTypeWithErrors
  {
  }

  public class TestTypeWithTooManyPublicConstructors : ITestTypeWithErrors
  {
    public TestTypeWithTooManyPublicConstructors ()
    {
    }

    public TestTypeWithTooManyPublicConstructors (StubService param)
    {
    }
  }

  public class TestTypeWithOnlyNonPublicConstructor : ITestTypeWithErrors
  {
    protected TestTypeWithOnlyNonPublicConstructor ()
    {
    }
  }
  
  public class TestTypeWithConstructorThrowingSingleDependency : ITestType
  {
    public TestTypeWithConstructorThrowingSingleDependency (ITestTypeWithErrors param)
    {
    }
  }

  public class TestTypeWithConstructorThrowingMultipleDependency : ITestType
  {
    public TestTypeWithConstructorThrowingMultipleDependency (IEnumerable<ITestTypeWithErrors> param)
    {
      param.ForceEnumeration();
    }
  }

  public class TestTypeWithConstructorThrowingException : ITestTypeWithErrors
  {
    public TestTypeWithConstructorThrowingException ()
    {
      throw new ApplicationException ("This exception comes from the ctor.");
    }
  }
}