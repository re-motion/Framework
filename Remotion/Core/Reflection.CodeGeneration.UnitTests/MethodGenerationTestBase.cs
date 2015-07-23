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

namespace Remotion.Reflection.CodeGeneration.UnitTests
{
  public abstract class MethodGenerationTestBase : CodeGenerationTestBase
  {
    private CustomClassEmitter _classEmitter;

    protected CustomClassEmitter ClassEmitter
    {
      get { return _classEmitter; }
    }

    public override void SetUp ()
    {
      base.SetUp();
      _classEmitter = new CustomClassEmitter (Scope, UniqueName, typeof (object), new Type[0], TypeAttributes.Class | TypeAttributes.Public, true);
    }

    public override void TearDown ()
    {
      if (!_classEmitter.HasBeenBuilt)
        _classEmitter.BuildType();

      base.TearDown();
    }

    protected Type BuildType ()
    {
      return _classEmitter.BuildType ();
    }

    protected object BuildInstance ()
    {
      return Activator.CreateInstance (_classEmitter.BuildType());
    }

    protected object BuildInstanceAndInvokeMethod (IMethodEmitter method, params object[] arguments)
    {
      object instance = BuildInstance();
      return InvokeMethod (instance, method, arguments);
    }

    protected object BuildInstanceAndInvokeMethod (IMethodEmitter method, Type[] typeArguments, params object[] arguments)
    {
      object instance = BuildInstance ();
      return InvokeMethod (instance, method, typeArguments, arguments);
    }

    protected object BuildTypeAndInvokeMethod (IMethodEmitter method, params object[] arguments)
    {
      Type builtType = _classEmitter.BuildType();
      return InvokeMethod (builtType, method, arguments);
    }

    protected object InvokeMethod (object instance, IMethodEmitter method, params object[] arguments)
    {
      var methodInfo = GetMethod (instance, method);
      return methodInfo.Invoke (instance, arguments);
    }

    protected object InvokeMethod (object instance, IMethodEmitter method, Type[] typeArguments, params object[] arguments)
    {
      var methodInfo = GetMethod (instance, method).MakeGenericMethod (typeArguments);
      return methodInfo.Invoke (instance, arguments);
    }

    private object InvokeMethod (Type type, IMethodEmitter method, params object[] arguments)
    {
      var methodInfo = GetMethod (type, method);
      return methodInfo.Invoke (null, arguments);
    }

    private MethodInfo GetMethod (Type builtType, IMethodEmitter method)
    {
      return builtType.GetMethod (method.Name);
    }

    protected MethodInfo GetMethod (object instance, IMethodEmitter method)
    {
      return GetMethod (instance.GetType(), method);
    }

    protected MethodInfo BuildTypeAndGetMethod (IMethodEmitter method)
    {
      Type builtType = _classEmitter.BuildType();
      return GetMethod (builtType, method);
    }
  }
}