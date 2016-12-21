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

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  public class OverrideInterfaceGenerator
  {
    public static OverrideInterfaceGenerator CreateNestedGenerator (MutableType outerType, string typeName)
    {
      ArgumentUtility.CheckNotNull ("outerType", outerType);
      ArgumentUtility.CheckNotNullOrEmpty ("typeName", typeName);

      var interfaceType = outerType.AddNestedType(typeName, TypeAttributes.Interface | TypeAttributes.NestedPublic | TypeAttributes.Abstract, null);
      return new OverrideInterfaceGenerator (interfaceType);
    }

    private readonly IAttributeGenerator _attributeGenerator = new AttributeGenerator();
    private readonly Dictionary<MethodInfo, MethodInfo> _interfaceMethods = new Dictionary<MethodInfo, MethodInfo> ();
    private readonly MutableType _interfaceType;

    private OverrideInterfaceGenerator (MutableType interfaceType)
    {
      ArgumentUtility.CheckNotNull ("interfaceType", interfaceType);

      _interfaceType = interfaceType;
    }

    public Type Type
    {
      get { return _interfaceType; }
    }

    public Dictionary<MethodInfo, MethodInfo> InterfaceMethodsForOverriddenMethods
    {
      get { return _interfaceMethods; }
    }

    public MethodInfo AddOverriddenMethod (MethodInfo overriddenMethod)
    {
      ArgumentUtility.CheckNotNull ("overriddenMethod", overriddenMethod);

      var name = overriddenMethod.Name;
      var attributes = MethodAttributes.Public | MethodAttributes.Abstract | MethodAttributes.Virtual;
      var md = MethodDeclaration.CreateEquivalent (overriddenMethod);
      var method = _interfaceType.AddMethod (name, attributes, md, bodyProvider: null);

      _attributeGenerator.AddOverrideInterfaceMappingAttribute (method, overriddenMethod);

      _interfaceMethods.Add (overriddenMethod, method);

      return method;
    }
  }
}