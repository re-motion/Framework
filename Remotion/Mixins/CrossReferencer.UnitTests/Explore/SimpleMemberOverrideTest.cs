// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
using System;
using System.Linq;
using NUnit.Framework;
using Remotion.Mixins;

namespace MixinXRef.UnitTests.Explore
{
  [TestFixture]
  public class SimpleMemberOverrideTest
  {
    [Test]
    public void SimpleOverrideByMixin ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew ()
          .ForClass (typeof (Target)).AddMixin (typeof (MixinOverrideTarget))
          .BuildConfiguration ();
      var targetClassDefiniton = TargetClassDefinitionUtility.GetConfiguration (typeof (Target), mixinConfiguration);
      var targetClassOverrides = targetClassDefiniton.GetAllMembers ().Single (mdb => mdb.Name == "WriteType").Overrides;
      var mixinOverrides = targetClassDefiniton.Mixins[0].GetAllMembers ().Single (mdb => mdb.Name == "WriteType").Overrides;

      Assert.That (targetClassOverrides.Count, Is.EqualTo (1));
      Assert.That (mixinOverrides.Count, Is.EqualTo (0));
      Assert.That (targetClassOverrides[0].DeclaringClass.Type, Is.EqualTo (typeof (MixinOverrideTarget)));
    }

    #region TestDomain for SimpleOverrideByMixin

    public class Target
    {
      public virtual void WriteType ()
      {
        Console.WriteLine (GetType ());
      }
    }

    public class MixinOverrideTarget
    {
      [OverrideTarget]
      public void WriteType ()
      {
        Console.WriteLine (GetType ());
      }
    }

    #endregion

    [Test]
    public void SimpleOverrideByTarget ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew ()
          .ForClass (typeof (TargetOverrideMixin)).AddMixin (typeof (TemplateMixin))
          .BuildConfiguration ();
      var targetClassDefiniton = TargetClassDefinitionUtility.GetConfiguration (typeof (TargetOverrideMixin), mixinConfiguration);
      var targetClassOverrides = targetClassDefiniton.GetAllMembers ().Single (mdb => mdb.Name == "WriteType").Overrides;
      var mixinOverrides = targetClassDefiniton.Mixins[0].GetAllMembers ().Single (mdb => mdb.Name == "WriteType").Overrides;

      Assert.That (targetClassOverrides.Count, Is.EqualTo (0));
      Assert.That (mixinOverrides.Count, Is.EqualTo (1));
      Assert.That (mixinOverrides[0].DeclaringClass.Type, Is.EqualTo (typeof (TargetOverrideMixin)));
    }

    #region TestDomain for SimpleOverrideByTarget

    public class TargetOverrideMixin
    {
      [OverrideMixin]
      public void WriteType ()
      {
        Console.WriteLine (GetType ());
      }
    }

    public class TemplateMixin : Mixin<TargetOverrideMixin>
    {
      public virtual void WriteType ()
      {
        Console.WriteLine (GetType ());
      }
    }

    #endregion
  }
}