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
  public class MixinWithCircularTargetCallDependency1 : Mixin<ICircular2>, ICircular1
  {
    public string Circular1 ()
    {
      return "MixinWithCircularTargetCallDependency1.Circular1-" + Target.Circular2 ();
    }
  }

  public class MixinWithCircularTargetCallDependency2 : Mixin<ICircular1>, ICircular2
  {
    public string Circular2 ()
    {
      return "MixinWithCircularTargetCallDependency2.Circular2";
    }

    public string Circular12 ()
    {
      return "MixinWithCircularTargetCallDependency2.Circular12-" + Target.Circular1();
    }
  }

  public interface ICircular1
  {
    string Circular1 ();
  }

  public interface ICircular2
  {
    string Circular2 ();
    string Circular12 ();
  }
}
