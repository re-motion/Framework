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
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions.Building;
using Remotion.Mixins.Definitions.Building.DependencySorting;
using Remotion.Mixins.UnitTests.Core.Definitions.TestDomain;
using Remotion.Mixins.UnitTests.Core.IntegrationTests.Ordering;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.ServiceLocation;

namespace Remotion.Mixins.UnitTests.Core.Definitions.Building
{
  [TestFixture]
  public class TargetClassDefinitionBuilderTest
  {
    private TargetClassDefinitionBuilder _builder;

    [SetUp]
    public void SetUp ()
    {
      var sorter = new MixinDefinitionSorter ();
      _builder = new TargetClassDefinitionBuilder (sorter);
    }

    [Test]
    public void Singleton_RegisteredAsDefaultInterfaceImplementation ()
    {
      var instance = SafeServiceLocator.Current.GetInstance<ITargetClassDefinitionBuilder> ();
      Assert.That (instance, Is.TypeOf<TargetClassDefinitionBuilder> ());

      Assert.That (instance, Is.SameAs (SafeServiceLocator.Current.GetInstance<ITargetClassDefinitionBuilder> ()));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "contains generic parameters", MatchType = MessageMatch.Contains)]
    public void Build_ThrowsOnGenericTargetClass ()
    {
      _builder.Build (ClassContextObjectMother.Create(typeof (BT3Mixin3<,>)));
    }

    [Test]
    public void Build_SetsContext ()
    {
      var classContext = ClassContextObjectMother.Create(typeof (BaseType1));

      var targetClassDefinition = _builder.Build (classContext);
      Assert.That (targetClassDefinition.ConfigurationContext, Is.SameAs (classContext));
    }

    [Test]
    public void Build_AddsPublicMembers ()
    {
      var classContext = ClassContextObjectMother.Create(typeof (ClassWithDifferentMemberVisibilities));
      
      var targetClassDefinition = _builder.Build (classContext);
      Assert.That (targetClassDefinition.Methods.Where (m => m.Name == "PublicMethod").ToArray (), Is.Not.Empty);
      Assert.That (targetClassDefinition.Properties.Where (p => p.Name == "PublicProperty").ToArray (), Is.Not.Empty);
      Assert.That (targetClassDefinition.Events.Where (e => e.Name == "PublicEvent").ToArray (), Is.Not.Empty);

      Assert.That (targetClassDefinition.Properties.Where (p => p.Name == "PropertyWithPrivateSetter").ToArray (), Is.Not.Empty);
    }

    [Test]
    public void Build_AddsProtectedMembers ()
    {
      var classContext = ClassContextObjectMother.Create(typeof (ClassWithDifferentMemberVisibilities));

      var targetClassDefinition = _builder.Build (classContext);
      Assert.That (targetClassDefinition.Methods.Where (m => m.Name == "ProtectedMethod").ToArray(), Is.Not.Empty);
      Assert.That (targetClassDefinition.Properties.Where (p => p.Name == "ProtectedProperty").ToArray(), Is.Not.Empty);
      Assert.That (targetClassDefinition.Events.Where (e => e.Name == "ProtectedEvent").ToArray(), Is.Not.Empty);
    }

    [Test]
    public void Build_AddsProtectedInternalMembers ()
    {
      var classContext = ClassContextObjectMother.Create(typeof (ClassWithDifferentMemberVisibilities));

      var targetClassDefinition = _builder.Build (classContext);
      Assert.That (targetClassDefinition.Methods.Where (m => m.Name == "ProtectedInternalMethod").ToArray (), Is.Not.Empty);
      Assert.That (targetClassDefinition.Properties.Where (p => p.Name == "ProtectedInternalProperty").ToArray (), Is.Not.Empty);
      Assert.That (targetClassDefinition.Events.Where (e => e.Name == "ProtectedInternalEvent").ToArray (), Is.Not.Empty);
    }

    [Test]
    public void Build_AddsExplicitInterfaceMembers ()
    {
      var classContext = ClassContextObjectMother.Create(typeof (ClassWithExplicitInterfaceImplementation));

      var targetClassDefinition = _builder.Build (classContext);
      Assert.That (targetClassDefinition.Methods.Where (m => m.Name == typeof (IInterfaceWithAllKindsOfMembers).FullName + ".Method").ToArray (), 
                   Is.Not.Empty);
      Assert.That (targetClassDefinition.Properties.Where (p => p.Name == typeof (IInterfaceWithAllKindsOfMembers).FullName + ".Property").ToArray (), 
                   Is.Not.Empty);
      Assert.That (targetClassDefinition.Events.Where (e => e.Name == typeof (IInterfaceWithAllKindsOfMembers).FullName + ".Event").ToArray (),
                   Is.Not.Empty);
    }

