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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests
{
  [TestFixture]
  public class SqlProviderLoadDataContainersTest : SqlProviderBaseTest
  {
    [Test]
    public void LoadDataContainers_ByNonExistingID ()
    {
      var id = new ObjectID("ClassWithAllDataTypes", new Guid ("{E067A627-BA3F-4ee5-8B61-1F46DC28DFC3}"));

      var result = Provider.LoadDataContainers (new[] { id }).ToList();

      Assert.That (result.Count, Is.EqualTo(1));
      Assert.That (result[0].LocatedObject, Is.Null);
      Assert.That (result[0].ObjectID, Is.SameAs(id));
    }

    [Test]
    public void LoadDataContainers_ByID ()
    {
      var id = new ObjectID("ClassWithAllDataTypes", new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

      var actualContainer = Provider.LoadDataContainers (new[] { id }).ToArray()[0].LocatedObject;

      var expectedContainer = TestDataContainerObjectMother.CreateClassWithAllDataTypes1DataContainer ();

      var checker = new DataContainerChecker ();
      checker.Check (expectedContainer, actualContainer);
    }

    [Test]
    public void LoadDataContainers_Multiple ()
    {
      var ids = new[] { DomainObjectIDs.Order3, DomainObjectIDs.OrderItem1, DomainObjectIDs.Order1, DomainObjectIDs.OrderItem2 };

      var containers = Provider.LoadDataContainers (ids).ToArray ();

      Assert.That (containers, Is.Not.Null);
      Assert.That (containers[0].LocatedObject.ID, Is.EqualTo (ids[0]));
      Assert.That (containers[1].LocatedObject.ID, Is.EqualTo (ids[1]));
      Assert.That (containers[2].LocatedObject.ID, Is.EqualTo (ids[2]));
      Assert.That (containers[3].LocatedObject.ID, Is.EqualTo (ids[3]));
    }

    [Test]
    public void LoadDataContainers_Empty ()
    {
      var result = Provider.LoadDataContainers (new ObjectID[0]).ToList();

      Assert.That (result, Is.Empty);
    }

    [Test]
    [ExpectedException (typeof (PersistenceException), ExpectedMessage = 
        "The ObjectID of one or more loaded DataContainers does not match the expected ObjectIDs:\r\n"
        + "Loaded DataContainer ID: Partner|5587a9c0-be53-477d-8c0a-4803c7fae1a9|System.Guid, "
        + "expected ObjectID(s): Distributor|5587a9c0-be53-477d-8c0a-4803c7fae1a9|System.Guid\r\n"
        + "Loaded DataContainer ID: Partner|b403e58e-9fa5-47ed-883c-73420d64deb3|System.Guid, "
        + "expected ObjectID(s): Distributor|b403e58e-9fa5-47ed-883c-73420d64deb3|System.Guid\r\n"
        + "Loaded DataContainer ID: Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid, "
        + "expected ObjectID(s): Distributor|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid")]
    public void LoadDataContainers_WithInvalidClassID ()
    {
      ObjectID id1 = new ObjectID("Distributor", (Guid) DomainObjectIDs.Partner1.Value);
      ObjectID id2 = new ObjectID("Distributor", (Guid) DomainObjectIDs.Partner2.Value);
      ObjectID id3 = new ObjectID("Distributor", (Guid) DomainObjectIDs.Customer1.Value);
      Provider.LoadDataContainers (new[] { id1, id2, id3 }).ToList();
    }
  }
}
