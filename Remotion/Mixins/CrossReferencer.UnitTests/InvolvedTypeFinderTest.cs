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
using Remotion.Mixins.CrossReferencer.Reflectors;
using Remotion.Mixins.CrossReferencer.UnitTests.TestDomain;
using Remotion.Mixins.CrossReferencer.Utilities;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;

namespace Remotion.Mixins.CrossReferencer.UnitTests
{
  [TestFixture]
  public class InvolvedTypeFinderTest
  {
    private ErrorAggregator<ConfigurationException> _configurationErrors;
    private ErrorAggregator<ValidationException> _validationErrors;

    [SetUp]
    public void SetUp ()
    {
      _configurationErrors = new ErrorAggregator<ConfigurationException>();
      _validationErrors = new ErrorAggregator<ValidationException>();
    }

    [Test]
    public void FindInvolvedTypes_EmptyConfiguration ()
    {
      var mixinConfiguration = new MixinConfiguration();
      var involvedTypeFinder = CreateInvolvedTypeFinder(mixinConfiguration, new Assembly[0]);

      var involvedTypes = involvedTypeFinder.FindInvolvedTypes();

      Assert.That(involvedTypes, Is.Empty);
    }

    [Test]
    public void FindInvolvedTypes_WithOneTarget ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew().ForClass<TargetClass1>().AddMixin<Mixin1>().BuildConfiguration();
      var assembly = typeof(Mixin1).Assembly;
      var involvedTypeFinder = CreateInvolvedTypeFinder(mixinConfiguration, new[] { assembly });

      var involvedTypes = involvedTypeFinder.FindInvolvedTypes();

      var expectedType1 = new InvolvedType(typeof(TargetClass1));
      expectedType1.ClassContext = mixinConfiguration.ClassContexts.First();
      expectedType1.TargetClassDefinition = CreateTargetClassDefintion<TargetClass1>(mixinConfiguration);

      var expectedType2 = new InvolvedType(typeof(Mixin1));
      expectedType2.TargetTypes.Add(expectedType1, expectedType1.TargetClassDefinition.GetMixinByConfiguredType(typeof(Mixin1)));

      AssertSameTypes(involvedTypes, expectedType1, expectedType2);
    }

    [Test]
    public void FindInvolvedTypes_WithMoreTargets ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<TargetClass1>().AddMixin<Mixin1>()
          .ForClass<TargetClass2>().AddMixin<Mixin2>()
          .BuildConfiguration();
      var involvedTypeFinder = CreateInvolvedTypeFinder(mixinConfiguration, new[] { typeof(Mixin1).Assembly });

      var involvedTypes = involvedTypeFinder.FindInvolvedTypes();

      var expectedType1 = new InvolvedType(typeof(TargetClass1));
      expectedType1.ClassContext = mixinConfiguration.ClassContexts.First();
      expectedType1.TargetClassDefinition = CreateTargetClassDefintion<TargetClass1>(mixinConfiguration);

      var expectedType2 = new InvolvedType(typeof(Mixin1));
      expectedType2.TargetTypes.Add(expectedType1, expectedType1.TargetClassDefinition.GetMixinByConfiguredType(typeof(Mixin1)));

      var expectedType3 = new InvolvedType(typeof(TargetClass2));
      expectedType3.ClassContext = mixinConfiguration.ClassContexts.Last();
      expectedType3.TargetClassDefinition = CreateTargetClassDefintion<TargetClass2>(mixinConfiguration);

      var expectedType4 = new InvolvedType(typeof(Mixin2));
      expectedType4.TargetTypes.Add(expectedType3, expectedType3.TargetClassDefinition.GetMixinByConfiguredType(typeof(Mixin2)));

