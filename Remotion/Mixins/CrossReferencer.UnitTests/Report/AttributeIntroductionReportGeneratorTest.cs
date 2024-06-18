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
using Remotion.Mixins.CrossReferencer.Reflectors;
using Remotion.Mixins.CrossReferencer.Report;
using Remotion.Mixins.CrossReferencer.UnitTests.TestDomain;
using Remotion.Mixins.CrossReferencer.Utilities;

namespace Remotion.Mixins.CrossReferencer.UnitTests.Report
{
  [TestFixture]
  public class AttributeIntroductionReportGeneratorTest
  {
    [Test]
    public void GenerateXml_NoIntroducedAttribute ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<TargetClass2>().AddMixin<Mixin1>()
          .BuildConfiguration();

      var type1 = new InvolvedType (typeof (TargetClass2));
      type1.ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First());

      var attributeIntroductions = GetAttributeIntroductions (type1, typeof (Mixin1), mixinConfiguration);
      var reportGenerator = new AttributeIntroductionReportGenerator (
          attributeIntroductions, new IdentifierGenerator<Type>(), Helpers.RemotionReflectorFactory.GetRemotionReflection ());
      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement ("AttributeIntroductions");

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    [Test]
    public void GenerateXml_WithIntroducedAttributes ()
    {
      var attributeIdentifierGenerator = new IdentifierGenerator<Type>();
      var mixinConfiguration = MixinConfiguration.BuildNew()
          .ForClass<UselessObject>().AddMixin<ObjectWithInheritableAttribute>()
          .BuildConfiguration();

      var type1 = new InvolvedType (typeof (UselessObject));
      type1.ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First());

      var attributeIntroductions = GetAttributeIntroductions (type1, typeof (ObjectWithInheritableAttribute), mixinConfiguration);
      var reportGenerator = new AttributeIntroductionReportGenerator (
          attributeIntroductions, attributeIdentifierGenerator, Helpers.RemotionReflectorFactory.GetRemotionReflection ());

      var output = reportGenerator.GenerateXml();

      var expectedOutput = new XElement (
          "AttributeIntroductions",
          new XElement (
              "IntroducedAttribute",
              new XAttribute ("ref", "0")
              ));

      Assert.That (output.ToString(), Is.EqualTo (expectedOutput.ToString()));
    }

    private ReflectedObject GetAttributeIntroductions (
        InvolvedType targetType, Type mixinType, MixinConfiguration mixinConfiguration)
    {
      var targetClassDefinition = TargetClassDefinitionUtility.GetConfiguration (targetType.Type, mixinConfiguration);
      return new ReflectedObject (targetClassDefinition.GetMixinByConfiguredType (mixinType).AttributeIntroductions);
    }
  }
}
