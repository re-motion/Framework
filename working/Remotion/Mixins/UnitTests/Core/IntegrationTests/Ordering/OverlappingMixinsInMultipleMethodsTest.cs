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
  public class OverlappingMixinsInMultipleMethodsTest : OrderingTestBase
  {
    [Test]
    public void MixinsOverlappingInMultipleMethods_WithDependencies ()
    {
      var instance1 = BuildMixedInstance<C> (
          b => b.AddMixinDependency<MixinB, MixinA> ().AddMixinDependency<MixinC, MixinB> (),
          typeof (MixinA), typeof (MixinB), typeof (MixinC));
      Assert.That (instance1.Method1 (), Is.EqualTo ("MixinC.Method1 - MixinA.Method1 - C.Method1"));
      Assert.That (instance1.Method2 (), Is.EqualTo ("MixinB.Method2 - MixinA.Method2 - C.Method2"));
      Assert.That (instance1.Method4 (), Is.EqualTo ("MixinC.Method4 - MixinB.Method4 - MixinA.Method4 - C.Method4"));
      CheckOrderedMixinTypes (instance1, typeof (MixinC), typeof (MixinB), typeof (MixinA));
    }

    [Test]
    public void TwoOverlappingMixins_WithoutDependencies_CauseException ()
    {
      CheckOrderingException (
          () => BuildMixedInstance<C> (typeof (MixinA), typeof (MixinB), typeof (MixinC)),
          typeof (C), 
          Tuple.Create (new[] { typeof (MixinA), typeof (MixinC) }, "Method1"),
          Tuple.Create (new[] { typeof (MixinA), typeof (MixinB) }, "Method2"),
          Tuple.Create (new[] { typeof (MixinA), typeof (MixinB), typeof (MixinC) }, "Method4"),
          Tuple.Create (new[] { typeof (MixinB), typeof (MixinC) }, "Method3"));
    }

    public class C
    {
      public virtual string Method1 () { return "C.Method1"; }
      public virtual string Method2 () { return "C.Method2"; }
      public virtual string Method3 () { return "C.Method3"; }
      public virtual string Method4 () { return "C.Method4"; }
    }

    public interface IC
    {
      string Method1 ();
      string Method2 ();
      string Method3 ();
      string Method4 ();
    }

    public class MixinA : Mixin<object, IC>
    {
      [OverrideTarget]
      public string Method1 () { return "MixinA.Method1 - " + Next.Method1(); }
      [OverrideTarget]
      public string Method2 () { return "MixinA.Method2 - " + Next.Method2 (); }
      [OverrideTarget]
      public string Method4 () { return "MixinA.Method4 - " + Next.Method4 (); }
    }

    public class MixinB : Mixin<object, IC>
    {
      [OverrideTarget]
      public string Method2 () { return "MixinB.Method2 - " + Next.Method2 (); }
      [OverrideTarget]
      public string Method3 () { return "MixinB.Method3 - " + Next.Method3 (); }
      [OverrideTarget]
      public string Method4 () { return "MixinB.Method4 - " + Next.Method4 (); }
    }

    public class MixinC : Mixin<object, IC>
    {
      [OverrideTarget]
      public string Method1 () { return "MixinC.Method1 - " + Next.Method1 (); }
      [OverrideTarget]
      public string Method3 () { return "MixinC.Method3 - " + Next.Method3 (); }
      [OverrideTarget]
      public string Method4 () { return "MixinC.Method4 - " + Next.Method4 (); }
    }
  }
}