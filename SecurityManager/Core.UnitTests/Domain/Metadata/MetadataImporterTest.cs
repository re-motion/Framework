// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Xml;
using System.Xml.Schema;
using NUnit.Framework;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata
{
  [TestFixture]
  public class MetadataImporterTest : DomainTest
  {
    private MetadataTestHelper _testHelper;
    private MetadataImporter _importer;

    public override void SetUp ()
    {
      base.SetUp();

      _testHelper = new MetadataTestHelper();
      _importer = new MetadataImporter(_testHelper.Transaction);
      _testHelper.Transaction.EnterNonDiscardingScope();
    }

    [Test]
    public void Import_EmptyMetadataFile ()
    {
      string metadataXml = @"
          <securityMetadata xmlns=""http://www.re-motion.org/Security/Metadata/1.0"">
            <classes />
            <stateProperties />
            <accessTypes />
            <abstractRoles />
          </securityMetadata>
          ";

      _importer.Import(GetXmlDocument(metadataXml));

      using (_testHelper.Transaction.EnterNonDiscardingScope())
      {
        Assert.That(_importer.Classes.Count, Is.EqualTo(0), "Class count");
        Assert.That(_importer.StateProperties.Count, Is.EqualTo(0), "State property count");
        Assert.That(_importer.AbstractRoles.Count, Is.EqualTo(0), "Abstract role count");
        Assert.That(_importer.AccessTypes.Count, Is.EqualTo(0), "Access type count");
      }
    }

    [Test]
    public void Import_1SecurableClass ()
    {
      string metadataXml =
          @"
          <securityMetadata xmlns=""http://www.re-motion.org/Security/Metadata/1.0"">
            <classes>
              <class id=""00000000-0000-0000-0001-000000000000"" name=""Remotion.Security.UnitTests.TestDomain.File"" />
            </classes>
            <stateProperties />
            <accessTypes />
            <abstractRoles />
          </securityMetadata>
          ";

      _importer.Import(GetXmlDocument(metadataXml));

      using (_testHelper.Transaction.EnterNonDiscardingScope())
      {
        Assert.That(_importer.Classes.Count, Is.EqualTo(1), "Class count");
        Assert.That(_importer.StateProperties.Count, Is.EqualTo(0), "State property count");
        Assert.That(_importer.AbstractRoles.Count, Is.EqualTo(0), "Abstract role count");
        Assert.That(_importer.AccessTypes.Count, Is.EqualTo(0), "Access type count");

        SecurableClassDefinition actualClass1 = _importer.Classes[new Guid("00000000-0000-0000-0001-000000000000")];
        Assert.That(actualClass1.Index, Is.EqualTo(0));
        Assert.That(actualClass1.Name, Is.EqualTo("Remotion.Security.UnitTests.TestDomain.File"));
      }
    }

    [Test]
    public void Import_2SecurableClasses ()
    {
      string metadataXml =
          @"
          <securityMetadata xmlns=""http://www.re-motion.org/Security/Metadata/1.0"">
            <classes>
              <class id=""00000000-0000-0000-0001-000000000000"" name=""Remotion.Security.UnitTests.TestDomain.File"" />
              <class id=""00000000-0000-0000-0002-000000000000"" name=""Remotion.Security.UnitTests.TestDomain.Directory"" />
            </classes>
            <stateProperties />
            <accessTypes />
            <abstractRoles />
          </securityMetadata>
          ";

      _importer.Import(GetXmlDocument(metadataXml));

      using (_testHelper.Transaction.EnterNonDiscardingScope())
      {
        Assert.That(_importer.Classes.Count, Is.EqualTo(2), "Class count");
        Assert.That(_importer.StateProperties.Count, Is.EqualTo(0), "State property count");
        Assert.That(_importer.AbstractRoles.Count, Is.EqualTo(0), "Abstract role count");
        Assert.That(_importer.AccessTypes.Count, Is.EqualTo(0), "Access type count");

        SecurableClassDefinition actualClass1 = _importer.Classes[new Guid("00000000-0000-0000-0001-000000000000")];
        Assert.That(actualClass1.Index, Is.EqualTo(0));
        Assert.That(actualClass1.Name, Is.EqualTo("Remotion.Security.UnitTests.TestDomain.File"));

        SecurableClassDefinition actualClass2 = _importer.Classes[new Guid("00000000-0000-0000-0002-000000000000")];
        Assert.That(actualClass2.Index, Is.EqualTo(1));
        Assert.That(actualClass2.Name, Is.EqualTo("Remotion.Security.UnitTests.TestDomain.Directory"));
      }
    }

    [Test]
    public void Import_3AbstractRoles ()
    {
      string metadataXml =
          @"
          <securityMetadata xmlns=""http://www.re-motion.org/Security/Metadata/1.0"">
            <classes />
            <stateProperties />
            <accessTypes />
            <abstractRoles>
              <abstractRole id=""00000003-0001-0000-0000-000000000000"" name=""Clerk|Remotion.Security.UnitTests.TestDomain.DomainAbstractRoles, Remotion.Security.UnitTests.TestDomain"" value=""0"" />
              <abstractRole id=""00000003-0002-0000-0000-000000000000"" name=""Secretary|Remotion.Security.UnitTests.TestDomain.DomainAbstractRoles, Remotion.Security.UnitTests.TestDomain"" value=""1"" />
              <abstractRole id=""00000004-0001-0000-0000-000000000000"" name=""Administrator|Remotion.Security.UnitTests.TestDomain.SpecialAbstractRoles, Remotion.Security.UnitTests.TestDomain"" value=""0"" />
            </abstractRoles>
          </securityMetadata>
          ";

      _importer.Import(GetXmlDocument(metadataXml));

      using (_testHelper.Transaction.EnterNonDiscardingScope())
      {
        Assert.That(_importer.Classes.Count, Is.EqualTo(0), "Class count");
        Assert.That(_importer.StateProperties.Count, Is.EqualTo(0), "State property count");
        Assert.That(_importer.AbstractRoles.Count, Is.EqualTo(3), "Abstract role count");
        Assert.That(_importer.AccessTypes.Count, Is.EqualTo(0), "Access type count");

        AbstractRoleDefinition expectedRole1 = _testHelper.CreateClerkAbstractRole(0);
        MetadataObjectAssert.AreEqual(expectedRole1, _importer.AbstractRoles[expectedRole1.MetadataItemID], "Abstract Role Clerk");

        AbstractRoleDefinition expectedRole2 = _testHelper.CreateSecretaryAbstractRole(1);
        MetadataObjectAssert.AreEqual(expectedRole2, _importer.AbstractRoles[expectedRole2.MetadataItemID], "Abstract Role Secretary");

        AbstractRoleDefinition expectedRole3 = _testHelper.CreateAdministratorAbstractRole(2);
        MetadataObjectAssert.AreEqual(expectedRole3, _importer.AbstractRoles[expectedRole3.MetadataItemID], "Abstract Role Administrator");
      }
    }

    [Test]
    public void Import_3AccessTypes ()
    {
      string metadataXml =
          @"
          <securityMetadata xmlns=""http://www.re-motion.org/Security/Metadata/1.0"">
            <classes />
            <stateProperties />
            <accessTypes>
              <accessType id=""1d6d25bc-4e85-43ab-a42d-fb5a829c30d5"" name=""Create|Remotion.Security.GeneralAccessTypes, Remotion.Security"" value=""0"" />
              <accessType id=""62dfcd92-a480-4d57-95f1-28c0f5996b3a"" name=""Read|Remotion.Security.GeneralAccessTypes, Remotion.Security"" value=""1"" />
              <accessType id=""11186122-6de0-4194-b434-9979230c41fd"" name=""Edit|Remotion.Security.GeneralAccessTypes, Remotion.Security"" value=""2"" />
            </accessTypes>
            <abstractRoles />
          </securityMetadata>
          ";

      _importer.Import(GetXmlDocument(metadataXml));

      using (_testHelper.Transaction.EnterNonDiscardingScope())
      {
        Assert.That(_importer.Classes.Count, Is.EqualTo(0), "Class count");
        Assert.That(_importer.StateProperties.Count, Is.EqualTo(0), "State property count");
        Assert.That(_importer.AbstractRoles.Count, Is.EqualTo(0), "Abstract role count");
        Assert.That(_importer.AccessTypes.Count, Is.EqualTo(3), "Access type count");

        AccessTypeDefinition expectedAccessType1 = _testHelper.CreateAccessTypeCreate(0);
        MetadataObjectAssert.AreEqual(expectedAccessType1, _importer.AccessTypes[expectedAccessType1.MetadataItemID], "Access Type Create");

        AccessTypeDefinition expectedAccessType2 = _testHelper.CreateAccessTypeRead(1);
        MetadataObjectAssert.AreEqual(expectedAccessType2, _importer.AccessTypes[expectedAccessType2.MetadataItemID], "Access Type Read");

        AccessTypeDefinition expectedAccessType3 = _testHelper.CreateAccessTypeEdit(2);
        MetadataObjectAssert.AreEqual(expectedAccessType3, _importer.AccessTypes[expectedAccessType3.MetadataItemID], "Access Type Edit");
      }
    }

    [Test]
    public void Import_2StateProperties ()
    {
      string metadataXml =
          @"
          <securityMetadata xmlns=""http://www.re-motion.org/Security/Metadata/1.0"">
            <classes />
            <stateProperties>
              <stateProperty id=""00000000-0000-0000-0002-000000000001"" name=""State"">
                <state name=""New"" value=""0"" />
                <state name=""Normal"" value=""1"" />
                <state name=""Archived"" value=""2"" />
              </stateProperty>

              <stateProperty id=""00000000-0000-0000-0001-000000000001"" name=""Confidentiality"">
                <state name=""Normal"" value=""0"" />
                <state name=""Confidential"" value=""1"" />
                <state name=""Private"" value=""2"" />
              </stateProperty>
            </stateProperties>
            <accessTypes />
            <abstractRoles />
          </securityMetadata>
          ";

      _importer.Import(GetXmlDocument(metadataXml));

      using (_testHelper.Transaction.EnterNonDiscardingScope())
      {
        Assert.That(_importer.Classes.Count, Is.EqualTo(0), "Class count");
        Assert.That(_importer.StateProperties.Count, Is.EqualTo(2), "State property count");
        Assert.That(_importer.AbstractRoles.Count, Is.EqualTo(0), "Abstract role count");
        Assert.That(_importer.AccessTypes.Count, Is.EqualTo(0), "Access type count");

        StatePropertyDefinition expectedProperty1 = _testHelper.CreateFileStateProperty(0);
        StatePropertyDefinition actualProperty1 = _importer.StateProperties[expectedProperty1.MetadataItemID];
        Assert.IsNotNull(actualProperty1, "State property not found");
        MetadataObjectAssert.AreEqual(expectedProperty1, actualProperty1, "State property");

        StatePropertyDefinition expectedProperty2 = _testHelper.CreateConfidentialityProperty(1);
        StatePropertyDefinition actualProperty2 = _importer.StateProperties[expectedProperty2.MetadataItemID];
        Assert.IsNotNull(actualProperty2, "Confidentiality property not found");
        MetadataObjectAssert.AreEqual(expectedProperty2, actualProperty2, "Confidentiality property");
      }
    }

    [Test]
    public void Import_ABaseClassAndADerivedClass ()
    {
      string metadataXml =
          @"
          <securityMetadata xmlns=""http://www.re-motion.org/Security/Metadata/1.0"">
            <classes>
              <class id=""00000000-0000-0000-0001-000000000000"" name=""Remotion.Security.UnitTests.TestDomain.File"" />
              <class id=""00000000-0000-0000-0002-000000000000"" name=""Remotion.Security.UnitTests.TestDomain.PaperFile"" base=""00000000-0000-0000-0001-000000000000"" />
            </classes>
            <stateProperties />
            <accessTypes />
            <abstractRoles />
          </securityMetadata>";

      _importer.Import(GetXmlDocument(metadataXml));

      using (_testHelper.Transaction.EnterNonDiscardingScope())
      {
        Assert.That(_importer.Classes.Count, Is.EqualTo(2), "Class count");
        Assert.That(_importer.StateProperties.Count, Is.EqualTo(0), "State property count");
        Assert.That(_importer.AbstractRoles.Count, Is.EqualTo(0), "Abstract role count");
        Assert.That(_importer.AccessTypes.Count, Is.EqualTo(0), "Access type count");

        SecurableClassDefinition baseClass = _importer.Classes[new Guid("00000000-0000-0000-0001-000000000000")];
        SecurableClassDefinition derivedClass = _importer.Classes[new Guid("00000000-0000-0000-0002-000000000000")];

        Assert.That(baseClass.DerivedClasses.Count, Is.EqualTo(1));
        Assert.That(baseClass.DerivedClasses[0], Is.SameAs(derivedClass));
        Assert.That(baseClass.BaseClass, Is.Null);

        Assert.That(derivedClass.DerivedClasses.Count, Is.EqualTo(0));
        Assert.That(derivedClass.BaseClass, Is.SameAs(baseClass));
      }
    }

    [Test]
    public void Import_1ClassAnd2StateProperties ()
    {
      string metadataXml =
          @"
          <securityMetadata xmlns=""http://www.re-motion.org/Security/Metadata/1.0"">
            <classes>
              <class id=""00000000-0000-0000-0001-000000000000"" name=""Remotion.Security.UnitTests.TestDomain.File"">
                <stateProperties>
                  <statePropertyRef>00000000-0000-0000-0001-000000000001</statePropertyRef>
                </stateProperties>
              </class>
            </classes>

            <stateProperties>
              <stateProperty id=""00000000-0000-0000-0002-000000000001"" name=""State"">
                <state name=""New"" value=""0"" />
                <state name=""Normal"" value=""1"" />
                <state name=""Archived"" value=""2"" />
              </stateProperty>

              <stateProperty id=""00000000-0000-0000-0001-000000000001"" name=""Confidentiality"">
                <state name=""Normal"" value=""0"" />
                <state name=""Confidential"" value=""1"" />
                <state name=""Private"" value=""2"" />
              </stateProperty>
            </stateProperties>
            <accessTypes />
            <abstractRoles />
          </securityMetadata>";

      _importer.Import(GetXmlDocument(metadataXml));

      using (_testHelper.Transaction.EnterNonDiscardingScope())
      {
        Assert.That(_importer.Classes.Count, Is.EqualTo(1), "Class count");
        Assert.That(_importer.StateProperties.Count, Is.EqualTo(2), "State property count");
        Assert.That(_importer.AbstractRoles.Count, Is.EqualTo(0), "Abstract role count");
        Assert.That(_importer.AccessTypes.Count, Is.EqualTo(0), "Access type count");

        SecurableClassDefinition classDefinition = _importer.Classes[new Guid("00000000-0000-0000-0001-000000000000")];
        StatePropertyDefinition property1 = _importer.StateProperties[new Guid("00000000-0000-0000-0002-000000000001")];
        StatePropertyDefinition property2 = _importer.StateProperties[new Guid("00000000-0000-0000-0001-000000000001")];

        Assert.That(classDefinition.StateProperties.Count, Is.EqualTo(1), "State property count");
        Assert.That(classDefinition.StateProperties[0], Is.SameAs(property2));
      }
    }

    [Test]
    public void Import_1ClassAnd8AccessTypes ()
    {
      string metadataXml =
          @"
          <securityMetadata xmlns=""http://www.re-motion.org/Security/Metadata/1.0"">
            <classes>
              <class id=""00000000-0000-0000-0001-000000000000"" name=""Remotion.Security.UnitTests.TestDomain.File"">
                <accessTypes>
                  <accessTypeRef>62dfcd92-a480-4d57-95f1-28c0f5996b3a</accessTypeRef>
                </accessTypes>
              </class>
            </classes>

            <stateProperties />

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
            
            <abstractRoles />
          </securityMetadata>";

      _importer.Import(GetXmlDocument(metadataXml));

      using (_testHelper.Transaction.EnterNonDiscardingScope())
      {
        Assert.That(_importer.Classes.Count, Is.EqualTo(1), "Class count");
        Assert.That(_importer.StateProperties.Count, Is.EqualTo(0), "State property count");
        Assert.That(_importer.AbstractRoles.Count, Is.EqualTo(0), "Abstract role count");
        Assert.That(_importer.AccessTypes.Count, Is.EqualTo(8), "Access type count");

        SecurableClassDefinition classDefinition = _importer.Classes[new Guid("00000000-0000-0000-0001-000000000000")];
        AccessTypeDefinition accessType = _importer.AccessTypes[new Guid("62dfcd92-a480-4d57-95f1-28c0f5996b3a")];

        Assert.That(classDefinition.AccessTypes.Count, Is.EqualTo(1), "Access type count");
        Assert.That(classDefinition.AccessTypes[0], Is.SameAs(accessType));
      }
    }

    [Test]
    public void Import_ABaseClassAndADerivedClassWith2StatePropertiesAnd8AccessTypesAnd3AbstractRoles ()
    {
      string metadataXml =
          @"
          <securityMetadata xmlns=""http://www.re-motion.org/Security/Metadata/1.0"">
            <classes>
              <class id=""00000000-0000-0000-0001-000000000000"" name=""Remotion.Security.UnitTests.TestDomain.File"">
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

              <class id=""00000000-0000-0000-0002-000000000000"" name=""Remotion.Security.UnitTests.TestDomain.PaperFile"" base=""00000000-0000-0000-0001-000000000000"">
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
                <state name=""New"" value=""0"" />
                <state name=""Normal"" value=""1"" />
                <state name=""Archived"" value=""2"" />
              </stateProperty>

              <stateProperty id=""00000000-0000-0000-0001-000000000001"" name=""Confidentiality"">
                <state name=""Normal"" value=""0"" />
                <state name=""Confidential"" value=""1"" />
                <state name=""Private"" value=""2"" />
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

      _importer.Import(GetXmlDocument(metadataXml));

      using (_testHelper.Transaction.EnterNonDiscardingScope())
      {
        Assert.That(_importer.Classes.Count, Is.EqualTo(2), "Class count");
        Assert.That(_importer.StateProperties.Count, Is.EqualTo(2), "State property count");
        Assert.That(_importer.AbstractRoles.Count, Is.EqualTo(3), "Abstract role count");
        Assert.That(_importer.AccessTypes.Count, Is.EqualTo(8), "Access type count");
      }
    }

    [Test]
    public void Import_InvalidXml ()
    {
      string metadataXml = @"
          <securityMetadata xmlns=""http://www.re-motion.org/Security/Metadata/1.0"">
            <classes />
          </securityMetadata>";
      Assert.That(
          () => _importer.Import(GetXmlDocument(metadataXml)),
          Throws.InstanceOf<XmlSchemaValidationException>());
    }

    [Test]
    public void Import_MissingBaseClass ()
    {
      string metadataXml = @"
          <securityMetadata xmlns=""http://www.re-motion.org/Security/Metadata/1.0"">
            <classes>
              <class id=""00000000-0000-0000-0002-000000000000"" name=""Remotion.Security.UnitTests.TestDomain.Directory"" base=""00000000-0000-0000-0001-000000000000"" />
            </classes>
            <stateProperties />
            <accessTypes />
            <abstractRoles />
          </securityMetadata>
          ";
      Assert.That(
          () => _importer.Import(GetXmlDocument(metadataXml)),
          Throws.InstanceOf<ImportException>()
              .With.Message.EqualTo(
                  "The base class '00000000-0000-0000-0001-000000000000' referenced by the class '00000000-0000-0000-0002-000000000000' could not be found."));
    }

    [Test]
    public void Import_MissingStateProperty ()
    {
      string metadataXml = @"
          <securityMetadata xmlns=""http://www.re-motion.org/Security/Metadata/1.0"">
            <classes>
              <class id=""00000000-0000-0000-0001-000000000000"" name=""Remotion.Security.UnitTests.TestDomain.File"">
                <stateProperties>
                  <statePropertyRef>00000000-0000-0000-0001-000000000001</statePropertyRef>
                </stateProperties>
              </class>
            </classes>

            <stateProperties>
              <stateProperty id=""00000000-0000-0000-0002-000000000001"" name=""State"">
                <state name=""New"" value=""0"" />
                <state name=""Normal"" value=""1"" />
                <state name=""Archived"" value=""2"" />
              </stateProperty>
            </stateProperties>
            <accessTypes />
            <abstractRoles />
          </securityMetadata>
          ";
      Assert.That(
          () => _importer.Import(GetXmlDocument(metadataXml)),
          Throws.InstanceOf<ImportException>()
              .With.Message.EqualTo(
                  "The state property '00000000-0000-0000-0001-000000000001' referenced by the class '00000000-0000-0000-0001-000000000000' could not be found."));
    }

    [Test]
    public void Import_MissingAccessType ()
    {
      string metadataXml = @"
          <securityMetadata xmlns=""http://www.re-motion.org/Security/Metadata/1.0"">
            <classes>
              <class id=""00000000-0000-0000-0001-000000000000"" name=""Remotion.Security.UnitTests.TestDomain.File"">
                <accessTypes>
                  <accessTypeRef>62dfcd92-a480-4d57-95f1-28c0f5996b3a</accessTypeRef>
                </accessTypes>
              </class>
            </classes>

            <stateProperties />

            <accessTypes>
              <accessType id=""1d6d25bc-4e85-43ab-a42d-fb5a829c30d5"" name=""Create|Remotion.Security.GeneralAccessTypes, Remotion.Security"" value=""0"" />
              <accessType id=""11186122-6de0-4194-b434-9979230c41fd"" name=""Edit|Remotion.Security.GeneralAccessTypes, Remotion.Security"" value=""2"" />
              <accessType id=""305fbb40-75c8-423a-84b2-b26ea9e7cae7"" name=""Delete|Remotion.Security.GeneralAccessTypes, Remotion.Security"" value=""3"" />
              <accessType id=""67cea479-0be7-4e2f-b2e0-bb1fcc9ea1d6"" name=""Search|Remotion.Security.GeneralAccessTypes, Remotion.Security"" value=""4"" />
              <accessType id=""203b7478-96f1-4bf1-b4ea-5bdd1206252c"" name=""Find|Remotion.Security.GeneralAccessTypes, Remotion.Security"" value=""5"" />
              <accessType id=""00000002-0001-0000-0000-000000000000"" name=""Journalize|Remotion.Security.UnitTests.TestDomain.DomainAccessTypes, Remotion.Security.UnitTests.TestDomain"" value=""0"" />
              <accessType id=""00000002-0002-0000-0000-000000000000"" name=""Archive|Remotion.Security.UnitTests.TestDomain.DomainAccessTypes, Remotion.Security.UnitTests.TestDomain"" value=""1"" />
            </accessTypes>
            
            <abstractRoles />
          </securityMetadata>
          ";
      Assert.That(
          () => _importer.Import(GetXmlDocument(metadataXml)),
          Throws.InstanceOf<ImportException>()
              .With.Message.EqualTo(
                  "The access type '62dfcd92-a480-4d57-95f1-28c0f5996b3a' referenced by the class '00000000-0000-0000-0001-000000000000' could not be found."));
    }

    private XmlDocument GetXmlDocument (string metadataXml)
    {
      XmlDocument metadataXmlDocument = new XmlDocument();
      metadataXmlDocument.LoadXml(metadataXml);

      return metadataXmlDocument;
    }
  }
}
