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
using Remotion.Collections;

namespace Remotion.Scripting.UnitTests.StableBindingImplementation
{
  public class MethodInfoEqualityComparer : CompoundValueEqualityComparer<MethodInfo> {
    private static readonly MethodInfoEqualityComparer s_equalityComparer = new MethodInfoEqualityComparer();

    public MethodInfoEqualityComparer (MethodAttributes methodAttributeMask)
        : base (
            x => new object[] {
                x.Name, x.ReturnType, x.Attributes & methodAttributeMask, x.IsGenericMethod ? x.GetGenericArguments().Length : 0,
                ComponentwiseEqualsAndHashcodeWrapper.New (x.IsGenericMethod ? new Type[0] : x.GetParameters ().Select (pi => pi.ParameterType.IsGenericParameter ? typeof(Object) : pi.ParameterType)),
                ComponentwiseEqualsAndHashcodeWrapper.New (x.IsGenericMethod ? x.GetParameters ().Select (pi => pi.ParameterType.IsGenericParameter ? pi.ParameterType.GenericParameterPosition : -1) : new int[0]),
                ComponentwiseEqualsAndHashcodeWrapper.New (x.GetParameters ().Select (pi => pi.Attributes)) 
            })
    {
    }

    // Masking out VtableLayoutMask is necessary, otherwise a virtual method and its override will not compare equal
    public MethodInfoEqualityComparer ()
        : this (~(MethodAttributes.ReservedMask | MethodAttributes.VtableLayoutMask))
    {
    }


    public static MethodInfoEqualityComparer Get
    {
      get { return s_equalityComparer; }
    }

 
  }
}
