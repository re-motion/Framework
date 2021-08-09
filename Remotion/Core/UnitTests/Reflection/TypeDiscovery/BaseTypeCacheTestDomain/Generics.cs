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

namespace Remotion.UnitTests.Reflection.TypeDiscovery.BaseTypeCacheTestDomain
{
  public class NonGenericBase
  {
  }

  public class OpenGenericBase<T> : NonGenericBase
  {
  }

  public class OpenGenericDerived<T> : OpenGenericBase<T>
  {
  }

  public class OpenGenericDerivedDerived<T> : OpenGenericDerived<T>
  {
  }

  public class ClosedGenericDerived : OpenGenericDerived<int>
  {
  }

  public class ClosedGenericDerived2 : OpenGenericDerived<string>
  {
  }

  public class ClosedGenericDerivedDerived : OpenGenericDerivedDerived<double>
  {
  }

  public interface IOpenGenericInterface<T>
  {
  }

  public interface IOpenGenericInterface2<T>
  {
  }

  public interface IOpenGenericInterfaceExtended<T> : IOpenGenericInterface<T>
  {
  }

  public class OpenGenericWithInterface<T> : IOpenGenericInterface<T>
  {
  }

  public class OpenGenericDerivedWithInterface<T> : OpenGenericWithInterface<T>
  {
  }

  public class OpenGenericDerivedWithInterfaceExtended<T> : OpenGenericWithInterface<T>, IOpenGenericInterfaceExtended<T>
  {
  }

  public class NonGenericWithMultipleClosedGenericInterfaces : IOpenGenericInterface<int>, IOpenGenericInterface<bool>
  {
  }

  public class ClosedGenericDerivedWithInterface : OpenGenericWithInterface<byte>
  {
  }

  public class OpenGenericWithClosedInterface<T> : IOpenGenericInterface<DateTime>
  {
  }

  public class ClosedGenericDerivedWithClosedInterface<T> : OpenGenericWithClosedInterface<TimeSpan>
  {
  }

  public class OpenGenericWithInterface1AndInterface2<T1, T2> : IOpenGenericInterface<T1>, IOpenGenericInterface2<T2>
  {
  }

  public class ClosedGenericDerivedWithInterface1AndInterface2 : OpenGenericWithInterface1AndInterface2<byte, short>
  {
  }
}