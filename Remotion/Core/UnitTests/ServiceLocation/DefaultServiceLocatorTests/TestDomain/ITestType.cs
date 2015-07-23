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
using System.Linq;

namespace Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain
{
  public interface ITestType
  {
  }

  public class TestImplementation1 : ITestType
  {
  }

  public class TestImplementation2 : ITestType
  {
  }

  public class TestImplementationWithOneConstructorParameter : ITestType
  {
    public InstanceService InstanceService { get; private set; }

    public TestImplementationWithOneConstructorParameter (InstanceService instanceService)
    {
      InstanceService = instanceService;
    }
  }

  public class TestImplementationWithMultipleConstructorParameters : ITestType
  {
    public InstanceService InstanceService1 { get; private set; }
    public InstanceService InstanceService2 { get; private set; }
    public SingletonService SingletonService { get; private set; }
    public MultipleService[] MultipleService { get; private set; }

    public TestImplementationWithMultipleConstructorParameters (
        InstanceService instanceService1,
        InstanceService instanceService2,
        SingletonService singletonService,
        IEnumerable<MultipleService> multipleService)
    {
      InstanceService1 = instanceService1;
      InstanceService2 = instanceService2;
      SingletonService = singletonService;
      MultipleService = multipleService.ToArray();
    }
  }

  public class TestImplementationWithRecursiceConstructorParameter : ITestType
  {
    public ParameterizedService ParameterizedService { get; private set; }

    public TestImplementationWithRecursiceConstructorParameter (ParameterizedService parameterizedService)
    {
      ParameterizedService = parameterizedService;
    }
  }
}