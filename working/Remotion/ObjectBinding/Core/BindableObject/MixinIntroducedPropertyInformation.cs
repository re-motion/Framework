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
  /// Represents a mixin property that is introduced to its target classes.
  /// </summary>
  /// <remarks>
  /// This is mainly just a wrapper around a <see cref="IPropertyInformation"/>
  /// describing the property as it is implemented on the mixin type itself. The <see cref="GetGetMethod"/> and <see cref="GetSetMethod"/> methods, 
  /// however, return instances of <see cref="MixinIntroducedMethodInformation"/>. This means that a <see cref="MixinIntroducedPropertyInformation"/>'s
  /// getter and setter can be used to get and set the value both via the mixin instance and via an instance of the target class. 
  /// <see cref="GetValue"/> and <see cref="SetValue"/> also work this way.
  /// </remarks>
  public sealed class MixinIntroducedPropertyInformation : IPropertyInformation
  {
    private readonly InterfaceImplementationPropertyInformation _interfaceImplementationPropertyInfo;

    public MixinIntroducedPropertyInformation (InterfaceImplementationPropertyInformation interfaceImplementationPropertyInfo)
    {
      ArgumentUtility.CheckNotNull ("interfaceImplementationPropertyInfo", interfaceImplementationPropertyInfo);

      _interfaceImplementationPropertyInfo = interfaceImplementationPropertyInfo;
    }

    public InterfaceImplementationPropertyInformation InterfaceImplementationPropertyInfo
    {
      get { return _interfaceImplementationPropertyInfo; }
    }

    public string Name
    {
      get { return _interfaceImplementationPropertyInfo.Name;  }
    }

    public ITypeInformation DeclaringType
    {
      get { return _interfaceImplementationPropertyInfo.DeclaringType; }
    }

    public ITypeInformation GetOriginalDeclaringType ()
    {
      return _interfaceImplementationPropertyInfo.GetOriginalDeclaringType();
    }

    public IPropertyInformation GetOriginalDeclaration ()
    {
      return _interfaceImplementationPropertyInfo.GetOriginalDeclaration();
    }

    public T GetCustomAttribute<T> (bool inherited) where T: class
    {
      return _interfaceImplementationPropertyInfo.GetCustomAttribute<T>(inherited);
    }

    public T[] GetCustomAttributes<T> (bool inherited) where T: class
    {
      return _interfaceImplementationPropertyInfo.GetCustomAttributes<T> (inherited);
    }

    public bool IsDefined<T> (bool inherited) where T: class
    {
      return _interfaceImplementationPropertyInfo.IsDefined<T> (inherited);
    }

    public IPropertyInformation FindInterfaceImplementation (Type implementationType)
    {
      ArgumentUtility.CheckNotNull ("implementationType", implementationType);

      return _interfaceImplementationPropertyInfo.FindInterfaceImplementation (implementationType);
    }

    public IEnumerable<IPropertyInformation> FindInterfaceDeclarations ()
    {
      return _interfaceImplementationPropertyInfo.FindInterfaceDeclarations();
    }

    public ParameterInfo[] GetIndexParameters ()
    {
      return _interfaceImplementationPropertyInfo.GetIndexParameters();
    }

    public IMethodInformation[] GetAccessors (bool nonPublic)
    {
      return _interfaceImplementationPropertyInfo.GetAccessors(nonPublic);
    }

    public Type PropertyType
    {
      get { return _interfaceImplementationPropertyInfo.PropertyType; }
    }

    public bool CanBeSetFromOutside
    {
      get { return GetSetMethod (false) != null; }
    }

    public object GetValue (object instance, object[] indexParameters)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);

      return _interfaceImplementationPropertyInfo.GetValue (instance, indexParameters);
    }

    public void SetValue (object instance, object value, object[] indexParameters)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);

      _interfaceImplementationPropertyInfo.SetValue (instance, value, indexParameters);
    }

    public IMethodInformation GetGetMethod (bool nonPublic)
    {
      return Maybe
          .ForValue (_interfaceImplementationPropertyInfo.GetGetMethod (nonPublic))
          .Select (mi => mi as InterfaceImplementationMethodInformation)
          .Select (mi => new MixinIntroducedMethodInformation (mi))
          .ValueOrDefault ();
    }

    public IMethodInformation GetSetMethod (bool nonPublic)
    {
      return Maybe
          .ForValue (_interfaceImplementationPropertyInfo.GetSetMethod (nonPublic))
          .Select (mi => mi as InterfaceImplementationMethodInformation)
          .Select (mi => new MixinIntroducedMethodInformation (mi))
          .ValueOrDefault ();
    }

    public override bool Equals (object obj)
    {
      if (obj == null)
        return false;
      if (obj.GetType() != GetType()) 
        return false;
      var other = (MixinIntroducedPropertyInformation) obj;

      return _interfaceImplementationPropertyInfo.Equals (other._interfaceImplementationPropertyInfo);
    }

    public override int GetHashCode ()
    {
      return _interfaceImplementationPropertyInfo.GetHashCode();
    }

    public override string ToString ()
    {
      return _interfaceImplementationPropertyInfo + " (Mixin)";
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}