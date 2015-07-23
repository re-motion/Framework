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

namespace Remotion.UnitTests.Reflection.TypeExtensionsTests
{
  public class ParameterType
  {
  }

  public interface IBaseInterface
  {
  }

  public interface IDerivedInterface : IBaseInterface
  {
  }

  public interface IGenericInterface<T> : IBaseInterface
  {
  }

  public interface IGenericInterface<T1, T2> : IBaseInterface
  {
  }

  public interface IDerivedGenericInterface : IGenericInterface<ParameterType>
  {
  }

  public interface IDoubleDerivedGenericInterface : IDerivedGenericInterface
  {
  }

  // inherits IGenericInterface<> twice
  public interface IDoubleInheritingGenericInterface : IGenericInterface<int>, IGenericInterface<ParameterType>
  {
  }

  public interface IGenericDerivedGenericInterface<T> : IGenericInterface<ParameterType>
      where T: struct
  {
  }

  public interface IDerivedGenericInterface<T> : IGenericInterface<T>
      where T: ParameterType
  {
  }

  public interface IDerivedOpenGenericInterface<T> : IGenericInterface<ParameterType, T>
  {
  }

  public class BaseType
  {
  }

  public class DerivedType : BaseType, IDerivedInterface
  {
  }

  public class GenericType<T> : BaseType
  {
  }

  public class GenericType<T1, T2> : BaseType
  {
  }

  public class DerivedGenericType : GenericType<ParameterType>
  {
  }

  public class GenericDerivedGenericType<T> : GenericType<ParameterType>
      where T: struct
  {
  }

  public class DerivedGenericType<T> : GenericType<T>
      where T: ParameterType
  {
  }

  public class DerivedOpenGenericType<T> : GenericType<ParameterType, T>
  {
  }

  public class TypeWithBaseInterface : IBaseInterface
  {
  }

  public class TypeWithGenericInterface<T> : TypeWithBaseInterface, IGenericInterface<T>
  {
  }

  public class TypeWithGenericInterface<T1, T2> : TypeWithBaseInterface, IGenericInterface<T1, T2>
  {
  }

  public class DerivedTypeWithGenericInterface<T> : TypeWithGenericInterface<T>
  {
  }

  public class TypeWithDerivedGenericInterface : TypeWithGenericInterface<ParameterType>
  {
  }

  public class TypeWithGenericDerivedGenericInterface<T> : BaseType, IGenericDerivedGenericInterface<T>
      where T: struct
  {
  }

  public class TypeWithDerivedGenericInterface<T> : TypeWithBaseInterface, IDerivedGenericInterface<T>
      where T: ParameterType
  {
  }

  public class TypeWithDerivedOpenGenericInterface<T> : TypeWithBaseInterface, IDerivedOpenGenericInterface<T>
  {
  }
}
