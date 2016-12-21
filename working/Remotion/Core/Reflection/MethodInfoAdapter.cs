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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Remotion.Collections;
using Remotion.FunctionalProgramming;
using Remotion.Utilities;

namespace Remotion.Reflection
{
  /// <summary>
  /// Implements the <see cref="IMethodInformation"/> to wrap a <see cref="MethodInfo"/> instance.
  /// </summary>
  [TypeConverter (typeof (MethodInfoAdapterConverter))]
  public sealed class MethodInfoAdapter : IMethodInformation
  {
    //If this is changed to an (expiring) cache, equals implementation must be updated.
    private static readonly IDataStore<MethodInfo, MethodInfoAdapter> s_dataStore =
        DataStoreFactory.CreateWithLocking<MethodInfo, MethodInfoAdapter> (MemberInfoEqualityComparer<MethodInfo>.Instance);

    private static readonly Func<MethodInfo, MethodInfoAdapter> s_ctorFunc = mi => new MethodInfoAdapter (mi); 

    public static MethodInfoAdapter Create (MethodInfo methodInfo)
    {
      ArgumentUtility.CheckNotNull ("methodInfo", methodInfo);
      return s_dataStore.GetOrCreateValue (methodInfo, s_ctorFunc);
    }

    private readonly MethodInfo _methodInfo;
    private readonly Lazy<ITypeInformation> _cachedDeclaringType;
    private readonly Lazy<ITypeInformation> _cachedOriginalDeclaringType;

    private readonly Lazy<IPropertyInformation> _declaringProperty;
    private readonly Lazy<IReadOnlyCollection<IMethodInformation>> _interfaceDeclarations;

    private MethodInfoAdapter (MethodInfo methodInfo)
    {
      _methodInfo = methodInfo;

      _cachedDeclaringType = new Lazy<ITypeInformation> (
          () => Maybe.ForValue (_methodInfo.DeclaringType).Select (TypeAdapter.Create).ValueOrDefault(),
          LazyThreadSafetyMode.ExecutionAndPublication);

      _cachedOriginalDeclaringType = new Lazy<ITypeInformation> (
          () => TypeAdapter.Create (_methodInfo.GetOriginalDeclaringType()),
          LazyThreadSafetyMode.ExecutionAndPublication);

      _declaringProperty = new Lazy<IPropertyInformation> (
          () => Maybe.ForValue ( _methodInfo.FindDeclaringProperty()).Select (PropertyInfoAdapter.Create).ValueOrDefault(),
          LazyThreadSafetyMode.ExecutionAndPublication);

      _interfaceDeclarations = new Lazy<IReadOnlyCollection<IMethodInformation>> (
          FindInterfaceDeclarationsImplementation,
          LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public MethodInfo MethodInfo
    {
      get { return _methodInfo; }
    }

    public Type ReturnType
    {
      get { return _methodInfo.ReturnType; }
    }

    public string Name
    {
      get { return _methodInfo.Name; }
    }

    public ITypeInformation DeclaringType
    {
      get { return _cachedDeclaringType.Value; }
    }

    public ITypeInformation GetOriginalDeclaringType ()
    {
      return _cachedOriginalDeclaringType.Value;
    }

    public T GetCustomAttribute<T> (bool inherited) where T: class
    {
      return AttributeUtility.GetCustomAttribute<T> (_methodInfo, inherited);
    }

    public T[] GetCustomAttributes<T> (bool inherited) where T: class
    {
      return AttributeUtility.GetCustomAttributes<T> (_methodInfo, inherited);
    }

    public object Invoke (object instance, object[] parameters)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);

      return _methodInfo.Invoke (instance, parameters);
    }

    public IMethodInformation FindInterfaceImplementation (Type implementationType)
    {
      ArgumentUtility.CheckNotNull ("implementationType", implementationType);

      if (!_methodInfo.DeclaringType.IsInterface)
        throw new InvalidOperationException ("This method is not an interface method.");

      if (implementationType.IsInterface)
        throw new ArgumentException ("The implementationType parameter must not be an interface.", "implementationType");

      if (!_methodInfo.DeclaringType.IsAssignableFrom (implementationType))
        return null;

      var interfaceMap = implementationType.GetInterfaceMap (_methodInfo.DeclaringType);
      var methodIndex = interfaceMap.InterfaceMethods
          .Select ((m, i) => new { InterfaceMethod = m, Index = i })
          .First (tuple => tuple.InterfaceMethod.Equals (_methodInfo)) // actually Single, but we can stop at the first matching method
          .Index;
      return Create(interfaceMap.TargetMethods[methodIndex]);
    }

    public IEnumerable<IMethodInformation> FindInterfaceDeclarations ()
    {
      return _interfaceDeclarations.Value.ToArray();
    }

    private IReadOnlyCollection<IMethodInformation> FindInterfaceDeclarationsImplementation ()
    {
      if (_methodInfo.DeclaringType.IsInterface)
        throw new InvalidOperationException ("This method is itself an interface member, so it cannot have an interface declaration.");

      return (from interfaceType in _methodInfo.DeclaringType.GetInterfaces()
        let map = _methodInfo.DeclaringType.GetInterfaceMap (interfaceType)
        from index in Enumerable.Range (0, map.TargetMethods.Length)
        where MemberInfoEqualityComparer<MethodInfo>.Instance.Equals (map.TargetMethods[index], _methodInfo)
        select (IMethodInformation) Create (map.InterfaceMethods[index])).ToArray();
    }

    public T GetFastInvoker<T> () where T: class
    {
      return (T) (object) GetFastInvoker (typeof (T));
    }

    public Delegate GetFastInvoker (Type delegateType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("delegateType", delegateType, typeof (Delegate));

      return DynamicMethodBasedMethodCallerFactory.CreateMethodCallerDelegate (_methodInfo, delegateType);
    }

    public IPropertyInformation FindDeclaringProperty ()
    {
      return _declaringProperty.Value;
    }

    public ParameterInfo[] GetParameters ()
    {
      return _methodInfo.GetParameters();
    }

    public IMethodInformation GetOriginalDeclaration ()
    {
      return Create (_methodInfo.GetBaseDefinition());
    }

    public bool IsDefined<T> (bool inherited) where T: class
    {
      return AttributeUtility.IsDefined<T> (_methodInfo, inherited);
    }

    public override bool Equals (object obj)
    {
      return ReferenceEquals (this, obj);
    }

    public override int GetHashCode ()
    {
      return RuntimeHelpers.GetHashCode (this);
    }

    public override string ToString ()
    {
      return _methodInfo.ToString ();
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}