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
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.UnitTests.CodeGeneration.MethodWrapperEmitterTests.TestDomain;

namespace Remotion.Reflection.UnitTests.CodeGeneration.MethodWrapperEmitterTests
{
  [TestFixture]
  public class ArgumentChecks_MethodWrapperEmitterTest : MethodWrapperEmitterTestBase
  {
    private ILGenerator _fakeILGenerator;

    public override void SetUp ()
    {
      base.SetUp ();
      _fakeILGenerator = (ILGenerator) FormatterServices.GetSafeUninitializedObject (typeof (ILGenerator));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The wrapperReturnType ('String') cannot be assigned from the return type ('SimpleReferenceType') of the wrappedMethod.\r\n"
        + "Parameter name: wrapperReturnType")]
    public void EmitMethodBody_ReturnTypesDoNotMatch ()
    {
      Type declaringType = typeof (ClassWithMethods);
      var methodInfo = declaringType.GetMethod ("InstanceMethodWithReferenceTypeReturnValue", BindingFlags.Public | BindingFlags.Instance);

      Type returnType = typeof (string);
      Type[] parameterTypes = new[] { typeof (object) };
      new MethodWrapperEmitter (_fakeILGenerator, methodInfo, parameterTypes, returnType);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The wrapperParameterType #1 ('String') cannot be assigned to the type ('SimpleReferenceType') of parameter 'value' of the wrappedMethod.\r\n"
        + "Parameter name: wrapperParameterTypes")]
    public void EmitMethodBody_ParameterTypesDoNotMatch ()
    {
      Type declaringType = typeof (ClassWithMethods);
      var methodInfo = declaringType.GetMethod ("InstanceMethodWithReferenceTypeParameter", BindingFlags.Public | BindingFlags.Instance);

      Type returnType = typeof (object);
      Type[] parameterTypes = new[] { typeof (object), typeof (string) };
      new MethodWrapperEmitter (_fakeILGenerator, methodInfo, parameterTypes, returnType);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The wrapperParameterType #0 ('String') cannot be assigned to the declaring type ('ClassWithMethods') of the wrappedMethod.\r\n"
        + "Parameter name: wrapperParameterTypes")]
    public void EmitMethodBody_InstanceTypesDoNotMatch ()
    {
      Type declaringType = typeof (ClassWithMethods);
      var methodInfo = declaringType.GetMethod ("InstanceMethodWithReferenceTypeParameter", BindingFlags.Public | BindingFlags.Instance);

      Type returnType = typeof (object);
      Type[] parameterTypes = new[] { typeof (string), typeof (object) };
      new MethodWrapperEmitter (_fakeILGenerator, methodInfo, parameterTypes, returnType);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The number of elements in the wrapperParameterTypes array (3) does not match the number of parameters required for invoking the wrappedMethod (5).\r\n"
        + "Parameter name: wrapperParameterTypes")]
    public void EmitMethodBody_ParameterCountsDoNotMatch ()
    {
      Type declaringType = typeof (ClassWithMethods);
      var methodInfo = declaringType.GetMethod ("InstanceMethodWithMultipleParameters", BindingFlags.Public | BindingFlags.Instance);

      Type returnType = typeof (object);
      Type[] parameterTypes = new[] { typeof (object), typeof (object), typeof (object) };
      new MethodWrapperEmitter (_fakeILGenerator, methodInfo, parameterTypes, returnType);
    }
  }
}