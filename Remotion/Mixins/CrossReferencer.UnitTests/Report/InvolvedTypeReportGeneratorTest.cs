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
using MixinXRef.Report;
using MixinXRef.UnitTests.Helpers;
using MixinXRef.UnitTests.TestDomain;
using MixinXRef.Utility;
using NUnit.Framework;
using Remotion.Mixins;
using Remotion.Mixins.Context;

namespace MixinXRef.UnitTests.Report
{
  [TestFixture]
  public class InvolvedTypeReportGeneratorTest
  {
    private IRemotionReflector _remotionReflector;
    private IOutputFormatter _outputFormatter;

    private IIdentifierGenerator<Type> _involvedTypeIdentifierGenerator;
    private IIdentifierGenerator<Type> _readOnlyInvolvedTypeIdentifierGenerator;
    private IIdentifierGenerator<Type> _interfaceIdentifierGenerator;
    private IIdentifierGenerator<Type> _attributeIdentifierGenerator;
    private IIdentifierGenerator<Assembly> _assemblyIdentifierGenerator;
    private IIdentifierGenerator<MemberInfo> _memberIdentifierGenerator;

    private readonly SummaryPicker _summaryPicker = new SummaryPicker ();
    private readonly TypeModifierUtility _typeModifierUtility = new TypeModifierUtility ();

    [SetUp]
    public void SetUp ()
    {
      _remotionReflector = Helpers.RemotionReflectorFactory.GetRemotionReflection ();
      _outputFormatter = new OutputFormatter ();

      _involvedTypeIdentifierGenerator = new IdentifierGenerator<Type> ();
      _interfaceIdentifierGenerator = new IdentifierGenerator<Type> ();
      _attributeIdentifierGenerator = new IdentifierGenerator<Type> ();
      _assemblyIdentifierGenerator = new IdentifierGenerator<Assembly> ();
      _memberIdentifierGenerator = new IdentifierGenerator<MemberInfo> ();

      _readOnlyInvolvedTypeIdentifierGenerator = new ReadonlyIdentifierGenerator<Type> (_involvedTypeIdentifierGenerator, "none");
    }

    [Test]
    public void GenerateXml_NoInvolvedTypes ()
    {
      var reportGenerator = ReportBuilder.CreateInvolvedTypeReportGenerator (_remotionReflector, _outputFormatter);
      var output = reportGenerator.GenerateXml ();

      var expectedOutput = new XElement ("InvolvedTypes");

      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }

    [Test]
    public void GenerateXml_ForGenericTypeDefinition ()
    {

      var involvedType1 = new InvolvedType (typeof (GenericTarget<,>));

      var reportGenerator = CreateInvolvedTypeReportGenerator (involvedType1);
      var output = reportGenerator.GenerateXml ();

      var expectedOutput = new XElement (
          "InvolvedTypes",
          new XElement (
              "InvolvedType",
              new XAttribute ("id", "0"),
              new XAttribute ("assembly-ref", "0"),
              new XAttribute ("namespace", "MixinXRef.UnitTests.TestDomain"),
              new XAttribute ("name", "GenericTarget<TParameter1, TParameter2>"),
              new XAttribute ("base", "Object"),
              new XAttribute ("base-ref", "none"),
              new XAttribute ("is-target", false),
              new XAttribute ("is-mixin", false),
              new XAttribute ("is-unusedmixin", false),
              new XAttribute ("is-generic-definition", true),
              new XAttribute ("is-interface", false),
              _outputFormatter.CreateModifierMarkup ("", _typeModifierUtility.GetTypeModifiers (involvedType1.Type)),
              _summaryPicker.GetSummary (involvedType1.Type),
              new MemberReportGenerator (involvedType1.Type, null, null, _memberIdentifierGenerator, _outputFormatter).GenerateXml (),
              new InterfaceReferenceReportGenerator (involvedType1, _interfaceIdentifierGenerator, _remotionReflector).GenerateXml (),
              new AttributeReferenceReportGenerator (involvedType1.Type, _attributeIdentifierGenerator, _remotionReflector).GenerateXml (),
              new MixinReferenceReportGenerator (
                  involvedType1, _assemblyIdentifierGenerator,
                  _readOnlyInvolvedTypeIdentifierGenerator,
                  _interfaceIdentifierGenerator,
                  _attributeIdentifierGenerator,
                  _remotionReflector,
                  _outputFormatter).
                  GenerateXml (),
              new TargetReferenceReportGenerator (involvedType1, _readOnlyInvolvedTypeIdentifierGenerator).GenerateXml ()
              ));

      XElementComparisonHelper.Compare (output, expectedOutput);
    }

