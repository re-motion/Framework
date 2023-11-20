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
using Remotion.Security.UnitTests.TestDomain;
using Remotion.Security.UnitTests.XmlAsserter;
using File = Remotion.Security.UnitTests.TestDomain.File;

namespace Remotion.Security.UnitTests.Metadata
{
  [TestFixture]
  public class MetadataToXmlConverterTest
  {
    private MetadataCache _cache;
    private MetadataToXmlConverter _converter;

    [SetUp]
    public void SetUp ()
    {
      _cache = new MetadataCache();
      _converter = new MetadataToXmlConverter();
    }

    [Test]
    public void Convert_EmptyMetadata ()
    {
      XmlDocument document = _converter.Convert(_cache);

      string expectedXml = @"<?xml version=""1.0""?>
          <securityMetadata xmlns=""http://www.re-motion.org/Security/Metadata/1.0"" />";

      XmlAssert.AreDocumentsEqual(expectedXml, document);
    }

    [Test]
    public void Convert_OneEmptyClass ()
    {
      SecurableClassInfo classInfo = new SecurableClassInfo();
      classInfo.ID = "4bbb1bab-8d37-40c0-918d-7a07cc7de44f";
      classInfo.Name = "File";
      _cache.AddSecurableClassInfo(typeof(File), classInfo);

      XmlDocument document = _converter.Convert(_cache);

      string expectedXml = @"<?xml version=""1.0""?>
          <securityMetadata xmlns=""http://www.re-motion.org/Security/Metadata/1.0"">
            <classes>
              <class id=""4bbb1bab-8d37-40c0-918d-7a07cc7de44f"" name=""File"" />
            </classes>
          </securityMetadata>";

      XmlAssert.AreDocumentsEqual(expectedXml, document);
    }

    [Test]
    public void Convert_OneStateProperty ()
    {
      StatePropertyInfo propertyInfo = new StatePropertyInfo();
      propertyInfo.ID = "4bbb1bab-8d37-40c0-918d-7a07cc7de44f";
      propertyInfo.Name = "Confidentiality";
      propertyInfo.Values.Add(new EnumValueInfo("Domain.Confidentiality, Domain", "Normal", 0));
      propertyInfo.Values.Add(new EnumValueInfo("Domain.Confidentiality, Domain", "Confidential", 1));
      propertyInfo.Values.Add(new EnumValueInfo("Domain.Confidentiality, Domain", "Private", 2));

      Type type = typeof(File);
      PropertyInfo property = type.GetProperty("Confidentiality");
      _cache.AddStatePropertyInfo(property, propertyInfo);

      XmlDocument document = _converter.Convert(_cache);

      string expectedXml = @"<?xml version=""1.0""?>
          <securityMetadata xmlns=""http://www.re-motion.org/Security/Metadata/1.0"">
            <stateProperties>
              <stateProperty id=""4bbb1bab-8d37-40c0-918d-7a07cc7de44f"" name=""Confidentiality"">
                <state name=""Normal|Domain.Confidentiality, Domain"" value=""0"" />
                <state name=""Confidential|Domain.Confidentiality, Domain"" value=""1"" />
                <state name=""Private|Domain.Confidentiality, Domain"" value=""2"" />
              </stateProperty>
            </stateProperties>
          </securityMetadata>";

      XmlAssert.AreDocumentsEqual(expectedXml, document);
    }

    [Test]
    public void Convert_OneAccessType ()
    {
      EnumValueInfo accessType = new EnumValueInfo("Domain.AccessType, Domain", "Archive", 0);
      accessType.ID = "4bbb1bab-8d37-40c0-918d-7a07cc7de44f";
      _cache.AddAccessType(DomainAccessTypes.Archive, accessType);

      XmlDocument document = _converter.Convert(_cache);

      string expectedXml = @"<?xml version=""1.0""?>
          <securityMetadata xmlns=""http://www.re-motion.org/Security/Metadata/1.0"">
            <accessTypes>
              <accessType id=""4bbb1bab-8d37-40c0-918d-7a07cc7de44f"" name=""Archive|Domain.AccessType, Domain"" value=""0"" />
            </accessTypes>
          </securityMetadata>";

      XmlAssert.AreDocumentsEqual(expectedXml, document);
    }

