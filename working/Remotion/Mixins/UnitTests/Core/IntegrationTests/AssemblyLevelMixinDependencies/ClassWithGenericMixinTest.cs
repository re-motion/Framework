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
  public class ClassWithGenericMixinTest : AssemblyLevelMixinDependenciesTestBase
  {
    [Test]
    public void DependencyAddedToOpenGenericMixin_ViaAssemblyLevelAttribute_AppliesWhenMixinIsConfiguredOpenGeneric ()
    {
      // Configuration and attribute match, so the dependency is valid
      PrepareMixinConfigurationWithAttributeDeclarations (
          new AdditionalMixinDependencyAttribute (
              typeof (ClassWithOpenMixin_WithDependencyForOpenMixin),
              typeof (M1<>),
              typeof (M2)));

      var instance = ObjectFactory.Create<ClassWithOpenMixin_WithDependencyForOpenMixin>();

      var result = instance.M();

      Assert.That (result, Is.EqualTo ("M1<ClassWithOpenMixin_WithDependencyForOpenMixin> M2 ClassWithOpenMixin_WithDependencyForOpenMixin"));
    }

    [Test]
    public void DependencyAddedToClosedGenericMixin_ViaAssemblyLevelAttribute_ErrorWhenMixinIsConfiguredOpenGeneric ()
    {
      // Configuration and attribute don't match, so the dependency is invalid - the mixin can't be found
      Assert.That (
          () => PrepareMixinConfigurationWithAttributeDeclarations (
              new AdditionalMixinDependencyAttribute (
                  typeof (ClassWithOpenMixin_WithDependencyForClosedMixin),
                  typeof (M1<ClassWithOpenMixin_WithDependencyForClosedMixin>),
                  typeof (M2))),
          Throws.TypeOf<ConfigurationException>().With.Message.EqualTo (
              "The mixin dependencies configured for type 'Remotion.Mixins.UnitTests.Core.IntegrationTests.AssemblyLevelMixinDependencies."
              + "ClassWithGenericMixinTest+ClassWithOpenMixin_WithDependencyForClosedMixin' could not be processed: The mixin "
              + "'Remotion.Mixins.UnitTests.Core.IntegrationTests.AssemblyLevelMixinDependencies.ClassWithGenericMixinTest+M1`1"
              + "[Remotion.Mixins.UnitTests.Core.IntegrationTests.AssemblyLevelMixinDependencies."
              + "ClassWithGenericMixinTest+ClassWithOpenMixin_WithDependencyForClosedMixin]' is not configured for class "
              + "'Remotion.Mixins.UnitTests.Core.IntegrationTests.AssemblyLevelMixinDependencies."
              + "ClassWithGenericMixinTest+ClassWithOpenMixin_WithDependencyForClosedMixin'."));
    }

    [Test]
    public void DependencyAddedToOpenGenericMixin_ViaAssemblyLevelAttribute_AppliesWhenMixinIsConfiguredClosedGeneric ()
    {
      // Configuration and attribute don't match, but it's obvious which mixin is meant, so the dependency is valid
      PrepareMixinConfigurationWithAttributeDeclarations (
          new AdditionalMixinDependencyAttribute (
              typeof (ClassWithClosedMixin_WithDependencyForOpenMixin),
              typeof (M1<>),
              typeof (M2)));

      var instance = ObjectFactory.Create<ClassWithClosedMixin_WithDependencyForOpenMixin>();

      var result = instance.M();

      Assert.That (result, Is.EqualTo ("M1<Int32> M2 ClassWithClosedMixin_WithDependencyForOpenMixin"));
    }

    [Test]
    public void DependencyAddedToClosedGenericMixin_ViaAssemblyLevelAttribute_AppliesWhenMixinIsConfiguredClosedGeneric ()
    {
      // Configuration and attribute match, so the dependency is valid
      PrepareMixinConfigurationWithAttributeDeclarations (
          new AdditionalMixinDependencyAttribute (
              typeof (ClassWithClosedMixin_WithDependencyForClosedMixin),
              typeof (M1<int>),
              typeof (M2)));

      var instance = ObjectFactory.Create<ClassWithClosedMixin_WithDependencyForClosedMixin>();

      var result = instance.M();

      Assert.That (result, Is.EqualTo ("M1<Int32> M2 ClassWithClosedMixin_WithDependencyForClosedMixin"));
    }

    public class ClassWithOpenMixin_WithDependencyForOpenMixin : IC
    {
      public virtual string M ()
      {
        return "ClassWithOpenMixin_WithDependencyForOpenMixin";
      }
    }

    public class ClassWithOpenMixin_WithDependencyForClosedMixin : IC
    {
      public virtual string M ()
      {
        return "ClassWithOpenMixin_WithDependencyForClosedMixin";
      }
    }

    public class ClassWithClosedMixin_WithDependencyForOpenMixin : IC
    {
      public virtual string M ()
      {
        return "ClassWithClosedMixin_WithDependencyForOpenMixin";
      }
    }

    public class ClassWithClosedMixin_WithDependencyForClosedMixin : IC
    {
      public virtual string M ()
      {
        return "ClassWithClosedMixin_WithDependencyForClosedMixin";
      }
    }

    public interface IC
    {
      string M ();
    }

    [Extends (typeof (ClassWithOpenMixin_WithDependencyForOpenMixin))]
    [Extends (typeof (ClassWithOpenMixin_WithDependencyForClosedMixin))]
    [Extends (typeof (ClassWithClosedMixin_WithDependencyForOpenMixin), MixinTypeArguments = new[] { typeof (int) })]
    [Extends (typeof (ClassWithClosedMixin_WithDependencyForClosedMixin), MixinTypeArguments = new[] { typeof (int) })]
    public class M1<[BindToTargetType] T> : Mixin<object, IC>
    {
      [OverrideTarget]
      public string M ()
      {
        return "M1<" + typeof (T).Name + "> " + Next.M();
      }
    }

    [Extends (typeof (ClassWithOpenMixin_WithDependencyForOpenMixin))]
    [Extends (typeof (ClassWithOpenMixin_WithDependencyForClosedMixin))]
    [Extends (typeof (ClassWithClosedMixin_WithDependencyForOpenMixin))]
    [Extends (typeof (ClassWithClosedMixin_WithDependencyForClosedMixin))]
    public class M2 : Mixin<object, IC>
    {
      [OverrideTarget]
      public string M ()
      {
        return "M2 " + Next.M();
      }
    }
  }
}