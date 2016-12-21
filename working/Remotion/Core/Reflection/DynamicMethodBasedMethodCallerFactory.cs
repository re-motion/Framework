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
using System.Reflection.Emit;
using Remotion.Reflection.CodeGeneration;
using Remotion.Utilities;

namespace Remotion.Reflection
{
  /// <summary>
  /// Factory class for creating <see cref="DynamicMethod"/>-based wrapper delegates for <see cref="MethodInfo"/> objects.
  /// </summary>
  public static class DynamicMethodBasedMethodCallerFactory
  {
    /// <summary>
    /// Creates a <see cref="Delegate"/> that can be used to invoke the method identified by the <paramref name="methodInfo"/>.
    /// </summary>
    /// <param name="methodInfo">The method to wrap.</param>
    /// <param name="delegateType">
    /// The <see cref="Delegate"/> type. The signature must always include the instance-parameter as first parameter even if the 
    /// <paramref name="methodInfo"/> refers to a static method.
    /// </param>
    /// <returns>
    /// An instance of the <paramref name="delegateType"/> that can be used to invoke the method identified by the <paramref name="methodInfo"/>.
    /// </returns>
    public static Delegate CreateMethodCallerDelegate (MethodInfo methodInfo, Type delegateType)
    {
      ArgumentUtility.CheckNotNull ("methodInfo", methodInfo);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("delegateType", delegateType, typeof (Delegate));

      var delegateMethod = delegateType.GetMethod ("Invoke");
      Assertion.IsNotNull (delegateMethod);
      var name = methodInfo.DeclaringType + "_" + methodInfo.Name + "_" + Guid.NewGuid();
      var returnType = delegateMethod.ReturnType;
      var parameterTypes = delegateMethod.GetParameters().Select (p => p.ParameterType).ToArray();

      DynamicMethod dynamicMethod;
      if (methodInfo.DeclaringType.IsInterface)
      {
        // Using the owner-less version for non-nested interfaces helps circumvent issues in the CLR regarding the combination of 
        // DynamicMethods and domain-neutrally loaded assemblies. This case could happen if the interface is from mscorlib, e.g. ICollection. 
        // See http://support.microsoft.com/kb/971030/en-us for details.

        if (methodInfo.DeclaringType.IsNested)
          dynamicMethod = new DynamicMethod (name, returnType, parameterTypes, methodInfo.DeclaringType.DeclaringType, false);
        else
          dynamicMethod = new DynamicMethod (name, returnType, parameterTypes, false);
      }
      else
      {
        dynamicMethod = new DynamicMethod (name, returnType, parameterTypes, methodInfo.DeclaringType, false);
      }

      var ilGenerator = dynamicMethod.GetILGenerator();

      var emitter = new MethodWrapperEmitter (ilGenerator, methodInfo, parameterTypes, returnType);
      emitter.EmitStaticMethodBody();

      return dynamicMethod.CreateDelegate (delegateType);
    }
  }
}