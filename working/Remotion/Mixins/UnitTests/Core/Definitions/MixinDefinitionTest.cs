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
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Rhino.Mocks;

namespace Remotion.Mixins.UnitTests.Core.Definitions
{
  [TestFixture]
  public class MixinDefinitionTest
  {
    [Test]
    public void NeedsDerivedMixinType_True_OverriddenMember()
    {
      var definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (ClassOverridingMixinMembers))
          .Mixins[typeof (MixinWithAbstractMembers)];
      Assert.That (definition.NeedsDerivedMixinType (), Is.True);
    }

    [Test]
    public void NeedsDerivedMixinType_True_ProtectedOverrider ()
    {
      using (MixinConfiguration.BuildNew().ForClass<BaseType1>().AddMixin<MixinWithProtectedOverrider>().EnterScope())
      {
        var definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1)).Mixins[typeof (MixinWithProtectedOverrider)];
        Assert.That (definition.NeedsDerivedMixinType(), Is.True);
      }
    }

    [Test]
    public void NeedsDerivedMixinType_True_AbstractClass ()
    {
      using (MixinConfiguration.BuildNew ().ForClass<NullTarget> ().AddMixin<AbstractMixinWithoutAbstractMembers> ().EnterScope ())
      {
        var definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (NullTarget)).Mixins[typeof (AbstractMixinWithoutAbstractMembers)];
        Assert.That (definition.NeedsDerivedMixinType (), Is.True);
      }
    }

    [Test]
    public void NeedsDerivedMixinType_False ()
    {
      var definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType1)).Mixins[typeof (BT1Mixin1)];
      Assert.That (definition.NeedsDerivedMixinType (), Is.False);
    }

    [Test]
    public void GetConcreteMixinTypeIdentifier_NoOverrides ()
    {
      var expectedIdentifier = new ConcreteMixinTypeIdentifier (typeof (NullMixin), new HashSet<MethodInfo>(), new HashSet<MethodInfo> ());

      var definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (NullTarget), typeof (NullMixin)).Mixins[0];
      Assert.That (definition.GetConcreteMixinTypeIdentifier (), Is.EqualTo (expectedIdentifier));
    }

    [Test]
    public void GetConcreteMixinTypeIdentifier_Overridden_TypeOverridesMethod ()
    {
      var overridden = typeof (MixinWithMethodsOverriddenByDifferentClasses).GetMethod ("M1");
      var expectedIdentifier = new ConcreteMixinTypeIdentifier (
          typeof (MixinWithMethodsOverriddenByDifferentClasses), 
          new HashSet<MethodInfo> (),
          new HashSet<MethodInfo> { overridden });

      var definition = DefinitionObjectMother
          .GetActiveTargetClassDefinition (typeof (DerivedClassOverridingMixinMethod))
          .Mixins[typeof (MixinWithMethodsOverriddenByDifferentClasses)];
      Assert.That (definition.GetConcreteMixinTypeIdentifier (), Is.EqualTo (expectedIdentifier));
    }

    [Test]
    public void GetConcreteMixinTypeIdentifier_Overriders()
    {
      var overrider = typeof (MixinOverridingToString).GetMethod ("ToString");

      var expectedIdentifier = new ConcreteMixinTypeIdentifier (
          typeof (MixinOverridingToString),
          new HashSet<MethodInfo> { overrider },
          new HashSet<MethodInfo> ());

      var definition = DefinitionObjectMother.
          BuildUnvalidatedDefinition (typeof (object), typeof (MixinOverridingToString))
          .Mixins[typeof (MixinOverridingToString)];
      Assert.That (definition.GetConcreteMixinTypeIdentifier (), Is.EqualTo (expectedIdentifier));
    }

    [Test]
    public void GetConcreteMixinTypeIdentifier_ProtectedOverriders ()
    {
      const BindingFlags bf = BindingFlags.NonPublic | BindingFlags.Instance;
      var overriders = new[] { 
          typeof (MixinWithProtectedOverrider).GetMethod ("VirtualMethod", bf), 
          typeof (MixinWithProtectedOverrider).GetMethod ("get_VirtualProperty", bf),
          typeof (MixinWithProtectedOverrider).GetMethod ("add_VirtualEvent", bf),
          typeof (MixinWithProtectedOverrider).GetMethod ("remove_VirtualEvent", bf),
      };

      var expectedIdentifier = new ConcreteMixinTypeIdentifier (
          typeof (MixinWithProtectedOverrider),
          new HashSet<MethodInfo> (overriders),
          new HashSet<MethodInfo> ());

      var definition = DefinitionObjectMother.BuildUnvalidatedDefinition (typeof (BaseType1), typeof (MixinWithProtectedOverrider))
          .Mixins[typeof (MixinWithProtectedOverrider)];
      Assert.That (definition.GetConcreteMixinTypeIdentifier (), Is.EqualTo (expectedIdentifier));
    }
    
    [Test]
    public void GetOrderRelevateDependencies()
    {
      using (MixinConfiguration.BuildNew ().ForClass<BaseType3> ()
          .AddMixin<BT3Mixin1> ().WithDependency<NullMixin>()
          .AddMixin<NullMixin>().EnterScope ())
      {
        var definition = DefinitionObjectMother.GetActiveTargetClassDefinition (typeof (BaseType3)).Mixins[typeof (BT3Mixin1)];
        var dependencies = definition.GetOrderRelevantDependencies ().ToArray();
        Assert.That (dependencies, Is.EquivalentTo (new DependencyDefinitionBase[] { definition.NextCallDependencies[0], definition.MixinDependencies[0] }));
      }
    }

    [Test]
    public void ChildSpecificAccept ()
    {
      var visitorMock = MockRepository.GenerateMock<IDefinitionVisitor>();
      var definition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (BaseType1), typeof (BT1Mixin1)).Mixins[0];
      
      var interfaceIntroduction = DefinitionObjectMother.CreateInterfaceIntroductionDefinition(definition);
      var nonInterfaceIntroduction = DefinitionObjectMother.CreateNonInterfaceIntroductionDefinition(definition);
      var attributeIntroduction = DefinitionObjectMother.CreateAttributeIntroductionDefinition(definition);
      var nonAttributeIntroduction = DefinitionObjectMother.CreateNonAttributeIntroductionDefinition(definition);
      var suppressedAttributeIntroduction = DefinitionObjectMother.CreateSuppressedAttributeIntroductionDefinition(definition);
      var targetCallDependency = DefinitionObjectMother.CreateTargetCallDependencyDefinition(definition);
      var nextCallDependency = DefinitionObjectMother.CreateNextCallDependencyDefinition(definition);
      var mixinDependency = DefinitionObjectMother.CreateMixinDependencyDefinition(definition);

      using (visitorMock.GetMockRepository ().Ordered ())
      {
        visitorMock.Expect (mock => mock.Visit (definition));
        visitorMock.Expect (mock => mock.Visit (interfaceIntroduction));
        visitorMock.Expect (mock => mock.Visit (nonInterfaceIntroduction));
        visitorMock.Expect (mock => mock.Visit (attributeIntroduction));
        visitorMock.Expect (mock => mock.Visit (nonAttributeIntroduction));
        visitorMock.Expect (mock => mock.Visit (suppressedAttributeIntroduction));
        visitorMock.Expect (mock => mock.Visit (targetCallDependency));
        visitorMock.Expect (mock => mock.Visit (nextCallDependency));
        visitorMock.Expect (mock => mock.Visit (mixinDependency));
      }

      visitorMock.Replay ();
      PrivateInvoke.InvokeNonPublicMethod (definition, "ChildSpecificAccept", visitorMock);
      visitorMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetAllOverrides()
    {
      var definition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (BaseType1), typeof (BT1Mixin1)).Mixins[0];
      
      var methodOverride = DefinitionObjectMother.CreateMethodDefinition (definition, definition.Type.GetMethod ("ToString"));
      var overriddenMethod = DefinitionObjectMother.CreateMethodDefinition (definition.TargetClass, definition.Type.GetMethod ("ToString"));
      DefinitionObjectMother.DeclareOverride(methodOverride, overriddenMethod);

      var propertyOverride = DefinitionObjectMother.CreatePropertyDefinition (definition, typeof (DateTime).GetProperty ("Now"));
      var overriddenProperty = DefinitionObjectMother.CreatePropertyDefinition (definition.TargetClass, typeof (DateTime).GetProperty ("Now"));
      DefinitionObjectMother.DeclareOverride (propertyOverride, overriddenProperty);

      var eventOverride = DefinitionObjectMother.CreateEventDefinition (definition, typeof (AppDomain).GetEvent ("ProcessExit"));
      var overriddenEvent = DefinitionObjectMother.CreateEventDefinition (definition.TargetClass, typeof (AppDomain).GetEvent ("ProcessExit"));
      DefinitionObjectMother.DeclareOverride (eventOverride, overriddenEvent);

      var nonOverride = DefinitionObjectMother.CreateMethodDefinition (definition, definition.Type.GetMethod ("GetHashCode"));

      var overrides = definition.GetAllOverrides ().ToArray();
      Assert.That (overrides, Is.EquivalentTo (new MemberDefinitionBase[] { methodOverride, propertyOverride, eventOverride }));
      Assert.That (overrides, Has.No.Member(nonOverride));
    }
  }
}