    [Test]
    public void GenerateXml_ForInterface ()
    {

      var involvedType1 = new InvolvedType (typeof (IUseless));

      var reportGenerator = CreateInvolvedTypeReportGenerator (involvedType1);
      var output = reportGenerator.GenerateXml ();

      var expectedOutput = new XElement (
          "InvolvedTypes",
          new XElement (
              "InvolvedType",
              new XAttribute ("id", "0"),
              new XAttribute ("assembly-ref", "0"),
              new XAttribute ("namespace", "MixinXRef.UnitTests.TestDomain"),
              new XAttribute ("name", "IUseless"),
              new XAttribute ("base", "none"),
              new XAttribute ("base-ref", "none"),
              new XAttribute ("is-target", false),
              new XAttribute ("is-mixin", false),
              new XAttribute ("is-unusedmixin", false),
              new XAttribute ("is-generic-definition", false),
              new XAttribute ("is-interface", true),
              _outputFormatter.CreateModifierMarkup ("", _typeModifierUtility.GetTypeModifiers (involvedType1.Type)),
              _summaryPicker.GetSummary (involvedType1.Type),
              new MemberReportGenerator (involvedType1.Type, null, null, _memberIdentifierGenerator, _outputFormatter).
                  GenerateXml (),
              new InterfaceReferenceReportGenerator (involvedType1, _interfaceIdentifierGenerator, _remotionReflector).GenerateXml (),
              new AttributeReferenceReportGenerator (involvedType1.Type, _attributeIdentifierGenerator, _remotionReflector).GenerateXml (),
              new MixinReferenceReportGenerator (
                  involvedType1, _assemblyIdentifierGenerator,
                  _readOnlyInvolvedTypeIdentifierGenerator,
                  _interfaceIdentifierGenerator,
                  _attributeIdentifierGenerator,
                  _remotionReflector,
                  _outputFormatter).
                  GenerateXml (),
              new TargetReferenceReportGenerator (involvedType1, _readOnlyInvolvedTypeIdentifierGenerator).GenerateXml ()
              ));

      XElementComparisonHelper.Compare (output, expectedOutput);
    }

    [Test]
    public void GenerateXml_InvolvedTypes ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew ()
          .ForClass<TargetClass1> ().AddMixin<Mixin1> ()
          .ForClass<TargetClass2> ().AddMixin<Mixin2> ()
          .BuildConfiguration ();

      var involvedType1 = new InvolvedType (typeof (TargetClass1));
      involvedType1.ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.First ());
      SetTargetClassDefinition (involvedType1, mixinConfiguration);

      var involvedType2 = new InvolvedType (typeof (TargetClass2));
      involvedType2.ClassContext = new ReflectedObject (mixinConfiguration.ClassContexts.Last ());
      SetTargetClassDefinition (involvedType2, mixinConfiguration);

      var involvedType3 = new InvolvedType (typeof (Mixin1));
      involvedType3.TargetTypes.Add (new InvolvedType (typeof (TargetClass1)), null);
      var involvedType4 = new InvolvedType (typeof (Mixin2));
      involvedType4.TargetTypes.Add (new InvolvedType (typeof (TargetClass2)), null);

      var reportGenerator = CreateInvolvedTypeReportGenerator (involvedType1, involvedType2, involvedType3, involvedType4);
      var output = reportGenerator.GenerateXml ();