    [Test]
    public void Convert_OneAbstractRole ()
    {
      EnumValueInfo abstractRole = new EnumValueInfo("Domain.SpecialAbstractRoles, Domain", "Administrator", 0);
      abstractRole.ID = "00000004-0001-0000-0000-000000000000";
      _cache.AddAbstractRole(SpecialAbstractRoles.Administrator, abstractRole);

      XmlDocument document = _converter.Convert(_cache);

      string expectedXml = @"<?xml version=""1.0""?>
          <securityMetadata xmlns=""http://www.re-motion.org/Security/Metadata/1.0"">
            <abstractRoles>
              <abstractRole id=""00000004-0001-0000-0000-000000000000"" name=""Administrator|Domain.SpecialAbstractRoles, Domain"" value=""0"" />
            </abstractRoles>
          </securityMetadata>";

      XmlAssert.AreDocumentsEqual(expectedXml, document);
    }

    [Test]
    public void Convert_MultipleClasses ()
    {
      SecurableClassInfo baseClassInfo = new SecurableClassInfo();
      baseClassInfo.ID = "4bbb1bab-8d37-40c0-918d-7a07cc7de44f";
      baseClassInfo.Name = "File";
      _cache.AddSecurableClassInfo(typeof(File), baseClassInfo);

      SecurableClassInfo derivedClassInfo1 = new SecurableClassInfo();
      derivedClassInfo1.ID = "00000000-0000-0000-0002-000000000000";
      derivedClassInfo1.Name = "PaperFile";
      _cache.AddSecurableClassInfo(typeof(PaperFile), derivedClassInfo1);

      SecurableClassInfo derivedClassInfo2 = new SecurableClassInfo();
      derivedClassInfo2.ID = "118a9d5e-4f89-40af-ade5-e4613e4638d5";
      derivedClassInfo2.Name = "InputFile";
      _cache.AddSecurableClassInfo(typeof(SecurableClassInfo), derivedClassInfo2);

      XmlDocument document = _converter.Convert(_cache);

      string expectedXml = @"<?xml version=""1.0""?>
          <securityMetadata xmlns=""http://www.re-motion.org/Security/Metadata/1.0"">
            <classes>
              <class id=""4bbb1bab-8d37-40c0-918d-7a07cc7de44f"" name=""File"" />
              <class id=""00000000-0000-0000-0002-000000000000"" name=""PaperFile"" />
              <class id=""118a9d5e-4f89-40af-ade5-e4613e4638d5"" name=""InputFile"" />
            </classes>
          </securityMetadata>";

      XmlAssert.AreDocumentsEqual(expectedXml, document);
    }

    [Test]
    public void Convert_DerivedClasses ()
    {
      SecurableClassInfo baseClassInfo = new SecurableClassInfo();
      baseClassInfo.ID = "4bbb1bab-8d37-40c0-918d-7a07cc7de44f";
      baseClassInfo.Name = "File";
      _cache.AddSecurableClassInfo(typeof(File), baseClassInfo);

      SecurableClassInfo derivedClassInfo1 = new SecurableClassInfo();
      derivedClassInfo1.ID = "00000000-0000-0000-0002-000000000000";
      derivedClassInfo1.Name = "PaperFile";
      _cache.AddSecurableClassInfo(typeof(PaperFile), derivedClassInfo1);

      SecurableClassInfo derivedClassInfo2 = new SecurableClassInfo();
      derivedClassInfo2.ID = "118a9d5e-4f89-40af-ade5-e4613e4638d5";
      derivedClassInfo2.Name = "InputFile";
      _cache.AddSecurableClassInfo(typeof(SecurableClassInfo), derivedClassInfo2);

      derivedClassInfo1.BaseClass = baseClassInfo;
      derivedClassInfo2.BaseClass = baseClassInfo;
      baseClassInfo.DerivedClasses.Add(derivedClassInfo1);
      baseClassInfo.DerivedClasses.Add(derivedClassInfo2);

      XmlDocument document = _converter.Convert(_cache);

      string expectedXml = @"<?xml version=""1.0""?>
          <securityMetadata xmlns=""http://www.re-motion.org/Security/Metadata/1.0"">
            <classes>
              <class id=""4bbb1bab-8d37-40c0-918d-7a07cc7de44f"" name=""File"" />
              <class id=""00000000-0000-0000-0002-000000000000"" name=""PaperFile"" base=""4bbb1bab-8d37-40c0-918d-7a07cc7de44f"" />
              <class id=""118a9d5e-4f89-40af-ade5-e4613e4638d5"" name=""InputFile"" base=""4bbb1bab-8d37-40c0-918d-7a07cc7de44f"" />
            </classes>
          </securityMetadata>";

      XmlAssert.AreDocumentsEqual(expectedXml, document);
    }

