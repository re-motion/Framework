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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.DomainImplementation.Transport
{
  public static class XmlSerializationStrings
  {
    private const char c_bom = (char)65279;

    public static string XmlForComputer1
    {
      get
      {
        return
            c_bom + @"<?xml version=""1.0"" encoding=""utf-8""?>"
                  + @"
<XmlTransportItem ID=""Computer|c7c26bf5-871d-48c7-822a-e9b05aac4e5a|System.Guid"">
  <Properties>
    <Property Name=""Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.SerialNumber"">
      <string>12345-xzy-56</string>
    </Property>
    <Property Name=""Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee"">Employee|3c4f3fc8-0db2-4c1f-aa00-ade72e9edb32|System.Guid</Property>
    <Property Name=""Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.Int32TransactionProperty"">
      <int>0</int>
    </Property>
    <Property Name=""Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.DateTimeTransactionProperty"">
      <dateTime>0001-01-01T00:00:00</dateTime>
    </Property>
    <Property Name=""Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.EmployeeTransactionProperty"">
      <null />
    </Property>
  </Properties>
</XmlTransportItem>";
      }
    }

    public static string XmlForComputer4
    {
      get
      {
        return
            c_bom + @"<?xml version=""1.0"" encoding=""utf-8""?>"
                  + @"
<XmlTransportItem ID=""Computer|d6f50e77-2041-46b8-a840-aaa4d2e1bf5a|System.Guid"">
  <Properties>
    <Property Name=""Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.SerialNumber"">
      <string>63457-kol-34</string>
    </Property>
    <Property Name=""Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee"">
      <null />
    </Property>
    <Property Name=""Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.Int32TransactionProperty"">
      <int>0</int>
    </Property>
    <Property Name=""Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.DateTimeTransactionProperty"">
      <dateTime>0001-01-01T00:00:00</dateTime>
    </Property>
    <Property Name=""Remotion.Data.DomainObjects.UnitTests.TestDomain.Computer.EmployeeTransactionProperty"">
      <null />
    </Property>
  </Properties>
</XmlTransportItem>";
      }
    }

    public static string XmlForCustomProperty
    {
      get
      {
        return
            c_bom + @"<?xml version=""1.0"" encoding=""utf-8""?>"
                  + @"
<XmlTransportItem ID=""Computer|c7c26bf5-871d-48c7-822a-e9b05aac4e5a|System.Guid"">
  <Properties>
    <Property Name=""Custom"" Type=""" + typeof(int).AssemblyQualifiedName + @""">
      <int>5</int>
    </Property>
  </Properties>
</XmlTransportItem>";
      }
    }

    public static string XmlForCustomObjectIDProperty
    {
      get
      {
        return
            c_bom + @"<?xml version=""1.0"" encoding=""utf-8""?>"
                  + @"
<XmlTransportItem ID=""Computer|c7c26bf5-871d-48c7-822a-e9b05aac4e5a|System.Guid"">
  <Properties>
    <Property Name=""CustomReference"" Type=""ObjectID"">Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid</Property>
  </Properties>
</XmlTransportItem>";
      }
    }

    public static string XmlForCustomNullProperty
    {
      get
      {
        return
            c_bom + @"<?xml version=""1.0"" encoding=""utf-8""?>"
                  + @"
<XmlTransportItem ID=""Computer|c7c26bf5-871d-48c7-822a-e9b05aac4e5a|System.Guid"">
  <Properties>
    <Property Name=""CustomNull"" Type=""null"">
      <null />
    </Property>
  </Properties>
</XmlTransportItem>";
      }
    }

    public static string XmlForCustomExtensibleEnumProperty
    {
      get
      {
        return
            c_bom + @"<?xml version=""1.0"" encoding=""utf-8""?>"
                  + @"
<XmlTransportItem ID=""Computer|c7c26bf5-871d-48c7-822a-e9b05aac4e5a|System.Guid"">
  <Properties>
    <Property Name=""CustomExtensibleEnum"" Type=""@assemblyQualifiedName"">Remotion.Data.DomainObjects.UnitTests.TestDomain.ColorExtensions.Red</Property>
  </Properties>
</XmlTransportItem>".Replace("@assemblyQualifiedName", typeof(Color).AssemblyQualifiedName);
      }
    }

    public static string XmlForOrder1Order2
    {
      get
      {
        return
            c_bom + @"<?xml version=""1.0"" encoding=""utf-8""?>"
                  + @"
<ArrayOfXmlTransportItem xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <XmlTransportItem ID=""Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid"">
    <Properties>
      <Property Name=""Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"">
        <int>1</int>
      </Property>
      <Property Name=""Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.DeliveryDate"">
        <dateTime>2005-01-01T00:00:00</dateTime>
      </Property>
      <Property Name=""Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Official"">Official|1|System.Int32</Property>
      <Property Name=""Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"">Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid</Property>
    </Properties>
  </XmlTransportItem>
  <XmlTransportItem ID=""Order|83445473-844a-4d3f-a8c3-c27f8d98e8ba|System.Guid"">
    <Properties>
      <Property Name=""Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"">
        <int>3</int>
      </Property>
      <Property Name=""Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.DeliveryDate"">
        <dateTime>2005-03-01T00:00:00</dateTime>
      </Property>
      <Property Name=""Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Official"">Official|1|System.Int32</Property>
      <Property Name=""Remotion.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"">Customer|dd3e3d55-c16f-497f-a3e1-384d08de0d66|System.Guid</Property>
    </Properties>
  </XmlTransportItem>
</ArrayOfXmlTransportItem>";
      }
    }
  }
}
