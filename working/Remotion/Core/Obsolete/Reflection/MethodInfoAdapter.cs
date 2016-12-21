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
  [TypeConverter (typeof (MethodInfoAdapterConverter))]
  internal sealed class MethodInfoAdapter : IMethodInformation
  {
    public static MethodInfoAdapter Create (MethodInfo methodInfo)
    {
      throw new NotImplementedException();
    }

    private MethodInfoAdapter (MethodInfo methodInfo)
    {
      throw new NotImplementedException();
    }

    public MethodInfo MethodInfo
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

    public Type ReturnType
    {
      get { throw new NotImplementedException(); }
    }

    public object Invoke (object instance, object[] parameters)
    {
      throw new NotImplementedException();
    }

    public IMethodInformation FindInterfaceImplementation (Type implementationType)
    {
      throw new NotImplementedException();
    }

    public IPropertyInformation FindDeclaringProperty ()
    {
      throw new NotImplementedException();
    }

    public IEnumerable<IMethodInformation> FindInterfaceDeclarations ()
    {
      throw new NotImplementedException();
    }

    public T GetFastInvoker<T> () where T : class
    {
      throw new NotImplementedException();
    }

    public Delegate GetFastInvoker (Type delegateType)
    {
      throw new NotImplementedException();
    }

    public ParameterInfo[] GetParameters ()
    {
      throw new NotImplementedException();
    }

    public IMethodInformation GetOriginalDeclaration ()
    {
      throw new NotImplementedException();
    }
  }
}