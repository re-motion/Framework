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
using Castle.DynamicProxy;
using Remotion.Utilities;

namespace Remotion.Mixins.Samples.DynamicMixinBuilding.Core
{
  public delegate object BaseMethodInvoker (object[] args);
  public delegate object MethodInvocationHandler (object instance, MethodInfo method, object[] args, BaseMethodInvoker baseMethod);

  public class DynamicMixinBuilder
  {
    public static ModuleScope Scope = new ModuleScope (true);

    private readonly Type _targetType;
    private readonly HashSet<MethodInfo> _methodsToOverride = new HashSet<MethodInfo> ();

    public DynamicMixinBuilder (Type targetType)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      _targetType = targetType;
    }

    public Type BuildMixinType (MethodInvocationHandler methodInvocationHandler)
    {
      ArgumentUtility.CheckNotNull ("methodInvocationHandler", methodInvocationHandler);
      return new DynamicMixinTypeGenerator (Scope, _targetType, _methodsToOverride, methodInvocationHandler).BuildType();
    }

    public void OverrideMethod (MethodInfo method)
    {
      ArgumentUtility.CheckNotNull ("method", method);

      if (method.DeclaringType != _targetType)
        throw new ArgumentException ("The declaring type of the method must be the target type.", "method");

      _methodsToOverride.Add (method);
    }

    public IEnumerable<MethodInfo> OverriddenMethods
    {
      get { return _methodsToOverride; }
    }
  }
}
