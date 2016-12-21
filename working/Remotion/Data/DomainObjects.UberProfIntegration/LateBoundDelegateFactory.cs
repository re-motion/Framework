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

namespace Remotion.Data.DomainObjects.UberProfIntegration
{
  /// <summary>
  /// Creates delegates for methods discovered at runtime.
  /// </summary>
  public class LateBoundDelegateFactory
  {
    public static TSignature CreateDelegate<TSignature> (object target, string methodName)
    {
      try
      {
        return (TSignature) (object) Delegate.CreateDelegate (typeof (TSignature), target, methodName, false, true);
      }
      catch (ArgumentException ex)
      {
        throw CreateMissingMethodException (target, methodName, typeof (TSignature), ex);
      }
    }

    public static TSignature CreateDelegate<TSignature> (Type target, string methodName)
    {
      return (TSignature) (object) CreateDelegate (target, methodName, typeof (TSignature));
    }

    public static Delegate CreateDelegate (Type target, string methodName, Type signature)
    {
      try
      {
        return Delegate.CreateDelegate (signature, target, methodName, false, true);
      }
      catch (ArgumentException ex)
      {
        throw CreateMissingMethodException (target, methodName, signature, ex);
      }
    }

    private static MissingMethodException CreateMissingMethodException (object target, string methodName, Type signatureType, Exception innerException)
    {
      Type targetType = (target is Type) ? (Type) target : target.GetType();

      Assertion.IsTrue (typeof (Delegate).IsAssignableFrom (signatureType));
      MethodInfo invoke = signatureType.GetMethod ("Invoke");
      Type returnType = invoke.ReturnType;
      var parameters = invoke.GetParameters().Select (p => p.ParameterType);

      return new MissingMethodException (
          String.Format (
              "Type {0} does not define a method {3} {1}({2}).",
              targetType.AssemblyQualifiedName,
              methodName,
              string.Join (", ", parameters),
              returnType == typeof (void) ? "void" : returnType.FullName),
          innerException);
    }
  }
}