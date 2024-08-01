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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation.Transport;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainImplementation.Transport
{
  [TestFixture]
  public class TransportItemTest : ClientTransactionBaseTest
  {
    [Test]
    public void Initialization ()
    {
      TransportItem item = new TransportItem(DomainObjectIDs.Order1);
      Assert.That(item.ID, Is.EqualTo(DomainObjectIDs.Order1));
    }

    [Test]
    public void PackageDataContainer ()
    {
      DataContainer container = DomainObjectIDs.Computer1.GetObject<Computer>().InternalDataContainer;
      TransportItem item = TransportItem.PackageDataContainer(container);

      CheckEqualData(container, item);
    }

    [Test]
    public void PackageDataContainers ()
    {
      DataContainer container1 = DomainObjectIDs.Computer1.GetObject<Computer>().InternalDataContainer;
      DataContainer container2 = DomainObjectIDs.Computer1.GetObject<Computer>().InternalDataContainer;
      TransportItem[] items = TransportItem.PackageDataContainers(new DataContainer[] { container1, container2 }).ToArray();

      CheckEqualData(container1, items[0]);
      CheckEqualData(container2, items[1]);
    }

    public static void CheckEqualData (DataContainer expectedData, TransportItem item)
    {
      Assert.That(item.ID, Is.EqualTo(expectedData.ID));
      foreach (var propertyDefinition in expectedData.ClassDefinition.GetPropertyDefinitions())
      {
        Assert.That(item.Properties.ContainsKey(propertyDefinition.PropertyName), Is.True, $"Property '{propertyDefinition.PropertyName}'");
        Assert.That(item.Properties[propertyDefinition.PropertyName], Is.EqualTo(expectedData.GetValue(propertyDefinition)), $"Property '{propertyDefinition.PropertyName}'");
      }
    }

  }
}
