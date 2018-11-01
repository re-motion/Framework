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
  public class CyclesTest : OrderingTestBase
  {
    [Test]
    public void TwoCyclicMixins_WithThirdNonCyclic ()
    {
      CheckCycleException (
          () => BuildMixedInstance<C> (
            b => b.AddMixinDependency<MixinB, MixinA>().AddMixinDependency<MixinA, MixinB>(), 
            typeof (MixinB), typeof (MixinA), typeof (MixinC)),
          typeof (C),
          typeof (MixinA), typeof (MixinB));
      CheckCycleException (
          () => BuildMixedInstance<C> (
              b => b.AddMixinDependency<MixinB, MixinA> ().AddMixinDependency<MixinA, MixinB> ().AddMixinDependency<MixinA, MixinC>(),
              typeof (MixinB), typeof (MixinA), typeof (MixinC)),
          typeof (C),
        // The cycle is only between MixinB and MixinA, but the algorithm doesn't isolate the culprits.
          typeof (MixinA), typeof (MixinB), typeof (MixinC));
      CheckCycleException (
          () => BuildMixedInstance<C> (
              b => b.AddMixinDependency<MixinB, MixinA>().AddMixinDependency<MixinA, MixinB>()
                    .AddMixinDependency<MixinA, MixinC>().AddMixinDependency<MixinB, MixinC>(),
              typeof (MixinB), typeof (MixinA), typeof (MixinC)),
          typeof (C),
        // The cycle is only between MixinB and MixinA, but the algorithm doesn't isolate the culprits.
          typeof (MixinA), typeof (MixinB), typeof (MixinC));
      CheckCycleException (
          () => BuildMixedInstance<C> (
              b => b.AddMixinDependency<MixinB, MixinA> ().AddMixinDependency<MixinA, MixinB> ()
                    .AddMixinDependency<MixinC, MixinA> ().AddMixinDependency<MixinC, MixinB> (),
              typeof (MixinB), typeof (MixinA), typeof (MixinC)),
          typeof (C),
          typeof (MixinA), typeof (MixinB));
    }

    [Test]
    public void ThreeCyclicMixins ()
    {
      CheckCycleException (
          () => BuildMixedInstance<C> (
              b => b.AddMixinDependency<MixinB, MixinA> ().AddMixinDependency<MixinA, MixinC> ().AddMixinDependency<MixinC, MixinB>(),
              typeof (MixinB), typeof (MixinA), typeof (MixinC)),
          typeof (C),
          typeof (MixinA), typeof (MixinB), typeof (MixinC));
    }

    [Test]
    public void TwoCycles ()
    {
      CheckCycleException (
          () => BuildMixedInstance<C> (
              b => b.AddMixinDependency<MixinB, MixinA> ().AddMixinDependency<MixinA, MixinB> ()
                    .AddMixinDependency<MixinC, MixinB> ().AddMixinDependency<MixinB, MixinC> (),
              typeof (MixinB), typeof (MixinA), typeof (MixinC)),
          typeof (C),
          typeof (MixinA), typeof (MixinB), typeof (MixinC));
    }

    [Test]
    public void SelfDependency ()
    {
      CheckCycleException (
          () => BuildMixedInstance<C> (
              b => b.AddMixinDependency<MixinB, MixinB> (),
              typeof (MixinB), typeof (MixinA), typeof (MixinC)),
          typeof (C),
          typeof (MixinB));
    }

    public class C
    {
    }

    public class MixinA
    {
    }

    public class MixinB
    {
    }

    public class MixinC
    {
    }
  }
}