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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.Definitions.Building
{
  [TestFixture]
  public class InterfaceIntroductionDefinitionBuildingIntegrationTest
  {
    [Test]
    public void IntroducedInterface ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(BaseType1));
      MixinDefinition mixin1 = targetClass.Mixins[typeof(BT1Mixin1)];

      Assert.That(mixin1.InterfaceIntroductions.ContainsKey(typeof(IBT1Mixin1)), Is.True);
      InterfaceIntroductionDefinition introducedInterface = mixin1.InterfaceIntroductions[typeof(IBT1Mixin1)];
      Assert.That(introducedInterface.Parent, Is.SameAs(mixin1));
      Assert.That(introducedInterface.Implementer, Is.SameAs(mixin1));
      Assert.That(targetClass.ReceivedInterfaces.ContainsKey(typeof(IBT1Mixin1)), Is.True);
      Assert.That(introducedInterface, Is.SameAs(targetClass.ReceivedInterfaces[typeof(IBT1Mixin1)]));
      Assert.That(introducedInterface.TargetClass, Is.SameAs(targetClass));
    }

    public interface IIntroducedBase
    {
      void Foo ();
      string FooP { get; set; }
      event EventHandler FooE;
    }

    public class BaseIntroducer : IIntroducedBase
    {
      public void Foo ()
      {
        throw new NotImplementedException();
      }

      public string FooP
      {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
      }

      public event EventHandler FooE;
    }

    public interface IIntroducedDerived : IIntroducedBase
    {
    }

    public class DerivedIntroducer : BaseIntroducer, IIntroducedDerived
    {
    }

    [Test]
    public void IntroducedInterfaceOverInheritance ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1>().Clear().AddMixins(typeof(DerivedIntroducer)).EnterScope())
      {
        TargetClassDefinition bt1 = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(BaseType1));
        Assert.That(bt1.ReceivedInterfaces.ContainsKey(typeof(IIntroducedDerived)), Is.True);
        Assert.That(bt1.ReceivedInterfaces.ContainsKey(typeof(IIntroducedBase)), Is.True);

        Assert.That(bt1.ReceivedInterfaces[typeof(IIntroducedDerived)].IntroducedMethods.Count, Is.EqualTo(0));
        Assert.That(bt1.ReceivedInterfaces[typeof(IIntroducedDerived)].IntroducedProperties.Count, Is.EqualTo(0));
        Assert.That(bt1.ReceivedInterfaces[typeof(IIntroducedDerived)].IntroducedEvents.Count, Is.EqualTo(0));

        Assert.That(bt1.ReceivedInterfaces[typeof(IIntroducedBase)].IntroducedMethods.Count, Is.EqualTo(1));
        Assert.That(bt1.ReceivedInterfaces[typeof(IIntroducedBase)].IntroducedProperties.Count, Is.EqualTo(1));
        Assert.That(bt1.ReceivedInterfaces[typeof(IIntroducedBase)].IntroducedEvents.Count, Is.EqualTo(1));

        Assert.That(bt1.ReceivedInterfaces[typeof(IIntroducedBase)].IntroducedMethods[0].InterfaceMember, Is.EqualTo(typeof(IIntroducedBase).GetMethod("Foo")));
        Assert.That(bt1.ReceivedInterfaces[typeof(IIntroducedBase)].IntroducedProperties[0].InterfaceMember, Is.EqualTo(typeof(IIntroducedBase).GetProperty("FooP")));
        Assert.That(bt1.ReceivedInterfaces[typeof(IIntroducedBase)].IntroducedEvents[0].InterfaceMember, Is.EqualTo(typeof(IIntroducedBase).GetEvent("FooE")));

        Assert.That(bt1.ReceivedInterfaces[typeof(IIntroducedBase)].IntroducedMethods[0].ImplementingMember.DeclaringClass, Is.EqualTo(bt1.Mixins[typeof(DerivedIntroducer)]));
        Assert.That(bt1.ReceivedInterfaces[typeof(IIntroducedBase)].IntroducedProperties[0].ImplementingMember.DeclaringClass, Is.EqualTo(bt1.Mixins[typeof(DerivedIntroducer)]));
        Assert.That(bt1.ReceivedInterfaces[typeof(IIntroducedBase)].IntroducedEvents[0].ImplementingMember.DeclaringClass, Is.EqualTo(bt1.Mixins[typeof(DerivedIntroducer)]));
      }
    }

    public class ExplicitBaseIntroducer : IIntroducedBase
    {
      void IIntroducedBase.Foo ()
      {
        throw new NotImplementedException();
      }

      string IIntroducedBase.FooP
      {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
      }

      event EventHandler IIntroducedBase.FooE
      {
        add { throw new NotImplementedException(); }
        remove { throw new NotImplementedException(); }
      }
    }

    public class ExplicitDerivedIntroducer : ExplicitBaseIntroducer, IIntroducedDerived
    {
    }

    [Test]
    public void ExplicitlyIntroducedInterfaceOverInheritance ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1>().Clear().AddMixins(typeof(ExplicitDerivedIntroducer)).EnterScope())
      {
        TargetClassDefinition bt1 = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(BaseType1));
        Assert.That(bt1.ReceivedInterfaces.ContainsKey(typeof(IIntroducedDerived)), Is.True);
        Assert.That(bt1.ReceivedInterfaces.ContainsKey(typeof(IIntroducedBase)), Is.True);

        Assert.That(bt1.ReceivedInterfaces[typeof(IIntroducedDerived)].IntroducedMethods.Count, Is.EqualTo(0));
        Assert.That(bt1.ReceivedInterfaces[typeof(IIntroducedDerived)].IntroducedProperties.Count, Is.EqualTo(0));
        Assert.That(bt1.ReceivedInterfaces[typeof(IIntroducedDerived)].IntroducedEvents.Count, Is.EqualTo(0));

        Assert.That(bt1.ReceivedInterfaces[typeof(IIntroducedBase)].IntroducedMethods.Count, Is.EqualTo(1));
        Assert.That(bt1.ReceivedInterfaces[typeof(IIntroducedBase)].IntroducedProperties.Count, Is.EqualTo(1));
        Assert.That(bt1.ReceivedInterfaces[typeof(IIntroducedBase)].IntroducedEvents.Count, Is.EqualTo(1));

        Assert.That(bt1.ReceivedInterfaces[typeof(IIntroducedBase)].IntroducedMethods[0].InterfaceMember, Is.EqualTo(typeof(IIntroducedBase).GetMethod("Foo")));
        Assert.That(bt1.ReceivedInterfaces[typeof(IIntroducedBase)].IntroducedProperties[0].InterfaceMember, Is.EqualTo(typeof(IIntroducedBase).GetProperty("FooP")));
        Assert.That(bt1.ReceivedInterfaces[typeof(IIntroducedBase)].IntroducedEvents[0].InterfaceMember, Is.EqualTo(typeof(IIntroducedBase).GetEvent("FooE")));

        Assert.That(
            bt1.ReceivedInterfaces[typeof(IIntroducedBase)].IntroducedMethods[0].ImplementingMember.DeclaringClass,
            Is.EqualTo(bt1.Mixins[typeof(ExplicitDerivedIntroducer)]));
        Assert.That(
            bt1.ReceivedInterfaces[typeof(IIntroducedBase)].IntroducedProperties[0].ImplementingMember.DeclaringClass,
            Is.EqualTo(bt1.Mixins[typeof(ExplicitDerivedIntroducer)]));
        Assert.That(
            bt1.ReceivedInterfaces[typeof(IIntroducedBase)].IntroducedEvents[0].ImplementingMember.DeclaringClass,
            Is.EqualTo(bt1.Mixins[typeof(ExplicitDerivedIntroducer)]));
      }
    }

    [Test]
    public void IntroducedMembers ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(BaseType1));
      MixinDefinition mixin1 = targetClass.Mixins[typeof(BT1Mixin1)];
      InterfaceIntroductionDefinition introducedInterface = mixin1.InterfaceIntroductions[typeof(IBT1Mixin1)];

      Assert.That(introducedInterface.IntroducedMethods.ContainsKey(typeof(IBT1Mixin1).GetMethod("IntroducedMethod")), Is.True);
      Assert.That(introducedInterface.IntroducedProperties.ContainsKey(typeof(IBT1Mixin1).GetProperty("IntroducedProperty")), Is.True);
      Assert.That(introducedInterface.IntroducedEvents.ContainsKey(typeof(IBT1Mixin1).GetEvent("IntroducedEvent")), Is.True);

      MethodIntroductionDefinition method = introducedInterface.IntroducedMethods[typeof(IBT1Mixin1).GetMethod("IntroducedMethod")];
      Assert.That(method, Is.Not.EqualTo(mixin1.Methods[typeof(BT1Mixin1).GetMethod("IntroducedMethod")]));
      Assert.That(method.ImplementingMember, Is.SameAs(mixin1.Methods[typeof(BT1Mixin1).GetMethod("IntroducedMethod")]));
      Assert.That(method.DeclaringInterface, Is.SameAs(introducedInterface));
      Assert.That(method.Parent, Is.SameAs(introducedInterface));

      PropertyIntroductionDefinition property = introducedInterface.IntroducedProperties[typeof(IBT1Mixin1).GetProperty("IntroducedProperty")];
      Assert.That(property, Is.Not.EqualTo(mixin1.Properties[typeof(BT1Mixin1).GetProperty("IntroducedProperty")]));
      Assert.That(property.ImplementingMember, Is.SameAs(mixin1.Properties[typeof(BT1Mixin1).GetProperty("IntroducedProperty")]));
      Assert.That(property.DeclaringInterface, Is.SameAs(introducedInterface));
      Assert.That(method.Parent, Is.SameAs(introducedInterface));

      EventIntroductionDefinition eventDefinition = introducedInterface.IntroducedEvents[typeof(IBT1Mixin1).GetEvent("IntroducedEvent")];
      Assert.That(eventDefinition, Is.Not.EqualTo(mixin1.Events[typeof(BT1Mixin1).GetEvent("IntroducedEvent")]));
      Assert.That(eventDefinition.ImplementingMember, Is.SameAs(mixin1.Events[typeof(BT1Mixin1).GetEvent("IntroducedEvent")]));
      Assert.That(eventDefinition.DeclaringInterface, Is.SameAs(introducedInterface));
      Assert.That(method.Parent, Is.SameAs(introducedInterface));
    }

    [Test]
    public void MixinCanImplementMethodsExplicitly ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1>().Clear().AddMixins(typeof(MixinWithExplicitImplementation)).EnterScope())
      {
        TargetClassDefinition bt1 = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(BaseType1));
        Assert.That(bt1.ReceivedInterfaces.ContainsKey(typeof(IExplicit)), Is.True);

        MethodInfo explicitMethod = typeof(MixinWithExplicitImplementation).GetMethod(
            "Remotion.Mixins.UnitTests.Core.TestDomain.IExplicit.Explicit", BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.That(explicitMethod, Is.Not.Null);

        MixinDefinition m1 = bt1.Mixins[typeof(MixinWithExplicitImplementation)];
        Assert.That(m1.Methods.ContainsKey(explicitMethod), Is.True);
      }
    }

    [Test]
    public void IInitializableMixinIsNotIntroduced ()
    {
      Assert.That(DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(BaseType3)).Mixins[typeof(BT3Mixin1)].InterfaceIntroductions[typeof(IInitializableMixin)], Is.Null);
    }

    [Test]
    public void IntroducesGetMethod ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1>().Clear().AddMixins(typeof(MixinImplementingFullPropertiesWithPartialIntroduction)).EnterScope())
      {
        InterfaceIntroductionDefinition introduction =
            DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(BaseType1)).ReceivedInterfaces[typeof(InterfaceWithPartialProperties)];
        PropertyIntroductionDefinition prop1 = introduction.IntroducedProperties[typeof(InterfaceWithPartialProperties).GetProperty("Prop1")];
        PropertyIntroductionDefinition prop2 = introduction.IntroducedProperties[typeof(InterfaceWithPartialProperties).GetProperty("Prop2")];
        Assert.That(prop1.IntroducesGetMethod, Is.True);
        Assert.That(prop1.IntroducesSetMethod, Is.False);
        Assert.That(prop2.IntroducesSetMethod, Is.True);
        Assert.That(prop2.IntroducesGetMethod, Is.False);
      }
    }

    private class BT1Mixin1A : BT1Mixin1
    {
    }

    [Test]
    public void ThrowsOnDoublyIntroducedInterface ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1>().Clear().AddMixins(typeof(BT1Mixin1), typeof(BT1Mixin1A)).EnterScope())
      {
        Assert.That(
            () => DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(BaseType1)),
            Throws.InstanceOf<ConfigurationException>()
                .With.Message.Matches("Two mixins introduce the same interface .* to base class .*"));
      }
    }

    [Test]
    public void InterfaceImplementedByTargetClassCannotBeIntroduced ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<ClassImplementingSimpleInterface>().Clear().AddMixins(typeof(MixinImplementingSimpleInterface)).EnterScope())
      {
        TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(ClassImplementingSimpleInterface));
        Assert.That(definition.ImplementedInterfaces.Contains(typeof(ISimpleInterface)), Is.True);
        Assert.That(definition.ReceivedInterfaces.ContainsKey(typeof(ISimpleInterface)), Is.False);
        Assert.That(definition.Mixins[typeof(MixinImplementingSimpleInterface)].NonInterfaceIntroductions.ContainsKey(typeof(ISimpleInterface)), Is.True);
      }
    }

    [Test]
    public void NonIntroducedInterfaceIsNotImplemented ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget>().Clear().AddMixins(typeof(MixinNonIntroducingSimpleInterface)).EnterScope())
      {
        TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(NullTarget));
        Assert.That(definition.ImplementedInterfaces.Contains(typeof(ISimpleInterface)), Is.False);
        Assert.That(definition.ReceivedInterfaces.ContainsKey(typeof(ISimpleInterface)), Is.False);
        Assert.That(definition.Mixins[typeof(MixinNonIntroducingSimpleInterface)].NonInterfaceIntroductions.ContainsKey(typeof(ISimpleInterface)), Is.True);
      }
    }

    [Test]
    public void ExplicitlyNonIntroducedInterface ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget>().Clear().AddMixins(typeof(MixinNonIntroducingSimpleInterface)).EnterScope())
      {
        TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(NullTarget));
        NonInterfaceIntroductionDefinition nonIntroductionDefinition =
            definition.Mixins[typeof(MixinNonIntroducingSimpleInterface)].NonInterfaceIntroductions[typeof(ISimpleInterface)];
        Assert.That(nonIntroductionDefinition.IsExplicitlySuppressed, Is.True);
        Assert.That(nonIntroductionDefinition.IsShadowed, Is.False);
        Assert.That(nonIntroductionDefinition.InterfaceType, Is.SameAs(typeof(ISimpleInterface)));
        Assert.That(nonIntroductionDefinition.Parent, Is.SameAs(definition.Mixins[typeof(MixinNonIntroducingSimpleInterface)]));
      }
    }

    [Test]
    public void ImplicitlyNonIntroducedInterface ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<ClassImplementingSimpleInterface>().Clear().AddMixins(typeof(MixinImplementingSimpleInterface)).EnterScope())
      {
        TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(ClassImplementingSimpleInterface));
        NonInterfaceIntroductionDefinition nonIntroductionDefinition =
            definition.Mixins[typeof(MixinImplementingSimpleInterface)].NonInterfaceIntroductions[typeof(ISimpleInterface)];
        Assert.That(nonIntroductionDefinition.IsExplicitlySuppressed, Is.False);
        Assert.That(nonIntroductionDefinition.IsShadowed, Is.True);
        Assert.That(nonIntroductionDefinition.InterfaceType, Is.SameAs(typeof(ISimpleInterface)));
        Assert.That(nonIntroductionDefinition.Parent, Is.SameAs(definition.Mixins[typeof(MixinImplementingSimpleInterface)]));
      }
    }

    [Test]
    public void MultipleSimilarInterfaces ()
    {
      var configuration = MixinConfiguration.BuildNew()
          .ForClass<ClassImplementingSimpleInterface>().AddMixin(typeof(MixinIntroducingInterfacesImplementingEachOther<>));
      using (configuration.EnterScope())
      {
        TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(ClassImplementingSimpleInterface));
        MixinDefinition mixinDefinition = definition.GetMixinByConfiguredType(typeof(MixinIntroducingInterfacesImplementingEachOther<>));

        Assert.That(definition.ReceivedInterfaces.ContainsKey(typeof(IList)), Is.True);
        Assert.That(definition.ReceivedInterfaces[typeof(IList)].Implementer, Is.SameAs(mixinDefinition));

        Assert.That(definition.ReceivedInterfaces.ContainsKey(typeof(ICollection<ClassImplementingSimpleInterface>)), Is.True);
        Assert.That(definition.ReceivedInterfaces[typeof(ICollection<ClassImplementingSimpleInterface>)].Implementer, Is.SameAs(mixinDefinition));

        Assert.That(definition.ReceivedInterfaces[typeof(IList)].IntroducedProperties.ContainsKey(typeof(IList).GetProperty("IsReadOnly")), Is.True);
        Assert.That(
            definition
                .ReceivedInterfaces[typeof(ICollection<ClassImplementingSimpleInterface>)]
                .IntroducedProperties.ContainsKey(typeof(ICollection<ClassImplementingSimpleInterface>).GetProperty("IsReadOnly")),
            Is.True);

        Assert.That(definition
            .ReceivedInterfaces[typeof(ICollection<ClassImplementingSimpleInterface>)]
            .IntroducedProperties[typeof(ICollection<ClassImplementingSimpleInterface>).GetProperty("IsReadOnly")]
            .ImplementingMember,
            Is.Not.EqualTo(definition.ReceivedInterfaces[typeof(IList)].IntroducedProperties[typeof(IList).GetProperty("IsReadOnly")].ImplementingMember));
      }
    }

    [Test]
    public void DefaultVisibility_Private ()
    {
      var configuration = MixinConfiguration.BuildNew()
          .ForClass<NullTarget>()
          .AddMixin<MixinIntroducingMembersWithDifferentVisibilities>().WithIntroducedMemberVisibility(MemberVisibility.Private);
      using (configuration.EnterScope())
      {
        TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(NullTarget));
        InterfaceIntroductionDefinition interfaceDefinition = definition.ReceivedInterfaces[typeof(IMixinIntroducingMembersWithDifferentVisibilities)];

        Assert.That(
            interfaceDefinition.IntroducedMethods[typeof(IMixinIntroducingMembersWithDifferentVisibilities).GetMethod("MethodWithDefaultVisibility")].Visibility,
            Is.EqualTo(MemberVisibility.Private));
        Assert.That(
            interfaceDefinition.IntroducedProperties[typeof(IMixinIntroducingMembersWithDifferentVisibilities).GetProperty("PropertyWithDefaultVisibility")].Visibility,
            Is.EqualTo(MemberVisibility.Private));
        Assert.That(
            interfaceDefinition.IntroducedEvents[typeof(IMixinIntroducingMembersWithDifferentVisibilities).GetEvent("EventWithDefaultVisibility")].Visibility,
            Is.EqualTo(MemberVisibility.Private));
      }
    }

    [Test]
    public void DefaultVisibility_Public ()
    {
      var configuration = MixinConfiguration.BuildNew()
          .ForClass<NullTarget>()
          .AddMixin<MixinIntroducingMembersWithDifferentVisibilities>().WithIntroducedMemberVisibility(MemberVisibility.Public);
      using (configuration.EnterScope())
      {
        TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(NullTarget));
        InterfaceIntroductionDefinition interfaceDefinition = definition.ReceivedInterfaces[typeof(IMixinIntroducingMembersWithDifferentVisibilities)];

        Assert.That(
            interfaceDefinition.IntroducedMethods[typeof(IMixinIntroducingMembersWithDifferentVisibilities).GetMethod("MethodWithDefaultVisibility")].Visibility,
            Is.EqualTo(MemberVisibility.Public));
        Assert.That(
            interfaceDefinition.IntroducedProperties[typeof(IMixinIntroducingMembersWithDifferentVisibilities).GetProperty("PropertyWithDefaultVisibility")].Visibility,
            Is.EqualTo(MemberVisibility.Public));
        Assert.That(
            interfaceDefinition.IntroducedEvents[typeof(IMixinIntroducingMembersWithDifferentVisibilities).GetEvent("EventWithDefaultVisibility")].Visibility,
            Is.EqualTo(MemberVisibility.Public));
      }
    }

    [Test]
    public void SpecialVisibility ()
    {
      var configuration = MixinConfiguration.BuildNew()
          .ForClass<NullTarget>()
          .AddMixin<MixinIntroducingMembersWithDifferentVisibilities>().WithIntroducedMemberVisibility(MemberVisibility.Private);
      using (configuration.EnterScope())
      {
        TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(NullTarget));
        InterfaceIntroductionDefinition interfaceDefinition = definition.ReceivedInterfaces[typeof(IMixinIntroducingMembersWithDifferentVisibilities)];

        Assert.That(
            interfaceDefinition.IntroducedMethods[typeof(IMixinIntroducingMembersWithDifferentVisibilities).GetMethod("MethodWithPublicVisibility")].Visibility,
            Is.EqualTo(MemberVisibility.Public));
        Assert.That(
            interfaceDefinition.IntroducedProperties[typeof(IMixinIntroducingMembersWithDifferentVisibilities).GetProperty("PropertyWithPublicVisibility")].Visibility,
            Is.EqualTo(MemberVisibility.Public));
        Assert.That(
            interfaceDefinition.IntroducedEvents[typeof(IMixinIntroducingMembersWithDifferentVisibilities).GetEvent("EventWithPublicVisibility")].Visibility,
            Is.EqualTo(MemberVisibility.Public));
      }
    }
  }
}
