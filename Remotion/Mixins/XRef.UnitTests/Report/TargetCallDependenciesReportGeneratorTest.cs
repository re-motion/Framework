// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using NUnit.Framework;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.XRef.Formatting;
using Remotion.Mixins.XRef.Report;
using Remotion.Mixins.XRef.UnitTests.Helpers;
using Remotion.Mixins.XRef.UnitTests.TestDomain;

namespace Remotion.Mixins.XRef.UnitTests.Report
{
  [TestFixture]
  public class TargetCallDependenciesReportGeneratorTest
  {
    private RemotionReflector _remotionReflector;
    private IOutputFormatter _outputFormatter;

    [SetUp]
    public void SetUp ()
    {
      _remotionReflector = new RemotionReflector();
      _outputFormatter = new OutputFormatter();
    }

    [Test]
    public void GenerateXml_InterfaceImplementedOnTargetClass ()
    {
      var targetType = new InvolvedType(typeof(TargetClass1));

      var mixinConfiguration = MixinConfiguration.BuildNew().ForClass<TargetClass1>().AddMixin<Mixin4>().BuildConfiguration();
      targetType.ClassContext = mixinConfiguration.ClassContexts.First();
      targetType.TargetClassDefinition = TargetClassDefinitionFactory.CreateWithoutValidation(mixinConfiguration.ClassContexts.GetExact(targetType.Type));
      var mixinContext = targetType.ClassContext.Mixins.First();
      var mixinDefinition = targetType.TargetClassDefinition.GetMixinByConfiguredType(mixinContext.MixinType);

      var assemblyIdentifierGenerator = new IdentifierGenerator<Assembly>();

      var output = new TargetCallDependenciesReportGenerator(
          mixinDefinition,
          assemblyIdentifierGenerator,
          _remotionReflector,
          _outputFormatter).GenerateXml();

      var expectedOutput = new XElement(
          "TargetCallDependencies",
          new XElement(
              "Dependency",
              new XAttribute("assembly-ref", "0"),
              new XAttribute("namespace", "System"),
              new XAttribute("name", "IDisposable"),
              new XAttribute("is-interface", true),
              new XAttribute("is-implemented-by-target", true),
              new XAttribute("is-added-by-mixin", false),
              new XAttribute("is-implemented-dynamically", false)));

      XElementComparisonHelper.Compare(output, expectedOutput);
    }

    [Test]
    public void GenerateXml_InterfaceDynamicallyImplemented ()
    {
      var targetType = new InvolvedType(typeof(TargetClass3));

      var mixinConfiguration = MixinConfiguration.BuildNew().ForClass<TargetClass3>().AddMixin<Mixin4>().BuildConfiguration();
      targetType.ClassContext = mixinConfiguration.ClassContexts.First();
      targetType.TargetClassDefinition = TargetClassDefinitionFactory.CreateWithoutValidation(mixinConfiguration.ClassContexts.GetExact(targetType.Type));
      var mixinContext = targetType.ClassContext.Mixins.First();
      var mixinDefinition = targetType.TargetClassDefinition.GetMixinByConfiguredType(mixinContext.MixinType);

      var assemblyIdentifierGenerator = new IdentifierGenerator<Assembly>();

      var output = new TargetCallDependenciesReportGenerator(
          mixinDefinition,
          assemblyIdentifierGenerator,
          _remotionReflector,
          _outputFormatter).GenerateXml();

      var expectedOutput = new XElement(
          "TargetCallDependencies",
          new XElement(
              "Dependency",
              new XAttribute("assembly-ref", "0"),
              new XAttribute("namespace", "System"),
              new XAttribute("name", "IDisposable"),
              new XAttribute("is-interface", true),
              new XAttribute("is-implemented-by-target", false),
              new XAttribute("is-added-by-mixin", false),
              new XAttribute("is-implemented-dynamically", true)));

      XElementComparisonHelper.Compare(output, expectedOutput);
    }
  }
}
