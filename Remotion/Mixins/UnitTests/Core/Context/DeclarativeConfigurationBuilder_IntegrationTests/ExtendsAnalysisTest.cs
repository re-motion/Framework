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
using NUnit.Framework;
using Remotion.Mixins.Context;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.Context.DeclarativeConfigurationBuilder_IntegrationTests
{
  [TestFixture]
  public class ExtendsAnalysisTest
  {
    [Extends(typeof(object))]
    [IgnoreForMixinConfiguration]
    public class Extender
    {
    }

    [Test]
    public void Origin ()
    {
      var configuration = new DeclarativeConfigurationBuilder(null).AddType(typeof(Extender)).BuildConfiguration();
      var context = configuration.GetContext(typeof(object));
      var mixinContext = context.Mixins.Single();

      var expectedOrigin = new MixinContextOrigin(
          "ExtendsAttribute",
          typeof(Extender).Assembly,
          "Remotion.Mixins.UnitTests.Core.Context.DeclarativeConfigurationBuilder_IntegrationTests.ExtendsAnalysisTest+Extender");
      Assert.That(mixinContext.Origin, Is.EqualTo(expectedOrigin));
    }

    [Extends(typeof(object))]
    [IgnoreForMixinConfiguration]
    public class ExtenderWithoutDependencies
    {
    }

    [Extends(typeof(object), AdditionalDependencies = new[] { typeof(string) })]
    [IgnoreForMixinConfiguration]
    public class ExtenderWithDependencies
    {
    }

    [Test]
    public void AdditionalDependencies ()
    {
      MixinConfiguration configuration =
          new DeclarativeConfigurationBuilder(null).AddType(typeof(ExtenderWithDependencies)).AddType(typeof(ExtenderWithoutDependencies)).BuildConfiguration();
      Assert.That(configuration.GetContext(typeof(object)).Mixins[typeof(ExtenderWithoutDependencies)].ExplicitDependencies.Count, Is.EqualTo(0));
      Assert.That(configuration.GetContext(typeof(object)).Mixins[typeof(ExtenderWithDependencies)].ExplicitDependencies.Count, Is.EqualTo(1));
      Assert.That(configuration.GetContext(typeof(object)).Mixins[typeof(ExtenderWithDependencies)].ExplicitDependencies, Has.Member(typeof(string)));
    }

    public class ExtendsTargetBase { }

    [Extends(typeof(ExtendsTargetBase))]
    [Extends(typeof(ExtendsTargetDerivedWithExtends))]
    [IgnoreForMixinConfiguration]
    public class ExtendingMixin { }

    public class ExtendsTargetDerivedWithoutExtends : ExtendsTargetBase { }

    [Test]
    public void ExtendsAttributeAppliesToInheritanceChain ()
    {
      MixinConfiguration configuration = new DeclarativeConfigurationBuilder(null).AddType(typeof(ExtendingMixin))
          .AddType(typeof(ExtendsTargetDerivedWithoutExtends)).BuildConfiguration();
      Assert.That(configuration.GetContext(typeof(ExtendsTargetDerivedWithoutExtends)).Mixins.ContainsKey(typeof(ExtendingMixin)), Is.True);
      Assert.That(configuration.GetContext(typeof(ExtendsTargetDerivedWithoutExtends)).Mixins.Count, Is.EqualTo(1));
    }

    public class ExtendsTargetDerivedWithExtends : ExtendsTargetBase { }

    [Test]
    public void InheritedDuplicateExtensionIsIgnored ()
    {
      MixinConfiguration configuration = new DeclarativeConfigurationBuilder(null).AddType(typeof(ExtendingMixin))
          .AddType(typeof(ExtendsTargetDerivedWithExtends)).BuildConfiguration();
      Assert.That(configuration.GetContext(typeof(ExtendsTargetDerivedWithExtends)).Mixins.ContainsKey(typeof(ExtendingMixin)), Is.True);
      Assert.That(configuration.GetContext(typeof(ExtendsTargetDerivedWithExtends)).Mixins.Count, Is.EqualTo(1));
    }

    [Extends(typeof(ExtendsTargetDerivedWithDerivedExtends))]
    [IgnoreForMixinConfiguration]
    public class DerivedExtendingMixin : ExtendingMixin { }

    public class ExtendsTargetDerivedWithDerivedExtends : ExtendsTargetBase { }

    [Test]
    public void SubclassExtensionOverridesBaseExtends ()
    {
      MixinConfiguration configuration = new DeclarativeConfigurationBuilder(null)
          .AddType(typeof(ExtendingMixin))
          .AddType(typeof(DerivedExtendingMixin))
          .BuildConfiguration();

      var classContext = configuration.GetContext(typeof(ExtendsTargetDerivedWithDerivedExtends));

      Assert.That(classContext.Mixins.ContainsKey(typeof(ExtendingMixin)), Is.False);
      Assert.That(classContext.Mixins.ContainsKey(typeof(DerivedExtendingMixin)), Is.True);
      Assert.That(classContext.Mixins.Count, Is.EqualTo(1));
    }

    [Extends(typeof(ExtendsTargetDerivedWithDerivedExtends))]
    [IgnoreForMixinConfiguration]
    public class DerivedExtendingMixin2 : DerivedExtendingMixin { }

    [Test]
    public void ExplicitApplicationOfBaseAndDerivedMixinToSameClass ()
    {
      MixinConfiguration configuration = new DeclarativeConfigurationBuilder(null).AddType(typeof(ExtendingMixin)).AddType(typeof(DerivedExtendingMixin))
          .AddType(typeof(DerivedExtendingMixin2)).BuildConfiguration();

      Assert.That(configuration.GetContext(typeof(ExtendsTargetDerivedWithDerivedExtends)).Mixins.ContainsKey(typeof(ExtendingMixin)), Is.False);
      Assert.That(configuration.GetContext(typeof(ExtendsTargetDerivedWithDerivedExtends)).Mixins.ContainsKey(typeof(DerivedExtendingMixin)), Is.True);
      Assert.That(configuration.GetContext(typeof(ExtendsTargetDerivedWithDerivedExtends)).Mixins.ContainsKey(typeof(DerivedExtendingMixin2)), Is.True);
      Assert.That(configuration.GetContext(typeof(ExtendsTargetDerivedWithDerivedExtends)).Mixins.Count, Is.EqualTo(2));
    }

    [Extends(typeof(ExtendsTargetBase))]
    [Extends(typeof(ExtendsTargetBase))]
    [IgnoreForMixinConfiguration]
    public class DoubleExtendingMixin { }

    [Test]
    public void ThrowsOnDuplicateExtendsForSameClass ()
    {
      Assert.That(
          () => new DeclarativeConfigurationBuilder(null).AddType(typeof(DoubleExtendingMixin)).BuildConfiguration(),
          Throws.InstanceOf<ConfigurationException>()
              .With.Message.Matches(
                  "Two instances of mixin .*DoubleExtendingMixin are "
                  + "configured for target type .*ExtendsTargetBase."));
    }

    [Extends(typeof(ExtendsTargetBase))]
    [Extends(typeof(ExtendsTargetDerivedWithoutExtends))]
    [IgnoreForMixinConfiguration]
    public class MixinExtendingBaseAndDerived { }

    [Test]
    public void DuplicateExtendsForSameClassInInheritanceHierarchyIsIgnored ()
    {
      MixinConfiguration configuration = new DeclarativeConfigurationBuilder(null).AddType(typeof(MixinExtendingBaseAndDerived)).BuildConfiguration();
      Assert.That(configuration.GetContext(typeof(ExtendsTargetBase)).Mixins.ContainsKey(typeof(MixinExtendingBaseAndDerived)), Is.True);
      Assert.That(configuration.GetContext(typeof(ExtendsTargetDerivedWithoutExtends)).Mixins.ContainsKey(typeof(MixinExtendingBaseAndDerived)), Is.True);
      Assert.That(configuration.GetContext(typeof(ExtendsTargetDerivedWithoutExtends)).Mixins.Count, Is.EqualTo(1));
    }

    [Extends(typeof(ExtendsTargetBase), MixinTypeArguments = new[] { typeof(List<int>), typeof(IList<int>) })]
    [IgnoreForMixinConfiguration]
    public class GenericMixinWithSpecialization<TTarget, TNext> : Mixin<TTarget, TNext>
        where TTarget : class
        where TNext : class
    {
    }

    [Test]
    public void ExtendsCanSpecializeGenericMixin ()
    {
      MixinConfiguration configuration = new DeclarativeConfigurationBuilder(null).AddType(typeof(GenericMixinWithSpecialization<,>)).BuildConfiguration();
      MixinContext mixinContext = new List<MixinContext>(configuration.GetContext(typeof(ExtendsTargetBase)).Mixins)[0];
      Assert.That(Reflection.TypeExtensions.CanAscribeTo(mixinContext.MixinType, typeof(GenericMixinWithSpecialization<,>)), Is.True);
      Assert.That(mixinContext.MixinType.IsGenericTypeDefinition, Is.False);
      Assert.That(mixinContext.MixinType.ContainsGenericParameters, Is.False);
      Assert.That(mixinContext.MixinType.GetGenericArguments(), Is.EqualTo(new[] {typeof(List<int>), typeof(IList<int>)}));
    }

    [Extends(typeof(ExtendsTargetBase), MixinTypeArguments = new[] { typeof(List<int>) })]
    [IgnoreForMixinConfiguration]
    public class InvalidGenericMixin<TTarget, TNext> : Mixin<TTarget, TNext>
        where TTarget : class
        where TNext : class
    {
    }

    [Test]
    public void InvalidTypeParametersThrowConfigurationException ()
    {
      Assert.That(
          () => new DeclarativeConfigurationBuilder(null).AddType(typeof(InvalidGenericMixin<,>)).BuildConfiguration(),
          Throws.InstanceOf<ConfigurationException>()
              .With.Message.Contains("generic type argument"));
    }

    [Test]
    public void ExtendsAppliedToSpecificGenericClass ()
    {
      MixinConfiguration configuration = new DeclarativeConfigurationBuilder(null).AddType(typeof(MixinExtendingSpecificGenericClass)).BuildConfiguration();

      Assert.That(configuration.GetContext(typeof(GenericClassExtendedByMixin<int>)), Is.Not.Null);
      Assert.That(configuration.GetContext(typeof(GenericClassExtendedByMixin<string>)), Is.Null);
      Assert.That(configuration.GetContext(typeof(GenericClassExtendedByMixin<>)), Is.Null);
    }
  }
}
