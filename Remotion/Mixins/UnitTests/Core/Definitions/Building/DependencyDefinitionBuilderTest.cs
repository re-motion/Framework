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
using NUnit.Framework;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Utilities;

namespace Remotion.Mixins.UnitTests.Core.Definitions.Building
{
  [TestFixture]
  public class DependencyDefinitionBuilderTest
  {
    [Test]
    public void FaceInterfaces ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(BaseType3));

      Assert.That(targetClass.RequiredTargetCallTypes.ContainsKey(typeof(IBaseType31)), Is.True);
      Assert.That(targetClass.RequiredTargetCallTypes.ContainsKey(typeof(IBaseType32)), Is.True);
      Assert.That(targetClass.RequiredTargetCallTypes.ContainsKey(typeof(IBaseType33)), Is.True);
      Assert.That(targetClass.RequiredTargetCallTypes.ContainsKey(typeof(IBaseType2)), Is.False);

      CheckAllRequiringEntities(
          targetClass.RequiredTargetCallTypes[typeof(IBaseType31)],
          targetClass.Mixins[typeof(BT3Mixin1)], targetClass.GetMixinByConfiguredType(typeof(BT3Mixin6<,>)));

      Assert.That(targetClass.RequiredTargetCallTypes[typeof(IBaseType31)].IsEmptyInterface, Is.False);
      Assert.That(targetClass.RequiredTargetCallTypes[typeof(IBaseType31)].IsAggregatorInterface, Is.False);

      targetClass = DefinitionObjectMother.BuildUnvalidatedDefinition(typeof(BaseType3), typeof(BT3Mixin4), typeof(Bt3Mixin7TargetCall));
      Assert.That(targetClass.RequiredTargetCallTypes.ContainsKey(typeof(ICBaseType3BT3Mixin4)), Is.True);

