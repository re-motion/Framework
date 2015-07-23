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
using Remotion.Mixins.CodeGeneration.TypePipe;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.TestDomain
{
  [ConcreteMixinType (
      new object[] {
          typeof (MixinWithAbstractMembers),
          new object[0],
          new object[] { 
              new object[] { typeof (MixinWithAbstractMembers), "AbstractMethod", "System.String AbstractMethod(Int32)" },
              new object[] { typeof (MixinWithAbstractMembers), "RaiseEvent", "System.String RaiseEvent()" },
              new object[] { typeof (MixinWithAbstractMembers), "get_AbstractProperty", "System.String get_AbstractProperty()" },
              new object[] { typeof (MixinWithAbstractMembers), "add_AbstractEvent", "Void add_AbstractEvent(System.Func`1[System.String])" },
              new object[] { typeof (MixinWithAbstractMembers), "remove_AbstractEvent", "Void remove_AbstractEvent(System.Func`1[System.String])" }
          }
      })]
  public class LoadableConcreteMixinTypeForMixinWithAbstractMembers
  {
    public interface IOverriddenMethods
    {
      [OverrideInterfaceMapping (typeof (MixinWithAbstractMembers), "AbstractMethod", "System.String AbstractMethod(Int32)")]
      string AbstractMethod (int i);
      [OverrideInterfaceMapping (typeof (MixinWithAbstractMembers), "RaiseEvent", "System.String RaiseEvent()")]
      string RaiseEvent ();
      [OverrideInterfaceMapping (typeof (MixinWithAbstractMembers), "get_AbstractProperty", "System.String get_AbstractProperty()")]
      string get_AbstractProperty ();
      [OverrideInterfaceMapping (typeof (MixinWithAbstractMembers), "add_AbstractEvent", "Void add_AbstractEvent(System.Func`1[System.String])")]
      void add_AbstractEvent (Func<String> handler);
      [OverrideInterfaceMapping (typeof (MixinWithAbstractMembers), "remove_AbstractEvent", "Void remove_AbstractEvent(System.Func`1[System.String])")]
      void remove_AbstractEvent (Func<String> handler);
    }
  }
}
