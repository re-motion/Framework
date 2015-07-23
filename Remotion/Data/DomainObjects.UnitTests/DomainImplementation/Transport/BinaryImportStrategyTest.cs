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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation.Transport;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DomainImplementation.Transport
{
  [TestFixture]
  public class BinaryImportStrategyTest : ClientTransactionBaseTest
  {
    [Test]
    public void Import_DeserializesData ()
    {
      var orderNumberPropertyDefinition = GetPropertyDefinition (typeof (Order), "OrderNumber");

      DataContainer expectedContainer1 = DomainObjectIDs.Order1.GetObject<Order> ().InternalDataContainer;
      DataContainer expectedContainer2 = DomainObjectIDs.Order3.GetObject<Order> ().InternalDataContainer;

      byte[] data = Serialize(expectedContainer1, expectedContainer2);
      TransportItem[] items = Import (data);
      Assert.That (items.Length, Is.EqualTo (2));

      Assert.That (items[0].ID, Is.EqualTo (expectedContainer1.ID));
      Assert.That (items[0].Properties[orderNumberPropertyDefinition.PropertyName], Is.EqualTo (expectedContainer1.GetValue (orderNumberPropertyDefinition)));

      Assert.That (items[1].ID, Is.EqualTo (expectedContainer2.ID));
      Assert.That (items[1].Properties[orderNumberPropertyDefinition.PropertyName], Is.EqualTo (expectedContainer2.GetValue (orderNumberPropertyDefinition)));
    }

    [Test]
    [ExpectedException (typeof (TransportationException), ExpectedMessage = "Invalid data specified: Attempting to deserialize an empty stream.")]
    public void Import_ThrowsOnInvalidFormat ()
    {
      var data = new byte[0];
      Import (data);
    }

    [Test]
    [ExpectedException (typeof (TransportationException), ExpectedMessage = "Invalid data specified: Unable to cast object of type 'System.String' "
        + "to type 'System.Collections.Generic.KeyValuePair`2[System.String,System.Collections.Generic.Dictionary`2[System.String,System.Object]][]'.")]
    public void Import_ThrowsOnInvalidSerializedData ()
    {
      byte[] data = Serializer.Serialize ("string");
      Import (data);
    }

    [Test]
    public void Import_ExportStrategy_IntegrationTest ()
    {
      var item1 = new TransportItem (DomainObjectIDs.Order1);
      item1.Properties.Add ("Foo", 12);
      var item2 = new TransportItem (DomainObjectIDs.Order3);
      item2.Properties.Add ("Bar", "42");

      byte[] package = BinaryExportStrategyTest.Export (item1, item2);
      TransportItem[] importedItems = Import (package);

      Assert.That (importedItems.Length, Is.EqualTo (2));
      Assert.That (importedItems[0].ID, Is.EqualTo (item1.ID));
      Assert.That (importedItems[0].Properties.Count, Is.EqualTo (1));
      Assert.That (importedItems[0].Properties["Foo"], Is.EqualTo (item1.Properties["Foo"]));

      Assert.That (importedItems[1].ID, Is.EqualTo (item2.ID));
      Assert.That (importedItems[1].Properties.Count, Is.EqualTo (1));
      Assert.That (importedItems[1].Properties["Bar"], Is.EqualTo (item2.Properties["Bar"]));
    }

    private byte[] Serialize (params DataContainer[] containers)
    {
      TransportItem[] items = TransportItem.PackageDataContainers (containers).ToArray ();
      KeyValuePair<string, Dictionary<string, object>>[] versionIndependentItems =
          Array.ConvertAll (
              items,
              item => new KeyValuePair<string, Dictionary<string, object>> (item.ID.ToString(), item.Properties));
      return Serializer.Serialize (versionIndependentItems);
    }

    public static TransportItem[] Import (byte[] data)
    {
      using (var stream = new MemoryStream (data))
      {
        return BinaryImportStrategy.Instance.Import (stream).ToArray();
      }
    }
  }
}