      CheckAllRequiringEntities(
          targetClass.RequiredTargetCallTypes[typeof(ICBaseType3BT3Mixin4)],
          targetClass.Mixins[typeof(Bt3Mixin7TargetCall)]);
      CheckAllRequiringEntities(
          targetClass.RequiredTargetCallTypes[typeof(BaseType3)],
          targetClass.Mixins[typeof(BT3Mixin4)]);
    }

    [Test]
    public void FaceInterfacesWithOpenGenericTypes ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(BaseType3));

      Assert.That(targetClass.GetMixinByConfiguredType(typeof(BT3Mixin3<,>)).TargetCallDependencies.ContainsKey(typeof(IBaseType31)), Is.False);
      Assert.That(targetClass.GetMixinByConfiguredType(typeof(BT3Mixin3<,>)).TargetCallDependencies.ContainsKey(typeof(IBaseType33)), Is.False);
      Assert.That(targetClass.GetMixinByConfiguredType(typeof(BT3Mixin3<,>)).TargetCallDependencies.ContainsKey(typeof(BaseType3)), Is.True);
    }

    [Test]
    public void FaceInterfacesAddedViaContext ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(BaseType6));

      Assert.That(targetClass.RequiredTargetCallTypes.ContainsKey(typeof(ICBT6Mixin1)), Is.True, "This is added via a dependency of BT6Mixin3.");
      Assert.That(targetClass.RequiredTargetCallTypes.ContainsKey(typeof(ICBT6Mixin2)), Is.True, "This is added via a dependency of BT6Mixin3.");
      Assert.That(targetClass.RequiredTargetCallTypes.ContainsKey(typeof(ICBT6Mixin3)), Is.True, "This is added because of the ComposedInterfaceAttribute.");
    }

    [Test]
    public void ThrowsIfAggregateTargetCallDependencyIsNotFullyImplemented ()
    {
      Assert.That(
          () => DefinitionObjectMother.BuildUnvalidatedDefinition(typeof(BaseType3), typeof(Bt3Mixin7TargetCall)),
          Throws.InstanceOf<ConfigurationException>()
              .With.Message.EqualTo(
                  "The dependency 'IBT3Mixin4' (required by mixin 'Remotion.Mixins.UnitTests.Core.TestDomain.Bt3Mixin7TargetCall' on class "
                  + "'Remotion.Mixins.UnitTests.Core.TestDomain.BaseType3') is not fulfilled - public or protected method 'System.String Foo()' "
                  + "could not be found on the target class."));
    }

    [Test]
    public void ThrowsIfAggregateNextCallDependencyIsNotFullyImplemented ()
    {
      Assert.That(
          () => DefinitionObjectMother.BuildUnvalidatedDefinition(typeof(BaseType3), typeof(BT3Mixin7Base)),
          Throws.InstanceOf<ConfigurationException>()
              .With.Message.EqualTo(
                  "The dependency 'IBT3Mixin4' (required by mixin 'Remotion.Mixins.UnitTests.Core.TestDomain.BT3Mixin7Base' on class "
                  + "'Remotion.Mixins.UnitTests.Core.TestDomain.BaseType3') is not fulfilled - public or protected method 'System.String Foo()' "
                  + "could not be found on the target class."));
    }

    [Test]
    public void BaseInterfaces ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(BaseType3));

      List<Type> requiredNextCallTypes = new List<RequiredNextCallTypeDefinition>(targetClass.RequiredNextCallTypes)
          .ConvertAll<Type>(delegate (RequiredNextCallTypeDefinition def) { return def.Type; });
      Assert.That(requiredNextCallTypes, Has.Member(typeof(IBaseType31)));
      Assert.That(requiredNextCallTypes, Has.Member(typeof(IBaseType33)));
      Assert.That(requiredNextCallTypes, Has.Member(typeof(IBaseType34)));
      Assert.That(requiredNextCallTypes.Contains(typeof(IBaseType35)), Is.False);

      CheckSomeRequiringMixin(
          targetClass.RequiredNextCallTypes[typeof(IBaseType33)],
          targetClass.GetMixinByConfiguredType(typeof(BT3Mixin3<,>)));
    }

    [Test]
    public void BaseMethods ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(BaseType3));

      RequiredNextCallTypeDefinition req1 = targetClass.RequiredNextCallTypes[typeof(IBaseType31)];
      Assert.That(req1.Methods.Count, Is.EqualTo(typeof(IBaseType31).GetMembers().Length));

      RequiredMethodDefinition member1 = req1.Methods[typeof(IBaseType31).GetMethod("IfcMethod")];
      Assert.That(member1.FullName, Is.EqualTo("Remotion.Mixins.UnitTests.Core.TestDomain.IBaseType31.IfcMethod"));
      Assert.That(member1.DeclaringRequirement, Is.SameAs(req1));
      Assert.That(member1.Parent, Is.SameAs(req1));

      Assert.That(member1.InterfaceMethod, Is.EqualTo(typeof(IBaseType31).GetMethod("IfcMethod")));
      Assert.That(member1.ImplementingMethod, Is.EqualTo(targetClass.Methods[typeof(BaseType3).GetMethod("IfcMethod")]));

      RequiredNextCallTypeDefinition req2 = targetClass.RequiredNextCallTypes[typeof(IBT3Mixin4)];
      Assert.That(req2.Methods.Count, Is.EqualTo(typeof(IBT3Mixin4).GetMembers().Length));

      RequiredMethodDefinition member2 = req2.Methods[typeof(IBT3Mixin4).GetMethod("Foo")];
      Assert.That(member2.FullName, Is.EqualTo("Remotion.Mixins.UnitTests.Core.TestDomain.IBT3Mixin4.Foo"));
      Assert.That(member2.DeclaringRequirement, Is.SameAs(req2));
      Assert.That(member2.Parent, Is.SameAs(req2));

      Assert.That(member2.InterfaceMethod, Is.EqualTo(typeof(IBT3Mixin4).GetMethod("Foo")));
      Assert.That(member2.ImplementingMethod, Is.EqualTo(targetClass.Mixins[typeof(BT3Mixin4)].Methods[typeof(BT3Mixin4).GetMethod("Foo")]));

      using (MixinConfiguration.BuildFromActive().ForClass<BaseType3>().Clear().AddMixins(typeof(BT3Mixin7Base), typeof(BT3Mixin4)).EnterScope())
      {
        TargetClassDefinition targetClass2 = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(BaseType3));

        RequiredNextCallTypeDefinition req3 = targetClass2.RequiredNextCallTypes[typeof(ICBaseType3BT3Mixin4)];
        Assert.That(req3.Methods.Count, Is.EqualTo(0));

        req3 = targetClass2.RequiredNextCallTypes[typeof(ICBaseType3)];
        Assert.That(req3.Methods.Count, Is.EqualTo(0));

        req3 = targetClass2.RequiredNextCallTypes[typeof(IBaseType31)];
        Assert.That(req3.Methods.Count, Is.EqualTo(1));

        req3 = targetClass2.RequiredNextCallTypes[typeof(IBT3Mixin4)];
        Assert.That(req3.Methods.Count, Is.EqualTo(1));

        RequiredMethodDefinition member3 = req3.Methods[typeof(IBT3Mixin4).GetMethod("Foo")];
        Assert.That(member3, Is.Not.Null);
        Assert.That(member3.ImplementingMethod, Is.EqualTo(targetClass2.Mixins[typeof(BT3Mixin4)].Methods[typeof(BT3Mixin4).GetMethod("Foo")]));
      }
    }

    [Test]
    public void DuckTypingFaceInterface ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseTypeWithDuckTargetCallMixin>().Clear().AddMixins(typeof(DuckTargetCallMixin)).EnterScope())
      {
        TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(BaseTypeWithDuckTargetCallMixin));
        Assert.That(targetClass.Mixins.ContainsKey(typeof(DuckTargetCallMixin)), Is.True);
        MixinDefinition mixin = targetClass.Mixins[typeof(DuckTargetCallMixin)];
        Assert.That(targetClass.RequiredTargetCallTypes.ContainsKey(typeof(IDuckTargetCallRequirements)), Is.True);
        CheckAllRequiringEntities(
            targetClass.RequiredTargetCallTypes[typeof(IDuckTargetCallRequirements)],
            mixin);

        Assert.That(mixin.TargetCallDependencies.ContainsKey(typeof(IDuckTargetCallRequirements)), Is.True);
        Assert.That(mixin.TargetCallDependencies[typeof(IDuckTargetCallRequirements)].GetImplementer(), Is.SameAs(targetClass));

        Assert.That(mixin.TargetCallDependencies[typeof(IDuckTargetCallRequirements)].Depender, Is.SameAs(mixin));
        Assert.That(mixin.TargetCallDependencies[typeof(IDuckTargetCallRequirements)].Aggregator, Is.Null);
        Assert.That(mixin.TargetCallDependencies[typeof(IDuckTargetCallRequirements)].AggregatedDependencies.Count, Is.EqualTo(0));

        Assert.That(
            mixin.TargetCallDependencies[typeof(IDuckTargetCallRequirements)].RequiredType,
            Is.SameAs(targetClass.RequiredTargetCallTypes[typeof(IDuckTargetCallRequirements)]));

        Assert.That(targetClass.RequiredTargetCallTypes[typeof(IDuckTargetCallRequirements)].Methods.Count, Is.EqualTo(2));
        Assert.That(
            targetClass.RequiredTargetCallTypes[typeof(IDuckTargetCallRequirements)].Methods[0].InterfaceMethod,
            Is.SameAs(typeof(IDuckTargetCallRequirements).GetMethod("MethodImplementedOnBase")));
        Assert.That(
            targetClass.RequiredTargetCallTypes[typeof(IDuckTargetCallRequirements)].Methods[0].ImplementingMethod,
            Is.SameAs(targetClass.Methods[typeof(BaseTypeWithDuckTargetCallMixin).GetMethod("MethodImplementedOnBase")]));
      }
    }

    [Test]
    public void ThrowsWhenUnfulfilledDuckTargetCall ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget>().Clear().AddMixins(typeof(DuckTargetCallMixinWithoutOverrides)).EnterScope())
      {
        Assert.That(
            () => DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(NullTarget)),
            Throws.InstanceOf<ConfigurationException>()
                .With.Message.Contains("is not fulfilled - public or protected method 'System.String MethodImplementedOnBase()' could not be found"));
      }
    }

    [Test]
    public void DuckTypingBaseInterface ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseTypeWithDuckBaseMixin>().Clear().AddMixins(typeof(DuckBaseMixin)).EnterScope())
      {
        TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(BaseTypeWithDuckBaseMixin));
        Assert.That(targetClass.Mixins.ContainsKey(typeof(DuckBaseMixin)), Is.True);
        MixinDefinition mixin = targetClass.Mixins[typeof(DuckBaseMixin)];
        Assert.That(targetClass.RequiredNextCallTypes.ContainsKey(typeof(IDuckBaseRequirements)), Is.True);
        CheckAllRequiringEntities(
            targetClass.RequiredNextCallTypes[typeof(IDuckBaseRequirements)],
            mixin);

        Assert.That(mixin.NextCallDependencies.ContainsKey(typeof(IDuckBaseRequirements)), Is.True);
        Assert.That(mixin.NextCallDependencies[typeof(IDuckBaseRequirements)].GetImplementer(), Is.SameAs(targetClass));

        Assert.That(mixin.NextCallDependencies[typeof(IDuckBaseRequirements)].Depender, Is.SameAs(mixin));
        Assert.That(mixin.NextCallDependencies[typeof(IDuckBaseRequirements)].Aggregator, Is.Null);
        Assert.That(mixin.NextCallDependencies[typeof(IDuckBaseRequirements)].AggregatedDependencies.Count, Is.EqualTo(0));

        Assert.That(mixin.NextCallDependencies[typeof(IDuckBaseRequirements)].RequiredType, Is.SameAs(targetClass.RequiredNextCallTypes[typeof(IDuckBaseRequirements)]));

        Assert.That(targetClass.RequiredNextCallTypes[typeof(IDuckBaseRequirements)].Methods.Count, Is.EqualTo(2));
        Assert.That(
            targetClass.RequiredNextCallTypes[typeof(IDuckBaseRequirements)].Methods[0].InterfaceMethod,
            Is.SameAs(typeof(IDuckBaseRequirements).GetMethod("MethodImplementedOnBase")));
        Assert.That(
            targetClass.RequiredNextCallTypes[typeof(IDuckBaseRequirements)].Methods[0].ImplementingMethod,
            Is.SameAs(targetClass.Methods[typeof(BaseTypeWithDuckBaseMixin).GetMethod("MethodImplementedOnBase")]));
      }
    }

    [Test]
    public void ThrowsWhenUnfulfilledDuckBase ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget>().Clear().AddMixins(typeof(DuckBaseMixinWithoutOverrides)).EnterScope())
      {
        Assert.That(
            () => DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(NullTarget)),
            Throws.InstanceOf<ConfigurationException>()
                .With.Message.Contains("is not fulfilled - public or protected method 'System.String MethodImplementedOnBase()' could not be found"));
      }
    }

    [Test]
    public void Dependencies ()
    {
      MixinDefinition bt3Mixin1 = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(BaseType3)).Mixins[typeof(BT3Mixin1)];

      Assert.That(bt3Mixin1.TargetCallDependencies.ContainsKey(typeof(IBaseType31)), Is.True);
      Assert.That(bt3Mixin1.TargetCallDependencies.Count, Is.EqualTo(1));

      Assert.That(bt3Mixin1.NextCallDependencies.ContainsKey(typeof(IBaseType31)), Is.True);
      Assert.That(bt3Mixin1.NextCallDependencies.Count, Is.EqualTo(1));

      MixinDefinition bt3Mixin2 = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(BaseType3)).Mixins[typeof(BT3Mixin2)];
      Assert.That(bt3Mixin2.TargetCallDependencies.ContainsKey(typeof(IBaseType32)), Is.True);
      Assert.That(bt3Mixin2.TargetCallDependencies.Count, Is.EqualTo(1));

      Assert.That(bt3Mixin2.NextCallDependencies.Count, Is.EqualTo(0));

      MixinDefinition bt3Mixin6 = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(BaseType3)).GetMixinByConfiguredType(typeof(BT3Mixin6<,>));

      Assert.That(bt3Mixin6.TargetCallDependencies.ContainsKey(typeof(IBaseType31)), Is.True);
      Assert.That(bt3Mixin6.TargetCallDependencies.ContainsKey(typeof(IBaseType32)), Is.True);
      Assert.That(bt3Mixin6.TargetCallDependencies.ContainsKey(typeof(IBaseType33)), Is.True);
      Assert.That(bt3Mixin6.TargetCallDependencies.ContainsKey(typeof(IBT3Mixin4)), Is.True);
      Assert.That(bt3Mixin6.TargetCallDependencies.ContainsKey(typeof(IBaseType34)), Is.False);

      Assert.That(bt3Mixin6.TargetCallDependencies[typeof(IBaseType31)].IsAggregate, Is.False);
      Assert.That(bt3Mixin6.TargetCallDependencies[typeof(IBT3Mixin4)].IsAggregate, Is.False);

      Assert.That(bt3Mixin6.TargetCallDependencies[typeof(IBaseType31)].AggregatedDependencies.Count, Is.EqualTo(0));

      Assert.That(bt3Mixin6.TargetCallDependencies[typeof(IBT3Mixin4)].RequiredType.RequiringDependencies.ContainsKey(
          bt3Mixin6.TargetCallDependencies[typeof(IBT3Mixin4)]), Is.True);
      Assert.That(bt3Mixin6.TargetCallDependencies[typeof(IBT3Mixin4)].Aggregator, Is.Null);

      Assert.That(bt3Mixin6.TargetCallDependencies[typeof(IBaseType31)].RequiredType, Is.SameAs(bt3Mixin6.TargetClass.RequiredTargetCallTypes[typeof(IBaseType31)]));

      Assert.That(bt3Mixin6.TargetCallDependencies[typeof(IBaseType32)].GetImplementer(), Is.SameAs(bt3Mixin6.TargetClass));
      Assert.That(bt3Mixin6.TargetCallDependencies[typeof(IBT3Mixin4)].GetImplementer(), Is.SameAs(bt3Mixin6.TargetClass.Mixins[typeof(BT3Mixin4)]));

      Assert.That(bt3Mixin6.NextCallDependencies.ContainsKey(typeof(IBaseType34)), Is.True);
      Assert.That(bt3Mixin6.NextCallDependencies.ContainsKey(typeof(IBT3Mixin4)), Is.True);
      Assert.That(bt3Mixin6.NextCallDependencies.ContainsKey(typeof(IBaseType31)), Is.False);
      Assert.That(bt3Mixin6.NextCallDependencies.ContainsKey(typeof(IBaseType32)), Is.False);
      Assert.That(bt3Mixin6.NextCallDependencies.ContainsKey(typeof(IBaseType33)), Is.True, "indirect dependency");

      Assert.That(bt3Mixin6.NextCallDependencies[typeof(IBaseType34)].RequiredType, Is.SameAs(bt3Mixin6.TargetClass.RequiredNextCallTypes[typeof(IBaseType34)]));

      Assert.That(bt3Mixin6.NextCallDependencies[typeof(IBaseType34)].GetImplementer(), Is.SameAs(bt3Mixin6.TargetClass));
      Assert.That(bt3Mixin6.NextCallDependencies[typeof(IBT3Mixin4)].GetImplementer(), Is.SameAs(bt3Mixin6.TargetClass.Mixins[typeof(BT3Mixin4)]));

      Assert.That(bt3Mixin6.NextCallDependencies[typeof(IBT3Mixin4)].IsAggregate, Is.False);
      Assert.That(bt3Mixin6.NextCallDependencies[typeof(IBT3Mixin4)].IsAggregate, Is.False);

      Assert.That(bt3Mixin6.NextCallDependencies[typeof(IBT3Mixin4)].AggregatedDependencies.Count, Is.EqualTo(0));

      Assert.That(bt3Mixin6.NextCallDependencies[typeof(IBT3Mixin4)].RequiredType.RequiringDependencies.ContainsKey(
          bt3Mixin6.NextCallDependencies[typeof(IBT3Mixin4)]), Is.True);
      Assert.That(bt3Mixin6.NextCallDependencies[typeof(IBT3Mixin4)].Aggregator, Is.Null);
    }

    [Test]
    public void ThrowsIfNextCallDependencyNotFulfilled ()
    {
      Assert.That(
          () => DefinitionObjectMother.BuildUnvalidatedDefinition(typeof(BaseType3), typeof(BT3Mixin7Base)),
          Throws.InstanceOf<ConfigurationException>()
              .With.Message.Matches("The dependency .* is not fulfilled"));
    }

    [Test]
    public void ThrowsIfRequiredBaseIsNotInterface ()
    {
      Assert.That(
          () => DefinitionObjectMother.BuildUnvalidatedDefinition(typeof(BaseType1), typeof(MixinWithClassBase)),
          Throws.InstanceOf<ConfigurationException>()
              .With.Message.Matches(
                  "Next call dependencies must be interfaces.*MixinWithClassBase"));
    }

    [Test]
    public void WorksIfRequiredBaseIsSystemObject ()
    {
      DefinitionObjectMother.BuildUnvalidatedDefinition(typeof(BaseType1), typeof(MixinWithObjectBase));
    }

    [Test]
    public void ComposedInterfacesAndDependenciesForFace ()
    {
      TargetClassDefinition bt3 = DefinitionObjectMother.BuildUnvalidatedDefinition(typeof(BaseType3), typeof(BT3Mixin4), typeof(Bt3Mixin7TargetCall));

      MixinDefinition m4 = bt3.Mixins[typeof(BT3Mixin4)];
      MixinDefinition m7 = bt3.Mixins[typeof(Bt3Mixin7TargetCall)];

      TargetCallDependencyDefinition d1 = m7.TargetCallDependencies[typeof(ICBaseType3BT3Mixin4)];
      Assert.That(d1.GetImplementer(), Is.Null);
      Assert.That(d1.FullName, Is.EqualTo("Remotion.Mixins.UnitTests.Core.TestDomain.ICBaseType3BT3Mixin4"));
      Assert.That(d1.Parent, Is.SameAs(m7));

      Assert.That(d1.IsAggregate, Is.True);
      Assert.That(d1.AggregatedDependencies[typeof(ICBaseType3)].IsAggregate, Is.True);
      Assert.That(d1.AggregatedDependencies[typeof(ICBaseType3)]
                       .AggregatedDependencies[typeof(IBaseType31)].IsAggregate, Is.False);
      Assert.That(d1.AggregatedDependencies[typeof(ICBaseType3)]
                       .AggregatedDependencies[typeof(IBaseType31)].GetImplementer(), Is.SameAs(bt3));

      Assert.That(d1.AggregatedDependencies[typeof(IBT3Mixin4)].IsAggregate, Is.False);
      Assert.That(d1.AggregatedDependencies[typeof(IBT3Mixin4)].GetImplementer(), Is.SameAs(m4));

      Assert.That(d1.AggregatedDependencies[typeof(IBT3Mixin4)].Aggregator, Is.SameAs(d1));

      Assert.That(bt3.RequiredTargetCallTypes[typeof(ICBaseType3)].IsEmptyInterface, Is.True);
      Assert.That(bt3.RequiredTargetCallTypes[typeof(ICBaseType3)].IsAggregatorInterface, Is.True);

      Assert.That(bt3.RequiredTargetCallTypes.ContainsKey(typeof(ICBaseType3BT3Mixin4)), Is.True);
      Assert.That(bt3.RequiredTargetCallTypes.ContainsKey(typeof(ICBaseType3)), Is.True);
      Assert.That(bt3.RequiredTargetCallTypes.ContainsKey(typeof(IBaseType31)), Is.True);
      Assert.That(bt3.RequiredTargetCallTypes.ContainsKey(typeof(IBT3Mixin4)), Is.True);
    }

    [Test]
    public void ComposedInterfacesAndDependenciesForBase ()
    {
      TargetClassDefinition bt3 =
          DefinitionObjectMother.BuildUnvalidatedDefinition(typeof(BaseType3), typeof(BT3Mixin4), typeof(BT3Mixin7Base));

      MixinDefinition m4 = bt3.Mixins[typeof(BT3Mixin4)];
      MixinDefinition m7 = bt3.Mixins[typeof(BT3Mixin7Base)];

      NextCallDependencyDefinition d2 = m7.NextCallDependencies[typeof(ICBaseType3BT3Mixin4)];
      Assert.That(d2.GetImplementer(), Is.Null);

      Assert.That(d2.IsAggregate, Is.True);

      Assert.That(d2.AggregatedDependencies[typeof(ICBaseType3)].IsAggregate, Is.True);
      Assert.That(d2.AggregatedDependencies[typeof(ICBaseType3)].Parent, Is.SameAs(d2));

      Assert.That(d2.AggregatedDependencies[typeof(ICBaseType3)]
                       .AggregatedDependencies[typeof(IBaseType31)].IsAggregate, Is.False);
      Assert.That(d2.AggregatedDependencies[typeof(ICBaseType3)]
                       .AggregatedDependencies[typeof(IBaseType31)].GetImplementer(), Is.SameAs(bt3));

      Assert.That(d2.AggregatedDependencies[typeof(IBT3Mixin4)].IsAggregate, Is.False);
      Assert.That(d2.AggregatedDependencies[typeof(IBT3Mixin4)].GetImplementer(), Is.SameAs(m4));

      Assert.That(d2.AggregatedDependencies[typeof(IBT3Mixin4)].Aggregator, Is.SameAs(d2));

      Assert.That(bt3.RequiredNextCallTypes[typeof(ICBaseType3)].IsEmptyInterface, Is.True);
      Assert.That(bt3.RequiredNextCallTypes[typeof(ICBaseType3)].IsAggregatorInterface, Is.True);

      Assert.That(bt3.RequiredNextCallTypes.ContainsKey(typeof(ICBaseType3BT3Mixin4)), Is.True);
      Assert.That(bt3.RequiredNextCallTypes.ContainsKey(typeof(ICBaseType3)), Is.True);
      Assert.That(bt3.RequiredNextCallTypes.ContainsKey(typeof(IBaseType31)), Is.True);
      Assert.That(bt3.RequiredNextCallTypes.ContainsKey(typeof(IBT3Mixin4)), Is.True);
    }

    [Test]
    public void ComposedInterfaces ()
    {
      TargetClassDefinition bt3 = DefinitionObjectMother.BuildUnvalidatedDefinition(typeof(BaseType3), Type.EmptyTypes, new[] { typeof(ICBaseType3) });

      var dependency = bt3.ComposedInterfaceDependencies[typeof(ICBaseType3)];
      Assert.That(dependency, Is.Not.Null);

      var requirement = dependency.RequiredType;
      Assert.That(requirement, Is.Not.Null);
      Assert.That(requirement, Is.SameAs(bt3.RequiredTargetCallTypes[typeof(ICBaseType3)]));
      Assert.That(requirement.RequiringDependencies, Is.EqualTo(new[] { dependency }));

      Assert.That(dependency, Is.TypeOf<ComposedInterfaceDependencyDefinition>());
      Assert.That(dependency.RequiredType, Is.SameAs(requirement));
      Assert.That(dependency.TargetClass, Is.SameAs(bt3));
      Assert.That(dependency.Depender, Is.SameAs(bt3));
      Assert.That(dependency.FullName, Is.EqualTo(typeof(ICBaseType3).FullName));
      Assert.That(dependency.Parent, Is.SameAs(dependency.Depender));
      Assert.That(dependency.ComposedInterface, Is.SameAs(typeof(ICBaseType3)));

      CheckSomeRequiringComposedInterface(requirement, typeof(ICBaseType3));

      Assert.That(dependency.IsAggregate, Is.True);
      Assert.That(dependency.AggregatedDependencies[typeof(IBaseType31)], Is.Not.Null);
      Assert.That(dependency.AggregatedDependencies[typeof(IBaseType31)], Is.TypeOf<ComposedInterfaceDependencyDefinition>());
      Assert.That(
          ((ComposedInterfaceDependencyDefinition)dependency.AggregatedDependencies[typeof(IBaseType31)]).ComposedInterface,
          Is.SameAs(typeof(ICBaseType3)));
      Assert.That(bt3.RequiredTargetCallTypes[typeof(IBaseType31)], Is.Not.Null);
      Assert.That(bt3.RequiredTargetCallTypes[typeof(IBaseType31)], Is.SameAs(dependency.AggregatedDependencies[typeof(IBaseType31)].RequiredType));
    }

    [Test]
    public void EmptyInterface ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<BaseType1>().Clear().AddMixins(typeof(MixinWithEmptyInterface), typeof(MixinRequiringEmptyInterface)).EnterScope())
      {
        TargetClassDefinition bt1 = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(BaseType1));
        MixinDefinition m1 = bt1.Mixins[typeof(MixinWithEmptyInterface)];
        MixinDefinition m2 = bt1.Mixins[typeof(MixinRequiringEmptyInterface)];
        NextCallDependencyDefinition dependency = m2.NextCallDependencies[0];
        RequiredNextCallTypeDefinition requirement = dependency.RequiredType;
        Assert.That(requirement.IsEmptyInterface, Is.True);
        Assert.That(requirement.IsAggregatorInterface, Is.False);
        Assert.That(dependency.GetImplementer(), Is.SameAs(m1));
      }
    }

    [Test]
    public void IndirectTargetCallDependencies ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(ClassImplementingIndirectRequirements));
      MixinDefinition mixin = targetClass.Mixins[typeof(MixinWithIndirectRequirements)];
      Assert.That(mixin, Is.Not.Null);

      Assert.That(targetClass.RequiredTargetCallTypes.ContainsKey(typeof(IIndirectTargetAggregator)), Is.True);
      Assert.That(targetClass.RequiredTargetCallTypes[typeof(IIndirectTargetAggregator)].IsAggregatorInterface, Is.True);

      Assert.That(targetClass.RequiredTargetCallTypes.ContainsKey(typeof(IIndirectRequirement1)), Is.True);
      Assert.That(targetClass.RequiredTargetCallTypes[typeof(IIndirectRequirement1)].IsAggregatorInterface, Is.False);
      Assert.That(targetClass.RequiredTargetCallTypes.ContainsKey(typeof(IIndirectRequirementBase1)), Is.True);
      Assert.That(targetClass.RequiredTargetCallTypes[typeof(IIndirectRequirementBase1)].IsAggregatorInterface, Is.False);

      Assert.That(targetClass.RequiredTargetCallTypes.ContainsKey(typeof(IIndirectRequirement2)), Is.True);
      Assert.That(targetClass.RequiredTargetCallTypes[typeof(IIndirectRequirement2)].IsAggregatorInterface, Is.True);
      Assert.That(targetClass.RequiredTargetCallTypes.ContainsKey(typeof(IIndirectRequirementBase2)), Is.True);
      Assert.That(targetClass.RequiredTargetCallTypes[typeof(IIndirectRequirementBase2)].IsAggregatorInterface, Is.False);
      Assert.That(targetClass.RequiredTargetCallTypes[typeof(IIndirectRequirementBase2)].IsEmptyInterface, Is.True);

      Assert.That(targetClass.RequiredTargetCallTypes.ContainsKey(typeof(IIndirectRequirement3)), Is.True);
      Assert.That(targetClass.RequiredTargetCallTypes[typeof(IIndirectRequirement3)].IsAggregatorInterface, Is.True);
      Assert.That(targetClass.RequiredTargetCallTypes.ContainsKey(typeof(IIndirectRequirementBase3)), Is.True);
      Assert.That(targetClass.RequiredTargetCallTypes[typeof(IIndirectRequirementBase3)].IsAggregatorInterface, Is.False);
      Assert.That(targetClass.RequiredTargetCallTypes[typeof(IIndirectRequirementBase3)].IsEmptyInterface, Is.False);

      Assert.That(mixin.TargetCallDependencies.ContainsKey(typeof(IIndirectTargetAggregator)), Is.True);
      Assert.That(mixin.TargetCallDependencies.ContainsKey(typeof(IIndirectRequirement1)), Is.True);
      Assert.That(mixin.TargetCallDependencies.ContainsKey(typeof(IIndirectRequirement2)), Is.True);
      Assert.That(mixin.TargetCallDependencies.ContainsKey(typeof(IIndirectRequirement3)), Is.True);
      Assert.That(mixin.TargetCallDependencies.ContainsKey(typeof(IIndirectRequirementBase1)), Is.True);
      Assert.That(mixin.TargetCallDependencies.ContainsKey(typeof(IIndirectRequirementBase2)), Is.True);
      Assert.That(mixin.TargetCallDependencies.ContainsKey(typeof(IIndirectRequirementBase3)), Is.True);
    }

    [Test]
    public void IndirectNextCallDependencies ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(ClassImplementingIndirectRequirements));
      MixinDefinition mixin = targetClass.Mixins[typeof(MixinWithIndirectRequirements)];
      Assert.That(mixin, Is.Not.Null);

      Assert.That(targetClass.RequiredNextCallTypes.ContainsKey(typeof(IIndirectBaseAggregator)), Is.True);
      Assert.That(targetClass.RequiredNextCallTypes[typeof(IIndirectBaseAggregator)].IsAggregatorInterface, Is.True);

      Assert.That(targetClass.RequiredNextCallTypes.ContainsKey(typeof(IIndirectRequirement1)), Is.True);
      Assert.That(targetClass.RequiredNextCallTypes[typeof(IIndirectRequirement1)].IsAggregatorInterface, Is.False);
      Assert.That(targetClass.RequiredNextCallTypes.ContainsKey(typeof(IIndirectRequirementBase1)), Is.True);
      Assert.That(targetClass.RequiredNextCallTypes[typeof(IIndirectRequirementBase1)].IsAggregatorInterface, Is.False);

      Assert.That(targetClass.RequiredNextCallTypes.ContainsKey(typeof(IIndirectRequirement2)), Is.False);
      Assert.That(targetClass.RequiredNextCallTypes.ContainsKey(typeof(IIndirectRequirementBase2)), Is.False);

      Assert.That(targetClass.RequiredNextCallTypes.ContainsKey(typeof(IIndirectRequirement3)), Is.True);
      Assert.That(targetClass.RequiredNextCallTypes[typeof(IIndirectRequirement3)].IsAggregatorInterface, Is.True);
      Assert.That(targetClass.RequiredNextCallTypes.ContainsKey(typeof(IIndirectRequirementBase3)), Is.True);
      Assert.That(targetClass.RequiredNextCallTypes[typeof(IIndirectRequirementBase3)].IsAggregatorInterface, Is.False);
      Assert.That(targetClass.RequiredNextCallTypes[typeof(IIndirectRequirementBase3)].IsEmptyInterface, Is.False);

      Assert.That(mixin.NextCallDependencies.ContainsKey(typeof(IIndirectBaseAggregator)), Is.True);
      Assert.That(mixin.NextCallDependencies.ContainsKey(typeof(IIndirectRequirement1)), Is.True);
      Assert.That(mixin.NextCallDependencies.ContainsKey(typeof(IIndirectRequirement2)), Is.False);
      Assert.That(mixin.NextCallDependencies.ContainsKey(typeof(IIndirectRequirement3)), Is.True);
      Assert.That(mixin.NextCallDependencies.ContainsKey(typeof(IIndirectRequirementBase1)), Is.True);
      Assert.That(mixin.NextCallDependencies.ContainsKey(typeof(IIndirectRequirementBase2)), Is.False);
      Assert.That(mixin.NextCallDependencies.ContainsKey(typeof(IIndirectRequirementBase3)), Is.True);
    }

    [Test]
    public void NoIndirectDependenciesForClassFaces ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(ClassImplementingInternalInterface));
      MixinDefinition mixin = targetClass.Mixins[typeof(MixinWithClassTargetCallImplementingInternalInterface)];
      Assert.That(mixin, Is.Not.Null);
      Assert.That(targetClass.RequiredTargetCallTypes.ContainsKey(typeof(ClassImplementingInternalInterface)), Is.True);
      Assert.That(targetClass.RequiredTargetCallTypes[typeof(ClassImplementingInternalInterface)].IsAggregatorInterface, Is.False);
      Assert.That(targetClass.RequiredTargetCallTypes.ContainsKey(typeof(IInternalInterface1)), Is.False);
      Assert.That(targetClass.RequiredTargetCallTypes.ContainsKey(typeof(IInternalInterface2)), Is.False);

      Assert.That(mixin.TargetCallDependencies.ContainsKey(typeof(ClassImplementingInternalInterface)), Is.True);
      Assert.That(mixin.TargetCallDependencies.ContainsKey(typeof(IInternalInterface1)), Is.False);
      Assert.That(mixin.TargetCallDependencies.ContainsKey(typeof(IInternalInterface2)), Is.False);
    }

    [Test]
    public void ExplicitDependenciesToInterfaceTypes ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(TargetClassWithAdditionalDependencies));
      MixinDefinition mixin = targetClass.Mixins[typeof(MixinWithAdditionalInterfaceDependency)];
      Assert.That(mixin.MixinDependencies.ContainsKey(typeof(IMixinWithAdditionalClassDependency)), Is.True);
      Assert.That(mixin.MixinDependencies[typeof(IMixinWithAdditionalClassDependency)].GetImplementer(), Is.SameAs(targetClass.Mixins[typeof(MixinWithAdditionalClassDependency)]));

      Assert.That(mixin.MixinDependencies[typeof(IMixinWithAdditionalClassDependency)].Depender, Is.SameAs(mixin));
      Assert.That(mixin.MixinDependencies[typeof(IMixinWithAdditionalClassDependency)].Parent, Is.SameAs(mixin));

      Assert.That(targetClass.RequiredMixinTypes.ContainsKey(typeof(IMixinWithAdditionalClassDependency)), Is.True);
      RequiredMixinTypeDefinition requirement = targetClass.RequiredMixinTypes[typeof(IMixinWithAdditionalClassDependency)];
      CheckAllRequiringEntities(requirement, mixin);

      Assert.That(requirement.RequiringDependencies.ContainsKey(mixin.MixinDependencies[typeof(IMixinWithAdditionalClassDependency)]), Is.True);

      Assert.That(requirement.Methods.Count, Is.EqualTo(0), "mixin type requirements do not contain method requirements");

      Assert.That(mixin.MixinDependencies[typeof(IMixinWithAdditionalClassDependency)].RequiredType, Is.SameAs(requirement));
    }

    [Test]
    public void ExplicitDependenciesToClassTypes ()
    {
      TargetClassDefinition targetClass = DefinitionObjectMother.GetActiveTargetClassDefinition(typeof(TargetClassWithAdditionalDependencies));
      MixinDefinition mixin = targetClass.Mixins[typeof(MixinWithAdditionalClassDependency)];
      Assert.That(mixin.MixinDependencies.ContainsKey(typeof(MixinWithNoAdditionalDependency)), Is.True);
      Assert.That(mixin.MixinDependencies[typeof(MixinWithNoAdditionalDependency)].GetImplementer(), Is.SameAs(targetClass.Mixins[typeof(MixinWithNoAdditionalDependency)]));

      Assert.That(mixin.MixinDependencies[typeof(MixinWithNoAdditionalDependency)].Depender, Is.SameAs(mixin));
      Assert.That(mixin.MixinDependencies[typeof(MixinWithNoAdditionalDependency)].Parent, Is.SameAs(mixin));

      Assert.That(targetClass.RequiredMixinTypes.ContainsKey(typeof(MixinWithNoAdditionalDependency)), Is.True);
      RequiredMixinTypeDefinition requirement = targetClass.RequiredMixinTypes[typeof(MixinWithNoAdditionalDependency)];
      CheckAllRequiringEntities(requirement, mixin);

      Assert.That(requirement.RequiringDependencies.ContainsKey(mixin.MixinDependencies[typeof(MixinWithNoAdditionalDependency)]), Is.True);

      Assert.That(requirement.Methods.Count, Is.EqualTo(0), "mixin type requirements do not contain method requirements");

      Assert.That(mixin.MixinDependencies[typeof(MixinWithNoAdditionalDependency)].RequiredType, Is.SameAs(requirement));
    }

    private void CheckAllRequiringEntities (RequirementDefinitionBase requirement, params MixinDefinition[] expectedRequiringMixins)
    {
      ArgumentUtility.CheckNotNull("requirement", requirement);
      ArgumentUtility.CheckNotNull("expectedRequiringMixins", expectedRequiringMixins);

      var requiringEntityDescription = requirement.GetRequiringEntityDescription();
      var requiringEntityDescriptionItems = requiringEntityDescription.Split(new[] { ", "}, StringSplitOptions.None);

      foreach (var mixinDefinition in expectedRequiringMixins)
        CheckSomeRequiringMixin(requirement, mixinDefinition);

      Assert.That(requiringEntityDescriptionItems, Has.Length.EqualTo(expectedRequiringMixins.Length), requiringEntityDescription);
    }

    private void CheckSomeRequiringMixin (RequirementDefinitionBase requirement, MixinDefinition expectedRequiringMixin)
    {
      ArgumentUtility.CheckNotNull("requirement", requirement);
      ArgumentUtility.CheckNotNull("expectedRequiringMixin", expectedRequiringMixin);

      var requirers = requirement.GetRequiringEntityDescription().Split(new[] { ", " }, StringSplitOptions.None);

      Assert.That(requirers, Has.Member("mixin '" + expectedRequiringMixin.FullName + "'"));
    }

    private void CheckSomeRequiringComposedInterface (RequirementDefinitionBase requirement, Type expectedRequiringComposedInterface)
    {
      ArgumentUtility.CheckNotNull("requirement", requirement);
      ArgumentUtility.CheckNotNull("expectedRequiringComposedInterface", expectedRequiringComposedInterface);

      var requirers = requirement.GetRequiringEntityDescription().Split(new[] { ", " }, StringSplitOptions.None);

      Assert.That(requirers, Has.Member("composed interface '" + expectedRequiringComposedInterface.FullName + "'"));
    }
  }
}
