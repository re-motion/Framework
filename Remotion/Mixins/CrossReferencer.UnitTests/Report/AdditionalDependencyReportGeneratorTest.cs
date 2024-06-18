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
using System.Xml.Linq;
using MixinXRef.Formatting;
using MixinXRef.Reflection;
using MixinXRef.Reflection.Utility;
using MixinXRef.Report;
using MixinXRef.UnitTests.Reflection;
using MixinXRef.UnitTests.TestDomain;
using MixinXRef.Utility;
using NUnit.Framework;
using Remotion.Mixins;

namespace MixinXRef.UnitTests.Report
{
  [TestFixture]
  public class AdditionalDependencyReportGeneratorTest
  {
    private MixinConfiguration _mixinConfiguration;
    private IOutputFormatter _outputFormatter;
    private IIdentifierGenerator<Type> _identifierGenerator;

    [SetUp]
    public void SetUp ()
    {
      _mixinConfiguration = MixinConfiguration.BuildNew().ForClass<AdditionalDependenciesTest.TargetClass>()
          .AddMixin<AdditionalDependenciesTest.Mixin1>()
          .AddMixin<AdditionalDependenciesTest.Mixin2>()
          .AddMixin<AdditionalDependenciesTest.Mixin3>()
          .WithDependencies<AdditionalDependenciesTest.Mixin1, AdditionalDependenciesTest.Mixin2>()
          .BuildConfiguration();
      _outputFormatter = new OutputFormatter();
      _identifierGenerator = new IdentifierGenerator<Type>();
    }

    [Test]
    public void GenerateXml_NoDependencies ()
    {
      // Mixin1 has no depencies
      var explicitDependencies = _mixinConfiguration.ClassContexts.Single().Mixins.First().ExplicitDependencies;

      var dependencies = new ReflectedObject (explicitDependencies);
      var output = new AdditionalDependencyReportGenerator (dependencies, _identifierGenerator, _outputFormatter).GenerateXml();
      var expectedOutput = new XElement ("AdditionalDependencies");

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithDependencies ()
    {
      var explicitDependencies = _mixinConfiguration.ClassContexts.Single().Mixins.Last().ExplicitDependencies;

      var dependencies = new ReflectedObject (explicitDependencies);
      var output = new AdditionalDependencyReportGenerator (dependencies, _identifierGenerator, _outputFormatter).GenerateXml ();
      var expectedOutput = new XElement (
          "AdditionalDependencies",
          new XElement (
              "AdditionalDependency",
              new XAttribute ("ref", "0"),
              new XAttribute ("instance-name", "AdditionalDependenciesTest+Mixin1")),
          new XElement (
              "AdditionalDependency",
              new XAttribute ("ref", "1"),
              new XAttribute ("instance-name", "AdditionalDependenciesTest+Mixin2"))
          );

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }
  }
}