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
using System.Reflection.Emit;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators.Emitters;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  internal class ClassEmitterSupportingOpenGenericBaseType : ClassEmitter
  {
    public ClassEmitterSupportingOpenGenericBaseType (ModuleScope modulescope, string name, Type baseType, Type[] interfaces, TypeAttributes flags,
        bool forceUnsigned)
        : base (modulescope, name, CheckBaseType (baseType), interfaces, flags, forceUnsigned)
    {
    }

    private static Type CheckBaseType (Type baseType)
    {
      if (baseType.DeclaringType != null && baseType.DeclaringType.ContainsGenericParameters)
        throw new NotSupportedException ("This emitter does not support nested types of non-closed generic types.");

      if (!baseType.IsGenericTypeDefinition && baseType.ContainsGenericParameters)
        throw new NotSupportedException (
            "This emitter does not support open constructed types as base types. Specify a closed type or a generic "
            + "type definition.");

      return baseType;
    }

    protected override IEnumerable<Type> InitializeGenericArgumentsFromBases (ref Type baseType, IEnumerable<Type> interfaces)
    {
      Assertion.IsTrue (baseType.DeclaringType == null || !baseType.DeclaringType.ContainsGenericParameters);
      if (baseType.IsGenericTypeDefinition)
      {
        Type[] baseTypeParameters = baseType.GetGenericArguments();
        string[] typeParameterNames = Array.ConvertAll<Type, string> (baseTypeParameters, delegate (Type t) { return t.Name; });

        Assertion.DebugAssert (
            Array.TrueForAll (baseTypeParameters, delegate (Type param) { return param.IsGenericParameter; }),
            "type definitions have no bound arguments");

        baseType = CloseBaseType (baseType, typeParameterNames, baseTypeParameters);
      }
      else
        Assertion.IsFalse (baseType.ContainsGenericParameters);

      return base.InitializeGenericArgumentsFromBases (ref baseType, interfaces); // checks that no interface contains generic parameters
    }

    private Type CloseBaseType (Type baseTypeDefinition, string[] genericParameterNames, Type[] baseTypeArguments)
    {
      Assertion.IsTrue (genericParameterNames.Length == baseTypeArguments.Length, "No pre-bound arguments supported at the moment.");
      Assertion.DebugAssert (
          Array.TrueForAll (baseTypeArguments, delegate (Type param) { return param.IsGenericParameter; }),
          "No pre-bound arguments supported at the moment.");

      GenericTypeParameterBuilder[] genericParameterBuilders = TypeBuilder.DefineGenericParameters (genericParameterNames);
      for (int i = 0; i < genericParameterBuilders.Length; ++i)
        CopyConstraints (baseTypeArguments[i], genericParameterBuilders, i);

      SetGenericTypeParameters (genericParameterBuilders);

      Type closedBaseType = baseTypeDefinition.MakeGenericType (genericParameterBuilders);
      return closedBaseType;
    }

    private void CopyConstraints (Type sourceParameter, GenericTypeParameterBuilder[] builders, int copyTargetIndex)
    {
      Assertion.IsTrue (sourceParameter.IsGenericParameter);

      GenericTypeParameterBuilder builder = builders[copyTargetIndex];
      builder.SetGenericParameterAttributes (sourceParameter.GenericParameterAttributes);

      Type[] sourceConstraints = sourceParameter.GetGenericParameterConstraints();
      Type baseConstraint = null;
      List<Type> interfaceConstraints = new List<Type>();

      for (int i = 0; i < sourceConstraints.Length; ++i)
      {
        Type sourceConstraint = sourceConstraints[i];
        if (sourceConstraint.IsGenericParameter)
        {
          // Note: Reflection and the CLR are nice enough to accept this without requiring cumbersome conversions to the respective builders
          // Note 2: Conversion would be especially cumbersome because a generic parameter could also be embedded inside a class/interface constraint
          interfaceConstraints.Add (sourceConstraint);
        }
        else if (sourceConstraint.IsClass)
        {
          Assertion.IsNull (baseConstraint, "only one base constraint per type");
          baseConstraint = sourceConstraint;
        }
        else
        {
          Assertion.IsTrue (sourceConstraint.IsInterface, "there are only class and interface constraints");
          interfaceConstraints.Add (sourceConstraint);
        }
      }

      if (baseConstraint != null)
        builder.SetBaseTypeConstraint (baseConstraint);
      if (interfaceConstraints.Count > 0)
        builder.SetInterfaceConstraints (interfaceConstraints.ToArray());
    }
  }
}
