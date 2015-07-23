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
  public class OverlappingMixinsWithAlphabeticOrderingTest : OrderingTestBase
  {
    [Test]
    public void TwoOverlappingMixins_WithDependencies ()
    {
      var instance1 = BuildMixedInstance<C> (
          b => b.AddMixinDependency<MixinA, MixinB>(),
          typeof (MixinA), typeof (MixinB));
      Assert.That (instance1.Method1(), Is.EqualTo ("MixinA.Method1 - MixinB.Method1 - C.Method1"));
      CheckOrderedMixinTypes (instance1, typeof (MixinA), typeof (MixinB));

      var instance2 = BuildMixedInstance<C> (
          b => b.AddMixinDependency<MixinB, MixinA> (),
          typeof (MixinA), typeof (MixinB));
      Assert.That (instance2.Method1 (), Is.EqualTo ("MixinB.Method1 - MixinA.Method1 - C.Method1"));
      CheckOrderedMixinTypes (instance2, typeof (MixinB), typeof (MixinA));
    }

    [Test]
    public void TwoOverlappingMixins_WithoutDependencies_AlphabeticOrderingAccepted ()
    {
      var instance1 = BuildMixedInstance<C> (typeof (MixinB), typeof (MixinA));
      Assert.That (instance1.Method1 (), Is.EqualTo ("MixinA.Method1 - MixinB.Method1 - C.Method1"));
      CheckOrderedMixinTypes (instance1, typeof (MixinA), typeof (MixinB));
    }

    [Test]
    public void ThreeOverlappingMixins_WithoutDependencies_AlphabeticOrderingAccepted ()
    {
      var instance = BuildMixedInstance<C> (typeof (MixinC), typeof (MixinA), typeof (MixinB));
      Assert.That (instance.Method1 (), Is.EqualTo ("MixinA.Method1 - MixinB.Method1 - MixinC.Method1 - C.Method1"));
      CheckOrderedMixinTypes (instance, typeof (MixinA), typeof (MixinB), typeof (MixinC));
    }

    [Test]
    public void OverlappingMixins_WithoutDependencies_AlphabeticOrderingNotAccepted_ByOne ()
    {
      var instance = BuildMixedInstance<C> (typeof (MixinB), typeof (MixinA), typeof (MixinD_NoAlphabeticOrdering));
      Assert.That (instance.Method1 (), Is.EqualTo ("MixinA.Method1 - MixinB.Method1 - MixinD_NoAlphabeticOrdering.Method1 - C.Method1"));
      CheckOrderedMixinTypes (instance, typeof (MixinA), typeof (MixinB), typeof (MixinD_NoAlphabeticOrdering));
    }

    [Test]
    public void OverlappingMixins_WithoutDependencies_AlphabeticOrderingNotAccepted_ByTwo ()
    {
      CheckOrderingException (
          () => BuildMixedInstance<C> (typeof (MixinB), typeof (MixinA), typeof (MixinD_NoAlphabeticOrdering), typeof (MixinE_NoAlphabeticOrdering)),
          typeof (C),
          Tuple.Create (new[] { typeof (MixinA), typeof (MixinB), typeof (MixinD_NoAlphabeticOrdering), typeof (MixinE_NoAlphabeticOrdering)}, "Method1"));
    }

    public class C
    {
      public virtual string Method1 () { return "C.Method1"; }
    }

    public interface IC
    {
      string Method1 ();
    }

    [AcceptsAlphabeticOrdering]
    public class MixinA : Mixin<object, IC>
    {
      [OverrideTarget]
      public string Method1 () { return "MixinA.Method1 - " + Next.Method1(); }
    }

    [AcceptsAlphabeticOrdering]
    public class MixinB : Mixin<object, IC>
    {
      [OverrideTarget]
      public string Method1 () { return "MixinB.Method1 - " + Next.Method1 (); }
    }

    [AcceptsAlphabeticOrdering]
    public class MixinC : Mixin<object, IC>
    {
      [OverrideTarget]
      public string Method1 () { return "MixinC.Method1 - " + Next.Method1 (); }
    }

    public class MixinD_NoAlphabeticOrdering : Mixin<object, IC>
    {
      [OverrideTarget]
      public string Method1 () { return "MixinD_NoAlphabeticOrdering.Method1 - " + Next.Method1 (); }
    }

    public class MixinE_NoAlphabeticOrdering : Mixin<object, IC>
    {
      [OverrideTarget]
      public string Method1 () { return "MixinE_NoAlphabeticOrdering.Method1 - " + Next.Method1 (); }
    }

  }
}