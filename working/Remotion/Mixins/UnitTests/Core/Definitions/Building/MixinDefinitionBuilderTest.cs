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
using Remotion.Mixins.Definitions;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.Definitions.Building
{
  [TestFixture]
  public class MixinDefinitionBuilderTest
  {
    [Test]
    public void CorrectlyCopiesContext()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1));
      Assert.That (targetClass.Parent, Is.Null);
      Assert.That (targetClass.Name, Is.EqualTo ("BaseType1"));

      Assert.That (targetClass.Mixins.ContainsKey (typeof (BT1Mixin1)), Is.True);
      Assert.That (targetClass.Mixins.ContainsKey (typeof (BT1Mixin2)), Is.True);
      Assert.That (targetClass.Mixins[typeof (BT1Mixin1)].Parent, Is.SameAs (targetClass));
    }

    [Test]
    public void MixinAppliedToInterface()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (IBaseType2), typeof (BT2Mixin1));
      Assert.That (targetClass.IsInterface, Is.True);

      MethodInfo method = typeof (IBaseType2).GetMethod ("IfcMethod");
      Assert.That (method, Is.Not.Null);

      MemberDefinitionBase member = targetClass.Methods[method];
      Assert.That (member, Is.Not.Null);

      MixinDefinition mixin = targetClass.Mixins[typeof (BT2Mixin1)];
      Assert.That (mixin, Is.Not.Null);
      Assert.That (mixin.TargetClass, Is.SameAs (targetClass));
    }

    [Test]
    public void MixinIndicesCorrespondToPositionInArray()
    {
      TargetClassDefinition bt3 = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType3));
      for (int i = 0; i < bt3.Mixins.Count; ++i)
        Assert.That (bt3.Mixins[i].MixinIndex, Is.EqualTo (i));
    }

    [Test]
    public void OverriddenMixinMethod()
    {
      TargetClassDefinition overrider = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassOverridingMixinMembers));
      MixinDefinition mixin = overrider.Mixins[typeof (MixinWithAbstractMembers)];
      Assert.That (mixin, Is.Not.Null);
      Assert.That (mixin.HasOverriddenMembers(), Is.True);

      MethodDefinition method = mixin.Methods[typeof (MixinWithAbstractMembers).GetMethod ("AbstractMethod", BindingFlags.Instance | BindingFlags.NonPublic)];
      Assert.That (method, Is.Not.Null);
      MethodDefinition overridingMethod = overrider.Methods[typeof (ClassOverridingMixinMembers).GetMethod ("AbstractMethod")];
      Assert.That (overridingMethod, Is.Not.Null);
      Assert.That (overridingMethod.Base, Is.SameAs (method));
      Assert.That (method.Overrides.ContainsKey (typeof (ClassOverridingMixinMembers)), Is.True);
      Assert.That (method.Overrides[typeof (ClassOverridingMixinMembers)], Is.SameAs (overridingMethod));
    }

    [Test]
    public void NotOverriddenAbstractMixinMethodSucceeds()
    {
      TargetClassDefinition bt1 = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinWithAbstractMembers));
      MixinDefinition mixin = bt1.Mixins[typeof (MixinWithAbstractMembers)];
      MethodDefinition method = mixin.Methods[typeof (MixinWithAbstractMembers).GetMethod ("AbstractMethod", BindingFlags.Instance | BindingFlags.NonPublic)];
      Assert.That (method.Overrides.Count, Is.EqualTo (0));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException))]
    public void ThrowsOnMixinMethodOverridedWrongSig()
    {
      DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (ClassOverridingMixinMethodWrongSig), typeof (MixinWithAbstractMembers));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException))]
    public void ThrowsOnMixinOverrideWithoutMixin()
    {
      DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (ClassOverridingMixinMembers));
    }

    [Test]
    public void GenericMixinsAreAllowed()
    {
      Assert.That (DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin3<,>))
                                         .HasMixinWithConfiguredType(typeof(BT3Mixin3<,>)), Is.True);
    }

    [Test]
    public void GenericMixinsAreClosed ()
    {
      MixinDefinition def = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin3<,>))
          .GetMixinByConfiguredType(typeof (BT3Mixin3<,>));
      Assert.That (def.Type.IsGenericTypeDefinition, Is.False);
    }

    [Test]
    public void GenericMixinsAreSetToConstraintOrBaseType ()
    {
      MixinDefinition def = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType3), typeof (BT3Mixin3<,>))
          .GetMixinByConfiguredType (typeof (BT3Mixin3<,>));
      Assert.That (def.Type.GetGenericArguments()[0], Is.EqualTo (typeof (BaseType3)));
      Assert.That (def.Type.GetGenericArguments()[1], Is.EqualTo (typeof (IBaseType33)));
    }

    private class MixinIntroducingGenericInterfaceWithTargetAsThisType<[BindToTargetType] T>: Mixin<T>, IEquatable<T>
        where T: class
    {
      public bool Equals (T other)
      {
        throw new NotImplementedException();
      }
    }

    [Test]
    public void GenericInterfaceArgumentIsBaseTypeWhenPossible()
    {
      TargetClassDefinition def = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType1),
                                                                                     typeof (MixinIntroducingGenericInterfaceWithTargetAsThisType<>));
      Assert.That (def.ReceivedInterfaces.ContainsKey (typeof (IEquatable<BaseType1>)), Is.True);
    }

    [Test]
    public void ExplicitMixinDependenciesCorrectlyCopied ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (TargetClassWithAdditionalDependencies));
      Assert.That (targetClass.RequiredMixinTypes.ContainsKey (typeof (MixinWithNoAdditionalDependency)), Is.True);
      Assert.That (targetClass.Mixins[typeof (MixinWithAdditionalClassDependency)].MixinDependencies.ContainsKey (typeof (MixinWithNoAdditionalDependency)), Is.True);
    }

    public class MixinWithDependency : Mixin<object, IMixinBeingDependedUpon>
    {
    }

    public interface IMixinBeingDependedUpon { }

    public class MixinBeingDependedUpon : IMixinBeingDependedUpon
    {
    }

    [Uses (typeof (MixinWithDependency), AdditionalDependencies = new Type[] { typeof (IMixinBeingDependedUpon) })]
    [Uses (typeof (MixinBeingDependedUpon))]
    public class MixinTargetWithExplicitDependencies { }

    [Test]
    public void DuplicateDependenciesDontMatter ()
    {
      TargetClassDefinition mt = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (MixinTargetWithExplicitDependencies));
      Assert.That (mt.RequiredNextCallTypes.ContainsKey (typeof (IMixinBeingDependedUpon)), Is.True);
      Assert.That (mt.Mixins[typeof (MixinWithDependency)].NextCallDependencies.ContainsKey (typeof (IMixinBeingDependedUpon)), Is.True);
      Assert.That (mt.Mixins[typeof (MixinWithDependency)].MixinDependencies.ContainsKey (typeof (IMixinBeingDependedUpon)), Is.True);

      // no exceptions occurred while ordering
    }

    [Test]
    public void HasOverriddenMembersTrue ()
    {
      TargetClassDefinition definition =
          DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (ClassOverridingMixinMembers), typeof (MixinWithAbstractMembers));
      Assert.That (definition.Mixins[0].HasOverriddenMembers (), Is.True);
    }

    [Test]
    public void HasOverriddenMembersFalse ()
    {
      TargetClassDefinition definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1));
      Assert.That (definition.Mixins[0].HasOverriddenMembers (), Is.False);
    }

    [Test]
    public void GetProtectedOverriders ()
    {
      const BindingFlags bf = BindingFlags.NonPublic | BindingFlags.Instance;

      TargetClassDefinition bt1 = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinWithProtectedOverrider));
      var overriders = bt1.Mixins[0].GetProtectedOverriders ();
      Assert.That (overriders.Select (m => m.MethodInfo).ToArray (), Is.EquivalentTo (new[] {
          typeof (MixinWithProtectedOverrider).GetMethod ("VirtualMethod", bf),
          typeof (MixinWithProtectedOverrider).GetMethod ("get_VirtualProperty", bf),
          typeof (MixinWithProtectedOverrider).GetMethod ("add_VirtualEvent", bf),
          typeof (MixinWithProtectedOverrider).GetMethod ("remove_VirtualEvent", bf),
      }));
    }

    [Test]
    public void HasProtectedOverridersTrue ()
    {
      TargetClassDefinition bt1 = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinWithProtectedOverrider));
      Assert.That (bt1.HasProtectedOverriders (), Is.False);
      Assert.That (bt1.Mixins[0].HasProtectedOverriders (), Is.True);
    }

    [Test]
    public void HasProtectedOverridersFalse ()
    {
      TargetClassDefinition bt1 = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1));
      Assert.That (bt1.HasProtectedOverriders (), Is.False);
      Assert.That (bt1.Mixins[0].HasProtectedOverriders (), Is.False);
    }

    [Test]
    public void AcceptsAlphabeticOrdering_True ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass<ClassWithMixinsAcceptingAlphabeticOrdering> ().AddMixin<MixinAcceptingAlphabeticOrdering1> ()
          .EnterScope ())
      {
        MixinDefinition accepter = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassWithMixinsAcceptingAlphabeticOrdering)).Mixins[typeof (MixinAcceptingAlphabeticOrdering1)];
        Assert.That (accepter.AcceptsAlphabeticOrdering, Is.True);
      }
    }

    [Test]
    public void AcceptsAlphabeticOrdering_False ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<NullTarget> ().AddMixin<NullMixin> ().EnterScope ())
      {
        MixinDefinition accepter = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (NullTarget)).Mixins[typeof (NullMixin)];
        Assert.That (accepter.AcceptsAlphabeticOrdering, Is.False);
      }
    }

    [Test]
    public void MixinKind_Extending ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<NullTarget> ().AddMixin<NullMixin> ().OfKind (MixinKind.Extending).EnterScope ())
      {
        MixinDefinition mixin = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (NullTarget)).Mixins[typeof (NullMixin)];
        Assert.That (mixin.MixinKind, Is.EqualTo (MixinKind.Extending));
      }
    }

    [Test]
    public void MixinKind_Used ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<NullTarget> ().AddMixin<NullMixin> ().OfKind (MixinKind.Used).EnterScope ())
      {
        MixinDefinition mixin = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (NullTarget)).Mixins[typeof (NullMixin)];
        Assert.That (mixin.MixinKind, Is.EqualTo (MixinKind.Used));
      }
    }
  }
}
