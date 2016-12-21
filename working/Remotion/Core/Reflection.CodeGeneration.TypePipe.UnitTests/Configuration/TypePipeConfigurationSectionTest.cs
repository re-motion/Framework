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
using System.Configuration;
using System.IO;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Configuration;
using Remotion.Reflection.CodeGeneration.TypePipe.Configuration;

namespace Remotion.Reflection.CodeGeneration.TypePipe.UnitTests.Configuration
{
  [TestFixture]
  public class TypePipeConfigurationSectionTest
  {
    private TypePipeConfigurationSection _section;

    [SetUp]
    public void SetUp ()
    {
      _section = new TypePipeConfigurationSection();
    }

    [Test]
    public void ExampleConfiguration ()
    {
      DeserializeSection (TypePipeConfigurationSection.ExampleConfiguration);

      Assert.That (_section.ForceStrongNaming.ElementInformation.IsPresent, Is.True);
      Assert.That (_section.ForceStrongNaming.KeyFilePath, Is.EqualTo ("keyFile.snk"));
      Assert.That(_section.EnableSerializationWithoutAssemblySaving.ElementInformation.IsPresent, Is.True);
    }

    [Test]
    public void Empty ()
    {
      var xmlFragment = @"<remotion.reflection.codeGeneration.typePipe {xmlns} />";
      DeserializeSection (xmlFragment);
      
      Assert.That (_section.ForceStrongNaming.ElementInformation.IsPresent, Is.False);
      Assert.That (_section.EnableSerializationWithoutAssemblySaving.ElementInformation.IsPresent, Is.False);
    }

    [Test]
    public void ForceStrongNaming ()
    {
      var xmlFragment = @"<remotion.reflection.codeGeneration.typePipe {xmlns}><forceStrongNaming/></remotion.reflection.codeGeneration.typePipe>";
      DeserializeSection (xmlFragment);

      Assert.That (_section.ForceStrongNaming.ElementInformation.IsPresent, Is.True);
    }

    [Test]
    public void EnableSerializationWithoutAssemblySaving ()
    {
      var xmlFragment = @"<remotion.reflection.codeGeneration.typePipe {xmlns}><enableSerializationWithoutAssemblySaving/></remotion.reflection.codeGeneration.typePipe>";
      DeserializeSection(xmlFragment);

      Assert.That(_section.EnableSerializationWithoutAssemblySaving.ElementInformation.IsPresent, Is.True);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException), ExpectedMessage = "Example configuration:", MatchType = MessageMatch.Contains)]
    public void InvalidSection ()
    {
      var xmlFragment = "<remotion.reflection.codeGeneration.typePipe {xmlns}><invalid /></remotion.reflection.codeGeneration.typePipe>";
      DeserializeSection (xmlFragment);
    }

    private void DeserializeSection (string xmlFragment)
    {
      xmlFragment = xmlFragment.Replace ("{xmlns}", "xmlns=\"" + _section.XmlNamespace + "\"");
      var xsdContent = GetSchemaContent();

      ConfigurationHelper.DeserializeSection (_section, xmlFragment, xsdContent);
    }

    private string GetSchemaContent ()
    {
      var assembly = typeof (TypePipeConfigurationSection).Assembly;
      using (var resourceStream = assembly.GetManifestResourceStream ("Remotion.Reflection.CodeGeneration.TypePipe.Schemas.TypePipeConfigurationSchema.xsd"))
      using (var reader = new StreamReader (resourceStream))
        return reader.ReadToEnd();
    }
  }
}