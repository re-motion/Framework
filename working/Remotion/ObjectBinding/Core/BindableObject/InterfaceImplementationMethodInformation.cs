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
using System.Reflection;
using Remotion.FunctionalProgramming;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// Represents a method that implements a method declared by an interface. <see cref="Invoke"/> and <see cref="GetFastInvoker"/> call the method
  /// via the interface.
  /// </summary>
  public sealed class InterfaceImplementationMethodInformation : IMethodInformation
  {
    private readonly IMethodInformation _implementationMethodInfo;
    private readonly IMethodInformation _declarationMethodInfo;

    public InterfaceImplementationMethodInformation (IMethodInformation implementationMethodInfo, IMethodInformation declarationMethodInfo)
    {
      ArgumentUtility.CheckNotNull ("implementationMethodInfo", implementationMethodInfo);
      ArgumentUtility.CheckNotNull ("declarationMethodInfo", declarationMethodInfo);

      _implementationMethodInfo = implementationMethodInfo;
      _declarationMethodInfo = declarationMethodInfo;
    }

    public string Name
    {
      get { return _implementationMethodInfo.Name; }
    }

    public ITypeInformation DeclaringType
    {
      get { return _implementationMethodInfo.DeclaringType; }
    }

    public ITypeInformation GetOriginalDeclaringType ()
    {
      return _implementationMethodInfo.GetOriginalDeclaringType();
    }

    public T GetCustomAttribute<T> (bool inherited) where T: class
    {
      return _implementationMethodInfo.GetCustomAttribute<T>(inherited);
    }

    public T[] GetCustomAttributes<T> (bool inherited) where T: class
    {
      return _implementationMethodInfo.GetCustomAttributes<T> (inherited);
    }

    public bool IsDefined<T> (bool inherited) where T: class
    {
      return _implementationMethodInfo.IsDefined<T> (inherited);
    }

    public IMethodInformation FindInterfaceImplementation (Type implementationType)
    {
      ArgumentUtility.CheckNotNull ("implementationType", implementationType);
      
      return _implementationMethodInfo.FindInterfaceImplementation (implementationType);
    }

    public IEnumerable<IMethodInformation> FindInterfaceDeclarations ()
    {
      return EnumerableUtility.Singleton (_declarationMethodInfo);
    }

    public T GetFastInvoker<T> () where T: class
    {
      return (T)(object)GetFastInvoker (typeof (T));
    }

    public Delegate GetFastInvoker (Type delegateType)
    {
      ArgumentUtility.CheckNotNull ("delegateType", delegateType);

      return _declarationMethodInfo.GetFastInvoker (delegateType);
    }

    public ParameterInfo[] GetParameters ()
    {
      return _implementationMethodInfo.GetParameters();
    }

    public IMethodInformation GetOriginalDeclaration ()
    {
      return _implementationMethodInfo.GetOriginalDeclaration();
    }

    public IPropertyInformation FindDeclaringProperty ()
    {
      return _implementationMethodInfo.FindDeclaringProperty();
    }

    public Type ReturnType
    {
      get { return _implementationMethodInfo.ReturnType; }
    }

    public object Invoke (object instance, object[] parameters)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);
      
      return _declarationMethodInfo.Invoke (instance, parameters);
    }

    public override bool Equals (object obj)
    {
      if (obj == null)
        return false;
      if (obj.GetType() != GetType()) 
        return false;

      var other = (InterfaceImplementationMethodInformation) obj;
      return _implementationMethodInfo.Equals (other._implementationMethodInfo) && _declarationMethodInfo.Equals (other._declarationMethodInfo);
    }

    public override int GetHashCode ()
    {
      return _implementationMethodInfo.GetHashCode() ^ _declarationMethodInfo.GetHashCode();
    }

    public override string ToString ()
    {
      return string.Format ("{0} (impl of '{1}')", _implementationMethodInfo.Name, _declarationMethodInfo.DeclaringType.Name);
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}