      var expectedOutput = new XElement (
          "InvolvedTypes",
          new XElement (
              "InvolvedType",
              new XAttribute ("id", "0"),
              new XAttribute ("assembly-ref", "0"),
              new XAttribute ("namespace", "MixinXRef.UnitTests.TestDomain"),
              new XAttribute ("name", "TargetClass1"),
              new XAttribute ("base", "Object"),
              new XAttribute ("base-ref", "none"),
              new XAttribute ("is-target", true),
              new XAttribute ("is-mixin", false),
              new XAttribute ("is-unusedmixin", false),
              new XAttribute ("is-generic-definition", false),
              new XAttribute ("is-interface", false),
              _outputFormatter.CreateModifierMarkup ("", _typeModifierUtility.GetTypeModifiers (involvedType1.Type)),
              _summaryPicker.GetSummary (involvedType1.Type),
              new MemberReportGenerator (involvedType1.Type, involvedType1, _involvedTypeIdentifierGenerator, _memberIdentifierGenerator, _outputFormatter).
                  GenerateXml (),
              new InterfaceReferenceReportGenerator (involvedType1, _interfaceIdentifierGenerator, _remotionReflector).GenerateXml (),
              new AttributeReferenceReportGenerator (involvedType1.Type, _attributeIdentifierGenerator, _remotionReflector).GenerateXml (),
              new MixinReferenceReportGenerator (
                  involvedType1, _assemblyIdentifierGenerator,
                  _readOnlyInvolvedTypeIdentifierGenerator,
                  _interfaceIdentifierGenerator,
                  _attributeIdentifierGenerator,
                  _remotionReflector,
                  _outputFormatter).
                  GenerateXml (),
              new TargetReferenceReportGenerator (involvedType1, _readOnlyInvolvedTypeIdentifierGenerator).GenerateXml ()
              ),
          new XElement (
              "InvolvedType",
              new XAttribute ("id", "1"),
              new XAttribute ("assembly-ref", "0"),
              new XAttribute ("namespace", "MixinXRef.UnitTests.TestDomain"),
              new XAttribute ("name", "TargetClass2"),
              new XAttribute ("base", "Object"),
              new XAttribute ("base-ref", "none"),
              new XAttribute ("is-target", true),
              new XAttribute ("is-mixin", false),
              new XAttribute ("is-unusedmixin", false),
              new XAttribute ("is-generic-definition", false),
              new XAttribute ("is-interface", false),
              _outputFormatter.CreateModifierMarkup ("", _typeModifierUtility.GetTypeModifiers (involvedType2.Type)),
              _summaryPicker.GetSummary (involvedType2.Type),
              new MemberReportGenerator (involvedType2.Type, involvedType2, _involvedTypeIdentifierGenerator, _memberIdentifierGenerator, _outputFormatter).
                  GenerateXml (),
              new InterfaceReferenceReportGenerator (involvedType2, _interfaceIdentifierGenerator, _remotionReflector).GenerateXml (),
              new AttributeReferenceReportGenerator (involvedType2.Type, _attributeIdentifierGenerator, _remotionReflector).GenerateXml (),
              new MixinReferenceReportGenerator (
                  involvedType2, _assemblyIdentifierGenerator,
                  _readOnlyInvolvedTypeIdentifierGenerator,
                  _interfaceIdentifierGenerator,
                  _attributeIdentifierGenerator,
                  _remotionReflector,
                  _outputFormatter).
                  GenerateXml (),
              new TargetReferenceReportGenerator (involvedType2, _readOnlyInvolvedTypeIdentifierGenerator).GenerateXml ()
              ),
          new XElement (
              "InvolvedType",
              new XAttribute ("id", "2"),
              new XAttribute ("assembly-ref", "0"),
              new XAttribute ("namespace", "MixinXRef.UnitTests.TestDomain"),
              new XAttribute ("name", "Mixin1"),
              new XAttribute ("base", "Object"),
              new XAttribute ("base-ref", "none"),
              new XAttribute ("is-target", false),
              new XAttribute ("is-mixin", true),
              new XAttribute ("is-unusedmixin", false),
              new XAttribute ("is-generic-definition", false),
              new XAttribute ("is-interface", false),
              _outputFormatter.CreateModifierMarkup ("", _typeModifierUtility.GetTypeModifiers (involvedType3.Type)),
              _summaryPicker.GetSummary (involvedType3.Type),
              new MemberReportGenerator (involvedType3.Type, involvedType3, _involvedTypeIdentifierGenerator, _memberIdentifierGenerator, _outputFormatter).
                  GenerateXml (),
              new InterfaceReferenceReportGenerator (involvedType3, _interfaceIdentifierGenerator, _remotionReflector).GenerateXml (),
              new AttributeReferenceReportGenerator (involvedType3.Type, _attributeIdentifierGenerator, _remotionReflector).GenerateXml (),
              new MixinReferenceReportGenerator (
                  involvedType3, _assemblyIdentifierGenerator,
                  _readOnlyInvolvedTypeIdentifierGenerator,
                  _interfaceIdentifierGenerator,
                  _attributeIdentifierGenerator,
                  _remotionReflector,
                  _outputFormatter).
                  GenerateXml (),
              new TargetReferenceReportGenerator (involvedType3, _readOnlyInvolvedTypeIdentifierGenerator).GenerateXml ()
              ),
          new XElement (
              "InvolvedType",
              new XAttribute ("id", "3"),
              new XAttribute ("assembly-ref", "0"),
              new XAttribute ("namespace", "MixinXRef.UnitTests.TestDomain"),
              new XAttribute ("name", "Mixin2"),
              new XAttribute ("base", "Object"),
              new XAttribute ("base-ref", "none"),
              new XAttribute ("is-target", false),
              new XAttribute ("is-mixin", true),
              new XAttribute ("is-unusedmixin", false),
              new XAttribute ("is-generic-definition", false),
              new XAttribute ("is-interface", false),
              _outputFormatter.CreateModifierMarkup ("", _typeModifierUtility.GetTypeModifiers (involvedType4.Type)),
              _summaryPicker.GetSummary (involvedType4.Type),
              new MemberReportGenerator (involvedType4.Type, involvedType4, _involvedTypeIdentifierGenerator, _memberIdentifierGenerator, _outputFormatter).
                  GenerateXml (),
              new InterfaceReferenceReportGenerator (involvedType4, _interfaceIdentifierGenerator, _remotionReflector).GenerateXml (),
              new AttributeReferenceReportGenerator (involvedType4.Type, _attributeIdentifierGenerator, _remotionReflector).GenerateXml (),
              new MixinReferenceReportGenerator (
                  involvedType4, _assemblyIdentifierGenerator,
                  _readOnlyInvolvedTypeIdentifierGenerator,
                  _interfaceIdentifierGenerator,
                  _attributeIdentifierGenerator,
                  _remotionReflector,
                  _outputFormatter).
                  GenerateXml (),
              new TargetReferenceReportGenerator (involvedType4, _readOnlyInvolvedTypeIdentifierGenerator).GenerateXml ()
              )
          );

