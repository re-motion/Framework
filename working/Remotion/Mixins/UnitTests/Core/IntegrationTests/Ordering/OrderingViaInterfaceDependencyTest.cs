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

namespace Remotion.Mixins.UnitTests.Core.IntegrationTests.Ordering
{
  [TestFixture]
  public class OrderingViaInterfaceDependencyTest : OrderingTestBase
  {
    [Test]
    public void InterfaceDependency ()
    {
      var instance1 = BuildMixedInstance<C> (
          b => b.AddMixinDependency<MixinA, IMixinB> (),
          typeof (MixinA), typeof (MixinB));
      Assert.That (instance1.Method1 (), Is.EqualTo ("MixinA.Method1 - MixinB.Method1 - C.Method1"));

      var instance2 = BuildMixedInstance<C> (
          b => b.AddMixinDependency<MixinB, IMixinA> (),
          typeof (MixinA), typeof (MixinB));
      Assert.That (instance2.Method1 (), Is.EqualTo ("MixinB.Method1 - MixinA.Method1 - C.Method1"));
    }

    public class C
    {
      public virtual string Method1 () { return "C.Method1"; }
    }

    public interface IC
    {
      string Method1 ();
    }

    public interface IMixinA { }
    public interface IMixinB { }

    public class MixinA : Mixin<object, IC>, IMixinA
    {
      [OverrideTarget]
      public string Method1 () { return "MixinA.Method1 - " + Next.Method1 (); }
    }

    public class MixinB : Mixin<object, IC>, IMixinB
    {
      [OverrideTarget]
      public string Method1 () { return "MixinB.Method1 - " + Next.Method1 (); }
    }
  }
}