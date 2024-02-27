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
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Mixins.Context;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.ServiceLocation;

namespace Remotion.Mixins.UnitTests.Core.Context.DeclarativeConfigurationBuilder_IntegrationTests
{
  [TestFixture]
  public class DeclarativeConfigurationBuilderGeneralTest
  {
    [Test]
    public void BuildFromAssemblies ()
    {
      var assemblies = new[] { typeof(BaseType1).Assembly, typeof(object).Assembly };
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies(assemblies);

      Assert.That(configuration.ClassContexts.ContainsWithInheritance(typeof(BaseType1)), Is.True);
      Assert.That(configuration.ClassContexts.ContainsWithInheritance(typeof(object)), Is.False);
    }

    [Test]
    public void BuildFromAssemblies_WithParentConfiguration ()
    {
      var parentConfiguration = new MixinConfiguration(new ClassContextCollection(ClassContextObjectMother.Create(typeof(object))));

      var assemblies = new[] { typeof(BaseType1).Assembly, typeof(object).Assembly };
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies(parentConfiguration, assemblies);

      Assert.That(configuration.ClassContexts.ContainsWithInheritance(typeof(BaseType1)), Is.True);
      Assert.That(configuration.ClassContexts.ContainsWithInheritance(typeof(object)), Is.True);
      Assert.That(configuration.GetContext(typeof(BaseType6)).ComposedInterfaces, Has.Member(typeof(ICBT6Mixin1)));
    }

    [Test]
    public void DuplicateAssembliesAreIgnored ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies(
          Assembly.GetExecutingAssembly(),
          Assembly.GetExecutingAssembly());

      ClassContext classContext = configuration.GetContext(typeof(BaseType1));
      Assert.That(classContext.Mixins.Count, Is.EqualTo(2));

      Assert.That(classContext.Mixins.ContainsKey(typeof(BT1Mixin1)), Is.True);
      Assert.That(classContext.Mixins.ContainsKey(typeof(BT1Mixin2)), Is.True);
    }

    [Test]
    public void DuplicateTypesAreIgnored ()
    {
      MixinConfiguration configuration = new DeclarativeConfigurationBuilder(null)
          .AddType(typeof(BaseType1))
          .AddType(typeof(BaseType1))
          .AddType(typeof(BT1Mixin1))
          .AddType(typeof(BT1Mixin1))
          .AddType(typeof(BT1Mixin2))
          .AddType(typeof(BT1Mixin2))
          .BuildConfiguration();

      ClassContext classContext = configuration.GetContext(typeof(BaseType1));
      Assert.That(classContext.Mixins.Count, Is.EqualTo(2));

      Assert.That(classContext.Mixins.ContainsKey(typeof(BT1Mixin1)), Is.True);
      Assert.That(classContext.Mixins.ContainsKey(typeof(BT1Mixin2)), Is.True);
    }

    [Test]
    public void BuildDefault ()
    {
      MixinConfiguration ac = DeclarativeConfigurationBuilder.BuildDefaultConfiguration();
      Assert.That(ac, Is.Not.Null);
      Assert.That(ac.ClassContexts.Count, Is.Not.EqualTo(0));
    }

    [Test]
    public void BuildDefault_DoesNotLockPersistedFile ()
    {
#if !FEATURE_ASSEMBLYBUILDER_SAVE
      Assert.Ignore(".NET does not support assembly persistence.");
#endif

      TypeGenerationHelper.ForceTypeGeneration(typeof(object));
      string[] paths = TypeGenerationHelper.Pipeline.CodeManager.FlushCodeToDisk();

      try
      {
        DeclarativeConfigurationBuilder.BuildDefaultConfiguration();
      }
      finally
      {
        var path = paths[0];
        File.Delete(path);
        File.Delete(path.Replace(".dll", ".pdb"));
      }
    }

    [Test]
    public void IgnoreForMixinConfiguration ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromTypes(
          null,
          new[] { typeof(BaseType1), typeof(BT1Mixin1), typeof(MixinWithIgnoreForMixinConfigurationAttribute) });

      Assert.That(configuration.GetContext(typeof(BaseType1)).Mixins.ContainsKey(typeof(MixinWithIgnoreForMixinConfigurationAttribute)), Is.False);
    }

    [Test]
    public void FilterExcludesSystemAssemblies ()
    {
      var filter = SafeServiceLocator.Current.GetInstance<IAssemblyLoaderFilter>();

      Assert.That(filter.ShouldConsiderAssembly(typeof(object).Assembly.GetName()), Is.False);
      Assert.That(filter.ShouldConsiderAssembly(typeof(Uri).Assembly.GetName()), Is.False);
    }

    [Test]
    public void FilterExcludesGeneratedAssemblies ()
    {
      var filter = SafeServiceLocator.Current.GetInstance<IAssemblyLoaderFilter>();

      Assembly generatedAssembly = TypeGenerationHelper.ForceTypeGeneration(typeof(object)).Assembly;

      Assert.That(filter.ShouldConsiderAssembly(generatedAssembly.GetName()), Is.False);
    }

    [Test]
    public void FilterIncludesAllNormalAssemblies ()
    {
      var filter = SafeServiceLocator.Current.GetInstance<IAssemblyLoaderFilter>();

      Assert.That(filter.ShouldConsiderAssembly(typeof(DeclarativeConfigurationBuilderGeneralTest).Assembly.GetName()), Is.True);
      Assert.That(filter.ShouldConsiderAssembly(typeof(DeclarativeConfigurationBuilder).Assembly.GetName()), Is.True);
      Assert.That(filter.ShouldConsiderAssembly(new AssemblyName("whatever")), Is.True);

      Assert.That(filter.ShouldIncludeAssembly(typeof(DeclarativeConfigurationBuilderGeneralTest).Assembly), Is.True);
      Assert.That(filter.ShouldIncludeAssembly(typeof(DeclarativeConfigurationBuilder).Assembly), Is.True);
    }

    [Test]
    public void MixinAttributeOnTargetClass ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies(Assembly.GetExecutingAssembly());

      ClassContext classContext = configuration.GetContext(typeof(TargetClassWithAdditionalDependencies));
      Assert.That(classContext, Is.Not.Null);

      Assert.That(classContext.Mixins.ContainsKey(typeof(MixinWithAdditionalClassDependency)), Is.True);
      Assert.That(
          classContext.Mixins[typeof(MixinWithAdditionalClassDependency)].ExplicitDependencies, Has.Member(typeof(MixinWithNoAdditionalDependency)));
    }

    [Test]
    public void MixinAttributeOnMixinClass ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies(Assembly.GetExecutingAssembly());

      ClassContext classContext = configuration.GetContext(typeof(BaseType1));
      Assert.That(classContext, Is.Not.Null);

      Assert.That(classContext.Mixins.ContainsKey(typeof(BT1Mixin1)), Is.True);
    }

    [Test]
    public void ComposedInterfaceConfiguredViaAttribute ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies(Assembly.GetExecutingAssembly());

      ClassContext classContext = configuration.GetContext(typeof(BaseType6));
      Assert.That(classContext, Is.Not.Null);

      Assert.That(classContext.ComposedInterfaces, Has.Member(typeof(ICBT6Mixin1)));
      Assert.That(classContext.ComposedInterfaces, Has.Member(typeof(ICBT6Mixin2)));
      Assert.That(classContext.ComposedInterfaces, Has.Member(typeof(ICBT6Mixin3)));
    }

    [Extends(typeof(BaseType1))]
    [IgnoreForMixinConfiguration]
    public class MixinWithIgnoreForMixinConfigurationAttribute { }
  }
}
