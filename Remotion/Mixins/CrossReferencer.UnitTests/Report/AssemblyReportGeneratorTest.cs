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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using MixinXRef.Report;
using MixinXRef.UnitTests.Helpers;
using MixinXRef.UnitTests.TestDomain;
using MixinXRef.Utility;
using NUnit.Framework;
using Rhino.Mocks;

namespace MixinXRef.UnitTests.Report
{
  [TestFixture]
  public class AssemblyReportGeneratorTest
  {
    private Assembly _assembly1;
    private Assembly _assembly2;

    [SetUp]
    public void SetUp ()
    {
      _assembly1 = typeof (CompositeReportGeneratorTest).Assembly;
      _assembly2 = typeof (object).Assembly;
    }

    [Test]
    public void GenerateXml_EmptyAssemblies ()
    {
      var reportGenerator = ReportBuilder.CreateAssemblyReportGenerator ();
      var output = reportGenerator.GenerateXml ();

      var expectedOutput = new XElement ("Assemblies");

      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }

    [Test]
    public void GenerateXml_OneAssembly ()
    {
      var involvedType1 = new InvolvedType (typeof (TargetClass1));
      var involvedType2 = new InvolvedType (typeof (TargetClass2));
      var involvedType3 = new InvolvedType (typeof (Mixin1));
      var involvedType4 = new InvolvedType (typeof (Mixin2));

      var involvedTypes = new[] { involvedType1, involvedType2, involvedType3, involvedType4 };

      var reportGenerator = ReportBuilder.CreateAssemblyReportGenerator (involvedTypes);
      var output = reportGenerator.GenerateXml ();

      var expectedOutput = new XElement (
          "Assemblies",
          new XElement (
              "Assembly",
              new XAttribute ("id", "0"),
              new XAttribute ("name", _assembly1.GetName ().Name),
              new XAttribute ("version", _assembly1.GetName ().Version),
              new XAttribute ("location", "./" + Path.GetFileName (_assembly1.Location)),
              new XAttribute ("culture", _assembly1.GetName ().CultureInfo),
              new XAttribute ("publicKeyToken", Convert.ToBase64String (_assembly1.GetName ().GetPublicKeyToken ())),
              new XElement ("InvolvedType-Reference", new XAttribute ("ref", "0")),
              new XElement ("InvolvedType-Reference", new XAttribute ("ref", "1")),
              new XElement ("InvolvedType-Reference", new XAttribute ("ref", "2")),
              new XElement ("InvolvedType-Reference", new XAttribute ("ref", "3"))
              ));

      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }

    [Test]
    public void GenerateXml_MoreAssemblies ()
    {
      var typeStub1 = MockRepository.GenerateStub<Type> ();
      typeStub1.Stub (t => t.Assembly).Return (_assembly1);

      var typeStub2 = MockRepository.GenerateStub<Type> ();
      typeStub2.Stub (t => t.Assembly).Return (_assembly2);

      var assemblyIdentifierGeneratorStub =
        StubFactory.CreateIdentifierGeneratorStub (new[] { _assembly1, _assembly2 });

      var typeIdentifierGeneratorStub =
        StubFactory.CreateIdentifierGeneratorStub (new[] { typeStub1, typeStub2 });

      var involvedTypes = new[] { new InvolvedType (typeStub1), new InvolvedType (typeStub2) };

      var reportGenerator = new AssemblyReportGenerator (involvedTypes, assemblyIdentifierGeneratorStub, typeIdentifierGeneratorStub);
      var output = reportGenerator.GenerateXml ();

      var expectedOutput = new XElement (
          "Assemblies",
          new XElement (
              "Assembly",
              new XAttribute ("id", "0"),
              new XAttribute ("name", _assembly1.GetName ().Name),
              new XAttribute ("version", _assembly1.GetName ().Version),
              new XAttribute ("location", "./" + Path.GetFileName (_assembly1.Location)),
              new XAttribute ("culture", _assembly1.GetName ().CultureInfo),
              new XAttribute ("publicKeyToken", Convert.ToBase64String (_assembly1.GetName ().GetPublicKeyToken ())),
              new XElement ("InvolvedType-Reference",
                new XAttribute ("ref", typeIdentifierGeneratorStub.GetIdentifier (typeStub1)))),
          new XElement (
              "Assembly",
              new XAttribute ("id", "1"),
              new XAttribute ("name", _assembly2.GetName ().Name),
              new XAttribute ("version", _assembly2.GetName ().Version),
        // _assembly2 is of type object - which is a GAC (mscorlib.dll)
              new XAttribute ("location", _assembly2.Location),
              new XAttribute ("culture", _assembly2.GetName ().CultureInfo),
              new XAttribute ("publicKeyToken", Convert.ToBase64String (_assembly2.GetName ().GetPublicKeyToken ())),
              new XElement ("InvolvedType-Reference",
                new XAttribute ("ref", typeIdentifierGeneratorStub.GetIdentifier (typeStub2)))));

      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }

    [Test]
    public void GenerateXml_AdditionalAssemblies ()
    {
      var assemblyIdentifierGenerator = StubFactory.CreateIdentifierGeneratorStub (new[] { _assembly2 });

      var reportGenerator = ReportBuilder.CreateAssemblyReportGenerator (assemblyIdentifierGenerator);
      var output = reportGenerator.GenerateXml ();

      var expectedOutput = new XElement (
        "Assemblies",
        new XElement (
          "Assembly",
          new XAttribute ("id", "0"),
          new XAttribute ("name", _assembly2.GetName ().Name),
          new XAttribute ("version", _assembly2.GetName ().Version),
          new XAttribute ("location", _assembly2.Location),
          new XAttribute ("culture", _assembly2.GetName ().CultureInfo),
          new XAttribute ("publicKeyToken", Convert.ToBase64String (_assembly2.GetName ().GetPublicKeyToken ()))
          ));

      Assert.That (output.ToString (), Is.EqualTo (expectedOutput.ToString ()));
    }

    [Test]
    public void GetShortAssemblyLocation ()
    {
      var reportGenerator = ReportBuilder.CreateAssemblyReportGenerator ();
      // non-GAC assembly
      Assert.That (reportGenerator.GetShortAssemblyLocation (_assembly1), Is.EqualTo ("./" + Path.GetFileName (_assembly1.Location)));
      // GAC assembly
      Assert.That (reportGenerator.GetShortAssemblyLocation (_assembly2), Is.EqualTo (_assembly2.Location));
    }
  }
}