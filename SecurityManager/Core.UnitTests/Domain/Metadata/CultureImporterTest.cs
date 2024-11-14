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
using System.Xml;
using System.Xml.Schema;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.UnitTests.Domain.Metadata
{
  [TestFixture]
  public class CultureImporterTest : DomainTest
  {
    private CultureImporter _importer;
    private ClientTransaction _transaction;

    public override void OneTimeSetUp ()
    {
      base.OneTimeSetUp();

      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      dbFixtures.CreateAndCommitSecurableClassDefinitionWithStates(ClientTransaction.CreateRootTransaction());
    }

    public override void SetUp ()
    {
      base.SetUp();

      _transaction = ClientTransaction.CreateRootTransaction();
      _importer = new CultureImporter(_transaction);
    }

    [Test]
    public void Import_EmptyCultureFile ()
    {
      string cultureXml =
          @"
          <localizedNames xmlns=""http://www.re-motion.org/Security/Metadata/Localization/1.0"" culture=""de"" />
          ";

      _importer.Import(GetXmlDocument(cultureXml));

      using (_transaction.EnterNonDiscardingScope())
      {
        Assert.That(_importer.LocalizedNames.Count, Is.EqualTo(0), "LocalizedNames count");
        Assert.That(_importer.Cultures, Is.Not.Null, "Cultures");
        Assert.That(_importer.Cultures.Count, Is.EqualTo(1));
        Assert.That(_importer.Cultures[0].CultureName, Is.EqualTo("de"));
      }
    }

    [Test]
    public void Import_OneLocalizedName ()
    {
      string cultureXml = @"
          <localizedNames xmlns=""http://www.re-motion.org/Security/Metadata/Localization/1.0"" culture=""de"">
            <localizedName ref=""b8621bc9-9ab3-4524-b1e4-582657d6b420"" comment=""Clerk|Remotion.Security.UnitTests.TestDomain.DomainAbstractRoles, Remotion.Security.UnitTests.TestDomain"">
              Beamter
            </localizedName>
          </localizedNames>
          ";

      _importer.Import(GetXmlDocument(cultureXml));

      using (_transaction.EnterNonDiscardingScope())
      {
        Assert.That(_importer.LocalizedNames.Count, Is.EqualTo(1), "LocalizedNames count");
        Assert.That(_importer.LocalizedNames[0].Text, Is.EqualTo("Beamter"));
        Assert.That(_importer.LocalizedNames[0].MetadataObject.MetadataItemID, Is.EqualTo(new Guid("b8621bc9-9ab3-4524-b1e4-582657d6b420")));
      }
    }

    [Test]
    public void Import_NotExistingMetadataObject ()
    {
      string cultureXml = @"
          <localizedNames xmlns=""http://www.re-motion.org/Security/Metadata/Localization/1.0"" culture=""de"">
            <localizedName ref=""ad1efa4c-cf5d-46b0-b775-d4e45f2dce7c"" comment=""Clerk|Remotion.Security.UnitTests.TestDomain.DomainAbstractRoles, Remotion.Security.UnitTests.TestDomain"">
              Beamter
            </localizedName>
          </localizedNames>
          ";
      Assert.That(
          () => _importer.Import(GetXmlDocument(cultureXml)),
          Throws.InstanceOf<ImportException>()
              .With.Message.EqualTo(
                  "The metadata object with the ID 'ad1efa4c-cf5d-46b0-b775-d4e45f2dce7c' "
                  + "('Clerk|Remotion.Security.UnitTests.TestDomain.DomainAbstractRoles, Remotion.Security.UnitTests.TestDomain') "
                  + "could not be found."));
    }

    [Test]
    public void Import_NotExistingMetadataObjectWithoutComment ()
    {
      string cultureXml = @"
          <localizedNames xmlns=""http://www.re-motion.org/Security/Metadata/Localization/1.0"" culture=""de"">
            <localizedName ref=""ad1efa4c-cf5d-46b0-b775-d4e45f2dce7c"">
              Beamter
            </localizedName>
          </localizedNames>
          ";
      Assert.That(
          () => _importer.Import(GetXmlDocument(cultureXml)),
          Throws.InstanceOf<ImportException>()
              .With.Message.EqualTo(
                  "The metadata object with the ID 'ad1efa4c-cf5d-46b0-b775-d4e45f2dce7c' could not be found."));
    }

    [Test]
    public void Import_MultipleLocalizedNames ()
    {
      string cultureXml =
          @"
          <localizedNames xmlns=""http://www.re-motion.org/Security/Metadata/Localization/1.0"" culture=""de"">
            <localizedName ref=""b8621bc9-9ab3-4524-b1e4-582657d6b420"" comment=""Clerk|Remotion.Security.UnitTests.TestDomain.DomainAbstractRoles, Remotion.Security.UnitTests.TestDomain"">
              Beamter
            </localizedName>
            <localizedName ref=""93969f13-65d7-49f4-a456-a1686a4de3de"" comment=""Confidentiality"">
              Vertraulichkeit
            </localizedName>
            <localizedName ref=""93969f13-65d7-49f4-a456-a1686a4de3de|0"" comment=""Confidentiality|Public"">
              Öffentlich
            </localizedName>
          </localizedNames>
          ";

      _importer.Import(GetXmlDocument(cultureXml));

      using (_transaction.EnterNonDiscardingScope())
      {
        Assert.That(_importer.LocalizedNames.Count, Is.EqualTo(3), "LocalizedNames count");
        Assert.That(_importer.LocalizedNames[0].Text, Is.EqualTo("Beamter"));
        Assert.That(_importer.LocalizedNames[0].MetadataObject.MetadataItemID, Is.EqualTo(new Guid("b8621bc9-9ab3-4524-b1e4-582657d6b420")));
        Assert.That(_importer.LocalizedNames[1].Text, Is.EqualTo("Vertraulichkeit"));
        Assert.That(_importer.LocalizedNames[1].MetadataObject.MetadataItemID, Is.EqualTo(new Guid("93969f13-65d7-49f4-a456-a1686a4de3de")));
        Assert.That(_importer.LocalizedNames[2].Text, Is.EqualTo("Öffentlich"));
      }
    }

    [Test]
    public void Import_InvalidXml ()
    {
      string cultureXml = @"
          <localizedNames xmlns=""http://www.re-motion.org/Security/Metadata/Localization/1.0"" culture=""de"">
            <localizedName rf=""b8621bc9-9ab3-4524-b1e4-582657d6b420"" comment=""Clerk|Remotion.Security.UnitTests.TestDomain.DomainAbstractRoles, Remotion.Security.UnitTests.TestDomain"">
              Beamter
            </localizedName>,
            <localizedName ref=""93969f13-65d7-49f4-a456-a1686a4de3de"" comment=""Confidentiality"">
              Vertraulichkeit
            </localizedName>
          </localizedNames>
          ";
      Assert.That(
          () => _importer.Import(GetXmlDocument(cultureXml)),
          Throws.InstanceOf<XmlSchemaValidationException>());
    }

    [Test]
    public void Import_LocalizedNamesTwice ()
    {
      string cultureXml = @"
          <localizedNames xmlns=""http://www.re-motion.org/Security/Metadata/Localization/1.0"" culture=""de"">
            <localizedName ref=""b8621bc9-9ab3-4524-b1e4-582657d6b420"" comment=""Clerk|Remotion.Security.UnitTests.TestDomain.DomainAbstractRoles, Remotion.Security.UnitTests.TestDomain"">
              Beamte
            </localizedName>
            <localizedName ref=""93969f13-65d7-49f4-a456-a1686a4de3de"" comment=""Confidentiality"">
              Vertraulichkeit
            </localizedName>
          </localizedNames>
          ";

      ObjectID clerkNameID;
      ObjectID confidentialityNameID;
      _importer.Import(GetXmlDocument(cultureXml));
      using (_transaction.EnterNonDiscardingScope())
      {
        clerkNameID = _importer.LocalizedNames[0].ID;
        confidentialityNameID = _importer.LocalizedNames[1].ID;
      }

      _importer = new CultureImporter(_transaction);

      cultureXml = @"
          <localizedNames xmlns=""http://www.re-motion.org/Security/Metadata/Localization/1.0"" culture=""de"">
            <localizedName ref=""b8621bc9-9ab3-4524-b1e4-582657d6b420"" comment=""Clerk|Remotion.Security.UnitTests.TestDomain.DomainAbstractRoles, Remotion.Security.UnitTests.TestDomain"">
              Beamter
            </localizedName>
            <localizedName ref=""93969f13-65d7-49f4-a456-a1686a4de3de"" comment=""Confidentiality"">
              Vertraulichkeit
            </localizedName>
            <localizedName ref=""93969f13-65d7-49f4-a456-a1686a4de3de|0"" comment=""Confidentiality|Public"">
              Öffentlich
            </localizedName>
          </localizedNames>
          ";

      _importer.Import(GetXmlDocument(cultureXml));

      using (_transaction.EnterNonDiscardingScope())
      {
        Assert.That(_importer.LocalizedNames.Count, Is.EqualTo(3), "LocalizedNames count");
        Assert.That(_importer.LocalizedNames[0].Text, Is.EqualTo("Beamter"));
        Assert.That(_importer.LocalizedNames[0].ID, Is.EqualTo(clerkNameID));
        Assert.That(_importer.LocalizedNames[0].MetadataObject.MetadataItemID, Is.EqualTo(new Guid("b8621bc9-9ab3-4524-b1e4-582657d6b420")));
        Assert.That(_importer.LocalizedNames[1].Text, Is.EqualTo("Vertraulichkeit"));
        Assert.That(_importer.LocalizedNames[1].ID, Is.EqualTo(confidentialityNameID));
        Assert.That(_importer.LocalizedNames[1].MetadataObject.MetadataItemID, Is.EqualTo(new Guid("93969f13-65d7-49f4-a456-a1686a4de3de")));
        Assert.That(_importer.LocalizedNames[2].Text, Is.EqualTo("Öffentlich"));
      }
    }

    private XmlDocument GetXmlDocument (string xml)
    {
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.LoadXml(xml);

      return xmlDocument;
    }
  }
}
