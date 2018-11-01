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
using System.Linq;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Utilities
{
  /// <summary>
  /// Deduces a possible instantiation for a generic type parameter based on the constraints placed on the parameter.
  /// </summary>
  public class ConstraintBasedGenericParameterInstantiator
  {
    public Type Instantiate (Type typeParameter)
    {
      ArgumentUtility.CheckNotNull ("typeParameter", typeParameter);

      if (!typeParameter.IsGenericParameter)
        throw new ArgumentException ("Type must be a generic parameter.", "typeParameter");

      Type candidate = InferFromGenericParameterConstraints(typeParameter);

      if (HasAttribute (typeParameter, GenericParameterAttributes.NotNullableValueTypeConstraint))
        candidate = UnifyConstraints (typeof (ValueType), candidate);

      if (candidate == null)
        candidate = typeof (object);
      else if (candidate == typeof (ValueType))
        candidate = typeof (int);

      return candidate;
    }

    private bool HasAttribute (Type typeParameter, GenericParameterAttributes attribute)
    {
      return (typeParameter.GenericParameterAttributes & attribute) == attribute;
    }

    private Type InferFromGenericParameterConstraints (Type typeParameter)
    {
      Type candidate = null;

      // Starting with .NET 4.6, the order follows different conventions. 
      // For better diagnostics, the value-type/reference-type constraint should be listed before any interface constraints.
      var constraints = typeParameter.GetGenericParameterConstraints().OrderBy (t => t.IsInterface).ThenBy (t => t.FullName);

      foreach (Type constraint in constraints)
      {
        if (constraint.ContainsGenericParameters)
        {
          string message = string.Format (
              "The generic type parameter has a constraint '{0}' which itself contains generic parameters.",
              constraint.Name);
          throw new NotSupportedException (message);
        }

        candidate = UnifyConstraints (constraint, candidate);
      }
      return candidate;
    }

    private Type UnifyConstraints (Type constraint, Type candidate)
    {
      if (candidate == null)
        candidate = constraint;
      else if (candidate.IsAssignableFrom (constraint))
        candidate = constraint;
      else if (!constraint.IsAssignableFrom (candidate))
      {
        if (constraint.IsInterface && candidate.IsInterface || candidate == typeof (ValueType))
        {
          string message = string.Format (
              "The generic type parameter has inconclusive constraints '{0}' and '{1}', which cannot be unified into a single type.",
              candidate,
              constraint);
          throw new NotSupportedException (message);
        }
        else
        {
          string message = string.Format (
              "The generic type parameter has incompatible constraints '{0}' and '{1}'.",
              candidate,
              constraint);
          throw new NotSupportedException (message);
        }
      }
      return candidate;
    }
  }
}
