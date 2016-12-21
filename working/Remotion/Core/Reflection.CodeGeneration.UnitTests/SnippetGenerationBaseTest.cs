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
using Remotion.Development.UnitTesting;

namespace Remotion.Reflection.CodeGeneration.UnitTests
{
  public class SnippetGenerationBaseTest : CodeGenerationTestBase
  {
    private static int s_typeCount;

    private CustomClassEmitter _classEmitter;
    private IMethodEmitter _methodEmitter;
    private Type _builtType;
    private object _builtInstance;
    private Type _unsavedBuiltType;
    private CustomClassEmitter _unsavedClassEmitter;

    public override void SetUp ()
    {
      base.SetUp ();
      _classEmitter = null;
      _unsavedClassEmitter = null;

      _methodEmitter = null;
      _builtType = null;
      _builtInstance = null;
      _unsavedBuiltType = null;
    }

    public CustomClassEmitter ClassEmitter
    {
      get
      {
        if (_classEmitter == null)
          _classEmitter = new CustomClassEmitter (Scope, GetType ().Name + s_typeCount++, typeof (object), Type.EmptyTypes, TypeAttributes.Public, true);
        return _classEmitter;
      }
    }

    public CustomClassEmitter UnsavedClassEmitter
    {
      get { 
        if (_unsavedClassEmitter == null)
          _unsavedClassEmitter = new CustomClassEmitter (UnsavedScope, GetType ().Name + "Unsaved" + s_typeCount++, typeof (object), Type.EmptyTypes, TypeAttributes.Public, true);
        return _unsavedClassEmitter; }
    }

    public IMethodEmitter GetMethodEmitter (bool isStatic, Type returnType, Type[] parameterTypes)
    {
      if (_methodEmitter == null)
      {
        MethodAttributes flags = MethodAttributes.Public;
        if (isStatic)
          flags |= MethodAttributes.Static;
        _methodEmitter = ClassEmitter.CreateMethod ("TestMethod", flags, returnType, parameterTypes);
      }
      return _methodEmitter;
    }

    public IMethodEmitter GetUnsavedMethodEmitter (bool isStatic, Type returnType, Type[] parameterTypes)
    {
      MethodAttributes flags = MethodAttributes.Public;
      if (isStatic)
        flags |= MethodAttributes.Static;
      var methodEmitter = UnsavedClassEmitter.CreateMethod ("TestMethod", flags, returnType, parameterTypes);
      return methodEmitter;
    }

    public Type GetBuiltType ()
    {
      if (_builtType == null)
        _builtType = ClassEmitter.BuildType ();
      return _builtType;
    }

    public Type GetUnsavedBuiltType ()
    {
      if (_unsavedBuiltType == null)
        _unsavedBuiltType = UnsavedClassEmitter.BuildType ();
      return _unsavedBuiltType;
    }

    public object GetBuiltInstance ()
    {
      if (_builtInstance == null)
        _builtInstance = Activator.CreateInstance (GetBuiltType ());
      return _builtInstance;
    }

    public object InvokeMethod (params object[] args)
    {
      if (_methodEmitter == null)
        throw new InvalidOperationException ("No method created.");
      else
      {
        if (_methodEmitter.MethodBuilder.IsStatic)
          return PrivateInvoke.InvokePublicStaticMethod (GetBuiltType (), _methodEmitter.Name, args);
        else
          return PrivateInvoke.InvokePublicMethod (GetBuiltInstance (), _methodEmitter.Name, args);
      }
    }
  }
}
