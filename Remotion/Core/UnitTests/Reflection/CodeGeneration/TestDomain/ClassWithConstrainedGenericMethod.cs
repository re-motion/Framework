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

namespace Remotion.UnitTests.Reflection.CodeGeneration.TestDomain
{
  public class ClassWithConstrainedGenericMethod
  {
    public virtual string GenericMethod<T1, T2, T3> (T1 t1, T2 t2, T3 t3)
        where T1 : IConvertible
        where T2 : struct
        where T3 : T1
    {
      return string.Format ("{0}, {1}, {2}", t1, t2, t3);
    }
  }
}
