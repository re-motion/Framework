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
using MixinXRef.Formatting;
using MixinXRef.Reflection;
using MixinXRef.Reflection.RemotionReflector;
using MixinXRef.Reflection.Utility;
using MixinXRef.Report;
using MixinXRef.UnitTests.TestDomain;
using MixinXRef.Utility;
using NUnit.Framework;
using Remotion.Mixins;

namespace MixinXRef.UnitTests.Report
{
  [TestFixture]
  public class MixinReferenceReportGeneratorTest
  {
    private IRemotionReflector _remotionReflector;
    private IOutputFormatter _outputFormatter;

    [SetUp]
    public void SetUp ()
    {
      _remotionReflector = Helpers.RemotionReflectorFactory.GetRemotionReflection ();
      _outputFormatter = new OutputFormatter ();
    }

    [Test]
    public void GenerateXml_NoMixins ()
    {
      var involvedTypeDummy = new InvolvedType (typeof (object));

      var reportGenerator = new MixinReferenceReportGenerator (
          involvedTypeDummy, new IdentifierGenerator<Assembly> (),
          new IdentifierGenerator<Type> (),
          new IdentifierGenerator<Type> (),
          new IdentifierGenerator<Type> (),
          _remotionReflector,
          _outputFormatter
          );

      var output = reportGenerator.GenerateXml ();

      Assert.That (output, Is.Null);
    }

    [Test]
    public void GenerateXml_WithMixins ()
    {
      var targetType = new InvolvedType (typeof (TargetClass1));

      var mixinConfiguration = MixinConfiguration.BuildNew ().ForClass<TargetClass1> ().AddMixin<Mixin1> ().BuildConfiguration ();
      targetType.ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First ());
      targetType.TargetClassDefinition = new ReflectedObject (TargetClassDefinitionUtility.GetConfiguration (targetType.Type, mixinConfiguration));

      var interfaceIdentifierGenerator = new IdentifierGenerator<Type> ();
      var attributeIdentifierGenerator = new IdentifierGenerator<Type> ();
      var assemblyIdentifierGenerator = new IdentifierGenerator<Assembly> ();

      var reportGenerator = new MixinReferenceReportGenerator (
          targetType, assemblyIdentifierGenerator,
          new IdentifierGenerator<Type> (),
          interfaceIdentifierGenerator,
          attributeIdentifierGenerator,
          _remotionReflector,
          _outputFormatter
          );

      var output = reportGenerator.GenerateXml ();

      var targetClassDefinition = TargetClassDefinitionUtility.GetConfiguration (targetType.Type, mixinConfiguration);
      var mixinDefinition = targetClassDefinition.GetMixinByConfiguredType (typeof (Mixin1));

      var expectedOutput = new XElement (
          "Mixins",
          new XElement (
              "Mixin",
              new XAttribute ("ref", "0"),
              new XAttribute ("index", "0"),
              new XAttribute ("relation", "Extends"),
              new XAttribute ("instance-name", "Mixin1"),
              new XAttribute ("introduced-member-visibility", "private"),
        // has no dependencies
              new XElement ("AdditionalDependencies"),
              new InterfaceIntroductionReportGenerator (new ReflectedObject (mixinDefinition.InterfaceIntroductions), interfaceIdentifierGenerator).
                  GenerateXml (),
              new AttributeIntroductionReportGenerator (
                  new ReflectedObject (mixinDefinition.AttributeIntroductions), attributeIdentifierGenerator, Helpers.RemotionReflectorFactory.GetRemotionReflection ()).
                  GenerateXml (),
              new MemberOverrideReportGenerator (new ReflectedObject (mixinDefinition.GetAllOverrides ())).GenerateXml (),
              new TargetCallDependenciesReportGenerator (new ReflectedObject (mixinDefinition), assemblyIdentifierGenerator, _remotionReflector, _outputFormatter).GenerateXml ()
              ));

      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }

    [Test]
    public void GenerateXml_ForGenericTypeDefinition ()
    {
      var targetType = new InvolvedType (typeof (GenericTarget<,>));

      var mixinConfiguration = MixinConfiguration.BuildNew ()
          .ForClass (typeof (GenericTarget<,>)).AddMixin<ClassWithBookAttribute> ().AddMixin<Mixin3> ()
          .BuildConfiguration ();
      targetType.ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First ());

      var interfaceIdentifierGenerator = new IdentifierGenerator<Type> ();
      var attributeIdentifierGenerator = new IdentifierGenerator<Type> ();
      var assemblyIdentifierGenerator = new IdentifierGenerator<Assembly> ();

      var reportGenerator = new MixinReferenceReportGenerator (
          targetType, assemblyIdentifierGenerator,
        // generic target class
          new IdentifierGenerator<Type> (),
          interfaceIdentifierGenerator,
          attributeIdentifierGenerator,
          _remotionReflector,
          _outputFormatter);

      var output = reportGenerator.GenerateXml ();
      var expectedOutput = new XElement (
          "Mixins",
          new XElement (
              "Mixin",
              new XAttribute ("ref", "0"),
              new XAttribute ("index", "n/a"),
              new XAttribute ("relation", "Extends"),
              new XAttribute ("instance-name", "ClassWithBookAttribute"),
              new XAttribute ("introduced-member-visibility", "private"),
        // has no dependencies
              new XElement ("AdditionalDependencies")
              ),
          new XElement (
              "Mixin",
              new XAttribute ("ref", "1"),
              new XAttribute ("index", "n/a"),
              new XAttribute ("relation", "Extends"),
              new XAttribute ("instance-name", "Mixin3"),
              new XAttribute ("introduced-member-visibility", "private"),
        // has no dependencies
              new XElement ("AdditionalDependencies")
              )
          );

      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }
  }
}