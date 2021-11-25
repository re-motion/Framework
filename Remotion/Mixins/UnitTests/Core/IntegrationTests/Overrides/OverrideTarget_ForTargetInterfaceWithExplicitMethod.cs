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
  public class OverrideTarget_ForTargetInterfaceWithExplicitMethod
  {
    [Test]
    [TestCase (typeof(MixinWithImplicitTargetSpecification), "TheMixin.M -> C.M")]
    [TestCase (typeof(MixinWithExplicitTargetSpecification), "TheMixin.M -> C.M", IgnoreReason = "RM-2745")]
    public void InstantiateTargetType_ShouldThrowConfigurationException (Type mixinType, string expectedMethodOutput)
    {
      using (MixinConfiguration.BuildNew().ForClass<C>().AddMixin(mixinType).EnterScope())
      {
        Assert.That(
            () => ObjectFactory.Create<C>(),
            Throws.TypeOf<ConfigurationException>().With.Message.EqualTo(
                $"The member overridden by 'System.String M()' declared by type '{mixinType.FullName}' could not be found. "
                + "Candidates: ."));
      }
    }

    [Test]
    [Ignore ("RM-2745")]
    [TestCase (typeof(MixinWithImplicitTargetSpecification), "TheMixin.M -> C.M")]
    [TestCase (typeof(MixinWithExplicitTargetSpecification), "TheMixin.M -> C.M")]
    public void InstantiateShadowOfTargetType_ShouldThrowConfigurationException (Type mixinType, string expectedMethodOutput)
    {
      using (MixinConfiguration.BuildNew().ForClass<C>().AddMixin(mixinType).EnterScope())
      {
        Assert.That(
            () => ObjectFactory.Create<C_Shadow>(),
            Throws.TypeOf<ConfigurationException>().With.Message.EqualTo(
                $"The member overridden by 'System.String M()' declared by type '{mixinType.FullName}' could not be found. "
                + "Candidates: ."));
      }
    }

    public interface IC
    {
      string M ();
    }

    public class C : IC
    {
      string IC.M ()
      {
        return "C.M";
      }
    }

    public class C_Shadow : C
    {
      public virtual string M ()
      {
        return "C_Shadow.M";
      }
    }

    public class MixinWithImplicitTargetSpecification : Mixin<IC, MixinWithImplicitTargetSpecification.INext>
    {
      public interface INext
      {
        string M ();
      }

      [OverrideTarget]
      public string M ()
      {
        return "TheMixin.M -> " + Next.M();
      }
    }

    public class MixinWithExplicitTargetSpecification : Mixin<object, MixinWithExplicitTargetSpecification.INext>
    {
      public interface INext
      {
        string M ();
      }

      // TODO 2745: Add specification here.
      [OverrideTarget]
      public string M ()
      {
        return "TheMixin.M -> " + Next.M();
      }
    }
  }
}
