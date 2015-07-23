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
using NUnit.Framework;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Mixins.Utilities;

namespace Remotion.Mixins.UnitTests.Core.Utilities
{
  [TestFixture]
  public class MixinGenericArgumentFinderTest
  {
    [Test]
    public void ThisArgumentFinder_Find ()
    {
      var thisArgument = MixinGenericArgumentFinder.TargetArgumentFinder.FindGenericArgument (typeof (BT3Mixin4));
      Assert.That (thisArgument, Is.SameAs (typeof (BaseType3)));
    }

    [Test]
    public void ThisArgumentFinder_Find_NoMixinBase ()
    {
      var thisArgument = MixinGenericArgumentFinder.TargetArgumentFinder.FindGenericArgument (typeof (object));
      Assert.That (thisArgument, Is.Null);
    }

    [Test]
    public void BaseArgumentFinder_Find ()
    {
      var baseArgument = MixinGenericArgumentFinder.NextArgumentFinder.FindGenericArgument (typeof (BT3Mixin4));
      Assert.That (baseArgument, Is.SameAs (typeof (IBaseType34)));
    }

    [Test]
    public void BaseArgumentFinder_Find_NoMixinBase ()
    {
      var baseArgument = MixinGenericArgumentFinder.NextArgumentFinder.FindGenericArgument (typeof (object));
      Assert.That (baseArgument, Is.Null);
    }

    [Test]
    public void BaseArgumentFinder_Find_NoBaseArgument ()
    {
      var baseArgument = MixinGenericArgumentFinder.NextArgumentFinder.FindGenericArgument (typeof (BT3Mixin2));
      Assert.That (baseArgument, Is.Null);
    }
  }
}
