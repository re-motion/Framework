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
using System.Runtime.InteropServices;

namespace Remotion.Reflection.UnitTests.CodeGeneration.MethodWrapperEmitterTests.TestDomain
{
  public class ClassWithMethods
  {
    // ReSharper disable UnusedMember.Global
    public static SimpleReferenceType StaticReferenceTypeValue { get; set; }

    public static SimpleReferenceType StaticMethodWithReferenceTypeReturnValue ()
    {
      return StaticReferenceTypeValue;
    }

    public static void StaticMethodWithReferenceTypeParameter (SimpleReferenceType value)
    {
      StaticReferenceTypeValue = value;
    }

    public SimpleReferenceType InstanceReferenceTypeValue { get; set; }
    public int InstanceValueTypeValue { get; set; }
    public int? InstanceNullableValueTypeValue { get; set; }

    public virtual SimpleReferenceType InstanceMethodWithReferenceTypeReturnValue ()
    {
      return InstanceReferenceTypeValue;
    }

    public void InstanceMethodWithReferenceTypeParameter (SimpleReferenceType value)
    {
      InstanceReferenceTypeValue = value;
    }

    public int InstanceMethodWithValueTypeReturnValue ()
    {
      return InstanceValueTypeValue;
    }

    public void InstanceMethodWithValueTypeParameter (int value)
    {
      InstanceValueTypeValue = value;
    }

    public int? InstanceMethodWithNullableValueTypeReturnValue ()
    {
      return InstanceNullableValueTypeValue;
    }

    public void InstanceMethodWithNullableValueTypeParameter (int? value)
    {
      InstanceNullableValueTypeValue = value;
    }

    public void InstanceMethodWithMultipleParameters (
        SimpleReferenceType referenceTypeValue, int valueTypeValue, int? nullableValueTypeValue, SimpleReferenceType staticReferenceTypeValue)
    {
      InstanceReferenceTypeValue = referenceTypeValue;
      InstanceValueTypeValue = valueTypeValue;
      InstanceNullableValueTypeValue = nullableValueTypeValue;
      StaticReferenceTypeValue = staticReferenceTypeValue;
    }

    public void InstanceMethodWithOutParameter (out SimpleReferenceType value)
    {
      value = InstanceReferenceTypeValue;
    }

    public void InstanceMethodWithByRefParameter (ref SimpleReferenceType value)
    {
      value = InstanceReferenceTypeValue;
    }

    public void InstanceMethodWithOptionalParameter ([Optional] SimpleReferenceType value)
    {
    }

    public T GenericInstanceMethod<T> (T value)
    {
      return value;
    }

    // ReSharper restore UnusedMember.Global  
  }
}