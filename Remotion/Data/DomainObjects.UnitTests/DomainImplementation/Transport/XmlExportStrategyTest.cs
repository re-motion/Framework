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
using System.Text;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation.Transport;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainImplementation.Transport
{
  [TestFixture]
  public class XmlExportStrategyTest : ClientTransactionBaseTest
  {
    private string ReplaceKnownXmlNamespaceDeclarations (string xml)
    {
      return xml
          .Replace (@"xmlns:xsd=""http://www.w3.org/2001/XMLSchema""", "XmlnsDeclaration")
          .Replace (@"xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""", "XmlnsDeclaration");
    }

    [Test]
    public void Export_SerializesData ()
    {
      DataContainer container1 = DomainObjectIDs.Order1.GetObject<Order> ().InternalDataContainer;
      DataContainer container2 = DomainObjectIDs.Order3.GetObject<Order> ().InternalDataContainer;

      TransportItem item1 = TransportItem.PackageDataContainer (container1);
      TransportItem item2 = TransportItem.PackageDataContainer (container2);

      var items = new[] { item1, item2 };
      using (var stream = new MemoryStream ())
      {
        XmlExportStrategy.Instance.Export (stream, items);
        var actualString = ReplaceKnownXmlNamespaceDeclarations (Encoding.UTF8.GetString (stream.ToArray()));

        Assert.That (actualString, Is.EqualTo (ReplaceKnownXmlNamespaceDeclarations (XmlSerializationStrings.XmlForOrder1Order2)));
      }
    }

    public static byte[] Export (params TransportItem[] transportItems)
    {
      using (var stream = new MemoryStream ())
      {
        XmlExportStrategy.Instance.Export (stream, transportItems);
        return stream.ToArray ();
      }
    }
  }
}