      AssertSameTypes(involvedTypes, expectedType1, expectedType2, expectedType3, expectedType4);
    }

    [Test]
    public void FindInvolvedTypes_WithMixedMixin ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<TargetClass1>().AddMixin<Mixin1>()
          .ForClass<Mixin1>().AddMixin<Mixin2>()
          .BuildConfiguration();
      var involvedTypeFinder = CreateInvolvedTypeFinder(mixinConfiguration, new[] { typeof(Mixin1).Assembly });

      var involvedTypes = involvedTypeFinder.FindInvolvedTypes();

      var expectedType1 = new InvolvedType(typeof(TargetClass1));
      expectedType1.ClassContext = mixinConfiguration.ClassContexts.First();
      expectedType1.TargetClassDefinition = CreateTargetClassDefintion<TargetClass1>(mixinConfiguration);

      var expectedType2 = new InvolvedType(typeof(Mixin1));
      expectedType2.ClassContext = mixinConfiguration.ClassContexts.Last();
      expectedType2.TargetClassDefinition = CreateTargetClassDefintion<Mixin1>(mixinConfiguration);
      expectedType2.TargetTypes.Add(expectedType1, expectedType1.TargetClassDefinition.GetMixinByConfiguredType(typeof(Mixin1)));

      var expectedType3 = new InvolvedType(typeof(Mixin2));
      expectedType3.TargetTypes.Add(expectedType2, expectedType2.TargetClassDefinition.GetMixinByConfiguredType(typeof(Mixin2)));

      AssertSameTypes(involvedTypes, expectedType1, expectedType2, expectedType3);
    }

    [Test]
    public void FindInvolvedTypes_WithTargetClassInheritance ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<UselessObject>().AddMixin<Mixin1>()
          .BuildConfiguration();
      var involvedTypeFinder = CreateInvolvedTypeFinder(mixinConfiguration, new[] { typeof(Mixin1).Assembly });

      var involvedTypes = involvedTypeFinder.FindInvolvedTypes();

      var expectedType1 = new InvolvedType(typeof(UselessObject));
      expectedType1.ClassContext = mixinConfiguration.ClassContexts.First();
      expectedType1.TargetClassDefinition = CreateTargetClassDefintion<UselessObject>(mixinConfiguration);

      var expectedType2 = new InvolvedType(typeof(ClassInheritsFromUselessObject));
      expectedType2.ClassContext = mixinConfiguration.ClassContexts.GetWithInheritance(typeof(ClassInheritsFromUselessObject));
      expectedType2.TargetClassDefinition = CreateTargetClassDefintion<ClassInheritsFromUselessObject>(mixinConfiguration);

      var expectedType3 = new InvolvedType(typeof(Mixin1));
      expectedType3.TargetTypes.Add(expectedType1, expectedType1.TargetClassDefinition.GetMixinByConfiguredType(typeof(Mixin1)));
      expectedType3.TargetTypes.Add(expectedType2, expectedType2.TargetClassDefinition.GetMixinByConfiguredType(typeof(Mixin2)));

      AssertSameTypes(involvedTypes, expectedType1, expectedType2, expectedType3);
    }

    [Test]
    public void FindInvolvedTypes_UnusedMixin ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .BuildConfiguration();
      var involvedTypeFinder = CreateInvolvedTypeFinder(mixinConfiguration, new[] { typeof(UnusedMixin).Assembly });

      var involvedTypes = involvedTypeFinder.FindInvolvedTypes();

      Assert.That(involvedTypes, Contains.Item(new InvolvedType(typeof(UnusedMixin))));
    }

    [Test]
    public void GetTargetClassDefinition ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<TargetClass1>().AddMixin<Mixin1>()
          .BuildConfiguration();
      var involvedTypeFinder = CreateInvolvedTypeFinder(mixinConfiguration, typeof(UselessObject).Assembly);
      var targetType = typeof(TargetClass1);
      var classContextForTargetType = mixinConfiguration.ClassContexts.GetWithInheritance(targetType);

      var expectedClassContext = mixinConfiguration.ClassContexts.GetExact(targetType);
      Assert.That(expectedClassContext, Is.Not.Null);

      var output = involvedTypeFinder.GetTargetClassDefinition(targetType, classContextForTargetType);

      Assert.That(output, Is.Not.Null);
      Assert.That(output.Type, Is.EqualTo(expectedClassContext.Type));
      Assert.That(output.Mixins.Select(m => m.Type), Is.EquivalentTo(expectedClassContext.Mixins.Select(m => m.MixinType)));
      Assert.That(output.ConfigurationContext, Is.SameAs(expectedClassContext));
    }

    [Test]
    public void GetTargetClassDefinition_GenericTarget ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass(typeof(GenericTarget<,>)).AddMixin<Mixin1>()
          .BuildConfiguration();
      var involvedTypeFinder = CreateInvolvedTypeFinder(mixinConfiguration, typeof(UselessObject).Assembly);
      var targetType = typeof(GenericTarget<,>);
      var classContextForTargetType = mixinConfiguration.ClassContexts.GetWithInheritance(targetType);
      var output = involvedTypeFinder.GetTargetClassDefinition(targetType, classContextForTargetType);

      Assert.That(_configurationErrors.Exceptions.Count(), Is.EqualTo(0));
      Assert.That(_validationErrors.Exceptions.Count(), Is.EqualTo(0));
      Assert.That(output, Is.Null);
    }

    [Test]
    public void GetTargetClassDefinition_Interface ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<IUseless>().AddMixin<Mixin1>()
          .BuildConfiguration();
      var involvedTypeFinder = CreateInvolvedTypeFinder(mixinConfiguration, typeof(UselessObject).Assembly);
      var targetType = typeof(IUseless);
      var classContextForTargetType = mixinConfiguration.ClassContexts.GetWithInheritance(targetType);
      var output = involvedTypeFinder.GetTargetClassDefinition(targetType, classContextForTargetType);

      Assert.That(_configurationErrors.Exceptions.Count(), Is.EqualTo(0));
      Assert.That(_validationErrors.Exceptions.Count(), Is.EqualTo(0));
      Assert.That(output, Is.Null);
    }

    [Test]
    public void GenerateXml_MixinConfigurationError ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<UselessObject>().AddMixin<MixinWithConfigurationError>()
          .BuildConfiguration();
      var involvedTypeFinder = CreateInvolvedTypeFinder(mixinConfiguration, typeof(UselessObject).Assembly);
      var targetType = typeof(UselessObject);
      var classContextForTargetType = mixinConfiguration.ClassContexts.GetWithInheritance(targetType);

      var output = involvedTypeFinder.GetTargetClassDefinition(targetType, classContextForTargetType);

      Assert.That(_configurationErrors.Exceptions.Count(), Is.EqualTo(1));
      Assert.That(output, Is.Null);
    }

    [Test]
    public void GenerateXml_MixinValidationError ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<UselessObject>().AddMixin<UselessObject>()
          .BuildConfiguration();
      var involvedTypeFinder = CreateInvolvedTypeFinder(mixinConfiguration, typeof(UselessObject).Assembly);
      var targetType = typeof(UselessObject);
      var classContextForTargetType = mixinConfiguration.ClassContexts.GetWithInheritance(targetType);

      var output = involvedTypeFinder.GetTargetClassDefinition(targetType, classContextForTargetType);

      Assert.That(_validationErrors.Exceptions.Count(), Is.EqualTo(1));
      Assert.That(output, Is.Null);
    }

    private InvolvedTypeFinder CreateInvolvedTypeFinder (MixinConfiguration mixinConfiguration, params Assembly[] assemblies)
    {
      return new InvolvedTypeFinder(
          mixinConfiguration,
          assemblies,
          _configurationErrors,
          _validationErrors,
          new RemotionReflector());
    }

    private TargetClassDefinition CreateTargetClassDefintion<ForType> (MixinConfiguration mixinConfiguration)
    {
      return TargetClassDefinitionFactory.CreateWithoutValidation(mixinConfiguration.ClassContexts.GetWithInheritance( typeof(ForType)));
    }

    private void AssertSameTypes (InvolvedType[] involvedTypes, params InvolvedType[] expectedTypes)
    {
      Assert.That(involvedTypes.Select(t => t.Type), Is.EquivalentTo(GetAdditonalAssemblyInvolvedTypes(expectedTypes).Select(t => t.Type)));
    }

    private InvolvedType[] GetAdditonalAssemblyInvolvedTypes (params InvolvedType[] explicitInvolvedTypes)
    {
      var implicitInvolvedTypes = new List<InvolvedType>();
      var remotionReflector = new RemotionReflector();
      var assembly = typeof(Mixin1).Assembly;

      foreach (var type in assembly.GetTypes())
      {
        // also add classes which inherit from Mixin<> or Mixin<,>, but are actually not used as Mixins (not in ClassContexts)
        if (remotionReflector.IsInheritedFromMixin(type) && !remotionReflector.IsInfrastructureType(type))
          implicitInvolvedTypes.Add(new InvolvedType(type));
      }

      var allInvolvedTypes = new InvolvedType[explicitInvolvedTypes.Length + implicitInvolvedTypes.Count];
      explicitInvolvedTypes.CopyTo(allInvolvedTypes, 0);
      implicitInvolvedTypes.CopyTo(allInvolvedTypes, explicitInvolvedTypes.Length);

      return allInvolvedTypes;
    }
  }
}
