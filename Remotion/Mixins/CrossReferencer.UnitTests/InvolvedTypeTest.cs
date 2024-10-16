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
using System.Reflection;
using NUnit.Framework;
using Remotion.Mixins.CrossReferencer.UnitTests.TestDomain;
using Remotion.Mixins.Definitions;

namespace Remotion.Mixins.CrossReferencer.UnitTests
{
  [TestFixture]
  public class InvolvedTypeTest
  {
    [Test]
    public void Equals_True ()
    {
      var type1 = new InvolvedType(typeof(TargetClass1));
      type1.ClassContext = ClassContextObjectMother.Create(typeof(TargetClass1));
      var type2 = new InvolvedType(typeof(TargetClass1));
      type2.ClassContext = ClassContextObjectMother.Create(typeof(TargetClass1));

      Assert.That(type1, Is.EqualTo(type2));
    }

    [Test]
    public void Equals_False_TypeDoesntMatch ()
    {
      var type1 = new InvolvedType(typeof(TargetClass1));
      var type2 = new InvolvedType(typeof(TargetClass2));

      Assert.That(type1, Is.Not.EqualTo(type2));
    }

    [Test]
    public void Equals_False_IsTargetDoesntMatch ()
    {
      var type1 = new InvolvedType(typeof(TargetClass1));
      type1.ClassContext = ClassContextObjectMother.Create(typeof(TargetClass1));
      var type2 = new InvolvedType(typeof(TargetClass1));

      Assert.That(type1, Is.Not.EqualTo(type2));
    }

    [Test]
    public void Equals_False_IsMixinDoesntMatch ()
    {
      var type1 = new InvolvedType(typeof(TargetClass1));
      var type2 = new InvolvedType(typeof(Mixin1));
      type2.TargetTypes.Add(new InvolvedType(typeof(TargetClass1)), null);

      Assert.That(type1, Is.Not.EqualTo(type2));
    }

    [Test]
    public void Equals_False_ClassContextDoesntMatch ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<TargetClass1>().AddMixin<Mixin1>()
          .ForClass<TargetClass2>().AddMixin<Mixin2>()
          .BuildConfiguration();

      var type1 = new InvolvedType(typeof(TargetClass1));
      var type2 = new InvolvedType(typeof(TargetClass1));

      type1.ClassContext = mixinConfiguration.ClassContexts.First();
      type2.ClassContext = mixinConfiguration.ClassContexts.Last();

      Assert.That(type1, Is.Not.EqualTo(type2));
    }

    [Test]
    public void Equals_False_TargetClassDefintionDoesntMatch ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<TargetClass1>().AddMixin<Mixin1>()
          .ForClass<TargetClass2>().AddMixin<Mixin2>()
          .BuildConfiguration();

      var type1 = new InvolvedType(typeof(TargetClass1));
      var type2 = new InvolvedType(typeof(TargetClass1));

      type1.TargetClassDefinition = TargetClassDefinitionFactory.CreateWithoutValidation(mixinConfiguration.ClassContexts.GetExact(typeof(TargetClass1)));
      type2.TargetClassDefinition = TargetClassDefinitionFactory.CreateWithoutValidation(mixinConfiguration.ClassContexts.GetExact(typeof(TargetClass2)));

      Assert.That(type1, Is.Not.EqualTo(type2));
    }

    [Test]
    public void GetHashCode_EqualObjects ()
    {
      var type1 = new InvolvedType(typeof(TargetClass1));
      var type2 = new InvolvedType(typeof(TargetClass1));

      Assert.That(type1.GetHashCode(), Is.EqualTo(type2.GetHashCode()));
    }

    [Test]
    public void ClassContextProperty_ForTargetClass ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<TargetClass1>().AddMixin<Mixin1>()
          .BuildConfiguration();

      var type1 = new InvolvedType(typeof(TargetClass1));
      type1.ClassContext = mixinConfiguration.ClassContexts.First();

      Assert.That(type1.IsTarget, Is.True);
      Assert.That(type1.ClassContext, Is.Not.Null);
    }

    [Test]
    public void ClassContextProperty_ForNonTargetClass ()
    {
      var type1 = new InvolvedType(typeof(object));

      Assert.That(type1.IsTarget, Is.False);
    }

    [Test]
    public void TargetClassDefinitionProperty_ForNonGenericTargetClass ()
    {
      var type = typeof(TargetClass1);
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass(type).AddMixin<Mixin1>()
          .BuildConfiguration();

      var type1 = new InvolvedType(type);
      type1.ClassContext = mixinConfiguration.ClassContexts.First();
      type1.TargetClassDefinition = TargetClassDefinitionFactory.CreateWithoutValidation(mixinConfiguration.ClassContexts.GetExact(type));

      Assert.That(type1.IsTarget, Is.True);
      Assert.That(type1.ClassContext, Is.Not.Null);
    }

    [Test]
    public void TargetClassDefinitionProperty_ForNonTargetClass ()
    {
      var type1 = new InvolvedType(typeof(object));

      Assert.That(type1.IsTarget, Is.False);
    }

    [Test]
    public void TargetClassDefinitionProperty_ForGenericTargetClass ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass(typeof(GenericTarget<,>)).AddMixin<Mixin1>()
          .BuildConfiguration();

      var type1 = new InvolvedType(typeof(GenericTarget<,>));
      type1.ClassContext = mixinConfiguration.ClassContexts.First();

      Assert.That(type1.IsTarget, Is.True);
    }

    [Test]
    public void MixinContextsProperty_ForNonMixin ()
    {
      var type1 = new InvolvedType(typeof(object));

      Assert.That(type1.IsMixin, Is.False);
      Assert.That(type1.TargetTypes.Count, Is.EqualTo(0));
    }

    [Test]
    public void MixinContextsProperty_ForMixin ()
    {
      var type1 = new InvolvedType(typeof(object));
      type1.TargetTypes.Add(new InvolvedType(typeof(TargetClass1)), null);

      Assert.That(type1.IsMixin, Is.True);
      Assert.That(type1.TargetTypes.Count, Is.GreaterThan(0));
    }

    [Test]
    public void GetMembers_ForClassWithNonPublicProperty ()
    {
      var type = new InvolvedType(typeof(ClassWithNonPublicProperty));

      var member = type.Members.FirstOrDefault(m => ((MemberInfo)m.MemberInfo).Name == "PropertyName");
      Assert.That(member, Is.Not.Null);
      var subMemberInfos = member.SubMemberInfos.ToArray();
      Assert.That(subMemberInfos.Length, Is.EqualTo(2));
      Assert.That((MemberInfo)(subMemberInfos[0]), Is.Not.Null);
      Assert.That((MemberInfo)(subMemberInfos[1]), Is.Not.Null);
    }

    [Test]
    public void GetMembers_ForClassWithNonPublicEvent ()
    {
      var type = new InvolvedType(typeof(ClassWithNonPublicEvent));

      var member = type.Members.FirstOrDefault(m => ((MemberInfo)m.MemberInfo).Name == "EventName");
      Assert.That(member, Is.Not.Null);
      var subMemberInfos = member.SubMemberInfos.ToArray();
      Assert.That(subMemberInfos.Length, Is.EqualTo(2));
      Assert.That((MemberInfo)(subMemberInfos[0]), Is.Not.Null);
      Assert.That((MemberInfo)(subMemberInfos[1]), Is.Not.Null);
    }
  }
}
