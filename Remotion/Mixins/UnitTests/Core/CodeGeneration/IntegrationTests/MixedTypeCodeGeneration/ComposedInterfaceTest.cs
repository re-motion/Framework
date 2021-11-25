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
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration
{
  [TestFixture]
  public class ComposedInterfaceTest : CodeGenerationBaseTest
  {
    [Test]
    public void GeneratedClass_ImplementsComposedInterface ()
    {
      var type = TypeFactory.GetConcreteType(typeof(ClassWithComposedInterface));
      Assert.That(type.GetInterfaces(), Has.Member(typeof(ClassWithComposedInterface.IComposedInterface)));

      var instance = (ClassWithComposedInterface.IComposedInterface)ObjectFactory.Create<ClassWithComposedInterface>();
      Assert.That(instance.M1(), Is.EqualTo("ClassWithComposedInterface.M1"));
      Assert.That(instance.Method(), Is.EqualTo("MixinImplementingSimpleInterface.Method"));
    }

    [Test]
    public void GeneratedClass_ImplementsComposedInterface_FromBase ()
    {
      var type = TypeFactory.GetConcreteType(typeof(DerivedClassWithComposedInterface));
      Assert.That(type.GetInterfaces(), Has.Member(typeof(ClassWithComposedInterface.IComposedInterface)));

      var instance = (ClassWithComposedInterface.IComposedInterface)ObjectFactory.Create<DerivedClassWithComposedInterface>();
      Assert.That(instance.M1(), Is.EqualTo("ClassWithComposedInterface.M1"));
      Assert.That(instance.Method(), Is.EqualTo("MixinImplementingSimpleInterface.Method"));
    }

    [Test]
    public void GeneratedClass_ImplementsComposedInterface_WithHasInterface_FromGenericBaseClass ()
    {
      var type = TypeFactory.GetConcreteType(typeof(ClassDerivedFromBaseClassWithHasComleteInterface));
      Assert.That(type.GetInterfaces(), Has.Member(typeof(ClassDerivedFromBaseClassWithHasComleteInterface.IComposedInterface)));

      var instance = (ClassDerivedFromBaseClassWithHasComleteInterface.IComposedInterface)ObjectFactory.Create<ClassDerivedFromBaseClassWithHasComleteInterface>();
      Assert.That(instance.M1(), Is.EqualTo("ClassDerivedFromBaseClassWithHasComleteInterface.M1"));
      Assert.That(instance.Method(), Is.EqualTo("MixinImplementingSimpleInterface.Method"));
    }

    [Test]
    public void GeneratedClass_ImplementsComposedInterface_WithHasInterface_FromBase ()
    {
      var type = TypeFactory.GetConcreteType(typeof(DerivedClassDerivedFromBaseClassWithHasComleteInterface));
      Assert.That(type.GetInterfaces(), Has.Member(typeof(ClassDerivedFromBaseClassWithHasComleteInterface.IComposedInterface)));

      var instance = (ClassDerivedFromBaseClassWithHasComleteInterface.IComposedInterface)ObjectFactory.Create<DerivedClassDerivedFromBaseClassWithHasComleteInterface>();
      Assert.That(instance.M1(), Is.EqualTo("ClassDerivedFromBaseClassWithHasComleteInterface.M1"));
      Assert.That(instance.Method(), Is.EqualTo("MixinImplementingSimpleInterface.Method"));
    }

    [Test]
    [Ignore ("RM-6845: System.NotSupportedException: Type 'Remotion.Mixins.UnitTests.Core.TestDomain.GenericClassWithComposedInterface`1_AssembledTypeProxy_1+NextCallProxy' was not completed.")]
    public void GeneratedGenericClass_ImplementsComposedInterface ()
    {
      var type = TypeFactory.GetConcreteType(typeof(GenericClassWithComposedInterface<string>));
      Assert.That(type.GetInterfaces(), Has.Member(typeof(IComposedInterfaceForGenericClassWithComposedInterface<string>)));

      var instance = ObjectFactory.Create<GenericClassWithComposedInterface<string>>();

      Assert.That(instance, Is.InstanceOf<IComposedInterfaceForGenericClassWithComposedInterface<string>>());
    }

    [Test]
    [Ignore ("RM-6845: Remotion.Mixins.ConfigurationException : The dependency 'IComposedInterfaceForGenericClassWithMembersWithComposedInterface`1' (required by composed interface 'Remotion.Mixins.UnitTests.Core.TestDomain.IComposedInterfaceForGenericClassWithMembersWithComposedInterface`1[T]' on class 'Remotion.Mixins.UnitTests.Core.TestDomain.GenericClassWithMembersWithComposedInterface`1') is not fulfilled - public or protected method 'System.String M1(T)' could not be found on the target class.")]
    public void GeneratedGenericClassWithMembers_ImplementsComposedInterface ()
    {
      var type = TypeFactory.GetConcreteType(typeof(GenericClassWithMembersWithComposedInterface<string>));
      Assert.That(type.GetInterfaces(), Has.Member(typeof(IComposedInterfaceForGenericClassWithMembersWithComposedInterface<string>)));

      var instance = (IComposedInterfaceForGenericClassWithMembersWithComposedInterface<string>)ObjectFactory.Create<GenericClassWithComposedInterface<string>>();
      Assert.That(instance.M1("v"), Is.EqualTo("GenericClassWithMembersWithComposedInterface.M1"));
      Assert.That(instance.Method("v"), Is.EqualTo("GenericMixinImplementingGenericInterfaceWithMembers.Method"));
    }

    [Test]
    [Ignore ("RM-6845: System.NotSupportedException: Type 'Remotion.Mixins.UnitTests.Core.TestDomain.GenericClassWithoutMixinWithComposedInterface`1_AssembledTypeProxy_7+NextCallProxy' was not completed.")]
    public void GeneratedGenericClassWithoutMixin_ImplementsComposedInterface ()
    {
      var type = TypeFactory.GetConcreteType(typeof(GenericClassWithoutMixinWithComposedInterface<string>));
      Assert.That(type.GetInterfaces(), Has.Member(typeof(IComposedInterfaceForGenericClassWithoutMixinWithComposedInterface<string>)));

      var instance = ObjectFactory.Create<GenericClassWithoutMixinWithComposedInterface<string>>();
      Assert.That(instance, Is.InstanceOf<IComposedInterfaceForGenericClassWithoutMixinWithComposedInterface<string>>());
    }

    [Test]
    [Ignore ("RM-6845: type.GetInterfaces() -> Expected: collection containing <Remotion.Mixins.UnitTests.Core.TestDomain.IComposedInterfaceForGenericClassWithMembersWithComposedInterfaceHasComposedInterface`1[System.String]>")]
    public void GeneratedGenericClassWithMembers_ImplementsComposedInterface_HasComposedInterface ()
    {
      var type = TypeFactory.GetConcreteType(typeof(GenericClassWithMembersWithComposedInterfaceWithHasComposedInterface<string>));
      Assert.That(type.GetInterfaces(), Has.Member(typeof(IComposedInterfaceForGenericClassWithMembersWithComposedInterfaceHasComposedInterface<string>)));

      var instance = (IComposedInterfaceForGenericClassWithMembersWithComposedInterfaceHasComposedInterface<string>)ObjectFactory.Create<GenericClassWithMembersWithComposedInterfaceWithHasComposedInterface<string>>();
      Assert.That(instance.M1("v"), Is.EqualTo("GenericClassWithMembersWithComposedInterfaceWithHasComposedInterface.M1"));
      Assert.That(instance.Method("v"), Is.EqualTo("GenericMixinImplementingGenericInterfaceWithMembers.Method"));
    }
  }
}