    [Test]
    public void Convert_MultipleStateProperties ()
    {
      StatePropertyInfo propertyInfo1 = new StatePropertyInfo();
      propertyInfo1.ID = "4bbb1bab-8d37-40c0-918d-7a07cc7de44f";
      propertyInfo1.Name = "Confidentiality";
      propertyInfo1.Values.Add(new EnumValueInfo("Domain.Confidentiality, Domain", "Normal", 0));
      propertyInfo1.Values.Add(new EnumValueInfo("Domain.Confidentiality, Domain", "Confidential", 1));
      propertyInfo1.Values.Add(new EnumValueInfo("Domain.Confidentiality, Domain", "Private", 2));

      StatePropertyInfo propertyInfo2 = new StatePropertyInfo();
      propertyInfo2.ID = "40749391-5c45-4fdd-a698-53a6cf167ae7";
      propertyInfo2.Name = "SomeEnum";
      propertyInfo2.Values.Add(new EnumValueInfo("Namespace.TypeName, Assembly", "First", 0));
      propertyInfo2.Values.Add(new EnumValueInfo("Namespace.TypeName, Assembly", "Second", 1));
      propertyInfo2.Values.Add(new EnumValueInfo("Namespace.TypeName, Assembly", "Third", 2));

      Type type = typeof(File);
      PropertyInfo property1 = type.GetProperty("Confidentiality");
      _cache.AddStatePropertyInfo(property1, propertyInfo1);
      PropertyInfo property2 = type.GetProperty("SimpleEnum");
      _cache.AddStatePropertyInfo(property2, propertyInfo2);

      XmlDocument document = _converter.Convert(_cache);

      string expectedXml = @"<?xml version=""1.0""?>
          <securityMetadata xmlns=""http://www.re-motion.org/Security/Metadata/1.0"">
            <stateProperties>
              <stateProperty id=""4bbb1bab-8d37-40c0-918d-7a07cc7de44f"" name=""Confidentiality"">
                <state name=""Normal|Domain.Confidentiality, Domain"" value=""0"" />
                <state name=""Confidential|Domain.Confidentiality, Domain"" value=""1"" />
                <state name=""Private|Domain.Confidentiality, Domain"" value=""2"" />
              </stateProperty>

              <stateProperty id=""40749391-5c45-4fdd-a698-53a6cf167ae7"" name=""SomeEnum"">
                <state name=""First|Namespace.TypeName, Assembly"" value=""0"" />
                <state name=""Second|Namespace.TypeName, Assembly"" value=""1"" />
                <state name=""Third|Namespace.TypeName, Assembly"" value=""2"" />
              </stateProperty>
            </stateProperties>
          </securityMetadata>";

      XmlAssert.AreDocumentsEqual(expectedXml, document);
    }

    [Test]
    public void Convert_MultipleAccessTypes ()
    {
      EnumValueInfo accessType1 = new EnumValueInfo("Domain.AccessType, Domain", "Archive", 0);
      accessType1.ID = "4bbb1bab-8d37-40c0-918d-7a07cc7de44f";
      _cache.AddAccessType(DomainAccessTypes.Archive, accessType1);

      EnumValueInfo accessType2 = new EnumValueInfo("Domain.AccessType, Domain", "Journalize", 1);
      accessType2.ID = "c6995b9b-7fed-42df-a2d1-897600b00fb0";
      _cache.AddAccessType(DomainAccessTypes.Journalize, accessType2);

      XmlDocument document = _converter.Convert(_cache);

      string expectedXml = @"<?xml version=""1.0""?>
          <securityMetadata xmlns=""http://www.re-motion.org/Security/Metadata/1.0"">
            <accessTypes>
              <accessType id=""4bbb1bab-8d37-40c0-918d-7a07cc7de44f"" name=""Archive|Domain.AccessType, Domain"" value=""0"" />
              <accessType id=""c6995b9b-7fed-42df-a2d1-897600b00fb0"" name=""Journalize|Domain.AccessType, Domain"" value=""1"" />
            </accessTypes>
          </securityMetadata>";

      XmlAssert.AreDocumentsEqual(expectedXml, document);
    }

