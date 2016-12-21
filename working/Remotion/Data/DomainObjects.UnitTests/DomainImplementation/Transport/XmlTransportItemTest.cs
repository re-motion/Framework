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
using System.Text;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation.Transport;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DomainImplementation.Transport
{
  [TestFixture]
  public class XmlTransportItemTest : ClientTransactionBaseTest
  {
    [Test]
    public void Wrap ()
    {
      TransportItem item1 = new TransportItem (DomainObjectIDs.Order1);
      TransportItem item2 = new TransportItem (DomainObjectIDs.Order3);

      XmlTransportItem[] xmlItems = XmlTransportItem.Wrap (new[] { item1, item2 });
      Assert.That (xmlItems.Length, Is.EqualTo (2));
      Assert.That (xmlItems[0].TransportItem, Is.EqualTo (item1));
      Assert.That (xmlItems[1].TransportItem, Is.EqualTo (item2));
    }

    [Test]
    public void Unwrap ()
    {
      TransportItem item1 = new TransportItem (DomainObjectIDs.Order1);
      TransportItem item2 = new TransportItem (DomainObjectIDs.Order3);

      TransportItem[] items = XmlTransportItem.Unwrap (new[] { new XmlTransportItem  (item1), new XmlTransportItem (item2)});
      Assert.That (items.Length, Is.EqualTo (2));
      Assert.That (items[0], Is.EqualTo (item1));
      Assert.That (items[1], Is.EqualTo (item2));
    }

    [Test]
    public void XmlSerialize ()
    {
      DataContainer container = DomainObjectIDs.Computer1.GetObject<Computer> ().InternalDataContainer;
      TransportItem item = TransportItem.PackageDataContainer (container);
      byte[] serializedArray = XmlSerializationHelper.XmlSerialize (new XmlTransportItem (item));
      string serializedString = Encoding.UTF8.GetString (serializedArray);

      Assert.That (serializedString, Is.EqualTo (XmlSerializationStrings.XmlForComputer1));
    }

    [Test]
    public void XmlSerialize_WithNullObjectID ()
    {
      DataContainer container = DomainObjectIDs.Computer4.GetObject<Computer> ().InternalDataContainer;
      TransportItem item = TransportItem.PackageDataContainer (container);
      byte[] serializedArray = XmlSerializationHelper.XmlSerialize (new XmlTransportItem (item));
      string serializedString = Encoding.UTF8.GetString (serializedArray);

      Assert.That (serializedString, Is.EqualTo (XmlSerializationStrings.XmlForComputer4));
    }

    [Test]
    public void XmlSerialize_WithCustomProperty ()
    {
      TransportItem item = new TransportItem (DomainObjectIDs.Computer1);
      item.Properties.Add ("Custom", 5);
      byte[] serializedArray = XmlSerializationHelper.XmlSerialize (new XmlTransportItem (item));
      string serializedString = Encoding.UTF8.GetString (serializedArray);

      Assert.That (serializedString, Is.EqualTo (XmlSerializationStrings.XmlForCustomProperty));
    }

    [Test]
    public void XmlSerialize_WithCustomObjectIDProperty ()
    {
      TransportItem item = new TransportItem (DomainObjectIDs.Computer1);
      item.Properties.Add ("CustomReference", DomainObjectIDs.Order3);
      byte[] serializedArray = XmlSerializationHelper.XmlSerialize (new XmlTransportItem (item));
      string serializedString = Encoding.UTF8.GetString (serializedArray);

      Assert.That (serializedString, Is.EqualTo (XmlSerializationStrings.XmlForCustomObjectIDProperty));
    }

    [Test]
    public void XmlSerialize_WithCustomNullProperty ()
    {
      TransportItem item = new TransportItem (DomainObjectIDs.Computer1);
      item.Properties.Add ("CustomNull", null);
      byte[] serializedArray = XmlSerializationHelper.XmlSerialize (new XmlTransportItem (item));
      string serializedString = Encoding.UTF8.GetString (serializedArray);

      Assert.That (serializedString, Is.EqualTo (XmlSerializationStrings.XmlForCustomNullProperty));
    }

    [Test]
    public void XmlSerialize_WithCustomExtensibleEnumProperty ()
    {
      TransportItem item = new TransportItem (DomainObjectIDs.Computer1);
      item.Properties.Add ("CustomExtensibleEnum", Color.Values.Red());
      byte[] serializedArray = XmlSerializationHelper.XmlSerialize (new XmlTransportItem (item));
      string serializedString = Encoding.UTF8.GetString (serializedArray);

      Assert.That (serializedString, Is.EqualTo (XmlSerializationStrings.XmlForCustomExtensibleEnumProperty));
    }

    [Test]
    public void XmlDeserialize ()
    {
      byte[] serializedArray = Encoding.UTF8.GetBytes (XmlSerializationStrings.XmlForComputer1);
      XmlTransportItem item = XmlSerializationHelper.XmlDeserialize<XmlTransportItem> (serializedArray);
      TransportItemTest.CheckEqualData (DomainObjectIDs.Computer1.GetObject<Computer> ().InternalDataContainer, item.TransportItem);
    }

    [Test]
    public void XmlDeserialize_WithNullObjectID ()
    {
      byte[] serializedArray = Encoding.UTF8.GetBytes (XmlSerializationStrings.XmlForComputer4);
      XmlTransportItem item = XmlSerializationHelper.XmlDeserialize<XmlTransportItem> (serializedArray);
      Assert.That (item.TransportItem.Properties[ReflectionMappingHelper.GetPropertyName (typeof (Computer), "Employee")], Is.Null);
    }

    [Test]
    public void XmlDeserialize_WithCustomProperty ()
    {
      byte[] serializedArray = Encoding.UTF8.GetBytes (XmlSerializationStrings.XmlForCustomProperty);
      XmlTransportItem item = XmlSerializationHelper.XmlDeserialize<XmlTransportItem> (serializedArray);
      Assert.That (item.TransportItem.Properties["Custom"], Is.EqualTo (5));
    }

    [Test]
    public void XmlDeserialize_WithCustomObjectIDProperty ()
    {
      byte[] serializedArray = Encoding.UTF8.GetBytes (XmlSerializationStrings.XmlForCustomObjectIDProperty);
      XmlTransportItem item = XmlSerializationHelper.XmlDeserialize<XmlTransportItem> (serializedArray);
      Assert.That (item.TransportItem.Properties["CustomReference"], Is.EqualTo (DomainObjectIDs.Order3));
    }

    [Test]
    public void XmlDeserialize_WithCustomNullProperty ()
    {
      byte[] serializedArray = Encoding.UTF8.GetBytes (XmlSerializationStrings.XmlForCustomNullProperty);
      XmlTransportItem item = XmlSerializationHelper.XmlDeserialize<XmlTransportItem> (serializedArray);
      Assert.That (item.TransportItem.Properties["CustomNull"], Is.EqualTo (null));
    }

    [Test]
    public void XmlDeserialize_WithCustomExtensibleEnumProperty ()
    {
      byte[] serializedArray = Encoding.UTF8.GetBytes (XmlSerializationStrings.XmlForCustomExtensibleEnumProperty);
      XmlTransportItem item = XmlSerializationHelper.XmlDeserialize<XmlTransportItem> (serializedArray);
      Assert.That (item.TransportItem.Properties["CustomExtensibleEnum"], Is.EqualTo (Color.Values.Red ()));
    }

    [Test]
    public void IntegrationTest_ID ()
    {
      DataContainer container = DomainObjectIDs.Computer1.GetObject<Computer> ().InternalDataContainer;
      TransportItem item = TransportItem.PackageDataContainer (container);
      TransportItem deserializedItem = SerializeAndDeserialize (item);

      Assert.That (deserializedItem.ID, Is.EqualTo (container.ID));
    }

    [Test]
    public void IntegrationTest_Properties ()
    {
      DataContainer container = DomainObjectIDs.Computer1.GetObject<Computer> ().InternalDataContainer;
      TransportItem item = TransportItem.PackageDataContainer (container);
      TransportItem deserializedItem = SerializeAndDeserialize (item);

      TransportItemTest.CheckEqualData (container, deserializedItem);
    }

    [Test]
    public void IntegrationTest_Properties_IntVsString ()
    {
      TransportItem item = new TransportItem (DomainObjectIDs.ClassWithAllDataTypes1);
      item.Properties.Add ("Int", 1);
      item.Properties.Add ("String", "1");
      TransportItem deserializedItem = SerializeAndDeserialize (item);

      Assert.That (deserializedItem.Properties["Int"], Is.EqualTo (1));
      Assert.That (deserializedItem.Properties["String"], Is.EqualTo ("1"));
    }

    [Test]
    public void IntegrationTest_Multiple ()
    {
      DataContainer container1 = DomainObjectIDs.Computer1.GetObject<Computer> ().InternalDataContainer;
      DataContainer container2 = DomainObjectIDs.Computer2.GetObject<Computer> ().InternalDataContainer;
      TransportItem item1 = TransportItem.PackageDataContainer (container1);
      TransportItem item2 = TransportItem.PackageDataContainer (container2);

      TransportItem[] deserializedItems = SerializeAndDeserialize (new[] { item1, item2 });

      TransportItemTest.CheckEqualData (container1, deserializedItems[0]);
      TransportItemTest.CheckEqualData (container2, deserializedItems[1]);
    }

    [Test]
    public void IntegrationTest_ClassesWithAllDataTypes ()
    {
      DataContainer container1 = DomainObjectIDs.ClassWithAllDataTypes1.GetObject<ClassWithAllDataTypes> ().InternalDataContainer;
      DataContainer container2 = DomainObjectIDs.ClassWithAllDataTypes2.GetObject<ClassWithAllDataTypes> ().InternalDataContainer;
      TransportItem item1 = TransportItem.PackageDataContainer (container1);
      TransportItem item2 = TransportItem.PackageDataContainer (container2);

      TransportItem[] deserializedItems = SerializeAndDeserialize (new[] { item1, item2 });

      TransportItemTest.CheckEqualData (container1, deserializedItems[0]);
      TransportItemTest.CheckEqualData (container2, deserializedItems[1]);
    }

    private TransportItem SerializeAndDeserialize (TransportItem item)
    {
      return XmlSerializationHelper.XmlSerializeAndDeserialize (new XmlTransportItem (item)).TransportItem;
    }

    private TransportItem[] SerializeAndDeserialize (TransportItem[] items)
    {
      return XmlTransportItem.Unwrap (XmlSerializationHelper.XmlSerializeAndDeserialize (XmlTransportItem.Wrap (items)));
    }
  }
}
