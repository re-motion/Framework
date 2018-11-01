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
  public class NonOverlappingMixinsTest : OrderingTestBase
  {
    [Test]
    public void NonOverlappingMixins_WithoutDependencies_AreSortedAlphabetically ()
    {
      var instance = BuildMixedInstance<C> (typeof (MixinB), typeof (MixinC), typeof (MixinA));

      Assert.That (instance.Method1 (), Is.EqualTo ("MixinA.Method1 - C.Method1"));
      Assert.That (instance.Method2 (), Is.EqualTo ("MixinB.Method2 - C.Method2"));
      Assert.That (actual: ((IMixinC) instance).Method3 (), expression: Is.EqualTo ("MixinC.Method3"));

      CheckOrderedMixinTypes (instance, typeof (MixinA), typeof (MixinB), typeof (MixinC));
    }

    [Test]
    public void NonOverlappingMixins_WithDependencies ()
    {
      var instance1 = BuildMixedInstance<C> (
          b => b.AddMixinDependency<MixinA, MixinC> ().AddMixinDependency<MixinB, MixinC> ().AddMixinDependency<MixinB, MixinA>(),
          typeof (MixinB), typeof (MixinC), typeof (MixinA));

      Assert.That (instance1.Method1 (), Is.EqualTo ("MixinA.Method1 - C.Method1"));
      Assert.That (instance1.Method2 (), Is.EqualTo ("MixinB.Method2 - C.Method2"));
      Assert.That (((IMixinC) instance1).Method3 (), Is.EqualTo ("MixinC.Method3"));

      CheckOrderedMixinTypes (instance1, typeof (MixinB), typeof (MixinA), typeof (MixinC));

      var instance2 = BuildMixedInstance<C> (
          b => b.AddMixinDependency<MixinC, MixinB> ().AddMixinDependency<MixinB, MixinA> (),
          typeof (MixinB), typeof (MixinC), typeof (MixinA));
      CheckOrderedMixinTypes (instance2, typeof (MixinC), typeof (MixinB), typeof (MixinA));
    }

    [Test]
    public void NonOverlappingMixins_WithSomeDependencies ()
    {
      var instance = BuildMixedInstance<C> (
          b => b.AddMixinDependency<MixinC, MixinA> ().AddMixinDependency<MixinC, MixinB> (),
          typeof (MixinB), typeof (MixinC), typeof (MixinA));

      Assert.That (instance.Method1 (), Is.EqualTo ("MixinA.Method1 - C.Method1"));
      Assert.That (instance.Method2 (), Is.EqualTo ("MixinB.Method2 - C.Method2"));
      Assert.That (((IMixinC) instance).Method3 (), Is.EqualTo ("MixinC.Method3"));

      CheckOrderedMixinTypes (instance, typeof (MixinC), typeof (MixinA), typeof (MixinB));

      var instance2 = BuildMixedInstance<C> (
          b => b.AddMixinDependency<MixinA, MixinB> ().AddMixinDependency<MixinC, MixinB> (),
          typeof (MixinB), typeof (MixinC), typeof (MixinA));
      CheckOrderedMixinTypes (instance2, typeof (MixinA), typeof (MixinC), typeof (MixinB));
    }

    public class C
    {
      public virtual string Method1 () { return "C.Method1"; }
      public virtual string Method2 () { return "C.Method2"; }
    }

    public interface IC
    {
      string Method1 ();
      string Method2 ();
    }

    public class MixinA : Mixin<object, IC>
    {
      [OverrideTarget]
      public string Method1 () { return "MixinA.Method1 - " + Next.Method1(); }
    }

    public class MixinB : Mixin<object, IC>
    {
      [OverrideTarget]
      public string Method2 () { return "MixinB.Method2 - " + Next.Method2 (); }
    }

    public interface IMixinC
    {
      string Method3 ();
    }

    public class MixinC : IMixinC
    {
      public string Method3 () { return "MixinC.Method3"; }
    }
  }
}