    [Test]
    public void Convert_MultipleAbstractRoles ()
    {
      EnumValueInfo abstractRole1 = new EnumValueInfo("Domain.SpecialAbstractRoles, Domain", "Administrator", 0);
      abstractRole1.ID = "00000004-0001-0000-0000-000000000000";
      _cache.AddAbstractRole(SpecialAbstractRoles.Administrator, abstractRole1);

      EnumValueInfo abstractRole2 = new EnumValueInfo("Domain.SpecialAbstractRoles, Domain", "PowerUser", 1);
      abstractRole2.ID = "3b84739a-7f35-4224-989f-3d5b05047cbb";
      _cache.AddAbstractRole(SomeEnum.First, abstractRole2);

      XmlDocument document = _converter.Convert(_cache);

      string expectedXml = @"<?xml version=""1.0""?>
          <securityMetadata xmlns=""http://www.re-motion.org/Security/Metadata/1.0"">
            <abstractRoles>
              <abstractRole id=""00000004-0001-0000-0000-000000000000"" name=""Administrator|Domain.SpecialAbstractRoles, Domain"" value=""0"" />
              <abstractRole id=""3b84739a-7f35-4224-989f-3d5b05047cbb"" name=""PowerUser|Domain.SpecialAbstractRoles, Domain"" value=""1"" />
            </abstractRoles>
          </securityMetadata>";

      XmlAssert.AreDocumentsEqual(expectedXml, document);
    }

    [Test]
    public void Convert_ClassWithStateProperties ()
    {
      SecurableClassInfo classInfo = new SecurableClassInfo();
      classInfo.ID = "4bbb1bab-8d37-40c0-918d-7a07cc7de44f";
      classInfo.Name = "File";
      _cache.AddSecurableClassInfo(typeof(File), classInfo);

      StatePropertyInfo propertyInfo = new StatePropertyInfo();
      propertyInfo.ID = "4bbb1bab-8d37-40c0-918d-7a07cc7de44f";
      propertyInfo.Name = "Confidentiality";
      propertyInfo.Values.Add(new EnumValueInfo("Domain.Confidentiality, Domain", "Normal", 0));
      propertyInfo.Values.Add(new EnumValueInfo("Domain.Confidentiality, Domain", "Confidential", 1));
      propertyInfo.Values.Add(new EnumValueInfo("Domain.Confidentiality, Domain", "Private", 2));

      Type type = typeof(File);
      PropertyInfo property = type.GetProperty("Confidentiality");
      _cache.AddStatePropertyInfo(property, propertyInfo);

      classInfo.Properties.Add(propertyInfo);

      XmlDocument document = _converter.Convert(_cache);

      string expectedXml = @"<?xml version=""1.0""?>
          <securityMetadata xmlns=""http://www.re-motion.org/Security/Metadata/1.0"">
            <classes>
              <class id=""4bbb1bab-8d37-40c0-918d-7a07cc7de44f"" name=""File"">
                <stateProperties>
                  <statePropertyRef>4bbb1bab-8d37-40c0-918d-7a07cc7de44f</statePropertyRef>
                </stateProperties>
              </class>
            </classes>

            <stateProperties>
              <stateProperty id=""4bbb1bab-8d37-40c0-918d-7a07cc7de44f"" name=""Confidentiality"">
                <state name=""Normal|Domain.Confidentiality, Domain"" value=""0"" />
                <state name=""Confidential|Domain.Confidentiality, Domain"" value=""1"" />
                <state name=""Private|Domain.Confidentiality, Domain"" value=""2"" />
              </stateProperty>
            </stateProperties>
          </securityMetadata>";

      XmlAssert.AreDocumentsEqual(expectedXml, document);
    }