      XElementComparisonHelper.Compare (output, expectedOutput);
    }

    [Test]
    public void GenerateXml_DifferentAssemblies ()
    {
      var involvedType1 = new InvolvedType (typeof (TargetClass1)) { ClassContext = new ReflectedObject (new ClassContext (typeof (TargetClass1))) };
      var involvedType2 = new InvolvedType (typeof (object));

      var reportGenerator = CreateInvolvedTypeReportGenerator (involvedType1, involvedType2);
      var output = reportGenerator.GenerateXml ();

      var expectedOutput = new XElement (
          "InvolvedTypes",
          new XElement (
              "InvolvedType",
              new XAttribute ("id", "0"),
              new XAttribute ("assembly-ref", "0"),
              new XAttribute ("namespace", "MixinXRef.UnitTests.TestDomain"),
              new XAttribute ("name", "TargetClass1"),
              new XAttribute ("base", "Object"),
              new XAttribute ("base-ref", "1"),
              new XAttribute ("is-target", true),
              new XAttribute ("is-mixin", false),
              new XAttribute ("is-unusedmixin", false),
              new XAttribute ("is-generic-definition", false),
              new XAttribute ("is-interface", false),
              _outputFormatter.CreateModifierMarkup ("", _typeModifierUtility.GetTypeModifiers (involvedType1.Type)),
              _summaryPicker.GetSummary (involvedType1.Type),
              new MemberReportGenerator (involvedType1.Type, involvedType1, _involvedTypeIdentifierGenerator, _memberIdentifierGenerator, _outputFormatter).
                  GenerateXml (),
              new InterfaceReferenceReportGenerator (involvedType1, _interfaceIdentifierGenerator, _remotionReflector).GenerateXml (),
              new AttributeReferenceReportGenerator (involvedType1.Type, _attributeIdentifierGenerator, _remotionReflector).GenerateXml (),
              new MixinReferenceReportGenerator (
                  involvedType1, _assemblyIdentifierGenerator,
                  _readOnlyInvolvedTypeIdentifierGenerator,
                  _interfaceIdentifierGenerator,
                  _attributeIdentifierGenerator,
                  _remotionReflector,
                  _outputFormatter).
                  GenerateXml (),
              new TargetReferenceReportGenerator (involvedType1, _readOnlyInvolvedTypeIdentifierGenerator).GenerateXml ()
              ),
          new XElement (
              "InvolvedType",
              new XAttribute ("id", "1"),
              new XAttribute ("assembly-ref", "1"),
              new XAttribute ("namespace", "System"),
              new XAttribute ("name", "Object"),
              new XAttribute ("base", "none"),
              new XAttribute ("base-ref", "none"),
              new XAttribute ("is-target", false),
              new XAttribute ("is-mixin", false),
              new XAttribute ("is-unusedmixin", false),
              new XAttribute ("is-generic-definition", false),
              new XAttribute ("is-interface", false),
              _outputFormatter.CreateModifierMarkup ("", _typeModifierUtility.GetTypeModifiers (involvedType2.Type)),
              _summaryPicker.GetSummary (involvedType2.Type),
              new MemberReportGenerator (involvedType2.Type, involvedType2, _involvedTypeIdentifierGenerator, _memberIdentifierGenerator, _outputFormatter).
                  GenerateXml (),
              new InterfaceReferenceReportGenerator (involvedType2, _interfaceIdentifierGenerator, _remotionReflector).GenerateXml (),
              new AttributeReferenceReportGenerator (involvedType2.Type, _attributeIdentifierGenerator, _remotionReflector).GenerateXml (),
              new MixinReferenceReportGenerator (
                  involvedType2, _assemblyIdentifierGenerator,
                  _readOnlyInvolvedTypeIdentifierGenerator,
                  _interfaceIdentifierGenerator,
                  _attributeIdentifierGenerator,
                  _remotionReflector,
                  _outputFormatter).
                  GenerateXml (),
              new TargetReferenceReportGenerator (involvedType2, _readOnlyInvolvedTypeIdentifierGenerator).GenerateXml ()
              ));

      XElementComparisonHelper.Compare (output, expectedOutput);
    }

    [Test]
    public void HasAlphabeticOrderingAttribute_False ()
    {
      var involvedType = new InvolvedType (typeof (object));

      var reportGenerator = ReportBuilder.CreateInvolvedTypeReportGenerator (_remotionReflector, _outputFormatter);

      var output = reportGenerator.GetAlphabeticOrderingAttribute (involvedType);

      Assert.That (output, Is.EqualTo (""));
    }

    [Test]
    public void HasAlphabeticOrderingAttribute_True ()
    {
      var mixinConfiguration =
          MixinConfiguration.BuildNew ().ForClass<UselessObject> ().AddMixin<ClassWithAlphabeticOrderingAttribute> ().BuildConfiguration ();
      var involvedType = new InvolvedType (typeof (ClassWithAlphabeticOrderingAttribute));
      involvedType.TargetTypes.Add (
          new InvolvedType (typeof (UselessObject)),
          new ReflectedObject (TargetClassDefinitionUtility.GetConfiguration (typeof (UselessObject), mixinConfiguration).Mixins[0]));

      var reportGenerator = ReportBuilder.CreateInvolvedTypeReportGenerator (_remotionReflector, _outputFormatter);

      var output = reportGenerator.GetAlphabeticOrderingAttribute (involvedType);

      Assert.That (output, Is.EqualTo ("AcceptsAlphabeticOrdering "));
    }

    private void SetTargetClassDefinition (InvolvedType involvedType, MixinConfiguration mixinConfiguration)
    {
      involvedType.TargetClassDefinition = new ReflectedObject (TargetClassDefinitionUtility.GetConfiguration (involvedType.Type, mixinConfiguration));
    }

    private InvolvedTypeReportGenerator CreateInvolvedTypeReportGenerator (params InvolvedType[] involvedTypes)
    {
      _readOnlyInvolvedTypeIdentifierGenerator = new IdentifierPopulator<Type> (involvedTypes.Select (i => i.Type)).GetReadonlyIdentifierGenerator ("none");
      return new InvolvedTypeReportGenerator (involvedTypes,
                                              _assemblyIdentifierGenerator,
                                              _readOnlyInvolvedTypeIdentifierGenerator,
                                              _memberIdentifierGenerator,
                                              _interfaceIdentifierGenerator,
                                              _attributeIdentifierGenerator,
                                              _remotionReflector,
                                              _outputFormatter);
    }
  }
}