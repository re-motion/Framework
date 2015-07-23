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
using System.Reflection;

namespace Remotion.Reflection
{
  [Obsolete ("Dummy declaration for DependDB. Moved to Remotion.Reflection.dll", true)]
  [TypeConverter (typeof (PropertyInfoAdapterConverter))]
  internal sealed class PropertyInfoAdapter : IPropertyInformation
  {
    public static PropertyInfoAdapter Create (PropertyInfo propertyInfo)
    {
      throw new NotImplementedException();
    }

    private PropertyInfoAdapter (PropertyInfo propertyInfo)
    {
      throw new NotImplementedException();
    }

    public PropertyInfo PropertyInfo
    {
      get { throw new NotImplementedException(); }
    }

    public string Name
    {
      get { throw new NotImplementedException(); }
    }

    public ITypeInformation DeclaringType
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

    public Type PropertyType
    {
      get { throw new NotImplementedException(); }
    }

    public bool CanBeSetFromOutside
    {
      get { throw new NotImplementedException(); }
    }

    public object GetValue (object instance, object[] indexParameters)
    {
      throw new NotImplementedException();
    }

    public void SetValue (object instance, object value, object[] indexParameters)
    {
      throw new NotImplementedException();
    }

    public IMethodInformation GetGetMethod (bool nonPublic)
    {
      throw new NotImplementedException();
    }

    public IMethodInformation GetSetMethod (bool nonPublic)
    {
      throw new NotImplementedException();
    }

    public IPropertyInformation FindInterfaceImplementation (Type implementationType)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<IPropertyInformation> FindInterfaceDeclarations ()
    {
      throw new NotImplementedException();
    }

    public ParameterInfo[] GetIndexParameters ()
    {
      throw new NotImplementedException();
    }

    public IMethodInformation[] GetAccessors (bool nonPublic)
    {
      throw new NotImplementedException();
    }

    public IPropertyInformation GetOriginalDeclaration ()
    {
      throw new NotImplementedException();
    }
  }
}