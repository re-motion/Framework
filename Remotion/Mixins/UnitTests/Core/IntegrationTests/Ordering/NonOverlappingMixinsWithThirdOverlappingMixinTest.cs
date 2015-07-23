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
  public class NonOverlappingMixinsWithThirdOverlappingMixinTest : OrderingTestBase
  {
    [Test]
    public void ThirdOverlappingMixin_WithoutDependencies_CausesException ()
    {
      CheckOrderingException (
          () => BuildMixedInstance<C> (typeof (MixinB), typeof (MixinC), typeof (MixinA)),
          typeof (C),
          Tuple.Create (new[] { typeof (MixinA), typeof (MixinC) }, "Method1"),
          Tuple.Create (new[] { typeof (MixinB), typeof (MixinC) }, "Method2"));
    }

    [Test]
    public void NonOverlappingMixins_AreSortedAlphabetically_ThirdMixin_SortedAccordingToDependencies_Front ()
    {
      var instance = BuildMixedInstance<C> (
          b => b.AddMixinDependency<MixinC, MixinA>().AddMixinDependency<MixinC, MixinB>(), 
          typeof (MixinB), typeof (MixinC), typeof (MixinA));

      Assert.That (instance.Method1 (), Is.EqualTo ("MixinC.Method1 - MixinA.Method1 - C.Method1"));
      Assert.That (instance.Method2 (), Is.EqualTo ("MixinC.Method2 - MixinB.Method2 - C.Method2"));

      CheckOrderedMixinTypes (instance, typeof (MixinC), typeof (MixinA), typeof (MixinB));
    }

    [Test]
    public void NonOverlappingMixins_AreSortedAlphabetically_ThirdMixin_SortedAccordingToDependencies_Back ()
    {
      var instance = BuildMixedInstance<C> (
          b => b.AddMixinDependency<MixinA, MixinC> ().AddMixinDependency<MixinB, MixinC> (),
          typeof (MixinB), typeof (MixinC), typeof (MixinA));

      Assert.That (instance.Method1 (), Is.EqualTo ("MixinA.Method1 - MixinC.Method1 - C.Method1"));
      Assert.That (instance.Method2 (), Is.EqualTo ("MixinB.Method2 - MixinC.Method2 - C.Method2"));

      CheckOrderedMixinTypes (instance, typeof (MixinA), typeof (MixinB), typeof (MixinC));
    }

    [Test]
    public void NonOverlappingMixins_WithFullDependencySpecification ()
    {
      var instance = BuildMixedInstance<C> (
          b => b.AddMixinDependency<MixinA, MixinC> ().AddMixinDependency<MixinB, MixinC> ().AddMixinDependency<MixinB, MixinA>(),
          typeof (MixinB), typeof (MixinC), typeof (MixinA));

      Assert.That (instance.Method1 (), Is.EqualTo ("MixinA.Method1 - MixinC.Method1 - C.Method1"));
      Assert.That (instance.Method2 (), Is.EqualTo ("MixinB.Method2 - MixinC.Method2 - C.Method2"));

      CheckOrderedMixinTypes (instance, typeof (MixinB), typeof (MixinA), typeof (MixinC));
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

    public class MixinC : Mixin<object, IC>
    {
      [OverrideTarget]
      public string Method1 () { return "MixinC.Method1 - " + Next.Method1 (); }

      [OverrideTarget]
      public string Method2 () { return "MixinC.Method2 - " + Next.Method2 (); }
    }
  }
}