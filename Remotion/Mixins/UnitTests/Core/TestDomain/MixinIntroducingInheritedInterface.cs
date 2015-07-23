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

namespace Remotion.Mixins.UnitTests.Core.TestDomain
{
  public interface IMixinIII1
  {
    string Method1();
  }

  public interface IMixinIII2 : IMixinIII1
  {
    string Method2 ();
  }

  public interface IMixinIII3 : IMixinIII1, IMixinIII2
  {
    string Method3 ();
  }

  public interface IMixinIII4 : IMixinIII3
  {
    string Method4 ();
  }

  public class MixinIntroducingInheritedInterface : IMixinIII4
  {
    public string Method4 ()
    {
      return "MixinIntroducingInheritedInterface.Method4";
    }

    public string Method3 ()
    {
      return "MixinIntroducingInheritedInterface.Method3";
    }

    public string Method1 ()
    {
      return "MixinIntroducingInheritedInterface.Method1";
    }

    public string Method2 ()
    {
      return "MixinIntroducingInheritedInterface.Method2";
    }
  }
}
