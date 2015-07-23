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
using Remotion.TypePipe;

namespace Remotion.Mixins.UnitTests.Core.TestDomain
{
  [Uses (typeof (Mixin1))]
  [Uses (typeof (Mixin2))]
  public class ClassDerivedFromComposedObject : ComposedObject<ClassDerivedFromComposedObject.IClassDerivedFromComposedObject>
  {
    public interface IClassDerivedFromComposedObject : IMixin1, IMixin2
    {
      string MT ();
    }

    public interface IMixin1
    {
      string M1 ();
    }

    public class Mixin1 : IMixin1
    {
      public string M1 ()
      {
        return "Mixin1.M1";
      }
    }

    public interface IMixin2
    {
      string M2 ();
    }

    public class Mixin2 : IMixin2
    {
      public string M2 ()
      {
        return "Mixin2.M2";
      }
    }

    public static IClassDerivedFromComposedObject NewObject()
    {
      return NewObject<ClassDerivedFromComposedObject>(ParamList.Empty);
    }

    public string MT()
    {
      return "ClassDerivedFromComposedObject.MT";
    }
  }
}