    [Test]
    public void Build_NoInternalOrPrivateMembers ()
    {
      var classContext = ClassContextObjectMother.Create(typeof (ClassWithDifferentMemberVisibilities));

      var targetClassDefinition = _builder.Build (classContext);
      Assert.That (targetClassDefinition.Methods.Where (m => m.Name == "PrivateMethod").ToArray (), Is.Empty);
      Assert.That (targetClassDefinition.Properties.Where (p => p.Name == "PrivateProperty").ToArray (), Is.Empty);
      Assert.That (targetClassDefinition.Events.Where (e => e.Name == "PrivateEvent").ToArray (), Is.Empty);

      Assert.That (targetClassDefinition.Methods.Where (m => m.Name == "InternalMethod").ToArray (), Is.Empty);
      Assert.That (targetClassDefinition.Properties.Where (p => p.Name == "InternalProperty").ToArray (), Is.Empty);
      Assert.That (targetClassDefinition.Events.Where (e => e.Name == "InternalEvent").ToArray (), Is.Empty);
    }

    [Test]
    public void Build_AddsAttributes ()
    {
      var classContext = ClassContextObjectMother.Create(typeof (BaseType1));
      var targetClassDefinition = _builder.Build (classContext);
      
      Assert.That (targetClassDefinition.CustomAttributes, Is.Not.Empty);
    }

    [Test]
    public void Build_AddsComposedInterfaces ()
    {
      var classContext = ClassContextObjectMother.Create (typeof (BaseType6), new[] { typeof (BT6Mixin1) }, new[] { typeof (ICBT6Mixin1) });
      var targetClassDefinition = _builder.Build (classContext);

      Assert.That (targetClassDefinition.ComposedInterfaceDependencies[typeof (ICBT6Mixin1)], Is.Not.Null);
      
      var requirement = targetClassDefinition.ComposedInterfaceDependencies[typeof (ICBT6Mixin1)].RequiredType;
      Assert.That (requirement, Is.Not.Null);
      Assert.That (targetClassDefinition.RequiredTargetCallTypes, Has.Member (requirement));
    }

    [Test]
    public void Build_AddsMixins ()
    {
      var classContext = ClassContextObjectMother.Create(typeof (BaseType1), typeof (BT1Mixin1));
      var targetClassDefinition = _builder.Build (classContext);

      Assert.That (targetClassDefinition.Mixins.Select (m => m.Type).ToArray (), Has.Member(typeof (BT1Mixin1)));
    }

    [Test]
    public void Build_SortsMixins ()
    {
      var classContext = ClassContextObjectMother.Create(typeof (BaseType7), 
                                typeof (BT7Mixin0), 
                                typeof (BT7Mixin1), 
                                typeof (BT7Mixin2), 
                                typeof (BT7Mixin3), 
                                typeof (BT7Mixin5), 
                                typeof (BT7Mixin9), 
                                typeof (BT7Mixin10));

      var targetClassDefinition = _builder.Build (classContext);

      Assert.That (targetClassDefinition.Mixins.Select (m => m.Type).ToArray (), Is.EqualTo (BigTestDomainScenarioTest.ExpectedBaseType7OrderedMixinTypesSmall));
    }

    [Test]
    public void Build_SetsIndexesOfSortedMixins ()
    {
      var classContext = ClassContextObjectMother.Create(
          typeof (BaseType7),
          typeof (BT7Mixin0),
          typeof (BT7Mixin1),
          typeof (BT7Mixin2),
          typeof (BT7Mixin3),
          typeof (BT7Mixin5),
          typeof (BT7Mixin9),
          typeof (BT7Mixin10));

      var targetClassDefinition = _builder.Build (classContext);

      Assert.That (targetClassDefinition.Mixins.Select (m => m.MixinIndex).ToArray (), Is.EqualTo (new[] { 0, 1, 2, 3, 4, 5, 6 }));
    }

    [Test]
    public void Build_DetectsCyclicDependencies ()
    {
      var classContext =
          ClassContextObjectMother.Create (typeof (NullTarget), typeof (NullMixin), typeof (NullMixin2)).ApplyMixinDependencies (
              new[]
              {
                  new MixinDependencySpecification (typeof (NullMixin), new[] { typeof (NullMixin2) }),
                  new MixinDependencySpecification (typeof (NullMixin2), new[] { typeof (NullMixin) })
              });

      Assert.That (
          () => _builder.Build (classContext),
          Throws.TypeOf<ConfigurationException> ().With.Message.EqualTo (
              "The mixins applied to target class 'Remotion.Mixins.UnitTests.Core.TestDomain.NullTarget' cannot be ordered. "
              + "The following group of mixins contains circular dependencies:\r\n"
              + "'Remotion.Mixins.UnitTests.Core.TestDomain.NullMixin',\r\n"
              + "'Remotion.Mixins.UnitTests.Core.TestDomain.NullMixin2'."));
    }

    [Test]
    public void Build_AppliesRequiredTargetCallTypeMethods ()
    {
      var classContext = ClassContextObjectMother.Create(typeof (BaseType3), typeof (BT3Mixin1));

      var targetClassDefinition = _builder.Build (classContext);

      Assert.That (targetClassDefinition.RequiredTargetCallTypes[typeof (IBaseType31)].Methods.Select (r => r.InterfaceMethod).ToArray (),
                   Has.Member(typeof (IBaseType31).GetMethod ("IfcMethod")));
    }

