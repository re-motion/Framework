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
using System.Xml.Linq;
using NUnit.Framework;
using Remotion.Mixins.CrossReferencer.Formatting;
using Remotion.Mixins.CrossReferencer.Report;
using Remotion.Mixins.CrossReferencer.UnitTests.TestDomain;
using Remotion.Mixins.CrossReferencer.Utilities;
using Remotion.Mixins.Definitions;

namespace Remotion.Mixins.CrossReferencer.UnitTests.Report
{
  [TestFixture]
  public class MixinReferenceReportGeneratorTest
  {
    private IOutputFormatter _outputFormatter;

    [SetUp]
    public void SetUp ()
    {
      _outputFormatter = new OutputFormatter();
    }

    [Test]
    public void GenerateXml_NoMixins ()
    {
      var involvedTypeDummy = new InvolvedType(typeof(object));

      var reportGenerator = new MixinReferenceReportGenerator(
          involvedTypeDummy,
          new IdentifierGenerator<Assembly>(),
          new IdentifierGenerator<Type>(),
          new IdentifierGenerator<Type>(),
          new IdentifierGenerator<Type>(),
          _outputFormatter
      );

      var output = reportGenerator.GenerateXml();

      Assert.That(output, Is.Null);
    }

    [Test]
    public void GenerateXml_WithMixins ()
    {
      var targetType = new InvolvedType(typeof(TargetClass1));

      var mixinConfiguration = MixinConfiguration.BuildNew().ForClass<TargetClass1>().AddMixin<Mixin1>().BuildConfiguration();
      targetType.ClassContext = mixinConfiguration.ClassContexts.First();
      targetType.TargetClassDefinition = TargetClassDefinitionFactory.CreateWithoutValidation(mixinConfiguration.ClassContexts.GetExact(targetType.Type));

      var interfaceIdentifierGenerator = new IdentifierGenerator<Type>();
      var attributeIdentifierGenerator = new IdentifierGenerator<Type>();
      var assemblyIdentifierGenerator = new IdentifierGenerator<Assembly>();

      var reportGenerator = new MixinReferenceReportGenerator(
          targetType,
          assemblyIdentifierGenerator,
          new IdentifierGenerator<Type>(),
          interfaceIdentifierGenerator,
          attributeIdentifierGenerator,
          _outputFormatter
      );

      var output = reportGenerator.GenerateXml();

      var targetClassDefinition = TargetClassDefinitionFactory.CreateWithoutValidation(mixinConfiguration.ClassContexts.GetExact(targetType.Type));
      var mixinDefinition = targetClassDefinition.GetMixinByConfiguredType(typeof(Mixin1));

      var expectedOutput = new XElement(
          "Mixins",
          new XElement(
              "Mixin",
              new XAttribute("ref", "0"),
              new XAttribute("index", "0"),
              new XAttribute("relation", "Extends"),
              new XAttribute("instance-name", "Mixin1"),
              new XAttribute("introduced-member-visibility", "private"),
              // has no dependencies
              new XElement("AdditionalDependencies"),
              new InterfaceIntroductionReportGenerator(mixinDefinition.InterfaceIntroductions, interfaceIdentifierGenerator).GenerateXml(),
              new AttributeIntroductionReportGenerator(
                  mixinDefinition.AttributeIntroductions,
                  attributeIdentifierGenerator).GenerateXml(),
              new MemberOverrideReportGenerator(mixinDefinition.GetAllOverrides()).GenerateXml(),
              new TargetCallDependenciesReportGenerator( mixinDefinition, assemblyIdentifierGenerator, _outputFormatter).GenerateXml()
          ));

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_ForGenericTypeDefinition ()
    {
      var targetType = new InvolvedType(typeof(GenericTarget<,>));

      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass(typeof(GenericTarget<,>)).AddMixin<ClassWithBookAttribute>().AddMixin<Mixin3>()
          .BuildConfiguration();
      targetType.ClassContext = mixinConfiguration.ClassContexts.First();

      var interfaceIdentifierGenerator = new IdentifierGenerator<Type>();
      var attributeIdentifierGenerator = new IdentifierGenerator<Type>();
      var assemblyIdentifierGenerator = new IdentifierGenerator<Assembly>();

      var reportGenerator = new MixinReferenceReportGenerator(
          targetType,
          assemblyIdentifierGenerator,
          // generic target class
          new IdentifierGenerator<Type>(),
          interfaceIdentifierGenerator,
          attributeIdentifierGenerator,
          _outputFormatter);

      var output = reportGenerator.GenerateXml();
      var expectedOutput = new XElement(
          "Mixins",
          new XElement(
              "Mixin",
              new XAttribute("ref", "0"),
              new XAttribute("index", "n/a"),
              new XAttribute("relation", "Extends"),
              new XAttribute("instance-name", "ClassWithBookAttribute"),
              new XAttribute("introduced-member-visibility", "private"),
              // has no dependencies
              new XElement("AdditionalDependencies")
          ),
          new XElement(
              "Mixin",
              new XAttribute("ref", "1"),
              new XAttribute("index", "n/a"),
              new XAttribute("relation", "Extends"),
              new XAttribute("instance-name", "Mixin3"),
              new XAttribute("introduced-member-visibility", "private"),
              // has no dependencies
              new XElement("AdditionalDependencies")
          )
      );

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }
  }
}
