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
  public class OrderingViaTNextTest : OrderingTestBase
  {
    [Test]
    public void NextDependency ()
    {
      var instance = BuildMixedInstance<C> (typeof (MixinA), typeof (MixinB_WithInterfaceDependency));
      Assert.That (instance.Method1 (), Is.EqualTo ("MixinB_WithInterfaceDependency.Method1 - MixinA.Method1 - C.Method1"));
    }

    [Test]
    public void GenericNextDependency ()
    {
      var instance = BuildMixedInstance<C> (typeof (MixinA), typeof (MixinB_WithGenericDependency<>));
      Assert.That (instance.Method1 (), Is.EqualTo ("MixinB_WithGenericDependency.Method1 - MixinA.Method1 - C.Method1"));
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

    public class MixinA : Mixin<object, IC>, IMixinA
    {
      [OverrideTarget]
      public string Method1 () { return "MixinA.Method1 - " + Next.Method1 (); }
    }

    public class MixinB_WithInterfaceDependency : Mixin<object, MixinB_WithInterfaceDependency.IMixinBNextDependencies>
    {
      public interface IMixinBNextDependencies : IC, IMixinA
      {
      }

      [OverrideTarget]
      public string Method1 () { return "MixinB_WithInterfaceDependency.Method1 - " + Next.Method1 (); }
    }

    public interface IMixinB_WithGenericDependency_NextDependencies : IC, IMixinA
    {
    }

    public class MixinB_WithGenericDependency<[BindToConstraints] TNext> : Mixin<object, TNext>
      where TNext : class, IMixinB_WithGenericDependency_NextDependencies
    {
      [OverrideTarget]
      public string Method1 () { return "MixinB_WithGenericDependency.Method1 - " + Next.Method1 (); }
    }
  }
}