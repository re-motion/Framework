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
using System.Data.SqlClient;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests
{
  [TestFixture]
  public class SqlProviderLoadDataContainerTest : SqlProviderBaseTest
  {
    [Test]
    public void LoadDataContainerWithGuidID ()
    {
      ObjectID id = new ObjectID(typeof(ClassWithGuidKey), new Guid("{7D1F5F2E-D111-433b-A675-300B55DC4756}"));

      DataContainer container = Provider.LoadDataContainer(id).LocatedObject;

      Assert.That(container, Is.Not.Null);
      Assert.That(id, Is.EqualTo(container.ID));
    }

    [Test]
    public void LoadDataContainerWithInvalidIDType ()
    {
      ObjectID id = new ObjectID(typeof(ClassWithKeyOfInvalidType), new Guid("{7D1F5F2E-D111-433b-A675-300B55DC4756}"));

      var rdbmsProviderException = Assert.Throws<RdbmsProviderException>(
          () =>
          {
            DataContainer container = Provider.LoadDataContainer(id).LocatedObject;
          });
      Assert.That(rdbmsProviderException.Message, Is.EqualTo("Error while executing SQL command: Operand type clash: uniqueidentifier is incompatible with datetime"));
      Assert.That(rdbmsProviderException.InnerException.GetType(), Is.EqualTo(typeof(SqlException)));
    }

    [Test]
    public void LoadDataContainerWithoutIDColumn ()
    {
      ObjectID id = new ObjectID(typeof(ClassWithoutIDColumn), new Guid("{7D1F5F2E-D111-433b-A675-300B55DC4756}"));

      var rdbmsProviderException = Assert.Throws<RdbmsProviderException>(
          () =>
          {
            DataContainer container = Provider.LoadDataContainer(id).LocatedObject;
          });
      Assert.That(rdbmsProviderException.Message, Is.EqualTo("Error while executing SQL command: Invalid column name 'ID'.\r\nInvalid column name 'ID'."));
      Assert.That(rdbmsProviderException.InnerException.GetType(), Is.EqualTo(typeof(SqlException)));
    }

    [Test]
    public void LoadDataContainerWithoutClassIDColumn ()
    {
      ObjectID id = new ObjectID(typeof(ClassWithoutClassIDColumn), new Guid("{DDD02092-355B-4820-90B6-7F1540C0547E}"));
      Assert.That(
          () => Provider.LoadDataContainer(id),
          Throws.InstanceOf<RdbmsProviderException>()
              .With.Message.EqualTo("Error while executing SQL command: Invalid column name 'ClassID'."));
    }

    [Test]
    public void LoadDataContainerWithoutTimestampColumn ()
    {
      ObjectID id = new ObjectID(typeof(ClassWithoutTimestampColumn), new Guid("{027DCBD7-ED68-461d-AE80-B8E145A7B816}"));
      Assert.That(
          () => Provider.LoadDataContainer(id),
          Throws.InstanceOf<RdbmsProviderException>()
              .With.Message.EqualTo("Error while executing SQL command: Invalid column name 'Timestamp'."));
    }

    [Test]
    public void LoadDataContainerWithNonExistingClassID ()
    {
      ObjectID id = new ObjectID(typeof(ClassWithGuidKey), new Guid("{C9F16F93-CF42-4357-B87B-7493882AAEAF}"));
      Assert.That(
          () => Provider.LoadDataContainer(id),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "Mapping does not contain class 'NonExistingClassID'."));
    }

    [Test]
    public void LoadDataContainerWithClassIDFromOtherClass ()
    {
      ObjectID id = new ObjectID(typeof(ClassWithGuidKey), new Guid("{895853EB-06CD-4291-B467-160560AE8EC1}"));
      Assert.That(
          () => Provider.LoadDataContainer(id),
          Throws.InstanceOf<RdbmsProviderException>()
              .With.Message.EqualTo(
                  "Error while reading property 'Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber' of object "
                  + "'Order|895853eb-06cd-4291-b467-160560ae8ec1|System.Guid': The column 'OrderNo' is not included in the query result and is not expected "
                  + "for this operation. The included and expected columns are: ID, ClassID, Timestamp."));
    }

    [Test]
    public void LoadDataContainerByNonExistingID ()
    {
      ObjectID id = new ObjectID(typeof(ClassWithAllDataTypes), new Guid("{E067A627-BA3F-4ee5-8B61-1F46DC28DFC3}"));

      Assert.That(Provider.LoadDataContainer(id).LocatedObject, Is.Null);
    }

    [Test]
    public void LoadDataContainerByID ()
    {
      ObjectID id = new ObjectID(typeof(ClassWithAllDataTypes), new Guid("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

      DataContainer actualContainer = Provider.LoadDataContainer(id).LocatedObject;

      DataContainer expectedContainer = TestDataContainerObjectMother.CreateClassWithAllDataTypes1DataContainer();

      DataContainerChecker checker = new DataContainerChecker();
      checker.Check(expectedContainer, actualContainer);
    }

    [Test]
    public void LoadDerivedDataContainerByID ()
    {
      DataContainer actualContainer = Provider.LoadDataContainer(DomainObjectIDs.Partner1).LocatedObject;
      DataContainer expectedContainer = TestDataContainerObjectMother.CreatePartner1DataContainer();

      DataContainerChecker checker = new DataContainerChecker();
      checker.Check(expectedContainer, actualContainer);
    }

    [Test]
    public void LoadTwiceDerivedDataContainerByID ()
    {
      DataContainer actualContainer = Provider.LoadDataContainer(DomainObjectIDs.Distributor2).LocatedObject;
      DataContainer expectedContainer = TestDataContainerObjectMother.CreateDistributor2DataContainer();

      DataContainerChecker checker = new DataContainerChecker();
      checker.Check(expectedContainer, actualContainer);
    }

    [Test]
    public void LoadDataContainerWithNullForeignKey ()
    {
      ObjectID id = new ObjectID(typeof(ClassWithValidRelations), new Guid("{6BE4FA61-E050-469c-9DBA-B47FFBB0F8AD}"));

      DataContainer container = Provider.LoadDataContainer(id).LocatedObject;

      Assert.IsNull(container.GetValue(GetPropertyDefinition(typeof(ClassWithValidRelations), "ClassWithGuidKeyOptional")), "PropertyValue.Value");
    }

    [Test]
    public void LoadDataContainerWithRelation ()
    {
      DataContainer orderTicketContainer = Provider.LoadDataContainer(DomainObjectIDs.OrderTicket1).LocatedObject;
      var propertyDefinition = GetPropertyDefinition(typeof(OrderTicket), "Order");
      Assert.That(orderTicketContainer.GetValue(propertyDefinition), Is.EqualTo(DomainObjectIDs.Order1));
    }

    [Test]
    public void LoadDataContainerWithRelationAndInheritance ()
    {
      DataContainer ceoContainer = Provider.LoadDataContainer(DomainObjectIDs.Ceo7).LocatedObject;
      var propertyDefinition = GetPropertyDefinition(typeof(Ceo), "Company");
      Assert.That(ceoContainer.GetValue(propertyDefinition), Is.EqualTo(DomainObjectIDs.Partner2));
    }

    [Test]
    public void LoadDataContainerWithoutRelatedIDColumn ()
    {
      ObjectID id = new ObjectID(typeof(ClassWithoutRelatedClassIDColumn), new Guid("{CD3BE83E-FBB7-4251-AAE4-B216485C5638}"));
      Assert.That(
          () => Provider.LoadDataContainer(id),
          Throws.InstanceOf<RdbmsProviderException>()
              .With.Message.EqualTo("Error while executing SQL command: Invalid column name 'DistributorIDClassID'."));
    }

    [Test]
    public void LoadDataContainerWithoutRelatedIDColumnAndDerivation ()
    {
      ObjectID id = new ObjectID(typeof(ClassWithoutRelatedClassIDColumnAndDerivation),
                           new Guid("{4821D7F7-B586-4435-B572-8A96A44B113E}"));
      Assert.That(
          () => Provider.LoadDataContainer(id),
          Throws.InstanceOf<RdbmsProviderException>()
              .With.Message.EqualTo("Error while executing SQL command: Invalid column name 'CompanyIDClassID'."));
    }

    [Test]
    public void LoadDataContainerWithObjectIDWithWrongStorageProviderID ()
    {
      ObjectID invalidID = new ObjectID(DomainObjectIDs.Official1.ClassID, (int)DomainObjectIDs.Official1.Value);
      Assert.That(
          () => Provider.LoadDataContainer(invalidID),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The StorageProvider 'UnitTestStorageProviderStub' of the provided ObjectID 'Official|1|System.Int32' does not match with this StorageProvider 'TestDomain'.",
                  "id"));
    }

    [Test]
    public void LoadDataContainerWithRelatedClassIDColumnAndNoInheritance ()
    {
      ObjectID id = new ObjectID("ClassWithRelatedClassIDColumnAndNoInheritance", new Guid("{CB72715D-F419-4ab9-8D49-ABCBA4E9EDB4}"));

      // The storage provider does not check whether a superfluous ClassID column is present. Therefore, the next line succeeds.
      Provider.LoadDataContainer(id);
    }

    [Test]
    public void LoadDataContainer_WithInvalidClassID ()
    {
      ObjectID id = new ObjectID("Distributor", (Guid)DomainObjectIDs.Partner1.Value);
      Assert.That(
          () => Provider.LoadDataContainer(id),
          Throws.InstanceOf<PersistenceException>()
              .With.Message.EqualTo(
                  "The ObjectID of the loaded DataContainer 'Partner|5587a9c0-be53-477d-8c0a-4803c7fae1a9|System.Guid' and the expected ObjectID "
                  + "'Distributor|5587a9c0-be53-477d-8c0a-4803c7fae1a9|System.Guid' differ."));
    }
  }
}