    [Test]
    public void Convert_ClassWithAccessTypes ()
    {
      SecurableClassInfo classInfo = new SecurableClassInfo();
      classInfo.ID = "4bbb1bab-8d37-40c0-918d-7a07cc7de44f";
      classInfo.Name = "File";
      _cache.AddSecurableClassInfo(typeof(File), classInfo);

      EnumValueInfo accessType1 = new EnumValueInfo("Domain.AccessType, Domain", "Archive", 0);
      accessType1.ID = "64d8f74e-685f-44ab-9705-1fda9ff836a4";
      _cache.AddAccessType(DomainAccessTypes.Archive, accessType1);

      EnumValueInfo accessType2 = new EnumValueInfo("Domain.AccessType, Domain", "Journalize", 1);
      accessType2.ID = "c6995b9b-7fed-42df-a2d1-897600b00fb0";
      _cache.AddAccessType(DomainAccessTypes.Journalize, accessType2);

      classInfo.AccessTypes.Add(accessType1);
      classInfo.AccessTypes.Add(accessType2);

      XmlDocument document = _converter.Convert(_cache);

      string expectedXml = @"<?xml version=""1.0""?>
          <securityMetadata xmlns=""http://www.re-motion.org/Security/Metadata/1.0"">
            <classes>
              <class id=""4bbb1bab-8d37-40c0-918d-7a07cc7de44f"" name=""File"">
                <accessTypes>
                  <accessTypeRef>64d8f74e-685f-44ab-9705-1fda9ff836a4</accessTypeRef>
                  <accessTypeRef>c6995b9b-7fed-42df-a2d1-897600b00fb0</accessTypeRef>
                </accessTypes>
              </class>
            </classes>

            <accessTypes>
              <accessType id=""64d8f74e-685f-44ab-9705-1fda9ff836a4"" name=""Archive|Domain.AccessType, Domain"" value=""0"" />
              <accessType id=""c6995b9b-7fed-42df-a2d1-897600b00fb0"" name=""Journalize|Domain.AccessType, Domain"" value=""1"" />
            </accessTypes>
          </securityMetadata>";

      XmlAssert.AreDocumentsEqual(expectedXml, document);
    }

