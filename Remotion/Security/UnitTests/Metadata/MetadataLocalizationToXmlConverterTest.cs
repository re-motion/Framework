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
using System.Globalization;
using System.IO;
using System.Xml;
using NUnit.Framework;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.XmlAsserter;

namespace Remotion.Security.UnitTests.Metadata
{
  [TestFixture]
  public class MetadataLocalizationToXmlConverterTest
  {
    [Test]
    public void Convert_Empty ()
    {
      LocalizedName[] localizedNames = new LocalizedName[0];
      MetadataLocalizationToXmlConverter converter = new MetadataLocalizationToXmlConverter();

      XmlDocument document = converter.Convert(localizedNames, "de");

      string expectedXml = @"<?xml version=""1.0""?>
          <localizedNames xmlns=""http://www.re-motion.org/Security/Metadata/Localization/1.0"" culture=""de"" />
          ";

      XmlAssert.AreDocumentsEqual(expectedXml, document);
    }

    [Test]
    public void Convert_OneLocalizedName ()
    {
      LocalizedName[] localizedNames = new LocalizedName[1];
      localizedNames[0] = new LocalizedName("b8621bc9-9ab3-4524-b1e4-582657d6b420", "Clerk|Remotion.Security.UnitTests.TestDomain.DomainAbstractRoles, Remotion.Security.UnitTests.TestDomain", "Beamter");

      MetadataLocalizationToXmlConverter converter = new MetadataLocalizationToXmlConverter();

      XmlDocument document = converter.Convert(localizedNames, "de");

      string expectedXml = @"<?xml version=""1.0""?>
          <localizedNames xmlns=""http://www.re-motion.org/Security/Metadata/Localization/1.0"" culture=""de"">
            <localizedName ref=""b8621bc9-9ab3-4524-b1e4-582657d6b420"" comment=""Clerk|Remotion.Security.UnitTests.TestDomain.DomainAbstractRoles, Remotion.Security.UnitTests.TestDomain"">
    Beamter
  </localizedName>
          </localizedNames>
          ";

      XmlAssert.AreDocumentsEqual(expectedXml, document);
    }

    [Test]
    public void Convert_TwoLocalizedNames ()
    {
      LocalizedName[] localizedNames = new LocalizedName[2];
      localizedNames[0] = new LocalizedName("b8621bc9-9ab3-4524-b1e4-582657d6b420", "Clerk|Remotion.Security.UnitTests.TestDomain.DomainAbstractRoles, Remotion.Security.UnitTests.TestDomain", "Beamter");
      localizedNames[1] = new LocalizedName("93969f13-65d7-49f4-a456-a1686a4de3de", "Confidentiality", "Vertraulichkeit");

      MetadataLocalizationToXmlConverter converter = new MetadataLocalizationToXmlConverter();

      XmlDocument document = converter.Convert(localizedNames, "de");

      string expectedXml = @"<?xml version=""1.0""?>
          <localizedNames xmlns=""http://www.re-motion.org/Security/Metadata/Localization/1.0"" culture=""de"">
            <localizedName ref=""b8621bc9-9ab3-4524-b1e4-582657d6b420"" comment=""Clerk|Remotion.Security.UnitTests.TestDomain.DomainAbstractRoles, Remotion.Security.UnitTests.TestDomain"">
    Beamter
  </localizedName>
            <localizedName ref=""93969f13-65d7-49f4-a456-a1686a4de3de"" comment=""Confidentiality"">
    Vertraulichkeit
  </localizedName>
          </localizedNames>
          ";

      XmlAssert.AreDocumentsEqual(expectedXml, document);
    }

    [Test]
    public void Convert_OneLocalizedNameForInvariantCulture ()
    {
      LocalizedName[] localizedNames = new LocalizedName[1];
      localizedNames[0] = new LocalizedName("b8621bc9-9ab3-4524-b1e4-582657d6b420", "Clerk|Remotion.Security.UnitTests.TestDomain.DomainAbstractRoles, Remotion.Security.UnitTests.TestDomain", "Beamter");

      MetadataLocalizationToXmlConverter converter = new MetadataLocalizationToXmlConverter();

      XmlDocument document = converter.Convert(localizedNames, CultureInfo.InvariantCulture.Name);

      string expectedXml = @"<?xml version=""1.0""?>
          <localizedNames xmlns=""http://www.re-motion.org/Security/Metadata/Localization/1.0"" culture="""">
            <localizedName ref=""b8621bc9-9ab3-4524-b1e4-582657d6b420"" comment=""Clerk|Remotion.Security.UnitTests.TestDomain.DomainAbstractRoles, Remotion.Security.UnitTests.TestDomain"">
    Beamter
  </localizedName>
          </localizedNames>
          ";

      XmlAssert.AreDocumentsEqual(expectedXml, document);
    }

    [Test]
    public void ConvertAndSave_NewFile ()
    {
      string testOutputPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
      Directory.CreateDirectory(testOutputPath);

      string filename = Path.Combine(testOutputPath, "metadata.xml");
      string expectedFilename = Path.Combine(testOutputPath, "metadata.Localization.de.xml");

      if (File.Exists(expectedFilename))
        File.Delete(expectedFilename);

      MetadataLocalizationToXmlConverter converter = new MetadataLocalizationToXmlConverter();
      converter.ConvertAndSave(new LocalizedName[0], new CultureInfo("de"), filename);

      Assert.That(File.Exists(expectedFilename), Is.True);
      Directory.Delete(testOutputPath, true);
    }
  }
}
