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
using System.Xml.Linq;
using NUnit.Framework;
using Remotion.Mixins.CrossReferencer.Formatting;
using Remotion.Mixins.CrossReferencer.UnitTests.Helpers;
using Remotion.Mixins.CrossReferencer.UnitTests.TestDomain;

namespace Remotion.Mixins.CrossReferencer.UnitTests.Report
{
  [TestFixture]
  public class InterfaceReportGeneratorTest
  {
    private IOutputFormatter _outputFormatter;

    [SetUp]
    public void SetUp ()
    {
      _outputFormatter = new OutputFormatter();
    }

    [Test]
    public void GenerateXml_ZeroInterfaces ()
    {
      var reportGenerator = ReportBuilder.CreateInterfaceReportGenerator(_outputFormatter);
      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement("Interfaces");
      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithInterfaces ()
    {
      // TargetClass1 implements IDisposable
      var involvedType = new InvolvedType(typeof(TargetClass1));

      var reportGenerator = ReportBuilder.CreateInterfaceReportGenerator(_outputFormatter, involvedType);
      var output = reportGenerator.GenerateXml();

      var memberReportGenerator = ReportBuilder.CreateMemberReportGenerator(typeof(IDisposable), _outputFormatter);
      var expectedOutput = new XElement(
          "Interfaces",
          new XElement(
              "Interface",
              new XAttribute("id", "0"),
              new XAttribute("assembly-ref", "0"),
              new XAttribute("namespace", "System"),
              new XAttribute("name", "IDisposable"),
              new XAttribute("is-composed-interface", false),
              memberReportGenerator.GenerateXml(),
              new XElement(
                  "ImplementedBy",
                  new XElement(
                      "InvolvedType-Reference",
                      new XAttribute("ref", "0"))
              )
          ));

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithComposedInterface ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<ComposedInterfacesTestClass.MyMixinTarget>()
          .AddComposedInterface<ComposedInterfacesTestClass.ICMyMixinTargetMyMixin>()
          .AddMixin<ComposedInterfacesTestClass.MyMixin>()
          .BuildConfiguration();

      var involvedType = new InvolvedType(typeof(ComposedInterfacesTestClass.MyMixinTarget));
      var classContext = mixinConfiguration.ClassContexts.GetWithInheritance(typeof(ComposedInterfacesTestClass.MyMixinTarget));
      involvedType.ClassContext = classContext;

      var reportGenerator = ReportBuilder.CreateInterfaceReportGenerator(_outputFormatter, involvedType);
      var output = reportGenerator.GenerateXml();

      var memberReportGenerator = ReportBuilder.CreateMemberReportGenerator(typeof(ComposedInterfacesTestClass.ICMyMixinTargetMyMixin), _outputFormatter);
      var expectedOutput = new XElement(
          "Interfaces",
          new XElement(
              "Interface",
              new XAttribute("id", "0"),
              new XAttribute("assembly-ref", "0"),
              new XAttribute("namespace", "Remotion.Mixins.CrossReferencer.UnitTests.TestDomain"),
              new XAttribute("name", "ComposedInterfacesTestClass+ICMyMixinTargetMyMixin"),
              new XAttribute("is-composed-interface", true),
              memberReportGenerator.GenerateXml(),
              new XElement(
                  "ImplementedBy",
                  new XElement(
                      "InvolvedType-Reference",
                      new XAttribute("ref", "0"))
              )
          ));

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    [Test]
    public void GetComposedInterfaces ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<ComposedInterfacesTestClass.MyMixinTarget>()
          .AddComposedInterface<ComposedInterfacesTestClass.ICMyMixinTargetMyMixin>()
          .AddMixin<ComposedInterfacesTestClass.MyMixin>()
          .BuildConfiguration();

      var involvedType = new InvolvedType(typeof(ComposedInterfacesTestClass.MyMixinTarget));
      var classContext = mixinConfiguration.ClassContexts.GetWithInheritance(typeof(ComposedInterfacesTestClass.MyMixinTarget));
      involvedType.ClassContext = classContext;

      var reportGenerator = ReportBuilder.CreateInterfaceReportGenerator(_outputFormatter, involvedType);
      var output = reportGenerator.GetComposedInterfaces();

      Assert.That(output, Is.EquivalentTo(classContext.ComposedInterfaces));
    }
  }
}
