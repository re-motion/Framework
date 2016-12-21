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

namespace Remotion.Mixins.UnitTests.Core.IntegrationTests.Overrides
{
  [TestFixture]
  [Ignore ("TODO 2745")]
  public class OverridingTargetMethodsWithShadowing
  {
    [Test]
    public void OverridingShadowedMethod_WithoutTargetSpecification_ShouldOverrideMostDerived ()
    {
      using (MixinConfiguration.BuildNew().ForClass<C>().AddMixin<MixinWithoutTargetSpecification>().EnterScope())
      {
        var instance = ObjectFactory.Create<D>();
        Assert.That (instance.M (), Is.EqualTo ("TheMixin.M -> D.M"));
      }
    }

    [Test]
    public void OverridingShadowedMethod_WithTargetSpecification_ShouldOverrideSpecified ()
    {
      using (MixinConfiguration.BuildNew().ForClass<C>().AddMixin<MixinWithTargetSpecification>().EnterScope())
      {
        var instance = ObjectFactory.Create<D> ();
        Assert.That (instance.M (), Is.EqualTo ("TheMixin.M -> C.M"));
      }
    }

    public class C
    {
      public virtual string M ()
      {
        return "C.M";
      }
    }

    public class D : C
    {
      public new virtual string M ()
      {
        return "D.M";
      }
    }

    public class MixinWithoutTargetSpecification : Mixin<C, MixinWithoutTargetSpecification.IC>
    {
      public interface IC
      {
        string M ();
      }

      [OverrideTarget]
      public string M ()
      {
        return "TheMixin.M -> " + Next.M ();
      }
    }

    public class MixinWithTargetSpecification : Mixin<C, MixinWithTargetSpecification.IC>
    {
      public interface IC
      {
        string M ();
      }

      // TODO 2745: Add specification here.
      [OverrideTarget]
      public string M ()
      {
        return "TheMixin.M -> " + Next.M ();
      }
    } 
  }
}