    [Test]
    public void Convert_IntegrationTest ()
    {
      SecurableClassInfo classInfo = new SecurableClassInfo();
      classInfo.ID = "4bbb1bab-8d37-40c0-918d-7a07cc7de44f";
      classInfo.Name = "File";
      _cache.AddSecurableClassInfo(typeof(File), classInfo);

      SecurableClassInfo derivedClassInfo = new SecurableClassInfo();
      derivedClassInfo.ID = "ac101f66-6d1f-4002-b32b-f951db36582c";
      derivedClassInfo.Name = "PaperFile";
      _cache.AddSecurableClassInfo(typeof(PaperFile), derivedClassInfo);

      classInfo.DerivedClasses.Add(derivedClassInfo);
      derivedClassInfo.BaseClass = classInfo;

      StatePropertyInfo propertyInfo1 = new StatePropertyInfo();
      propertyInfo1.ID = "d81b1521-ea06-4338-af6f-ff8510394efd";
      propertyInfo1.Name = "Confidentiality";
      propertyInfo1.Values.Add(new EnumValueInfo("Domain.Confidentiality, Domain", "Normal", 0));
      propertyInfo1.Values.Add(new EnumValueInfo("Domain.Confidentiality, Domain", "Confidential", 1));
      propertyInfo1.Values.Add(new EnumValueInfo("Domain.Confidentiality, Domain", "Private", 2));

      StatePropertyInfo propertyInfo2 = new StatePropertyInfo();
      propertyInfo2.ID = "40749391-5c45-4fdd-a698-53a6cf167ae7";
      propertyInfo2.Name = "SomeEnum";
      propertyInfo2.Values.Add(new EnumValueInfo("Namespace.TypeName, Assembly", "First", 0));
      propertyInfo2.Values.Add(new EnumValueInfo("Namespace.TypeName, Assembly", "Second", 1));
      propertyInfo2.Values.Add(new EnumValueInfo("Namespace.TypeName, Assembly", "Third", 2));

      Type type = typeof(File);
      PropertyInfo property1 = type.GetProperty("Confidentiality");
      _cache.AddStatePropertyInfo(property1, propertyInfo1);
      PropertyInfo property2 = type.GetProperty("SimpleEnum");
      _cache.AddStatePropertyInfo(property2, propertyInfo2);

      classInfo.Properties.Add(propertyInfo1);
      derivedClassInfo.Properties.Add(propertyInfo1);
      derivedClassInfo.Properties.Add(propertyInfo2);

      EnumValueInfo accessType1 = new EnumValueInfo("Domain.AccessType, Domain", "Archive", 0);
      accessType1.ID = "64d8f74e-685f-44ab-9705-1fda9ff836a4";
      _cache.AddAccessType(DomainAccessTypes.Archive, accessType1);

      EnumValueInfo accessType2 = new EnumValueInfo("Domain.AccessType, Domain", "Journalize", 1);
      accessType2.ID = "c6995b9b-7fed-42df-a2d1-897600b00fb0";
      _cache.AddAccessType(DomainAccessTypes.Journalize, accessType2);

      classInfo.AccessTypes.Add(accessType1);
      derivedClassInfo.AccessTypes.Add(accessType1);
      derivedClassInfo.AccessTypes.Add(accessType2);

      EnumValueInfo abstractRole1 = new EnumValueInfo("Domain.AbstractRole, Domain", "Administrator", 0);
      abstractRole1.ID = "00000004-0001-0000-0000-000000000000";
      _cache.AddAbstractRole(SpecialAbstractRoles.Administrator, abstractRole1);

      EnumValueInfo abstractRole2 = new EnumValueInfo("Domain.AbstractRole, Domain", "PowerUser", 1);
      abstractRole2.ID = "3b84739a-7f35-4224-989f-3d5b05047cbb";
      _cache.AddAbstractRole(SomeEnum.First, abstractRole2);

      XmlDocument document = _converter.Convert(_cache);

      string expectedXml = @"<?xml version=""1.0""?>
          <securityMetadata xmlns=""http://www.re-motion.org/Security/Metadata/1.0"">
            <classes>
              <class id=""4bbb1bab-8d37-40c0-918d-7a07cc7de44f"" name=""File"">
                <stateProperties>
                  <statePropertyRef>d81b1521-ea06-4338-af6f-ff8510394efd</statePropertyRef>
                </stateProperties>

                <accessTypes>
                  <accessTypeRef>64d8f74e-685f-44ab-9705-1fda9ff836a4</accessTypeRef>
                </accessTypes>
              </class>

              <class id=""ac101f66-6d1f-4002-b32b-f951db36582c"" name=""PaperFile"" base=""4bbb1bab-8d37-40c0-918d-7a07cc7de44f"">
                <stateProperties>
                  <statePropertyRef>d81b1521-ea06-4338-af6f-ff8510394efd</statePropertyRef>
                  <statePropertyRef>40749391-5c45-4fdd-a698-53a6cf167ae7</statePropertyRef>
                </stateProperties>

                <accessTypes>
                  <accessTypeRef>64d8f74e-685f-44ab-9705-1fda9ff836a4</accessTypeRef>
                  <accessTypeRef>c6995b9b-7fed-42df-a2d1-897600b00fb0</accessTypeRef>
                </accessTypes>
              </class>
            </classes>

            <stateProperties>
              <stateProperty id=""d81b1521-ea06-4338-af6f-ff8510394efd"" name=""Confidentiality"">
                <state name=""Normal|Domain.Confidentiality, Domain"" value=""0"" />
                <state name=""Confidential|Domain.Confidentiality, Domain"" value=""1"" />
                <state name=""Private|Domain.Confidentiality, Domain"" value=""2"" />
              </stateProperty>

              <stateProperty id=""40749391-5c45-4fdd-a698-53a6cf167ae7"" name=""SomeEnum"">
                <state name=""First|Namespace.TypeName, Assembly"" value=""0"" />
                <state name=""Second|Namespace.TypeName, Assembly"" value=""1"" />
                <state name=""Third|Namespace.TypeName, Assembly"" value=""2"" />
              </stateProperty>
            </stateProperties>

            <accessTypes>
              <accessType id=""64d8f74e-685f-44ab-9705-1fda9ff836a4"" name=""Archive|Domain.AccessType, Domain"" value=""0"" />
              <accessType id=""c6995b9b-7fed-42df-a2d1-897600b00fb0"" name=""Journalize|Domain.AccessType, Domain"" value=""1"" />
            </accessTypes>

            <abstractRoles>
              <abstractRole id=""00000004-0001-0000-0000-000000000000"" name=""Administrator|Domain.AbstractRole, Domain"" value=""0"" />
              <abstractRole id=""3b84739a-7f35-4224-989f-3d5b05047cbb"" name=""PowerUser|Domain.AbstractRole, Domain"" value=""1"" />
            </abstractRoles>
          </securityMetadata>";

      XmlAssert.AreDocumentsEqual(expectedXml, document);
    }

    [Test]
    public void ConvertAndSave_NewFile ()
    {
      string testOutputPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
      Directory.CreateDirectory(testOutputPath);

      string filename = Path.Combine(testOutputPath, "metadata.xml");

      if (System.IO.File.Exists(filename))
        System.IO.File.Delete(filename);

      _converter.ConvertAndSave(_cache, filename);

      Assert.That(System.IO.File.Exists(filename), Is.True);
      Directory.Delete(testOutputPath, true);
    }
  }
}
