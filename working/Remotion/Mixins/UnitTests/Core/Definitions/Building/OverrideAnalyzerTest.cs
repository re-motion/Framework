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
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.Definitions.Building
{
  [TestFixture]
  public class OverrideAnalyzerTest
  {
    [Test]
    public void MethodOverrides ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1));
      MixinDefinition mixin1 = targetClass.Mixins[typeof (BT1Mixin1)];
      MixinDefinition mixin2 = targetClass.Mixins[typeof (BT1Mixin2)];

      Assert.That (mixin1.HasOverriddenMembers(), Is.False);
      Assert.That (mixin2.HasOverriddenMembers (), Is.False);
      Assert.That (targetClass.HasOverriddenMembers (), Is.True);

      MethodInfo baseMethod1 = typeof (BaseType1).GetMethod ("VirtualMethod", new Type[0]);
      MethodInfo baseMethod2 = typeof (BaseType1).GetMethod ("VirtualMethod", new[] {typeof (string)});
      MethodInfo mixinMethod1 = typeof (BT1Mixin1).GetMethod ("VirtualMethod", new Type[0]);

      MethodDefinition overridden = targetClass.Methods[baseMethod1];

      Assert.That (overridden.Overrides.ContainsKey (typeof (BT1Mixin1)), Is.True);
      MethodDefinition overrider = overridden.Overrides[typeof (BT1Mixin1)];

      Assert.That (mixin1.Methods[mixinMethod1], Is.SameAs (overrider));
      Assert.That (overrider.Base, Is.Not.Null);
      Assert.That (overrider.Base, Is.SameAs (overridden));

      MethodDefinition notOverridden = targetClass.Methods[baseMethod2];
      Assert.That (notOverridden.Overrides.Count, Is.EqualTo (0));

      Assert.That (overridden.Overrides.ContainsKey (typeof (BT1Mixin2)), Is.True);
      overrider = overridden.Overrides[typeof (BT1Mixin2)];

      Assert.That (new List<MemberDefinitionBase> (mixin2.GetAllOverrides()).Contains (overrider), Is.True);
      Assert.That (overrider.Base, Is.SameAs (overridden));
    }

    [Test]
    public void PropertyOverrides ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1));
      MixinDefinition mixin1 = targetClass.Mixins[typeof (BT1Mixin1)];
      MixinDefinition mixin2 = targetClass.Mixins[typeof (BT1Mixin2)];

      PropertyInfo baseProperty1 = typeof (BaseType1).GetProperty ("VirtualProperty");
      PropertyInfo baseProperty2 = typeof (BaseType1).GetProperty ("Item", new[] {typeof (string)});
      PropertyInfo mixinProperty1 = typeof (BT1Mixin1).GetProperty ("VirtualProperty");

      PropertyDefinition overridden = targetClass.Properties[baseProperty1];

      Assert.That (overridden.Overrides.ContainsKey (typeof (BT1Mixin1)), Is.True);

      PropertyDefinition overrider = overridden.Overrides[typeof (BT1Mixin1)];

      Assert.That (mixin1.Properties[mixinProperty1], Is.SameAs (overrider));
      Assert.That (overrider.Base, Is.Not.Null);
      Assert.That (overrider.Base, Is.SameAs (overridden));
      Assert.That (overrider.SetMethod.Base, Is.SameAs (overridden.SetMethod));

      PropertyDefinition notOverridden = targetClass.Properties[baseProperty2];
      Assert.That (notOverridden.Overrides.Count, Is.EqualTo (0));

      Assert.That (overridden.Overrides.ContainsKey (typeof (BT1Mixin2)), Is.True);
      overrider = overridden.Overrides[typeof (BT1Mixin2)];

      Assert.That (new List<MemberDefinitionBase> (mixin2.GetAllOverrides()).Contains (overrider), Is.True);
      Assert.That (overrider.Base, Is.SameAs (overridden));
      Assert.That (overrider.GetMethod.Base, Is.SameAs (overridden.GetMethod));
    }

    [Test]
    public void EventOverrides ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1));
      MixinDefinition mixin1 = targetClass.Mixins[typeof (BT1Mixin1)];
      MixinDefinition mixin2 = targetClass.Mixins[typeof (BT1Mixin2)];

      EventInfo baseEvent1 = typeof (BaseType1).GetEvent ("VirtualEvent");
      EventInfo baseEvent2 = typeof (BaseType1).GetEvent ("ExplicitEvent");
      EventInfo mixinEvent1 = typeof (BT1Mixin1).GetEvent ("VirtualEvent");

      EventDefinition overridden = targetClass.Events[baseEvent1];

      Assert.That (overridden.Overrides.ContainsKey (typeof (BT1Mixin1)), Is.True);

      EventDefinition overrider = overridden.Overrides[typeof (BT1Mixin1)];

      Assert.That (mixin1.Events[mixinEvent1], Is.SameAs (overrider));
      Assert.That (overrider.Base, Is.Not.Null);
      Assert.That (overrider.Base, Is.SameAs (overridden));
      Assert.That (overrider.RemoveMethod.Base, Is.SameAs (overridden.RemoveMethod));
      Assert.That (overrider.AddMethod.Base, Is.SameAs (overridden.AddMethod));

      EventDefinition notOverridden = targetClass.Events[baseEvent2];
      Assert.That (notOverridden.Overrides.Count, Is.EqualTo (0));

      Assert.That (overridden.Overrides.ContainsKey (typeof (BT1Mixin2)), Is.True);
      overrider = overridden.Overrides[typeof (BT1Mixin2)];

      Assert.That (new List<MemberDefinitionBase> (mixin2.GetAllOverrides()).Contains (overrider), Is.True);
      Assert.That (overrider.Base, Is.SameAs (overridden));
      Assert.That (overrider.AddMethod.Base, Is.SameAs (overridden.AddMethod));
      Assert.That (overrider.RemoveMethod.Base, Is.SameAs (overridden.RemoveMethod));
    }

    [Test]
    public void OverrideNonVirtualMethod ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType4), typeof (BT4Mixin1));
      MixinDefinition mixin = targetClass.Mixins[typeof (BT4Mixin1)];
      Assert.That (mixin, Is.Not.Null);

      MethodDefinition overrider = mixin.Methods[typeof (BT4Mixin1).GetMethod ("NonVirtualMethod")];
      Assert.That (overrider, Is.Not.Null);
      Assert.That (overrider.Base, Is.Not.Null);

      Assert.That (overrider.Base.DeclaringClass, Is.SameAs (targetClass));

      var overrides = new List<MethodDefinition> (targetClass.Methods[typeof (BaseType4).GetMethod ("NonVirtualMethod")].Overrides);
      Assert.That (overrides.Count, Is.EqualTo (1));
      Assert.That (overrides[0], Is.SameAs (overrider));
    }

    [Test]
    public void OverrideNonVirtualProperty ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType4), typeof (BT4Mixin1));
      MixinDefinition mixin = targetClass.Mixins[typeof (BT4Mixin1)];
      Assert.That (mixin, Is.Not.Null);

      PropertyDefinition overrider = mixin.Properties[typeof (BT4Mixin1).GetProperty ("NonVirtualProperty")];
      Assert.That (overrider, Is.Not.Null);
      Assert.That (overrider.Base, Is.Not.Null);

      Assert.That (overrider.Base.DeclaringClass, Is.SameAs (targetClass));

      var overrides = new List<PropertyDefinition> (targetClass.Properties[typeof (BaseType4).GetProperty ("NonVirtualProperty")].Overrides);
      Assert.That (overrides.Count, Is.EqualTo (1));
      Assert.That (overrides[0], Is.SameAs (overrider));
    }

    [Test]
    public void OverrideNonVirtualEvent ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType4), typeof (BT4Mixin1));
      MixinDefinition mixin = targetClass.Mixins[typeof (BT4Mixin1)];
      Assert.That (mixin, Is.Not.Null);

      EventDefinition overrider = mixin.Events[typeof (BT4Mixin1).GetEvent ("NonVirtualEvent")];
      Assert.That (overrider, Is.Not.Null);
      Assert.That (overrider.Base, Is.Not.Null);

      Assert.That (overrider.Base.DeclaringClass, Is.SameAs (targetClass));

      var overrides = new List<EventDefinition> (targetClass.Events[typeof (BaseType4).GetEvent ("NonVirtualEvent")].Overrides);
      Assert.That (overrides.Count, Is.EqualTo (1));
      Assert.That (overrides[0], Is.SameAs (overrider));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The member overridden by 'Void Method()' declared by type "
                                                                           + "'Remotion.Mixins.UnitTests.Core.TestDomain.BT5Mixin1' could not be found. Candidates: 'System.String Method()' (on "
                                                                           + "'Remotion.Mixins.UnitTests.Core.TestDomain.BaseType5').")]
    public void ThrowsWhenInexistingOverrideBaseMethod ()
    {
      DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType5), typeof (BT5Mixin1));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The member overridden by 'System.String Property' declared by type "
                                                                           + "'Remotion.Mixins.UnitTests.Core.TestDomain.BT5Mixin4' could not be found. Candidates: 'Int32 Property' (on "
                                                                           + "'Remotion.Mixins.UnitTests.Core.TestDomain.BaseType5').")]
    public void ThrowsWhenInexistingOverrideBaseProperty ()
    {
      DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType5), typeof (BT5Mixin4));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), 
        ExpectedMessage = "The member overridden by 'System.EventHandler Event' declared by type 'Remotion.Mixins.UnitTests.Core.TestDomain.BT5Mixin5' "
                          + "could not be found. Candidates: 'System.Action`1[System.String] Event' (on 'Remotion.Mixins.UnitTests.Core.TestDomain.BaseType5').")]
    public void ThrowsWhenInexistingOverrideBaseEvent ()
    {
      DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType5), typeof (BT5Mixin5));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Ambiguous override: Member 'System.String AbstractMethod(Int32)' "
                                                                           + "declared by type 'Remotion.Mixins.UnitTests.Core.TestDomain.ClassOverridingMixinMembers' could override any of the following: "
                                                                           + "'System.String AbstractMethod(Int32)' (on 'Remotion.Mixins.UnitTests.Core.TestDomain.MixinWithAbstractMembers'); "
                                                                           + "'System.String AbstractMethod(Int32)' (on 'Remotion.Mixins.UnitTests.Core.TestDomain.MixinWithSingleAbstractMethod2').")]
    public void ThrowsOnTargetClassOverridingMultipleMixinMethods()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<ClassOverridingMixinMembers> ().Clear().AddMixins (typeof (MixinWithAbstractMembers), typeof(MixinWithSingleAbstractMethod2)).EnterScope())
      {
        DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassOverridingMixinMembers));
      }
    }

    [Test]
    public void TargetClassOverridingSpecificMixinMethod ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<ClassOverridingSpecificMixinMember> ().Clear().AddMixins (typeof (MixinWithVirtualMethod), typeof (MixinWithVirtualMethod2)).EnterScope())
      {
        TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassOverridingSpecificMixinMember));
        MethodDefinition method = definition.Methods[typeof (ClassOverridingSpecificMixinMember).GetMethod ("VirtualMethod")];
        Assert.That (definition.Mixins[typeof (MixinWithVirtualMethod)], Is.SameAs (method.Base.DeclaringClass));
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), 
        ExpectedMessage = "The member overridden by 'System.String VirtualMethod()' declared by type "
                          + "'Remotion.Mixins.UnitTests.Core.TestDomain.ClassOverridingSpecificMixinMember' could not be found. "
                          + "Candidates: 'System.String VirtualMethod()' (on 'Remotion.Mixins.UnitTests.Core.TestDomain.MixinWithVirtualMethod2').")]
    public void TargetClassOverridingSpecificUnconfiguredMixinMethod ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<ClassOverridingSpecificMixinMember> ().Clear().AddMixins (typeof (MixinWithVirtualMethod2)).EnterScope())
      {
        DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassOverridingSpecificMixinMember));
      }
    }

    [Test]
    public void TargetClassOverridingSpecificGenericMethod ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<ClassOverridingSpecificGenericMixinMember> ().Clear().AddMixins (typeof (GenericMixinWithVirtualMethod<>), typeof (GenericMixinWithVirtualMethod2<>)).EnterScope())
      {
        TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassOverridingSpecificGenericMixinMember));
        MethodDefinition method = definition.Methods[typeof (ClassOverridingSpecificGenericMixinMember).GetMethod ("VirtualMethod")];
        Assert.That (definition.GetMixinByConfiguredType (typeof (GenericMixinWithVirtualMethod<>)), Is.SameAs (method.Base.DeclaringClass));
      }
    }

    [Test]
    public void OverridingProtectedInheritedClassMethod ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassWithInheritedMethod));
      MethodDefinition inheritedMethod = targetClass.Methods[typeof (BaseClassWithInheritedMethod).GetMethod ("ProtectedInheritedMethod",
                                                                                                              BindingFlags.NonPublic | BindingFlags.Instance)];
      Assert.That (inheritedMethod, Is.Not.Null);
      Assert.That (inheritedMethod.Overrides.Count, Is.EqualTo (1));
      Assert.That (inheritedMethod.Overrides[0], Is.SameAs (targetClass.Mixins[typeof (MixinOverridingInheritedMethod)].Methods[typeof (MixinOverridingInheritedMethod).GetMethod ("ProtectedInheritedMethod")]));
    }

    [Test]
    public void OverridingProtectedInternalInheritedClassMethod ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassWithInheritedMethod));
      MethodDefinition inheritedMethod = targetClass.Methods[typeof (BaseClassWithInheritedMethod).GetMethod ("ProtectedInternalInheritedMethod",
                                                                                                              BindingFlags.NonPublic | BindingFlags.Instance)];
      Assert.That (inheritedMethod, Is.Not.Null);
      Assert.That (inheritedMethod.Overrides.Count, Is.EqualTo (1));
      Assert.That (inheritedMethod.Overrides[0], Is.SameAs (targetClass.Mixins[typeof (MixinOverridingInheritedMethod)].Methods[typeof (MixinOverridingInheritedMethod).GetMethod ("ProtectedInternalInheritedMethod")]));
    }

    [Test]
    public void OverridingPublicInheritedClassMethod ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassWithInheritedMethod));
      MethodDefinition inheritedMethod = targetClass.Methods[typeof (BaseClassWithInheritedMethod).GetMethod ("PublicInheritedMethod")];
      Assert.That (inheritedMethod, Is.Not.Null);
      Assert.That (inheritedMethod.Overrides.Count, Is.EqualTo (1));
      Assert.That (inheritedMethod.Overrides[0], Is.SameAs (targetClass.Mixins[typeof (MixinOverridingInheritedMethod)].Methods[typeof (MixinOverridingInheritedMethod).GetMethod ("PublicInheritedMethod")]));
    }

    [Test]
    public void OverridingProtectedInheritedMixinMethod ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassOverridingInheritedMixinMethod));
      MethodDefinition inheritedMethod = targetClass.Methods[typeof (ClassOverridingInheritedMixinMethod).GetMethod ("ProtectedInheritedMethod")];
      Assert.That (inheritedMethod, Is.Not.Null);
      Assert.That (inheritedMethod.Base, Is.Not.Null);
      Assert.That (inheritedMethod.Base, Is.SameAs (targetClass.Mixins[typeof (MixinWithInheritedMethod)].Methods[
          typeof (BaseMixinWithInheritedMethod).GetMethod ("ProtectedInheritedMethod", BindingFlags.NonPublic | BindingFlags.Instance)]));
    }

    [Test]
    public void OverridingProtectedInternelInheritedMixinMethod ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassOverridingInheritedMixinMethod));
      MethodDefinition inheritedMethod = targetClass.Methods[typeof (ClassOverridingInheritedMixinMethod).GetMethod ("ProtectedInternalInheritedMethod")];
      Assert.That (inheritedMethod, Is.Not.Null);
      Assert.That (inheritedMethod.Base, Is.Not.Null);
      Assert.That (inheritedMethod.Base, Is.SameAs (targetClass.Mixins[typeof (MixinWithInheritedMethod)].Methods[
          typeof (BaseMixinWithInheritedMethod).GetMethod ("ProtectedInternalInheritedMethod", BindingFlags.NonPublic | BindingFlags.Instance)]));
    }

    [Test]
    public void OverridingPublicInheritedMixinMethod ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassOverridingInheritedMixinMethod));
      MethodDefinition inheritedMethod = targetClass.Methods[typeof (ClassOverridingInheritedMixinMethod).GetMethod ("PublicInheritedMethod")];
      Assert.That (inheritedMethod, Is.Not.Null);
      Assert.That (inheritedMethod.Base, Is.Not.Null);
      Assert.That (inheritedMethod.Base, Is.SameAs (targetClass.Mixins[typeof (MixinWithInheritedMethod)].Methods[typeof (BaseMixinWithInheritedMethod).GetMethod ("PublicInheritedMethod")]));
    }
  }
}
