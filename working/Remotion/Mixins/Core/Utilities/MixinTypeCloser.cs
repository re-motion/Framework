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
using Remotion.Utilities;

namespace Remotion.Mixins.Utilities
{
  /// <summary>
  /// Takes a mixin type and closes it when it is a generic type definitions, inferring its generic arguments from the mixin's target type and the
  /// parameters' constraints.
  /// </summary>
  public class MixinTypeCloser
  {
    private readonly ConstraintBasedGenericParameterInstantiator _parameterInstantiator = new ConstraintBasedGenericParameterInstantiator ();
    private readonly Type _targetClass;

    public MixinTypeCloser(Type targetClass)
    {
      ArgumentUtility.CheckNotNull ("targetClass", targetClass);

      if (targetClass.ContainsGenericParameters)
        throw new ArgumentException ("The target class must not contain generic parameters.", "targetClass");

      _targetClass = targetClass;
    }

    public Type GetClosedMixinType (Type configuredMixinType)
    {
      ArgumentUtility.CheckNotNull ("configuredMixinType", configuredMixinType);

      if (configuredMixinType.ContainsGenericParameters)
      {
        Type[] genericParameterInstantiations = GetGenericParameterInstantiations (configuredMixinType);
        try
        {
          return configuredMixinType.MakeGenericType (genericParameterInstantiations);
        }
        catch (ArgumentException ex)
        {
          var message = string.Format (
              "Cannot close the generic mixin type '{0}' applied to class '{1}' - the inferred type arguments violate the generic parameter "
              + "constraints. Specify the arguments manually, modify the parameter binding specification, or relax the constraints. {2}",
              configuredMixinType,
              _targetClass,
              ex.Message);
          throw new ConfigurationException (message, ex);
        }
      }
      else
        return configuredMixinType;
    }

    private Type[] GetGenericParameterInstantiations (Type mixinType)
    {
      var genericArguments = mixinType.GetGenericArguments ();

      var instantiations = new Type[genericArguments.Length];

      int firstNonPositionalIndex = FillPositionalInstantiations (instantiations, genericArguments, mixinType);
      FillNonPositionalInstantiations (instantiations, genericArguments, firstNonPositionalIndex, mixinType);

      return instantiations;
    }

    private int FillPositionalInstantiations (Type[] instantiations, Type[] genericArguments, Type mixinType)
    {
      int index = 0;
      var targetGenericParameters = _targetClass.GetGenericArguments ();
      while (index < genericArguments.Length && IsBoundToPositionalTargetParameter (genericArguments[index]))
      {
        if (index >= targetGenericParameters.Length)
        {
          var message = string.Format (
              "Cannot bind generic parameter '{0}' of mixin '{1}' to generic parameter number {2} of target type '{3}': The target type does not have "
              + "so many parameters.",
              genericArguments[index].Name,
              mixinType,
              index,
              _targetClass);
          throw new ConfigurationException (message);
        }

        if (IsBoundToConstraint (genericArguments[index]) || IsBoundToTargetType (genericArguments[index]))
        {
          var message = string.Format (
              "Type parameter '{0}' of mixin '{1}' has more than one binding specification.",
              genericArguments[index].Name,
              mixinType);
          throw new ConfigurationException (message);
        }

        instantiations[index] = targetGenericParameters[index];
        ++index;
      }
      return index;
    }

    private void FillNonPositionalInstantiations (Type[] instantiations, Type[] genericArguments, int startIndex, Type mixinType)
    {
      for (int i = startIndex; i < genericArguments.Length; ++i)
      {
        instantiations[i] = GetNonPositionalInstantiation (mixinType, genericArguments[i]);
      }
    }

    private Type GetNonPositionalInstantiation (Type mixinType, Type genericArgument)
    {
      Assertion.IsTrue (genericArgument.IsGenericParameter, "Types with partially specified generic parameters are not supported.");

      if (IsBoundToPositionalTargetParameter (genericArgument))
      {
        var message = string.Format (
            "Type parameter '{0}' of mixin '{1}' applied to target class '{2}' has a BindToGenericTargetParameterAttribute, but it is not at the "
            + "front of the generic parameters. The type parameters with BindToGenericTargetParameterAttributes must all be at the front, before any "
            + "other generic parameters.",
            genericArgument.Name,
            mixinType,
            _targetClass);
        throw new ConfigurationException (message);
      }

      Type instantiation = null;
      if (IsBoundToTargetType (genericArgument))
        instantiation = _targetClass;
        
      if (IsBoundToConstraint (genericArgument))
      {
        if (instantiation != null)
        {
          var message = string.Format (
              "Type parameter '{0}' of mixin '{1}' has more than one binding specification.",
              genericArgument.Name,
              mixinType);
          throw new ConfigurationException (message);
        }

        instantiation = GetConstraintBasedInstantiation (mixinType, genericArgument);
      }

      if (instantiation == null)
      {
        string message = string.Format (
            "The generic mixin '{0}' applied to class '{1}' cannot be automatically closed because its type parameter '{2}' does not have any "
            + "binding information. Apply the BindToTargetTypeAttribute, BindToConstraintsAttribute, or BindToGenericTargetParameterAttribute to the "
            + "type parameter or specify the parameter's instantiation when configuring the mixin for the target class.",
            mixinType,
            _targetClass,
            genericArgument.Name);
        throw new ConfigurationException (message);
      }

      return instantiation;
    }

    private bool IsBoundToPositionalTargetParameter (Type genericArgument)
    {
      return genericArgument.IsDefined (typeof (BindToGenericTargetParameterAttribute), false);
    }

    private bool IsBoundToTargetType (Type genericArgument)
    {
      return genericArgument.IsDefined (typeof (BindToTargetTypeAttribute), false);
    }

    private bool IsBoundToConstraint (Type genericArgument)
    {
      return genericArgument.IsDefined (typeof (BindToConstraintsAttribute), false);
    }

    private Type GetConstraintBasedInstantiation (Type mixinType, Type genericArgument)
    {
      try
      {
        return _parameterInstantiator.Instantiate (genericArgument);
      }
      catch (NotSupportedException ex)
      {
        var message = string.Format (
            "The generic mixin '{0}' applied to class '{1}' cannot be automatically closed because the constraints of its type parameter '{2}' "
            + "cannot be unified into a single type. {3}",
            mixinType,
            _targetClass,
            genericArgument.Name,
            ex.Message);
        throw new ConfigurationException (message, ex);
      }
    }
  }
}
