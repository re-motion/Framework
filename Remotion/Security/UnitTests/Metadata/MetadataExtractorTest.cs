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
using System.IO;
using System.Reflection;
using System.Xml;
using NUnit.Framework;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.XmlAsserter;

namespace Remotion.Security.UnitTests.Metadata
{
  [TestFixture]
  public class MetadataExtractorTest
  {
    private string _xmlTempFilename;
    private MetadataExtractor _extractor;

    [SetUp]
    public void SetUp ()
    {
      _xmlTempFilename = Path.GetTempFileName ();
      _extractor = new MetadataExtractor (new MetadataToXmlConverter ());
    }

    [TearDown]
    public void TearDown ()
    {
      File.Delete (_xmlTempFilename);
    }

    [Test]
    public void NoAssemblies ()
    {
      _extractor.Save (_xmlTempFilename);

      XmlDocument xmlDocument = new XmlDocument ();
      xmlDocument.Load (_xmlTempFilename);

      string expectedXml = @"<?xml version=""1.0""?>
          <securityMetadata xmlns=""http://www.re-motion.org/Security/Metadata/1.0"" />";

      XmlAssert.AreDocumentsEqual (expectedXml, xmlDocument);
    }

    [Test]
    public void OneAssembly ()
    {
      Assembly testDomainAssembly = typeof (Remotion.Security.UnitTests.TestDomain.File).Assembly;
      _extractor.AddAssembly (testDomainAssembly);

      _extractor.Save (_xmlTempFilename);

      XmlDocument xmlDocument = new XmlDocument ();
      xmlDocument.Load (_xmlTempFilename);

      string expectedXml = @"<?xml version=""1.0""?>
          <securityMetadata xmlns=""http://www.re-motion.org/Security/Metadata/1.0"">
            <classes>
              <class id=""00000000-0000-0000-0001-000000000000"" name=""Remotion.Security.UnitTests.TestDomain.File, Remotion.Security.UnitTests.TestDomain"">
                <stateProperties>
                  <statePropertyRef>00000000-0000-0000-0001-000000000001</statePropertyRef>
                </stateProperties>

                <accessTypes>
                  <accessTypeRef>1d6d25bc-4e85-43ab-a42d-fb5a829c30d5</accessTypeRef>
                  <accessTypeRef>62dfcd92-a480-4d57-95f1-28c0f5996b3a</accessTypeRef>
                  <accessTypeRef>11186122-6de0-4194-b434-9979230c41fd</accessTypeRef>
                  <accessTypeRef>305fbb40-75c8-423a-84b2-b26ea9e7cae7</accessTypeRef>
                  <accessTypeRef>67cea479-0be7-4e2f-b2e0-bb1fcc9ea1d6</accessTypeRef>
                  <accessTypeRef>203b7478-96f1-4bf1-b4ea-5bdd1206252c</accessTypeRef>
                  <accessTypeRef>00000002-0001-0000-0000-000000000000</accessTypeRef>
                </accessTypes>
              </class>

              <class id=""00000000-0000-0000-0002-000000000000"" name=""Remotion.Security.UnitTests.TestDomain.PaperFile, Remotion.Security.UnitTests.TestDomain"" base=""00000000-0000-0000-0001-000000000000"">
                <stateProperties>
                  <statePropertyRef>00000000-0000-0000-0001-000000000001</statePropertyRef>
                  <statePropertyRef>00000000-0000-0000-0002-000000000001</statePropertyRef>
                </stateProperties>

                <accessTypes>
                  <accessTypeRef>1d6d25bc-4e85-43ab-a42d-fb5a829c30d5</accessTypeRef>
                  <accessTypeRef>62dfcd92-a480-4d57-95f1-28c0f5996b3a</accessTypeRef>
                  <accessTypeRef>11186122-6de0-4194-b434-9979230c41fd</accessTypeRef>
                  <accessTypeRef>305fbb40-75c8-423a-84b2-b26ea9e7cae7</accessTypeRef>
                  <accessTypeRef>67cea479-0be7-4e2f-b2e0-bb1fcc9ea1d6</accessTypeRef>
                  <accessTypeRef>203b7478-96f1-4bf1-b4ea-5bdd1206252c</accessTypeRef>
                  <accessTypeRef>00000002-0001-0000-0000-000000000000</accessTypeRef>
                </accessTypes>
              </class>
            </classes>

            <stateProperties>
              <stateProperty id=""00000000-0000-0000-0002-000000000001"" name=""State"">
                <state name=""New|Remotion.Security.UnitTests.TestDomain.FileState, Remotion.Security.UnitTests.TestDomain"" value=""0"" />
                <state name=""Normal|Remotion.Security.UnitTests.TestDomain.FileState, Remotion.Security.UnitTests.TestDomain"" value=""1"" />
                <state name=""Archived|Remotion.Security.UnitTests.TestDomain.FileState, Remotion.Security.UnitTests.TestDomain"" value=""2"" />
              </stateProperty>

              <stateProperty id=""00000000-0000-0000-0001-000000000001"" name=""Confidentiality"">
                <state name=""Normal|Remotion.Security.UnitTests.TestDomain.Confidentiality, Remotion.Security.UnitTests.TestDomain"" value=""0"" />
                <state name=""Confidential|Remotion.Security.UnitTests.TestDomain.Confidentiality, Remotion.Security.UnitTests.TestDomain"" value=""1"" />
                <state name=""Private|Remotion.Security.UnitTests.TestDomain.Confidentiality, Remotion.Security.UnitTests.TestDomain"" value=""2"" />
              </stateProperty>
            </stateProperties>

            <accessTypes>
              <accessType id=""1d6d25bc-4e85-43ab-a42d-fb5a829c30d5"" name=""Create|Remotion.Security.GeneralAccessTypes, Remotion.Security"" value=""0"" />
              <accessType id=""62dfcd92-a480-4d57-95f1-28c0f5996b3a"" name=""Read|Remotion.Security.GeneralAccessTypes, Remotion.Security"" value=""1"" />
              <accessType id=""11186122-6de0-4194-b434-9979230c41fd"" name=""Edit|Remotion.Security.GeneralAccessTypes, Remotion.Security"" value=""2"" />
              <accessType id=""305fbb40-75c8-423a-84b2-b26ea9e7cae7"" name=""Delete|Remotion.Security.GeneralAccessTypes, Remotion.Security"" value=""3"" />
              <accessType id=""67cea479-0be7-4e2f-b2e0-bb1fcc9ea1d6"" name=""Search|Remotion.Security.GeneralAccessTypes, Remotion.Security"" value=""4"" />
              <accessType id=""203b7478-96f1-4bf1-b4ea-5bdd1206252c"" name=""Find|Remotion.Security.GeneralAccessTypes, Remotion.Security"" value=""5"" />
              <accessType id=""00000002-0001-0000-0000-000000000000"" name=""Journalize|Remotion.Security.UnitTests.TestDomain.DomainAccessTypes, Remotion.Security.UnitTests.TestDomain"" value=""0"" />
              <accessType id=""00000002-0002-0000-0000-000000000000"" name=""Archive|Remotion.Security.UnitTests.TestDomain.DomainAccessTypes, Remotion.Security.UnitTests.TestDomain"" value=""1"" />
            </accessTypes>

            <abstractRoles>
              <abstractRole id=""00000003-0001-0000-0000-000000000000"" name=""Clerk|Remotion.Security.UnitTests.TestDomain.DomainAbstractRoles, Remotion.Security.UnitTests.TestDomain"" value=""0"" />
              <abstractRole id=""00000003-0002-0000-0000-000000000000"" name=""Secretary|Remotion.Security.UnitTests.TestDomain.DomainAbstractRoles, Remotion.Security.UnitTests.TestDomain"" value=""1"" />
              <abstractRole id=""00000004-0001-0000-0000-000000000000"" name=""Administrator|Remotion.Security.UnitTests.TestDomain.SpecialAbstractRoles, Remotion.Security.UnitTests.TestDomain"" value=""0"" />
            </abstractRoles>
          </securityMetadata>";

      XmlAssert.AreDocumentsSimilar (expectedXml, xmlDocument);
    }

