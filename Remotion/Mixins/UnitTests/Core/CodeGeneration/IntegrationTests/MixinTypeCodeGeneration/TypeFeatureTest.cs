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
using NUnit.Framework;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.MixinTypeCodeGeneration
{
  [TestFixture]
  public class TypeFeatureTest : CodeGenerationBaseTest
  {
    [Test]
    public void GeneratedTypeImplementsMarkerInterface ()
    {
      Type generatedType = CodeGenerationTypeMother.GetGeneratedMixinType (typeof (ClassOverridingMixinMembers), typeof (MixinWithAbstractMembers));
      Assert.That (typeof (IGeneratedMixinType).IsAssignableFrom (generatedType), Is.True);
    }

    [Test]
    public void GeneratedMixinTypeHasMixinTypeAttribute ()
    {
      Type generatedType = CodeGenerationTypeMother.GetGeneratedMixinType (typeof (ClassOverridingMixinMembers), typeof (MixinWithAbstractMembers));
      Assert.That (generatedType.IsDefined (typeof (ConcreteMixinTypeAttribute), false), Is.True);

      var attributes = (ConcreteMixinTypeAttribute[]) generatedType.GetCustomAttributes (typeof (ConcreteMixinTypeAttribute), false);
      Assert.That (attributes.Length, Is.EqualTo (1));
    }

    [Test]
    public void MixinTypeAttribute_CanBeUsedToGetIdentifier ()
    {
      var requestingClass = MixinConfiguration.ActiveConfiguration.GetContext (typeof (ClassOverridingMixinMembers));

      MixinDefinition mixinDefinition = DefinitionObjectMother.GetTargetClassDefinition (requestingClass).Mixins[typeof (MixinWithAbstractMembers)];
      Assert.That (mixinDefinition, Is.Not.Null);

      Type generatedType = CodeGenerationTypeMother.GetGeneratedMixinType (requestingClass, typeof (MixinWithAbstractMembers));
      Assert.That (generatedType.IsDefined (typeof (ConcreteMixinTypeAttribute), false), Is.True);

      var attributes = (ConcreteMixinTypeAttribute[]) generatedType.GetCustomAttributes (typeof (ConcreteMixinTypeAttribute), false);
      Assert.That (attributes[0].GetIdentifier(), Is.EqualTo (mixinDefinition.GetConcreteMixinTypeIdentifier()));
    }

    [Test]
    public void IdentifierMember_HoldsIdentifier ()
    {
      var requestingClass = MixinConfiguration.ActiveConfiguration.GetContext (typeof (ClassOverridingMixinMembers));

      MixinDefinition mixinDefinition = DefinitionObjectMother.GetTargetClassDefinition (requestingClass).Mixins[typeof (MixinWithAbstractMembers)];
      Assert.That (mixinDefinition, Is.Not.Null);
      Type generatedType = CodeGenerationTypeMother.GetGeneratedMixinType (requestingClass, typeof (MixinWithAbstractMembers));

      var identifier = generatedType.GetField ("__identifier").GetValue (null);

      Assert.That (identifier, Is.EqualTo (mixinDefinition.GetConcreteMixinTypeIdentifier()));
    }

    [Test]
    public void AbstractMixinWithoutAbstractMembers()
    {
      var instance = CreateMixedObject<NullTarget> (typeof (AbstractMixinWithoutAbstractMembers));
      var m1 = Mixin.Get<AbstractMixinWithoutAbstractMembers> (instance);
      Assert.That (m1, Is.Not.Null);
      Assert.That (m1, Is.InstanceOf (typeof (AbstractMixinWithoutAbstractMembers)));
      Assert.That (m1.GetType (), Is.Not.SameAs (typeof (AbstractMixinWithoutAbstractMembers)));
      Assert.That (m1.M1 (), Is.EqualTo ("AbstractMixinWithoutAbstractMembers.M1"));
    }

    [Test]
    public void NestedInterfaceWithOverrides ()
    {
      var generatedType = CodeGenerationTypeMother.GetGeneratedMixinType (typeof (ClassOverridingMixinMembers), typeof (MixinWithAbstractMembers));
      var overrideInterface = generatedType.GetNestedType ("IOverriddenMethods");

      Assert.That (overrideInterface, Is.Not.Null);
      
      var method = overrideInterface.GetMethod ("AbstractMethod");
      Assert.That (method, Is.Not.Null);
      Assert.That (method.ReturnType, Is.SameAs (typeof (string)));

      var parameters = method.GetParameters();
      Assert.That (parameters.Length, Is.EqualTo (1));
      Assert.That (parameters[0].ParameterType, Is.SameAs (typeof (int)));
      Assert.That (parameters[0].Name, Is.EqualTo ("i"));

      var propertyAccessor = overrideInterface.GetMethod ("get_AbstractProperty");
      Assert.That (propertyAccessor, Is.Not.Null);

      var eventAccessor = overrideInterface.GetMethod ("add_AbstractEvent");
      Assert.That (eventAccessor, Is.Not.Null);
    }
  }
}
