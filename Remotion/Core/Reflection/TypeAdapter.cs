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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Reflection
{
  /// <summary>
  /// Implements the <see cref="ITypeInformation"/> to wrap a <see cref="System.Type"/> instance.
  /// </summary>
  [TypeConverter(typeof(TypeAdapterConverter))]
  public sealed class TypeAdapter : ITypeInformation
  {
    private static readonly ConcurrentDictionary<Type, TypeAdapter> s_dataStore =
        new ConcurrentDictionary<Type, TypeAdapter>(ReferenceEqualityComparer<Type>.Instance);

    ///<remarks>Optimized for memory allocations</remarks>
    private static readonly Func<Type, TypeAdapter> s_ctorFunc = t => new TypeAdapter(t);

    public static TypeAdapter Create (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);

      return s_dataStore.GetOrAdd(type, s_ctorFunc);
    }

    [ContractAnnotation("null => null; notnull => notnull")]
    [return: NotNullIfNotNull("type")]
    public static TypeAdapter? CreateOrNull (Type? type)
    {
      if (type == null)
        return null;

      return s_dataStore.GetOrAdd(type, s_ctorFunc);
    }

    private readonly Type _type;
    private readonly Lazy<ITypeInformation?> _cachedDeclaringType;

    private TypeAdapter (Type type)
    {
      _type = type;

      _cachedDeclaringType = new Lazy<ITypeInformation?>(
          () => TypeAdapter.CreateOrNull(_type.DeclaringType),
          LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public Type Type
    {
      get { return _type; }
    }

    public string Name
    {
      get { return _type.Name; }
    }

    public string? FullName
    {
      get { return _type.FullName; }
    }

    public string? Namespace
    {
      get { return _type.Namespace; }
    }

    public string? AssemblyQualifiedName
    {
      get { return _type.AssemblyQualifiedName; }
    }

    public Assembly Assembly
    {
      get { return _type.Assembly; }
    }

    public ITypeInformation? DeclaringType
    {
      get { return _cachedDeclaringType.Value; }
    }

    public ITypeInformation? GetOriginalDeclaringType ()
    {
      return _cachedDeclaringType.Value;
    }

    public bool IsClass
    {
      get { return _type.IsClass; }
    }

    public bool IsValueType
    {
      get { return _type.IsValueType; }
    }

    public bool IsInterface
    {
      get { return _type.IsInterface; }
    }

    public bool IsArray
    {
      get { return _type.IsArray; }
    }

    public int GetArrayRank ()
    {
      return _type.GetArrayRank();
    }

    public ITypeInformation MakeArrayType (int rank)
    {
      return TypeAdapter.Create(_type.MakeArrayType(rank));
    }

    public ITypeInformation MakeArrayType ()
    {
      return TypeAdapter.Create(_type.MakeArrayType());
    }

    public bool IsEnum
    {
      get { return _type.IsEnum; }
    }

    public ITypeInformation GetUnderlyingTypeOfEnum ()
    {
      if (!_type.IsEnum)
        throw new InvalidOperationException(string.Format("The type '{0}' is not an enum type.", _type.GetFullNameSafe()));
      return TypeAdapter.Create(Enum.GetUnderlyingType(_type));
    }

    public bool IsNullableValueType
    {
      get { return Nullable.GetUnderlyingType(_type) != null; }
    }

    public ITypeInformation GetUnderlyingTypeOfNullableValueType ()
    {
      var underlyingType = Nullable.GetUnderlyingType(_type);
      if (underlyingType == null)
        throw new InvalidOperationException(string.Format("The type '{0}' is not a nullable value type.", _type.GetFullNameSafe()));
      return TypeAdapter.Create(underlyingType);
    }

    public bool IsPointer
    {
      get { return _type.IsPointer; }
    }

    public ITypeInformation MakePointerType ()
    {
      return TypeAdapter.Create(_type.MakePointerType());
    }

    public bool IsByRef
    {
      get { return _type.IsByRef; }
    }

    public ITypeInformation MakeByRefType ()
    {
      return TypeAdapter.Create(_type.MakeByRefType());
    }

    public bool IsSealed
    {
      get { return _type.IsSealed; }
    }

    public bool IsAbstract
    {
      get { return _type.IsAbstract; }
    }

    public bool IsNested
    {
      get { return _type.IsNested; }
    }

    public bool HasElementType
    {
      get { return _type.HasElementType; }
    }

    public ITypeInformation? GetElementType ()
    {
      return TypeAdapter.CreateOrNull(_type.GetElementType());
    }

    public bool IsGenericType
    {
      get { return _type.IsGenericType; }
    }

    public bool IsGenericTypeDefinition
    {
      get { return _type.IsGenericTypeDefinition; }
    }

    public ITypeInformation GetGenericTypeDefinition ()
    {
      return TypeAdapter.Create(_type.GetGenericTypeDefinition());
    }

    public bool ContainsGenericParameters
    {
      get { return _type.ContainsGenericParameters; }
    }

    public ITypeInformation[] GetGenericArguments ()
    {
      return ConvertToTypeAdapters(_type.GetGenericArguments());
    }

    public bool IsGenericParameter
    {
      get { return _type.IsGenericParameter; }
    }

    public int GenericParameterPosition
    {
      get { return _type.GenericParameterPosition; }
    }

    public ITypeInformation[] GetGenericParameterConstraints ()
    {
      return ConvertToTypeAdapters(_type.GetGenericParameterConstraints());
    }

    public GenericParameterAttributes GenericParameterAttributes
    {
      get { return _type.GenericParameterAttributes; }
    }


    public T? GetCustomAttribute<T> (bool inherited) where T : class
    {
      return AttributeUtility.GetCustomAttribute<T>(_type, inherited);
    }

    public T[] GetCustomAttributes<T> (bool inherited) where T : class
    {
      return AttributeUtility.GetCustomAttributes<T>(_type, inherited);
    }

    public bool IsDefined<T> (bool inherited) where T : class
    {
      return AttributeUtility.IsDefined<T>(_type, inherited);
    }

    public ITypeInformation? BaseType
    {
      get { return TypeAdapter.CreateOrNull(_type.BaseType); }
    }

    public bool IsInstanceOfType (object? o)
    {
      return _type.IsInstanceOfType(o);
    }

    public bool IsSubclassOf (ITypeInformation c)
    {
      ArgumentUtility.CheckNotNull("c", c);

      var otherTypeAsTypeAdapter = c as TypeAdapter;
      if (otherTypeAsTypeAdapter == null)
        return false;

      return _type.IsSubclassOf(otherTypeAsTypeAdapter.Type);
    }

    public bool IsAssignableFrom (ITypeInformation? c)
    {
      var otherTypeAsTypeAdapter = c as TypeAdapter;
      if (otherTypeAsTypeAdapter == null)
        return false;
      return _type.IsAssignableFrom(otherTypeAsTypeAdapter.Type);
    }

    public bool CanAscribeTo (ITypeInformation c)
    {
      ArgumentUtility.CheckNotNull("c", c);

      var otherTypeAsTypeAdapter = c as TypeAdapter;
      if (otherTypeAsTypeAdapter == null)
        return false;

      return _type.CanAscribeTo(otherTypeAsTypeAdapter.Type);
    }

    public ITypeInformation[] GetAscribedGenericArgumentsFor (ITypeInformation c)
    {
      var otherTypeAsTypeAdapter = ArgumentUtility.CheckNotNullAndType<TypeAdapter>("c", c);

      return ConvertToTypeAdapters(_type.GetAscribedGenericArguments(otherTypeAsTypeAdapter.Type));
    }

    public override bool Equals (object? obj)
    {
      return ReferenceEquals(this, obj);
    }

    public override int GetHashCode ()
    {
      return RuntimeHelpers.GetHashCode(this);
    }

    public override string ToString ()
    {
      return _type.ToString();
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }

    private ITypeInformation[] ConvertToTypeAdapters (IEnumerable<Type> types)
    {
      return types.Select(t => (ITypeInformation)TypeAdapter.Create(t)).ToArray();
    }

    /*
     // use this.GetProperties {name.equals(value.name), returntype.equals(value.returntype)}
     public static PropertyInfo GetProperty (this IPropertyInformation
        string name, BindingFlags bindingAttr, Type returnType, Type[] types);
    public abstract PropertyInfo[] GetProperties (BindingFlags bindingAttr);
         // use this.GetProperties {name.equals(value.name), returntype.equals(value.returntype)}
     public static MethodInfo GetMethod (this IPropertyInformation
        string name, BindingFlags bindingAttr, Type returnType, Type[] types);
    public abstract MethodInfo[] GetMethods (BindingFlags bindingAttr);

    public abstract Type[] GetInterfaces ();
    public virtual InterfaceMapping GetInterfaceMap (Type interfaceType);

     */
  }
}
