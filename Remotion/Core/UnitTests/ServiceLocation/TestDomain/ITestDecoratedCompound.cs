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
  public interface IITestDecoratedCompound
  {
  }

  [ImplementationFor (typeof (IITestDecoratedCompound), RegistrationType = RegistrationType.Multiple, Position = 1)]
  public class ITestDecoratedCompoundObject1 : IITestDecoratedCompound
  {
  }

  [ImplementationFor (typeof (IITestDecoratedCompound), RegistrationType = RegistrationType.Multiple, Position = 2)]
  public class ITestDecoratedCompoundObject2 : IITestDecoratedCompound
  {
  }

  [ImplementationFor (typeof (IITestDecoratedCompound), RegistrationType = RegistrationType.Decorator)]
  public class ITestDecoratedCompoundDecorator : IITestDecoratedCompound
  {
    private readonly IITestDecoratedCompound _decoratedObject;

    public ITestDecoratedCompoundDecorator (IITestDecoratedCompound decoratedObject)
    {
      _decoratedObject = decoratedObject;
    }

    public IITestDecoratedCompound DecoratedObject
    {
      get { return _decoratedObject; }
    }
  }

  [ImplementationFor (typeof (IITestDecoratedCompound), RegistrationType = RegistrationType.Compound)]
  public class ITestDecoratedCompoundCompound : IITestDecoratedCompound
  {
    private readonly IEnumerable<IITestDecoratedCompound> _innerObjects;

    public ITestDecoratedCompoundCompound (IEnumerable<IITestDecoratedCompound> innerObjects)
    {
      _innerObjects = innerObjects;
    }

    public IEnumerable<IITestDecoratedCompound> InnerObjects
    {
      get { return _innerObjects; }
    }
  }
}