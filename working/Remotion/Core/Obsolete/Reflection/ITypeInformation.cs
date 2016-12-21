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
using System.Reflection;

namespace Remotion.Reflection
{
  [Obsolete ("Dummy declaration for DependDB. Moved to Remotion.Reflection.dll", true)]
  internal interface ITypeInformation : IMemberInformation
  {
    string FullName { get; }

    string Namespace { get; }

    string AssemblyQualifiedName { get; }

    Assembly Assembly { get; }

    new ITypeInformation DeclaringType { get; }

    bool IsClass { get; }

    bool IsValueType { get; }

    bool IsInterface { get; }

    bool IsArray { get; }

    int GetArrayRank ();

    ITypeInformation MakeArrayType (int rank);

    ITypeInformation MakeArrayType ();

    bool IsEnum { get; }

    ITypeInformation GetUnderlyingTypeOfEnum ();

    bool IsNullableValueType { get; }

    ITypeInformation GetUnderlyingTypeOfNullableValueType ();

    bool IsPointer { get; }

    ITypeInformation MakePointerType ();

    bool IsByRef { get; }

    ITypeInformation MakeByRefType ();

    bool IsSealed { get; }

    bool IsAbstract { get; }

    bool IsNested { get; }

    bool IsSerializable { get; }

    bool HasElementType { get; }

    ITypeInformation GetElementType ();

    bool IsGenericType { get; }

    bool IsGenericTypeDefinition { get; }

    ITypeInformation GetGenericTypeDefinition ();

    bool ContainsGenericParameters { get; }

    ITypeInformation[] GetGenericArguments ();

    bool IsGenericParameter { get; }

    int GenericParameterPosition { get; }

    ITypeInformation[] GetGenericParameterConstraints ();

    GenericParameterAttributes GenericParameterAttributes { get; }

    ITypeInformation BaseType { get; }

    bool IsInstanceOfType (object o);

    bool IsSubclassOf (ITypeInformation c);

    bool IsAssignableFrom (ITypeInformation c);

    bool CanAscribeTo (ITypeInformation c);

    ITypeInformation[] GetAscribedGenericArgumentsFor (ITypeInformation c);
  }
}