    [Test]
    public void Build_AppliesRequiredBaseTypeMethods ()
    {
      var classContext = ClassContextObjectMother.Create(typeof (BaseType3), typeof (BT3Mixin1));

      var targetClassDefinition = _builder.Build (classContext);

      Assert.That (targetClassDefinition.RequiredNextCallTypes[typeof (IBaseType31)].Methods.Select (r => r.InterfaceMethod).ToArray (),
                   Has.Member(typeof (IBaseType31).GetMethod ("IfcMethod")));
    }

    [Test]
    public void Build_AnalyzesMethodOverrides ()
    {
      const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;

      var classContext = ClassContextObjectMother.Create(typeof (ClassOverridingMixinMembers), typeof (MixinWithAbstractMembers));
      var targetClassDefinition = _builder.Build (classContext);

      var overrider = targetClassDefinition.Methods[typeof (ClassOverridingMixinMembers).GetMethod ("AbstractMethod")];
      var overridden = targetClassDefinition.Mixins[0].Methods[typeof (MixinWithAbstractMembers).GetMethod ("AbstractMethod", bindingFlags)];
      
      Assert.That (overrider.Base, Is.SameAs (overridden));
      Assert.That (overridden.Overrides.ToArray(), Has.Member(overrider));
    }

    [Test]
    public void Build_AnalyzesPropertyOverrides ()
    {
      const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;

      var classContext = ClassContextObjectMother.Create(typeof (ClassOverridingMixinMembers), typeof (MixinWithAbstractMembers));
      var targetClassDefinition = _builder.Build (classContext);

      var overrider = targetClassDefinition.Properties[typeof (ClassOverridingMixinMembers).GetProperty ("AbstractProperty")];
      var overridden = targetClassDefinition.Mixins[0].Properties[typeof (MixinWithAbstractMembers).GetProperty ("AbstractProperty", bindingFlags)];

      Assert.That (overrider.Base, Is.SameAs (overridden));
      Assert.That (overridden.Overrides.ToArray (), Has.Member(overrider));
    }

    [Test]
    public void Build_AnalyzesEventOverrides ()
    {
      const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;

      var classContext = ClassContextObjectMother.Create(typeof (ClassOverridingMixinMembers), typeof (MixinWithAbstractMembers));
      var targetClassDefinition = _builder.Build (classContext);

      var overrider = targetClassDefinition.Events[typeof (ClassOverridingMixinMembers).GetEvent ("AbstractEvent")];
      var overridden = targetClassDefinition.Mixins[0].Events[typeof (MixinWithAbstractMembers).GetEvent ("AbstractEvent", bindingFlags)];

      Assert.That (overrider.Base, Is.SameAs (overridden));
      Assert.That (overridden.Overrides.ToArray (), Has.Member(overrider));
    }

    [Test]
    public void Build_AnalyzesAttributeIntroductions ()
    {
      var classContext = ClassContextObjectMother.Create(typeof (NullTarget), typeof (MixinAddingBT1Attribute));
      var targetClassDefinition = _builder.Build (classContext);

      Assert.That (targetClassDefinition.ReceivedAttributes.Select (a => a.AttributeType).ToArray (), Has.Member(typeof (BT1Attribute)));
    }

    [Test]
    public void Build_AnalyzesSuppressorsOnClass ()
    {
      var classContext = ClassContextObjectMother.Create(typeof (ClassWithSuppressAttribute), typeof (MixinAddingBT1Attribute));
      var targetClassDefinition = _builder.Build (classContext);

      Assert.That (targetClassDefinition.ReceivedAttributes.Select (a => a.AttributeType).ToArray (), Is.Empty);
    }

    [Test]
    public void Build_AnalyzesSuppressorsOnMixins ()
    {
      var classContext = ClassContextObjectMother.Create(typeof (NullTarget), typeof (MixinAddingBT1Attribute), typeof (MixinSuppressingBT1Attribute));
      var targetClassDefinition = _builder.Build (classContext);

      Assert.That (targetClassDefinition.ReceivedAttributes.Select (a => a.AttributeType).ToArray (), Has.No.Member(typeof (BT1Attribute)));
    }

    [Test]
    public void Build_AnalyzesAttributeIntroductionsOnMembers ()
    {
      var classContext = ClassContextObjectMother.Create(typeof (GenericTargetClass<string>), typeof (MixinAddingBT1AttributeToMember));
      var targetClassDefinition = _builder.Build (classContext);

      var methodInfo = typeof (GenericTargetClass<string>).GetMethod ("VirtualMethod");
      Assert.That (targetClassDefinition.Methods[methodInfo].ReceivedAttributes.Select (a => a.AttributeType).ToArray (), 
                   Has.Member(typeof (BT1Attribute)));
    }
  }
}
