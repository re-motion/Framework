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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Definitions.Building;
using Remotion.Mixins.UnitTests.Core.Definitions.TestDomain.MemberFiltering;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.Definitions.Building
{
  [TestFixture]
  public class MemberDefinitionBuilderTest
  {
    [Test]
    public void Methods ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1));

      MethodInfo baseMethod1 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[0]);
      MethodInfo baseMethod2 = typeof (BaseType1).GetMethod ("VirtualMethod", new[] {typeof (string)});
      MethodInfo mixinMethod1 = typeof (BT1Mixin1).GetMethod ("VirtualMethod", new Type[0]);

      Assert.That (targetClass.Methods.ContainsKey (baseMethod1), Is.True);
      Assert.That (targetClass.Methods.ContainsKey (mixinMethod1), Is.False);

      MemberDefinitionBase member = targetClass.Methods[baseMethod1];

      Assert.That (new List<MemberDefinitionBase> (targetClass.GetAllMembers()).Contains (member), Is.True);
      Assert.That (new List<MemberDefinitionBase> (targetClass.Mixins[typeof (BT1Mixin1)].GetAllMembers()).Contains (member), Is.False);

      Assert.That (member.Name, Is.EqualTo ("VirtualMethod"));
      Assert.That (member.FullName, Is.EqualTo (typeof (BaseType1).FullName + ".VirtualMethod"));
      Assert.That (member.IsMethod, Is.True);
      Assert.That (member.IsProperty, Is.False);
      Assert.That (member.IsEvent, Is.False);
      Assert.That (member.DeclaringClass, Is.SameAs (targetClass));
      Assert.That (member.Parent, Is.SameAs (targetClass));

      Assert.That (targetClass.Methods.ContainsKey (baseMethod2), Is.True);
      Assert.That (targetClass.Methods[baseMethod2], Is.Not.SameAs (member));

      MixinDefinition mixin1 = targetClass.Mixins[typeof (BT1Mixin1)];

      Assert.That (mixin1.Methods.ContainsKey (baseMethod1), Is.False);
      Assert.That (mixin1.Methods.ContainsKey (mixinMethod1), Is.True);
      member = mixin1.Methods[mixinMethod1];

      Assert.That (new List<MemberDefinitionBase> (mixin1.GetAllMembers()).Contains (member), Is.True);

      Assert.That (member.Name, Is.EqualTo ("VirtualMethod"));
      Assert.That (member.FullName, Is.EqualTo (typeof (BT1Mixin1).FullName + ".VirtualMethod"));
      Assert.That (member.IsMethod, Is.True);
      Assert.That (member.IsProperty, Is.False);
      Assert.That (member.IsEvent, Is.False);
      Assert.That (member.DeclaringClass, Is.SameAs (mixin1));
    }

    [Test]
    public void Properties ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1));

      PropertyInfo baseProperty = typeof (BaseType1).GetProperty ("VirtualProperty");
      PropertyInfo indexedProperty1 = typeof (BaseType1).GetProperty ("Item", new[] {typeof (int)});
      PropertyInfo indexedProperty2 = typeof (BaseType1).GetProperty ("Item", new[] {typeof (string)});
      PropertyInfo mixinProperty = typeof (BT1Mixin1).GetProperty ("VirtualProperty", new Type[0]);

      Assert.That (targetClass.Properties.ContainsKey (baseProperty), Is.True);
      Assert.That (targetClass.Properties.ContainsKey (indexedProperty1), Is.True);
      Assert.That (targetClass.Properties.ContainsKey (indexedProperty2), Is.True);
      Assert.That (targetClass.Properties.ContainsKey (mixinProperty), Is.False);

      PropertyDefinition member = targetClass.Properties[baseProperty];

      Assert.That (new List<MemberDefinitionBase> (targetClass.GetAllMembers()).Contains (member), Is.True);
      Assert.That (new List<MemberDefinitionBase> (targetClass.Mixins[typeof (BT1Mixin1)].GetAllMembers()).Contains (member), Is.False);

      Assert.That (member.Name, Is.EqualTo ("VirtualProperty"));
      Assert.That (member.FullName, Is.EqualTo (typeof (BaseType1).FullName + ".VirtualProperty"));
      Assert.That (member.IsProperty, Is.True);
      Assert.That (member.IsMethod, Is.False);
      Assert.That (member.IsEvent, Is.False);
      Assert.That (member.DeclaringClass, Is.SameAs (targetClass));
      Assert.That (member.GetMethod, Is.Not.Null);
      Assert.That (member.SetMethod, Is.Not.Null);

      Assert.That (targetClass.Methods.ContainsKey (member.GetMethod.MethodInfo), Is.False);
      Assert.That (targetClass.Methods.ContainsKey (member.SetMethod.MethodInfo), Is.False);

      Assert.That (member.GetMethod.Parent, Is.SameAs (member));
      Assert.That (member.SetMethod.Parent, Is.SameAs (member));

      member = targetClass.Properties[indexedProperty1];
      Assert.That (targetClass.Properties[indexedProperty2], Is.Not.SameAs (member));

      Assert.That (member.GetMethod, Is.Not.Null);
      Assert.That (member.SetMethod, Is.Null);

      member = targetClass.Properties[indexedProperty2];

      Assert.That (member.GetMethod, Is.Null);
      Assert.That (member.SetMethod, Is.Not.Null);

      MixinDefinition mixin1 = targetClass.Mixins[typeof (BT1Mixin1)];

      Assert.That (mixin1.Properties.ContainsKey (baseProperty), Is.False);
      Assert.That (mixin1.Properties.ContainsKey (mixinProperty), Is.True);

      member = mixin1.Properties[mixinProperty];

      Assert.That (new List<MemberDefinitionBase> (mixin1.GetAllMembers()).Contains (member), Is.True);

      Assert.That (member.Name, Is.EqualTo ("VirtualProperty"));
      Assert.That (member.FullName, Is.EqualTo (typeof (BT1Mixin1).FullName + ".VirtualProperty"));
      Assert.That (member.IsProperty, Is.True);
      Assert.That (member.IsMethod, Is.False);
      Assert.That (member.IsEvent, Is.False);
      Assert.That (member.DeclaringClass, Is.SameAs (mixin1));

      Assert.That (member.GetMethod, Is.Null);
      Assert.That (member.SetMethod, Is.Not.Null);
    }

    [Test]
    public void Events ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1));

      EventInfo baseEvent1 = typeof (BaseType1).GetEvent ("VirtualEvent");
      EventInfo baseEvent2 = typeof (BaseType1).GetEvent ("ExplicitEvent");
      EventInfo mixinEvent = typeof (BT1Mixin1).GetEvent ("VirtualEvent");

      Assert.That (targetClass.Events.ContainsKey (baseEvent1), Is.True);
      Assert.That (targetClass.Events.ContainsKey (baseEvent2), Is.True);
      Assert.That (targetClass.Events.ContainsKey (mixinEvent), Is.False);

      EventDefinition member = targetClass.Events[baseEvent1];

      Assert.That (new List<MemberDefinitionBase> (targetClass.GetAllMembers()).Contains (member), Is.True);
      Assert.That (new List<MemberDefinitionBase> (targetClass.Mixins[typeof (BT1Mixin1)].GetAllMembers()).Contains (member), Is.False);

      Assert.That (member.Name, Is.EqualTo ("VirtualEvent"));
      Assert.That (member.FullName, Is.EqualTo (typeof (BaseType1).FullName + ".VirtualEvent"));
      Assert.That (member.IsEvent, Is.True);
      Assert.That (member.IsMethod, Is.False);
      Assert.That (member.IsProperty, Is.False);
      Assert.That (member.DeclaringClass, Is.SameAs (targetClass));
      Assert.That (member.AddMethod, Is.Not.Null);
      Assert.That (member.RemoveMethod, Is.Not.Null);

      Assert.That (targetClass.Methods.ContainsKey (member.AddMethod.MethodInfo), Is.False);
      Assert.That (targetClass.Methods.ContainsKey (member.RemoveMethod.MethodInfo), Is.False);

      Assert.That (member.AddMethod.Parent, Is.SameAs (member));
      Assert.That (member.RemoveMethod.Parent, Is.SameAs (member));

      member = targetClass.Events[baseEvent2];
      Assert.That (member.AddMethod, Is.Not.Null);
      Assert.That (member.RemoveMethod, Is.Not.Null);

      MixinDefinition mixin1 = targetClass.Mixins[typeof (BT1Mixin1)];

      Assert.That (mixin1.Events.ContainsKey (baseEvent1), Is.False);
      Assert.That (mixin1.Events.ContainsKey (mixinEvent), Is.True);

      member = mixin1.Events[mixinEvent];

      Assert.That (new List<MemberDefinitionBase> (mixin1.GetAllMembers()).Contains (member), Is.True);

      Assert.That (member.Name, Is.EqualTo ("VirtualEvent"));
      Assert.That (member.FullName, Is.EqualTo (typeof (BT1Mixin1).FullName + ".VirtualEvent"));
      Assert.That (member.IsEvent, Is.True);
      Assert.That (member.IsMethod, Is.False);
      Assert.That (member.IsProperty, Is.False);
      Assert.That (member.DeclaringClass, Is.SameAs (mixin1));

      Assert.That (member.AddMethod, Is.Not.Null);
      Assert.That (member.RemoveMethod, Is.Not.Null);
    }

    [Test]
    public void ShadowedMembers_AreExplicitlyRetrieved()
    {
      var classDefinition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (DerivedWithNewVirtualMembers));
      var builder = new MemberDefinitionBuilder (classDefinition, mi => true);

      builder.Apply (typeof (DerivedWithNewVirtualMembers));

      const BindingFlags bf = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

      Assert.That (classDefinition.Methods.ContainsKey (typeof (BaseWithVirtualMembers).GetMethod ("Method", bf)), Is.True);
      Assert.That (classDefinition.Methods.ContainsKey (typeof (DerivedWithNewVirtualMembers).GetMethod ("Method", bf)), Is.True);

      Assert.That (classDefinition.Properties.ContainsKey (typeof (BaseWithVirtualMembers).GetProperty ("Property", bf)), Is.True);
      Assert.That (classDefinition.Properties.ContainsKey (typeof (DerivedWithNewVirtualMembers).GetProperty ("Property", bf)), Is.True);

      Assert.That (classDefinition.Events.ContainsKey (typeof (BaseWithVirtualMembers).GetEvent ("Event", bf)), Is.True);
      Assert.That (classDefinition.Events.ContainsKey (typeof (DerivedWithNewVirtualMembers).GetEvent ("Event", bf)), Is.True);
    }

    [Test]
    public void OverriddenMethods_AreFiltered ()
    {
      var classDefinition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (DerivedDerivedWithOverrides));
      var builder = new MemberDefinitionBuilder (classDefinition, mi => true);

      builder.Apply (typeof (DerivedDerivedWithOverrides));

      const BindingFlags bf = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

      Assert.That (classDefinition.Methods.ContainsKey (typeof (DerivedWithNewVirtualMembers).GetMethod ("Method", bf)), Is.False);
      Assert.That (classDefinition.Methods.ContainsKey (typeof (DerivedDerivedWithOverrides).GetMethod ("Method", bf)), Is.True);

      Assert.That (classDefinition.Properties.ContainsKey (typeof (DerivedWithNewVirtualMembers).GetProperty ("Property", bf)), Is.False);
      Assert.That (classDefinition.Properties.ContainsKey (typeof (DerivedDerivedWithOverrides).GetProperty ("Property", bf)), Is.True);

      Assert.That (classDefinition.Events.ContainsKey (typeof (DerivedWithNewVirtualMembers).GetEvent ("Event", bf)), Is.False);
      Assert.That (classDefinition.Events.ContainsKey (typeof (DerivedDerivedWithOverrides).GetEvent ("Event", bf)), Is.True);
    }

    [Test]
    public void ProtectedInternalMembers ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassWithInheritedMethod));
      Assert.That (targetClass.Methods.ContainsKey (typeof (BaseClassWithInheritedMethod).GetMethod ("ProtectedInternalInheritedMethod",
                                                                                                     BindingFlags.Instance | BindingFlags.NonPublic)), Is.True);
    }
  }
}
