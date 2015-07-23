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

namespace Remotion.Mixins.UnitTests.Core.IntegrationTests.AssemblyLevelMixinDependencies
{
  [TestFixture]
  public class GenericClassTest : AssemblyLevelMixinDependenciesTestBase
  {
    [Test]
    public void DependencyAddedToOpenGenericClass_ViaAssemblyLevelAttribute_AppliesWhenMixinIsInheritedByClosedClass ()
    {
      // M1 is applied to C<>, dependency via C<> => ok
      PrepareMixinConfigurationWithAttributeDeclarations (
          new AdditionalMixinDependencyAttribute (typeof (ClassWithMixinAppliedToOpenType_WithDependencyForOpenType<>), typeof (M1), typeof (M2)));

      var instance = ObjectFactory.Create<ClassWithMixinAppliedToOpenType_WithDependencyForOpenType<int>>();

      var result = instance.M();

      Assert.That (result, Is.EqualTo ("M1 M2 ClassWithMixinAppliedToOpenType_WithDependencyForOpenType<Int32>"));
    }

    [Test]
    public void DependencyAddedToOpenGenericClass_ViaAssemblyLevelAttribute_ErrorWhenMixinIsOnlyPresentOnClosedClass ()
    {
      // M1 is applied to C<int>, dependency via C<> => error
      Assert.That (
          () => PrepareMixinConfigurationWithAttributeDeclarations (
              new AdditionalMixinDependencyAttribute (typeof (ClassWithMixinAppliedToClosedType_WithDependencyForOpenType<>),
                  typeof (M1),
                  typeof (M2))),
          Throws.TypeOf<ConfigurationException>().With.Message.EqualTo (
              "The mixin dependencies configured for type "
              + "'Remotion.Mixins.UnitTests.Core.IntegrationTests.AssemblyLevelMixinDependencies.GenericClassTest+"
              + "ClassWithMixinAppliedToClosedType_WithDependencyForOpenType`1[T]' could not be processed: The mixin "
              + "'Remotion.Mixins.UnitTests.Core.IntegrationTests.AssemblyLevelMixinDependencies.GenericClassTest+M1' is not configured for class "
              + "'Remotion.Mixins.UnitTests.Core.IntegrationTests.AssemblyLevelMixinDependencies.GenericClassTest+"
              + "ClassWithMixinAppliedToClosedType_WithDependencyForOpenType`1[T]'."));
    }

    [Test]
    public void DependencyAddedToClosedGenericClass_ViaAssemblyLevelAttribute_SucceedsWhenMixinIsPresentOnOpenClass ()
    {
      // M1 is applied to C<>, dependency via C<int> => ok (when C<int> is instantiated)
      PrepareMixinConfigurationWithAttributeDeclarations (
          new AdditionalMixinDependencyAttribute (
              typeof (ClassWithMixinAppliedToOpenType_WithDependencyForClosedType<int>),
              typeof (M1),
              typeof (M2)));

      var instance = ObjectFactory.Create<ClassWithMixinAppliedToOpenType_WithDependencyForClosedType<int>>();

      var result = instance.M();

      Assert.That (result, Is.EqualTo ("M1 M2 ClassWithMixinAppliedToOpenType_WithDependencyForClosedType<Int32>"));
    }

    [Test]
    public void DependencyAddedToClosedGenericClass_ViaAssemblyLevelAttribute_DoesNotAffectOtherClosedGenericVariant ()
    {
      // M1 is applied to C<>, dependency via C<int> => missing ordering when C<string> is instantiated
      PrepareMixinConfigurationWithAttributeDeclarations (
          new AdditionalMixinDependencyAttribute (
              typeof (ClassWithMixinAppliedToOpenType_WithDependencyForClosedType<int>),
              typeof (M1),
              typeof (M2)));

      Assert.That (
          () => ObjectFactory.Create<ClassWithMixinAppliedToOpenType_WithDependencyForClosedType<string>>(),
          Throws.TypeOf<ConfigurationException>().With.Message.StringContaining ("base call ordering"));
    }

    [Test]
    public void DependencyAddedToClosedGenericClass_ViaAssemblyLevelAttribute_SucceedsWhenMixinIsPresentOnClosedClass ()
    {
      // M1 is applied to C<int>, dependency via C<int> => ok
      PrepareMixinConfigurationWithAttributeDeclarations (
          new AdditionalMixinDependencyAttribute (
              typeof (ClassWithMixinAppliedToClosedType_WithDependencyForClosedType<int>),
              typeof (M1),
              typeof (M2)));

      var instance = ObjectFactory.Create<ClassWithMixinAppliedToClosedType_WithDependencyForClosedType<int>>();

      var result = instance.M();

      Assert.That (result, Is.EqualTo ("M1 M2 ClassWithMixinAppliedToClosedType_WithDependencyForClosedType<Int32>"));
    }

    public class ClassWithMixinAppliedToOpenType_WithDependencyForOpenType<T> : IC
    {
      public virtual string M ()
      {
        return "ClassWithMixinAppliedToOpenType_WithDependencyForOpenType<" + typeof (T).Name + ">";
      }
    }

    public class ClassWithMixinAppliedToClosedType_WithDependencyForOpenType<T> : IC
    {
      public virtual string M ()
      {
        return "ClassWithMixinAppliedToClosedType_WithDependencyForOpenType<" + typeof (T).Name + ">";
      }
    }

    public class ClassWithMixinAppliedToOpenType_WithDependencyForClosedType<T> : IC
    {
      public virtual string M ()
      {
        return "ClassWithMixinAppliedToOpenType_WithDependencyForClosedType<" + typeof (T).Name + ">";
      }
    }

    public class ClassWithMixinAppliedToClosedType_WithDependencyForClosedType<T> : IC
    {
      public virtual string M ()
      {
        return "ClassWithMixinAppliedToClosedType_WithDependencyForClosedType<" + typeof (T).Name + ">";
      }
    }

    public interface IC
    {
      string M ();
    }

    [Extends (typeof (ClassWithMixinAppliedToOpenType_WithDependencyForOpenType<>))]
    [Extends (typeof (ClassWithMixinAppliedToClosedType_WithDependencyForOpenType<int>))]
    [Extends (typeof (ClassWithMixinAppliedToOpenType_WithDependencyForClosedType<>))]
    [Extends (typeof (ClassWithMixinAppliedToClosedType_WithDependencyForClosedType<int>))]
    public class M1 : Mixin<IC, IC>
    {
      [OverrideTarget]
      public string M ()
      {
        return "M1 " + Next.M();
      }
    }

    [Extends (typeof (ClassWithMixinAppliedToOpenType_WithDependencyForOpenType<>))]
    [Extends (typeof (ClassWithMixinAppliedToClosedType_WithDependencyForOpenType<>))]
    [Extends (typeof (ClassWithMixinAppliedToOpenType_WithDependencyForClosedType<>))]
    [Extends (typeof (ClassWithMixinAppliedToClosedType_WithDependencyForClosedType<>))]
    public class M2 : Mixin<IC, IC>
    {
      [OverrideTarget]
      public string M ()
      {
        return "M2 " + Next.M();
      }
    }
  }
}