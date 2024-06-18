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
using System.Linq;
using System.Reflection;
using MixinXRef.Reflection;
using MixinXRef.Reflection.Utility;
using MixinXRef.UnitTests.TestDomain;
using MixinXRef.Utility;
using NUnit.Framework;
using Remotion.Mixins;

namespace MixinXRef.UnitTests.Reflection
{
  [TestFixture]
  public class MemberInfoEqualityUtilityTest
  {
    [Test]
    public void MemberInfo_Equals_False ()
    {
      var targetType = typeof (BaseMemberOverrideTestClass.Target);

      var mixinConfiguration =
          MixinConfiguration.BuildNew ()
              .ForClass<BaseMemberOverrideTestClass.Target> ().AddMixin<BaseMemberOverrideTestClass.Mixin1> ()
              .BuildConfiguration ();
      var targetClassDefinition = new ReflectedObject (TargetClassDefinitionUtility.GetConfiguration (targetType, mixinConfiguration));
      var involvedType = new InvolvedType (targetType)
                         {
                           TargetClassDefinition = targetClassDefinition,
                           ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First ())
                         };

      var memberInfo1 = targetType.GetMember ("OverriddenMethod")[0];

      var output =
        involvedType.TargetClassDefinition.CallMethod ("GetAllMembers").SingleOrDefault (
          mdb => mdb.GetProperty ("MemberInfo").To<MemberInfo> () == memberInfo1);

      Assert.That (output, Is.Null);
    }

    [Test]
    public void MemberEquals_True ()
    {
      var targetType = typeof (BaseMemberOverrideTestClass.Target);

      var mixinConfiguration =
          MixinConfiguration.BuildNew ()
              .ForClass<BaseMemberOverrideTestClass.Target> ().AddMixin<BaseMemberOverrideTestClass.Mixin1> ()
              .BuildConfiguration ();
      var targetClassDefinition = new ReflectedObject (TargetClassDefinitionUtility.GetConfiguration (targetType, mixinConfiguration));
      var involvedType = new InvolvedType (targetType)
                         {
                           TargetClassDefinition = targetClassDefinition,
                           ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First ())
                         };

      var memberInfo1 = targetType.GetMember ("OverriddenMethod")[0];

      var output =
        involvedType.TargetClassDefinition.CallMethod ("GetAllMembers").SingleOrDefault (
          mdb => MemberInfoEqualityUtility.MemberEquals (mdb.GetProperty ("MemberInfo").To<MemberInfo> (), memberInfo1));

      Assert.That (output.To<object> (), Is.Not.Null);
    }

    [Test]
    public void MemberEquals_False ()
    {
      var targetType = typeof (BaseMemberOverrideTestClass.Target);

      var mixinConfiguration =
          MixinConfiguration.BuildNew ()
              .ForClass<BaseMemberOverrideTestClass.Target> ().AddMixin<BaseMemberOverrideTestClass.Mixin1> ()
              .BuildConfiguration ();
      var targetClassDefinition = new ReflectedObject (TargetClassDefinitionUtility.GetConfiguration (targetType, mixinConfiguration));
      var involvedType = new InvolvedType (targetType)
      {
        TargetClassDefinition = targetClassDefinition,
        ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First ())
      };

      var memberInfo1 = typeof (HiddenMemberTestClass.Target).GetMember ("HiddenMethod")[0];

      var output =
        involvedType.TargetClassDefinition.CallMethod ("GetAllMembers").SingleOrDefault (
          mdb => MemberInfoEqualityUtility.MemberEquals (mdb.GetProperty ("MemberInfo").To<MemberInfo> (), memberInfo1));

      Assert.That (output, Is.Null);
    }
  }
}