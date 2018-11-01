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
using NUnit.Framework;
using Remotion.Mixins.Definitions.Building.RequiredMethodDefinitionBuilding;
using Remotion.Mixins.UnitTests.Core.Definitions.Building.RequiredMethodDefinitionBuilding.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.Definitions.Building.RequiredMethodDefinitionBuilding
{
  [TestFixture]
  public class DuckTypingRequiredMethodDefinitionCollectorTest
  {
    [Test]
    public void CreateRequiredMethodDefinitions ()
    {
      var targetClassDefinition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (ClassImplementingInterfaceWithDuckTyping));
      var m1 = DefinitionObjectMother.CreateMethodDefinition (
          targetClassDefinition, 
          typeof (ClassImplementingInterfaceWithDuckTyping).GetMethod ("Method1"));
      var m2 = DefinitionObjectMother.CreateMethodDefinition (
          targetClassDefinition, 
          typeof (ClassImplementingInterfaceWithDuckTyping).GetMethod ("Method2"));

      var requirement = DefinitionObjectMother.CreateRequiredTargetCallTypeDefinition (targetClassDefinition, typeof (IInterface));
      var builder = new DuckTypingRequiredMethodDefinitionCollector (targetClassDefinition);
      
      var definitions = builder.CreateRequiredMethodDefinitions (requirement).OrderBy (def => def.FullName).ToArray();
      Assert.That (definitions.Length, Is.EqualTo (2));

      Assert.That (definitions[0].DeclaringRequirement, Is.SameAs (requirement));
      Assert.That (definitions[0].InterfaceMethod, Is.EqualTo (typeof (IInterface).GetMethod ("Method1")));
      Assert.That (definitions[0].ImplementingMethod, Is.SameAs (m1));

      Assert.That (definitions[1].DeclaringRequirement, Is.SameAs (requirement));
      Assert.That (definitions[1].InterfaceMethod, Is.EqualTo (typeof (IInterface).GetMethod ("Method2")));
      Assert.That (definitions[1].ImplementingMethod, Is.SameAs (m2));
    }

    [Test]
    public void CreateRequiredMethodDefinitions_WithOverloads ()
    {
      var targetClassDefinition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (ClassImplementingInterfaceWithDuckTypingWithOverloads));
      DefinitionObjectMother.CreateMethodDefinition (
          targetClassDefinition,
          typeof (ClassImplementingInterfaceWithDuckTypingWithOverloads).GetMethod ("Method1", new[] { typeof (string ) }));
      var m1b = DefinitionObjectMother.CreateMethodDefinition (
          targetClassDefinition,
          typeof (ClassImplementingInterfaceWithDuckTypingWithOverloads).GetMethod ("Method1", Type.EmptyTypes));
      DefinitionObjectMother.CreateMethodDefinition (
          targetClassDefinition,
          typeof (ClassImplementingInterfaceWithDuckTypingWithOverloads).GetMethod ("Method2"));

      var requirement = DefinitionObjectMother.CreateRequiredTargetCallTypeDefinition (targetClassDefinition, typeof (IInterface));
      var builder = new DuckTypingRequiredMethodDefinitionCollector (targetClassDefinition);

      var definitions = builder.CreateRequiredMethodDefinitions (requirement).OrderBy (def => def.FullName).ToArray ();

      Assert.That (definitions[0].DeclaringRequirement, Is.SameAs (requirement));
      Assert.That (definitions[0].InterfaceMethod, Is.EqualTo (typeof (IInterface).GetMethod ("Method1")));
      Assert.That (definitions[0].ImplementingMethod, Is.SameAs (m1b));
    }

    [Test]
    public void CreateRequiredMethodDefinitions_BaseWithSameMembers ()
    {
      var targetClassDefinition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (ClassImplementingInterfaceWithDuckTypingWithBaseWithSameMembers));
      DefinitionObjectMother.CreateMethodDefinition (
          targetClassDefinition,
          typeof (ClassImplementingInterfaceWithDuckTyping).GetMethod ("Method1"));
      DefinitionObjectMother.CreateMethodDefinition (
          targetClassDefinition,
          typeof (ClassImplementingInterfaceWithDuckTyping).GetMethod ("Method2"));
      var m1b = DefinitionObjectMother.CreateMethodDefinition (
          targetClassDefinition,
          typeof (ClassImplementingInterfaceWithDuckTypingWithBaseWithSameMembers).GetMethod ("Method1"));
      var m2b = DefinitionObjectMother.CreateMethodDefinition (
          targetClassDefinition,
          typeof (ClassImplementingInterfaceWithDuckTypingWithBaseWithSameMembers).GetMethod ("Method2"));

      var requirement = DefinitionObjectMother.CreateRequiredTargetCallTypeDefinition (targetClassDefinition, typeof (IInterface));
      var builder = new DuckTypingRequiredMethodDefinitionCollector (targetClassDefinition);

      var definitions = builder.CreateRequiredMethodDefinitions (requirement).OrderBy (def => def.FullName).ToArray ();
      Assert.That (definitions.Length, Is.EqualTo (2));

      Assert.That (definitions[0].DeclaringRequirement, Is.SameAs (requirement));
      Assert.That (definitions[0].InterfaceMethod, Is.EqualTo (typeof (IInterface).GetMethod ("Method1")));
      Assert.That (definitions[0].ImplementingMethod, Is.SameAs (m1b));

      Assert.That (definitions[1].DeclaringRequirement, Is.SameAs (requirement));
      Assert.That (definitions[1].InterfaceMethod, Is.EqualTo (typeof (IInterface).GetMethod ("Method2")));
      Assert.That (definitions[1].ImplementingMethod, Is.SameAs (m2b));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage =
        "The dependency 'IBaseType2' (required by mixin 'System.String' on class 'System.Object') is not fulfilled - public or protected "
        + "method 'System.String IfcMethod()' could not be found on the target class.")]
    public void CreateRequiredMethodDefinitions_NoMatch ()
    {
      var targetClassDefinition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (object));
      var requiringMixin = DefinitionObjectMother.CreateMixinDefinition (targetClassDefinition, typeof (string));

      var dependency = DefinitionObjectMother.CreateTargetCallDependencyDefinition (requiringMixin);
      var requirement = dependency.RequiredType;
      DefinitionObjectMother.AddRequiringDependency (requirement, dependency);
      
      var builder = new DuckTypingRequiredMethodDefinitionCollector (targetClassDefinition);

      builder.CreateRequiredMethodDefinitions (requirement).ToArray ();
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage =
        "The dependency 'IInterface' (required by <unknown> on class 'System.Object') is not fulfilled - public or protected method "
        + "'Void Method1()' could not be found on the target class.")]
    public void CreateRequiredMethodDefinitions_NoMatch_NoRequiringMixin ()
    {
      var targetClassDefinition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (object));

      var requirement = DefinitionObjectMother.CreateRequiredTargetCallTypeDefinition (targetClassDefinition, typeof (IInterface));
      var builder = new DuckTypingRequiredMethodDefinitionCollector (targetClassDefinition);

      builder.CreateRequiredMethodDefinitions (requirement).ToArray ();
    }

  }
}
