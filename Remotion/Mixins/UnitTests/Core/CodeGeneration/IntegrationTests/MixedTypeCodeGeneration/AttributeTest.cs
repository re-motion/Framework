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
using System.Reflection;
using NUnit.Framework;
using Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration.TestDomain;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class AttributeTest : CodeGenerationBaseTest
  {
    [Test]
    public void AttributesReplicatedFromMixinViaIntroduction ()
    {
      BaseType1 bt1 = CreateMixedObject<BaseType1>(typeof(MixinWithPropsEventAtts));

      Assert.That(bt1.GetType().IsDefined(typeof(BT1Attribute), false), Is.False);
      Assert.That(bt1.GetType().IsDefined(typeof(BT1Attribute), true), Is.True, "Attribute is inherited");
      Assert.That(bt1.GetType().IsDefined(typeof(ReplicatableAttribute), false), Is.True);

      var atts = (ReplicatableAttribute[])bt1.GetType().GetCustomAttributes(typeof(ReplicatableAttribute), false);
      Assert.That(atts.Length, Is.EqualTo(1));
      Assert.That(atts[0].I, Is.EqualTo(4));

      PropertyInfo property = bt1.GetType().GetProperty(typeof(IMixinWithPropsEventsAtts).FullName + ".Property",
                                                         BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.That(property, Is.Not.Null);
      atts = (ReplicatableAttribute[])property.GetCustomAttributes(typeof(ReplicatableAttribute), false);
      Assert.That(atts.Length, Is.EqualTo(1));
      Assert.That(atts[0].S, Is.EqualTo("bla"));
      Assert.That(property.GetGetMethod(true).IsSpecialName, Is.True);
      atts = (ReplicatableAttribute[])property.GetGetMethod(true).GetCustomAttributes(typeof(ReplicatableAttribute), false);
      Assert.That(atts[0].Named2, Is.EqualTo(1.0));

      Assert.That(property.GetSetMethod(true).IsSpecialName, Is.True);
      atts = (ReplicatableAttribute[])property.GetSetMethod(true).GetCustomAttributes(typeof(ReplicatableAttribute), false);
      Assert.That(atts[0].Named2, Is.EqualTo(2.0));

      EventInfo eventInfo = bt1.GetType().GetEvent(typeof(IMixinWithPropsEventsAtts).FullName + ".Event",
                                                    BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.That(eventInfo, Is.Not.Null);
      atts = (ReplicatableAttribute[])eventInfo.GetCustomAttributes(typeof(ReplicatableAttribute), false);
      Assert.That(atts.Length, Is.EqualTo(1));
      Assert.That(atts[0].S, Is.EqualTo("blo"));
      Assert.That(eventInfo.GetAddMethod(true).IsSpecialName, Is.True);
      Assert.That(eventInfo.GetAddMethod(true).IsDefined(typeof(ReplicatableAttribute), false), Is.True);
      Assert.That(eventInfo.GetRemoveMethod(true).IsSpecialName, Is.True);
      Assert.That(eventInfo.GetRemoveMethod(true).IsDefined(typeof(ReplicatableAttribute), false), Is.True);
    }

    [Test]
    public void IntroducedAttributes ()
    {
      Type concreteType = TypeFactory.GetConcreteType(typeof(BaseType1));
      Assert.That(concreteType.GetCustomAttributes(typeof(BT1Attribute), true).Length, Is.EqualTo(1));
      Assert.That(concreteType.GetCustomAttributes(typeof(BT1M1Attribute), true).Length, Is.EqualTo(1));

      MethodInfo bt1VirtualMethod = concreteType.GetMethod("VirtualMethod", Type.EmptyTypes);
      Assert.That(bt1VirtualMethod.GetCustomAttributes(typeof(BT1M1Attribute), true).Length, Is.EqualTo(1));

      PropertyInfo bt1VirtualProperty = concreteType.GetProperty("VirtualProperty",
                                                                  BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
      Assert.That(bt1VirtualProperty.GetCustomAttributes(typeof(BT1M1Attribute), true).Length, Is.EqualTo(1));

      EventInfo bt1VirtualEvent = concreteType.GetEvent("VirtualEvent");
      Assert.That(bt1VirtualEvent.GetCustomAttributes(typeof(BT1M1Attribute), true).Length, Is.EqualTo(1));
    }

    [Test]
    public void IntroducedMultiAttributes ()
    {
      Type concreteType = CreateMixedType(
          typeof(BaseTypeWithAllowMultiple),
          typeof(MixinAddingAllowMultipleToClassAndMember),
          typeof(MixinAddingAllowMultipleToClassAndMember2));

      Assert.That(concreteType.GetCustomAttributes(typeof(MultiAttribute), true).Length, Is.EqualTo(4));
      Assert.That(concreteType.GetMethod("Foo").GetCustomAttributes(typeof(MultiAttribute), true).Length, Is.EqualTo(3));
    }

    [Test]
    public void IntroducedAttributesTargetClassWins ()
    {
      Type concreteType = CreateMixedType(typeof(BaseType1), typeof(MixinAddingBT1Attribute));
      Assert.That(concreteType.GetCustomAttributes(typeof(BT1Attribute), true).Length, Is.EqualTo(1));

      concreteType = CreateMixedType(typeof(BaseType1), typeof(MixinAddingBT1AttributeToMember));
      Assert.That(concreteType.GetMethod("VirtualMethod", Type.EmptyTypes).GetCustomAttributes(typeof(BT1Attribute), true).Length, Is.EqualTo(1));
    }

    private object[] GetRelevantAttributes (ICustomAttributeProvider source)
    {
      object[] attributes = source.GetCustomAttributes(true);
      return Array.FindAll(attributes,
                            o =>
                            o is MultiInheritedAttribute || o is MultiNonInheritedAttribute || o is NonMultiInheritedAttribute
                            || o is NonMultiNonInheritedAttribute);
    }

    [Test]
    public void AttributesOnMixedTypesBehaveLikeOnDerivedTypes ()
    {
      object[] attributes = GetRelevantAttributes(CreateMixedType(typeof(TargetWithoutAttributes), typeof(MixinWithAttributes)));
      Assert.That(attributes.Length, Is.EqualTo(2));
      Assert.That(
          attributes, Is.EquivalentTo(new object[] {new MultiInheritedAttribute(), new NonMultiInheritedAttribute()}));

      attributes = GetRelevantAttributes(CreateMixedType(typeof(TargetWithAttributes), typeof(MixinWithAttributes)));
      Assert.That(attributes.Length, Is.EqualTo(5));

      Assert.That(attributes, Is.EquivalentTo(new object[]
                                                  {
                                                      new MultiInheritedAttribute(), new MultiInheritedAttribute(),
                                                      new NonMultiNonInheritedAttribute(), new MultiNonInheritedAttribute(),
                                                      new NonMultiInheritedAttribute()
                                                  }));
    }

    [Test]
    public void AttributesSuppressedByMixin_AreNotReplicatedFromBaseType ()
    {
      object[] attributes = GetRelevantAttributes(CreateMixedType(typeof(TargetWithNonInheritedAttributes), typeof(MixinSuppressingAllAttributes)));
      Assert.That(attributes.Length, Is.EqualTo(0));
    }

    [Test]
    public void AttributesSuppressedByMixin_AreNotIntroducedFromOtherMixin ()
    {
      object[] attributes =
          GetRelevantAttributes(CreateMixedType(typeof(NullTarget), typeof(MixinSuppressingAllAttributes), typeof(MixinAddingAttributes)));
      Assert.That(attributes.Length, Is.EqualTo(0));
    }

    [Test]
    public void AttributesSuppressedByMixin_AreIntroducedForSameMixin ()
    {
      object[] attributes = GetRelevantAttributes(CreateMixedType(typeof(NullTarget), typeof(MixinSuppressingAllAttributesAddingAttributes)));
      Assert.That(attributes.Length, Is.EqualTo(2));
      Assert.That(attributes, Is.EquivalentTo(new object[] {new MultiInheritedAttribute(), new NonMultiInheritedAttribute()}));
    }

    [Test]
    public void AttributesOnDerivedMethodsBehaveLikeOnDerivedTypes ()
    {
      object[] attributes =
          GetRelevantAttributes(CreateMixedType(typeof(TargetWithoutAttributes), typeof(MixinWithAttributes)).GetMethod("Method"));
      Assert.That(attributes.Length, Is.EqualTo(2));
      Assert.That(
          attributes, Is.EquivalentTo(new object[] {new MultiInheritedAttribute(), new NonMultiInheritedAttribute()}));

      attributes =
          GetRelevantAttributes(CreateMixedType(typeof(TargetWithAttributes), typeof(MixinWithAttributes)).GetMethod("Method"));
      Assert.That(attributes.Length, Is.EqualTo(5));

      Assert.That(attributes, Is.EquivalentTo(new object[]
                                                  {
                                                      new MultiInheritedAttribute(), new MultiInheritedAttribute(),
                                                      new NonMultiNonInheritedAttribute(), new MultiNonInheritedAttribute(),
                                                      new NonMultiInheritedAttribute()
                                                  }));
    }

    [Test]
    public void AttributesOnDerivedPropertiesBehaveLikeMethods ()
    {
      object[] attributes =
          GetRelevantAttributes(CreateMixedType(typeof(TargetWithoutAttributes), typeof(MixinWithAttributes)).GetProperty("Property"));
      Assert.That(attributes.Length, Is.EqualTo(2));
      Assert.That(
          attributes, Is.EquivalentTo(new object[] {new MultiInheritedAttribute(), new NonMultiInheritedAttribute()}));

      attributes =
          GetRelevantAttributes(CreateMixedType(typeof(TargetWithAttributes), typeof(MixinWithAttributes)).GetProperty("Property"));
      Assert.That(attributes.Length, Is.EqualTo(5));

      Assert.That(attributes, Is.EquivalentTo(new object[]
                                                  {
                                                      new MultiInheritedAttribute(), new MultiInheritedAttribute(),
                                                      new NonMultiNonInheritedAttribute(), new MultiNonInheritedAttribute(),
                                                      new NonMultiInheritedAttribute()
                                                  }));
    }

    [Test]
    public void AttributesOnDerivedEventsBehaveLikeMethods ()
    {
      object[] attributes =
          GetRelevantAttributes(CreateMixedType(typeof(TargetWithoutAttributes), typeof(MixinWithAttributes)).GetEvent("Event"));
      Assert.That(attributes.Length, Is.EqualTo(2));
      Assert.That(
          attributes, Is.EquivalentTo(new object[] {new MultiInheritedAttribute(), new NonMultiInheritedAttribute()}));

      attributes =
          GetRelevantAttributes(CreateMixedType(typeof(TargetWithAttributes), typeof(MixinWithAttributes)).GetEvent("Event"));
      Assert.That(attributes.Length, Is.EqualTo(5));

      Assert.That(attributes, Is.EquivalentTo(new object[]
                                                  {
                                                      new MultiInheritedAttribute(), new MultiInheritedAttribute(),
                                                      new NonMultiNonInheritedAttribute(), new MultiNonInheritedAttribute(),
                                                      new NonMultiInheritedAttribute()
                                                  }));
    }
  }
}
