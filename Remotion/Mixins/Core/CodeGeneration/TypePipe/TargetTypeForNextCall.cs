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
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;
using ReflectionUtility = Remotion.Mixins.Utilities.ReflectionUtility;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  public class TargetTypeForNextCall : ITargetTypeForNextCall
  {
    private readonly MutableType _concreteTarget;
    private readonly FieldInfo _extensionsField;
    private readonly Dictionary<MethodInfo, MethodInfo> _baseCallMethods = new Dictionary<MethodInfo, MethodInfo>();

    public TargetTypeForNextCall (MutableType concreteTarget, FieldInfo extensionsField)
    {
      ArgumentUtility.CheckNotNull ("concreteTarget", concreteTarget);
      ArgumentUtility.CheckNotNull ("extensionsField", extensionsField);
      
      _concreteTarget = concreteTarget;
      _extensionsField = extensionsField;
    }

    public FieldInfo ExtensionsField
    {
      get { return _extensionsField; }
    }

    public MethodInfo GetBaseCallMethod (MethodInfo overriddenMethod)
    {
      ArgumentUtility.CheckNotNull ("overriddenMethod", overriddenMethod);
      Assertion.IsNotNull (overriddenMethod.DeclaringType);

      if (!overriddenMethod.DeclaringType.IsAssignableFrom (_concreteTarget.BaseType))
      {
        string message = string.Format (
            "Cannot create base call method for a method defined on a different type than the base type: {0}.{1}.",
            overriddenMethod.DeclaringType.FullName,
            overriddenMethod.Name);
        throw new ArgumentException (message, "overriddenMethod");
      }

      if (!_baseCallMethods.ContainsKey (overriddenMethod))
        _baseCallMethods.Add (overriddenMethod, ImplementBaseCallMethod (overriddenMethod));

      return _baseCallMethods[overriddenMethod];
    }

    private MethodInfo ImplementBaseCallMethod (MethodInfo baseMethod)
    {
      Assertion.IsTrue (ReflectionUtility.IsPublicOrProtected (baseMethod));
      if (baseMethod.IsAbstract)
      {
        var message = string.Format ("The given method {0}.{1} is abstract.", baseMethod.DeclaringType.FullName, baseMethod.Name);
        throw new ArgumentException (message, "baseMethod");
      }

      var attributes = MethodAttributes.Public | MethodAttributes.HideBySig;
      var name = "__base__" + baseMethod.Name;
      var md = MethodDeclaration.CreateEquivalent (baseMethod);

      return _concreteTarget.AddMethod (name, attributes, md, ctx => ctx.DelegateToBase (baseMethod));
    }
  }
}