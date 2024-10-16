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
using System.Xml.Linq;
using NUnit.Framework;
using Remotion.Mixins.CrossReferencer.Report;
using Remotion.Mixins.CrossReferencer.UnitTests.TestDomain;
using Remotion.Mixins.CrossReferencer.Utilities;
using Remotion.Mixins.Definitions;

namespace Remotion.Mixins.CrossReferencer.UnitTests.Report
{
  [TestFixture]
  public class InterfaceIntroductionReportGeneratorTest
  {
    [Test]
    public void GenerateXm_NoIntroducedInterfaces ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<TargetClass2>().AddMixin<Mixin2>()
          .BuildConfiguration();

      var type1 = new InvolvedType(typeof(TargetClass2));
      type1.ClassContext = mixinConfiguration.ClassContexts.First();

      var interfaceIntroductions = GetInterfaceIntroductions(type1, typeof(Mixin2), mixinConfiguration);
      var reportGenerator = new InterfaceIntroductionReportGenerator(interfaceIntroductions, new IdentifierGenerator<Type>());

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement("InterfaceIntroductions");

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithIntroducedInterfaces ()
    {
      var interfaceIdentifierGenerator = new IdentifierGenerator<Type>();
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<TargetClass2>().AddMixin<Mixin3>()
          .BuildConfiguration();

      var type1 = new InvolvedType(typeof(TargetClass2));
      type1.ClassContext = mixinConfiguration.ClassContexts.First();

      // TargetClass2 does not implement any interface
      // Mixin3 introduces interface IDisposable
      var interfaceIntroductions = GetInterfaceIntroductions(type1, typeof(Mixin3), mixinConfiguration);
      var reportGenerator = new InterfaceIntroductionReportGenerator(interfaceIntroductions, interfaceIdentifierGenerator);

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement(
          "InterfaceIntroductions",
          new XElement(
              "IntroducedInterface",
              new XAttribute("ref", "0")
          ));

      Assert.That(output.ToString(), Is.EqualTo(expectedOutput.ToString()));
    }

    private UniqueDefinitionCollection<Type, InterfaceIntroductionDefinition> GetInterfaceIntroductions (InvolvedType targetType, Type mixinType, MixinConfiguration mixinConfiguration)
    {
      var targetClassDefinition = TargetClassDefinitionFactory.CreateWithoutValidation(mixinConfiguration.ClassContexts.GetExact(targetType.Type));
      return targetClassDefinition.GetMixinByConfiguredType(mixinType).InterfaceIntroductions;
    }
  }
}
