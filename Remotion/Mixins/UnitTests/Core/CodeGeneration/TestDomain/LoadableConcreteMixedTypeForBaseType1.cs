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
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.TestDomain
{
  [ConcreteMixedType (
      new object[] 
      {
        typeof (BaseType1), 
        new object[] 
        { 
          new object[] 
          {
              typeof (BT1Mixin1), 
              MixinKind.Used, 
              MemberVisibility.Private, 
              new Type[0],
              new object[] { "some kind", "mscorlib", "some location" }
          }
        }, 
        new Type[0] 
      }, 
      new[] { typeof (BT1Mixin1) })]
  public class LoadableConcreteMixedTypeForBaseType1
  { }
}
