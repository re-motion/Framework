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

namespace Remotion.Validation.UnitTests.Implementation.TestDomain
{
  public interface IBaseInterface1 : IBaseBaseInterface1
  {
  }

  public interface IBaseInterface2
  {
  }

  public interface IBaseInterface3
  {

  }

  public interface IBaseBaseInterface1
  {
  }

  public interface ITypeWithSeveralInterfacesImplementingInterface3 : IBaseInterface2, IBaseInterface3
  {
  }

  public interface ITypeWithSeveralInterfacesImplementingInterface2
  {
  }

  public interface ITypeWithSeveralInterfacesImplementingInterface1 : IBaseInterface1
  {
  }

  public class TypeWithSeveralInterfacesImplementingInterface : ITypeWithSeveralInterfacesImplementingInterface1, ITypeWithSeveralInterfacesImplementingInterface2, ITypeWithSeveralInterfacesImplementingInterface3
  {
  }
}