    [Test]
    public void OneAssemblyByPathName ()
    {
      _extractor.AddAssembly ("Remotion.Security.UnitTests.TestDomain");

      _extractor.Save (_xmlTempFilename);

      XmlDocument xmlDocument = new XmlDocument ();
      xmlDocument.Load (_xmlTempFilename);

      string expectedXml = @"<?xml version=""1.0""?>
          <securityMetadata xmlns=""http://www.re-motion.org/Security/Metadata/1.0"">
            <classes>
              <class id=""00000000-0000-0000-0001-000000000000"" name=""Remotion.Security.UnitTests.TestDomain.File, Remotion.Security.UnitTests.TestDomain"">
                <stateProperties>
                  <statePropertyRef>00000000-0000-0000-0001-000000000001</statePropertyRef>
                </stateProperties>

                <accessTypes>
                  <accessTypeRef>1d6d25bc-4e85-43ab-a42d-fb5a829c30d5</accessTypeRef>
                  <accessTypeRef>62dfcd92-a480-4d57-95f1-28c0f5996b3a</accessTypeRef>
                  <accessTypeRef>11186122-6de0-4194-b434-9979230c41fd</accessTypeRef>
                  <accessTypeRef>305fbb40-75c8-423a-84b2-b26ea9e7cae7</accessTypeRef>
                  <accessTypeRef>67cea479-0be7-4e2f-b2e0-bb1fcc9ea1d6</accessTypeRef>
                  <accessTypeRef>203b7478-96f1-4bf1-b4ea-5bdd1206252c</accessTypeRef>
                  <accessTypeRef>00000002-0001-0000-0000-000000000000</accessTypeRef>
                </accessTypes>
              </class>

              <class id=""00000000-0000-0000-0002-000000000000"" name=""Remotion.Security.UnitTests.TestDomain.PaperFile, Remotion.Security.UnitTests.TestDomain"" base=""00000000-0000-0000-0001-000000000000"">
                <stateProperties>
                  <statePropertyRef>00000000-0000-0000-0001-000000000001</statePropertyRef>
                  <statePropertyRef>00000000-0000-0000-0002-000000000001</statePropertyRef>
                </stateProperties>

                <accessTypes>
                  <accessTypeRef>1d6d25bc-4e85-43ab-a42d-fb5a829c30d5</accessTypeRef>
                  <accessTypeRef>62dfcd92-a480-4d57-95f1-28c0f5996b3a</accessTypeRef>
                  <accessTypeRef>11186122-6de0-4194-b434-9979230c41fd</accessTypeRef>
                  <accessTypeRef>305fbb40-75c8-423a-84b2-b26ea9e7cae7</accessTypeRef>
                  <accessTypeRef>67cea479-0be7-4e2f-b2e0-bb1fcc9ea1d6</accessTypeRef>
                  <accessTypeRef>203b7478-96f1-4bf1-b4ea-5bdd1206252c</accessTypeRef>
                  <accessTypeRef>00000002-0001-0000-0000-000000000000</accessTypeRef>
                </accessTypes>
              </class>
            </classes>

            <stateProperties>
              <stateProperty id=""00000000-0000-0000-0002-000000000001"" name=""State"">
                <state name=""New|Remotion.Security.UnitTests.TestDomain.FileState, Remotion.Security.UnitTests.TestDomain"" value=""0"" />
                <state name=""Normal|Remotion.Security.UnitTests.TestDomain.FileState, Remotion.Security.UnitTests.TestDomain"" value=""1"" />
                <state name=""Archived|Remotion.Security.UnitTests.TestDomain.FileState, Remotion.Security.UnitTests.TestDomain"" value=""2"" />
              </stateProperty>

              <stateProperty id=""00000000-0000-0000-0001-000000000001"" name=""Confidentiality"">
                <state name=""Normal|Remotion.Security.UnitTests.TestDomain.Confidentiality, Remotion.Security.UnitTests.TestDomain"" value=""0"" />
                <state name=""Confidential|Remotion.Security.UnitTests.TestDomain.Confidentiality, Remotion.Security.UnitTests.TestDomain"" value=""1"" />
                <state name=""Private|Remotion.Security.UnitTests.TestDomain.Confidentiality, Remotion.Security.UnitTests.TestDomain"" value=""2"" />
              </stateProperty>
            </stateProperties>

            <accessTypes>
              <accessType id=""1d6d25bc-4e85-43ab-a42d-fb5a829c30d5"" name=""Create|Remotion.Security.GeneralAccessTypes, Remotion.Security"" value=""0"" />
              <accessType id=""62dfcd92-a480-4d57-95f1-28c0f5996b3a"" name=""Read|Remotion.Security.GeneralAccessTypes, Remotion.Security"" value=""1"" />
              <accessType id=""11186122-6de0-4194-b434-9979230c41fd"" name=""Edit|Remotion.Security.GeneralAccessTypes, Remotion.Security"" value=""2"" />
              <accessType id=""305fbb40-75c8-423a-84b2-b26ea9e7cae7"" name=""Delete|Remotion.Security.GeneralAccessTypes, Remotion.Security"" value=""3"" />
              <accessType id=""67cea479-0be7-4e2f-b2e0-bb1fcc9ea1d6"" name=""Search|Remotion.Security.GeneralAccessTypes, Remotion.Security"" value=""4"" />
              <accessType id=""203b7478-96f1-4bf1-b4ea-5bdd1206252c"" name=""Find|Remotion.Security.GeneralAccessTypes, Remotion.Security"" value=""5"" />
              <accessType id=""00000002-0001-0000-0000-000000000000"" name=""Journalize|Remotion.Security.UnitTests.TestDomain.DomainAccessTypes, Remotion.Security.UnitTests.TestDomain"" value=""0"" />
              <accessType id=""00000002-0002-0000-0000-000000000000"" name=""Archive|Remotion.Security.UnitTests.TestDomain.DomainAccessTypes, Remotion.Security.UnitTests.TestDomain"" value=""1"" />
            </accessTypes>

            <abstractRoles>
              <abstractRole id=""00000003-0001-0000-0000-000000000000"" name=""Clerk|Remotion.Security.UnitTests.TestDomain.DomainAbstractRoles, Remotion.Security.UnitTests.TestDomain"" value=""0"" />
              <abstractRole id=""00000003-0002-0000-0000-000000000000"" name=""Secretary|Remotion.Security.UnitTests.TestDomain.DomainAbstractRoles, Remotion.Security.UnitTests.TestDomain"" value=""1"" />
              <abstractRole id=""00000004-0001-0000-0000-000000000000"" name=""Administrator|Remotion.Security.UnitTests.TestDomain.SpecialAbstractRoles, Remotion.Security.UnitTests.TestDomain"" value=""0"" />
            </abstractRoles>
          </securityMetadata>";

      XmlAssert.AreDocumentsSimilar (expectedXml, xmlDocument);
    }
  }
}
