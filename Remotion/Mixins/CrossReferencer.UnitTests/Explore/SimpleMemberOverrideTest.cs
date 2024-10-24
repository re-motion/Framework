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
using System.Linq;
using NUnit.Framework;
using Remotion.Mixins.Definitions;

namespace Remotion.Mixins.CrossReferencer.UnitTests.Explore
{
  [TestFixture]
  public class SimpleMemberOverrideTest
  {
    [Test]
    public void SimpleOverrideByMixin ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass(typeof(Target)).AddMixin(typeof(MixinOverrideTarget))
          .BuildConfiguration();
      var targetClassDefinition = TargetClassDefinitionFactory.CreateWithoutValidation(mixinConfiguration.ClassContexts.GetExact(typeof(Target)));
      var targetClassOverrides = targetClassDefinition.GetAllMembers().Single(mdb => mdb.Name == "WriteType").Overrides;
      var mixinOverrides = targetClassDefinition.Mixins[0].GetAllMembers().Single(mdb => mdb.Name == "WriteType").Overrides;

      Assert.That(targetClassOverrides.Count, Is.EqualTo(1));
      Assert.That(mixinOverrides.Count, Is.EqualTo(0));
      Assert.That(targetClassOverrides[0].DeclaringClass.Type, Is.EqualTo(typeof(MixinOverrideTarget)));
    }

    #region TestDomain for SimpleOverrideByMixin

    public class Target
    {
      public virtual void WriteType ()
      {
        Console.WriteLine(GetType());
      }
    }

    public class MixinOverrideTarget
    {
      [OverrideTarget]
      public void WriteType ()
      {
        Console.WriteLine(GetType());
      }
    }

    #endregion

    [Test]
    public void SimpleOverrideByTarget ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass(typeof(TargetOverrideMixin)).AddMixin(typeof(TemplateMixin))
          .BuildConfiguration();
      var targetClassDefinition = TargetClassDefinitionFactory.CreateWithoutValidation(mixinConfiguration.ClassContexts.GetExact(typeof(TargetOverrideMixin)));
      var targetClassOverrides = targetClassDefinition.GetAllMembers().Single(mdb => mdb.Name == "WriteType").Overrides;
      var mixinOverrides = targetClassDefinition.Mixins[0].GetAllMembers().Single(mdb => mdb.Name == "WriteType").Overrides;

      Assert.That(targetClassOverrides.Count, Is.EqualTo(0));
      Assert.That(mixinOverrides.Count, Is.EqualTo(1));
      Assert.That(mixinOverrides[0].DeclaringClass.Type, Is.EqualTo(typeof(TargetOverrideMixin)));
    }

    #region TestDomain for SimpleOverrideByTarget

    public class TargetOverrideMixin
    {
      [OverrideMixin]
      public void WriteType ()
      {
        Console.WriteLine(GetType());
      }
    }

    public class TemplateMixin : Mixin<TargetOverrideMixin>
    {
      public virtual void WriteType ()
      {
        Console.WriteLine(GetType());
      }
    }

    #endregion
  }
}
