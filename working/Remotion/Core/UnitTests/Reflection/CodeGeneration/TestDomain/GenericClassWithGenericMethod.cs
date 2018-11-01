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
using System.Collections.Generic;

namespace Remotion.UnitTests.Reflection.CodeGeneration.TestDomain
{
  public class GenericClassWithGenericMethod<TConcreteInterfaceConstraint, TConcreteClassConstraint, TStructConstraint, TClassNewConstraint,
      TSelfConstraint, TComplexSelfConstraint>
      where TConcreteInterfaceConstraint : IConvertible
      where TConcreteClassConstraint : List<string>
      where TStructConstraint : struct
      where TClassNewConstraint : class, new ()
      where TSelfConstraint : TConcreteInterfaceConstraint
      where TComplexSelfConstraint : List<List<TSelfConstraint[]>>
  {
    public virtual string GenericMethod<T1, T2> (T1 t1, T2 t2, TComplexSelfConstraint tSelf)
        where T1 : TConcreteInterfaceConstraint
        where T2 : List<TStructConstraint[]>
    {
      return string.Format ("{0}, {1}, {2}", t1, t2, tSelf);
    }
  }
}
