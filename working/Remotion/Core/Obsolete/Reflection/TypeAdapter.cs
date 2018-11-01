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
using System.ComponentModel;
using System.Reflection;

namespace Remotion.Reflection
{
  [Obsolete ("Dummy declaration for DependDB. Moved to Remotion.Reflection.dll", true)]
  [TypeConverter (typeof (TypeAdapterConverter))]
  internal sealed class TypeAdapter : ITypeInformation
  {
    public static TypeAdapter Create (Type type)
    {
      throw new NotImplementedException();
    }

    private TypeAdapter (Type type)
    {
    }

    public Type Type
    {
      get { throw new NotImplementedException(); }
    }

    public string Name
    {
      get { throw new NotImplementedException(); }
    }

    ITypeInformation ITypeInformation.DeclaringType
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsClass
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsValueType
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsInterface
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsArray
    {
      get { throw new NotImplementedException(); }
    }

    public int GetArrayRank ()
    {
      throw new NotImplementedException();
    }

    public ITypeInformation MakeArrayType (int rank)
    {
      throw new NotImplementedException();
    }

    public ITypeInformation MakeArrayType ()
    {
      throw new NotImplementedException();
    }

    public bool IsEnum
    {
      get { throw new NotImplementedException(); }
    }

    public ITypeInformation GetUnderlyingTypeOfEnum ()
    {
      throw new NotImplementedException();
    }

    public bool IsNullableValueType
    {
      get { throw new NotImplementedException(); }
    }

    public ITypeInformation GetUnderlyingTypeOfNullableValueType ()
    {
      throw new NotImplementedException();
    }

    public bool IsPointer
    {
      get { throw new NotImplementedException(); }
    }

    public ITypeInformation MakePointerType ()
    {
      throw new NotImplementedException();
    }

    public bool IsByRef
    {
      get { throw new NotImplementedException(); }
    }

    public ITypeInformation MakeByRefType ()
    {
      throw new NotImplementedException();
    }

    public bool IsSealed
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsAbstract
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsNested
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsSerializable
    {
      get { throw new NotImplementedException(); }
    }

    public bool HasElementType
    {
      get { throw new NotImplementedException(); }
    }

    public ITypeInformation GetElementType ()
    {
      throw new NotImplementedException();
    }

    public bool IsGenericType
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsGenericTypeDefinition
    {
      get { throw new NotImplementedException(); }
    }

    public ITypeInformation GetGenericTypeDefinition ()
    {
      throw new NotImplementedException();
    }

    public bool ContainsGenericParameters
    {
      get { throw new NotImplementedException(); }
    }

    public ITypeInformation[] GetGenericArguments ()
    {
      throw new NotImplementedException();
    }

    public bool IsGenericParameter
    {
      get { throw new NotImplementedException(); }
    }

    public int GenericParameterPosition
    {
      get { throw new NotImplementedException(); }
    }

    public ITypeInformation[] GetGenericParameterConstraints ()
    {
      throw new NotImplementedException();
    }

    public GenericParameterAttributes GenericParameterAttributes
    {
      get { throw new NotImplementedException(); }
    }

    public ITypeInformation BaseType
    {
      get { throw new NotImplementedException(); }
    }

    public bool IsInstanceOfType (object o)
    {
      throw new NotImplementedException();
    }

    public bool IsSubclassOf (ITypeInformation c)
    {
      throw new NotImplementedException();
    }

    public bool IsAssignableFrom (ITypeInformation c)
    {
      throw new NotImplementedException();
    }

    public bool CanAscribeTo (ITypeInformation c)
    {
      throw new NotImplementedException();
    }

    public ITypeInformation[] GetAscribedGenericArgumentsFor (ITypeInformation c)
    {
      throw new NotImplementedException();
    }

    public string FullName
    {
      get { throw new NotImplementedException(); }
    }

    public string Namespace
    {
      get { throw new NotImplementedException(); }
    }

    public string AssemblyQualifiedName
    {
      get { throw new NotImplementedException(); }
    }

    public Assembly Assembly
    {
      get { throw new NotImplementedException(); }
    }

    ITypeInformation IMemberInformation.DeclaringType
    {
      get { throw new NotImplementedException(); }
    }

    public ITypeInformation GetOriginalDeclaringType ()
    {
      throw new NotImplementedException();
    }

    public T GetCustomAttribute<T> (bool inherited) where T : class
    {
      throw new NotImplementedException();
    }

    public T[] GetCustomAttributes<T> (bool inherited) where T : class
    {
      throw new NotImplementedException();
    }

    public bool IsDefined<T> (bool inherited) where T : class
    {
      throw new NotImplementedException();
    }
  }
}