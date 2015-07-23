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
using System.Reflection;
using System.Reflection.Emit;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration;

namespace Remotion.Reflection.UnitTests.CodeGeneration.MethodWrapperEmitterTests
{
  public class MethodWrapperEmitterTestBase
  {    
    private static int s_counter;

    private bool _hasBeenBuilt;
    private TypeBuilder _typeBuilder;

    [SetUp]
    public virtual void SetUp ()
    {
      var uniqueName = GetType().Name + "." + s_counter;
      s_counter++;

      _typeBuilder = SetUpFixture.ModuleBuilder.DefineType (uniqueName, TypeAttributes.Class | TypeAttributes.Public, typeof (object), new Type[0]);
      _hasBeenBuilt = false;
    }

    [TearDown]
    public virtual void TearDown ()
    {
      if (!_hasBeenBuilt)
        _typeBuilder.CreateType();
    }

    protected MethodBuilder GetWrapperMethodFromEmitter (
        MethodBase executingTestMethod, Type[] publicParameterTypes, Type publicReturnType, MethodInfo innerMethod)
    {
      var methodName = executingTestMethod.DeclaringType.Name + "_" + executingTestMethod.Name;

      var methodBuilder = _typeBuilder.DefineMethod (methodName, MethodAttributes.Public | MethodAttributes.Static, publicReturnType, publicParameterTypes);

      var ilGenerator = methodBuilder.GetILGenerator();
      var emitter = new MethodWrapperEmitter (ilGenerator, innerMethod, publicParameterTypes, publicReturnType);
      emitter.EmitStaticMethodBody();

      return methodBuilder;
    }

    protected object BuildTypeAndInvokeMethod (MethodBuilder methodBuilder, params object[] arguments)
    {
      _hasBeenBuilt = true;
      Type builtType = _typeBuilder.CreateType();
      var methodInfo = builtType.GetMethod (methodBuilder.Name);
      return methodInfo.Invoke (null, arguments);
    }
  }
}