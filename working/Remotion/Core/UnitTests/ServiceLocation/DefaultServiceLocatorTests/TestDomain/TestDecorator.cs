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
  public class TestDecorator1 : ITestType
  {
    public ITestType DecoratedObject { get; private set; }

    public TestDecorator1 (ITestType decoratedObject)
    {
      DecoratedObject = decoratedObject;
    }
  }

  public class TestDecorator2 : ITestType
  {
    public ITestType DecoratedObject { get; private set; }

    public TestDecorator2 (ITestType decoratedObject)
    {
      DecoratedObject = decoratedObject;
    }
  }

  public class TestDecoratorWithAdditionalConstructorParameters : ITestType
  {
    public SingleService SingleService { get; private set; }
    public MultipleService[] MultipleService { get; private set; }

    public ITestType DecoratedObject { get; private set; }

    public TestDecoratorWithAdditionalConstructorParameters (
        SingleService singleService,
        ITestType decoratedObject,
        IEnumerable<MultipleService> stubService2)
    {
      SingleService = singleService;
      MultipleService = stubService2.ToArray();
      DecoratedObject = decoratedObject;
    }
  }
}