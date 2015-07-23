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
using System.Xml;
using System.Xml.Schema;
using NUnit.Framework;
using Remotion.Xml;

namespace Remotion.UnitTests.Xml.XmlSerializationUtilityTests
{
  [TestFixture]
  public class DeserializeUsingSchema
  {
    [Test]
    public void WithSchemaUriAndSchemaSet()
    {
      SampleClass actual = (SampleClass) XmlSerializationUtility.DeserializeUsingSchema (
          GetReaderForDefaultFragment (1),
          typeof (SampleClass),
          GetXmlSchemaSet());

      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.Value, Is.EqualTo (1));
    }

    [Test]
    public void WithSchemaUriAndSchemaReader()
    {
      SampleClass actual = (SampleClass) XmlSerializationUtility.DeserializeUsingSchema (
          GetReaderForDefaultFragment (1),
          typeof (SampleClass),
          SampleClass.SchemaUri,
          SampleClass.GetSchemaReader());

      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.Value, Is.EqualTo (1));
    }

    [Test]
    public void WithNamespaceAndSchemaSet()
    {
      SampleClass actual = (SampleClass) XmlSerializationUtility.DeserializeUsingSchema (
          GetReaderForDefaultFragment (1),
          typeof (SampleClass),
          "http://www.re-motion.org/core/unitTests",
          GetXmlSchemaSet());

      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.Value, Is.EqualTo (1));
    }

    [Test]
    [ExpectedException (typeof (XmlSchemaValidationException))]
    public void WithNamespaceAndSchemaSet_HavingInvalidDataTypeInXmlFragment ()
    {
      try
      {
        XmlSerializationUtility.DeserializeUsingSchema (
            GetReaderForDefaultFragment ("data"),
            typeof (SampleClass),
            "http://www.re-motion.org/core/unitTests",
            GetXmlSchemaSet());
      }
      catch (XmlSchemaValidationException e)
      {
        // Assert.AreEqual (2, e.LineNumber);
        // Assert.AreEqual (26, e.LinePosition);
        Assert.That (e.SourceUri, Is.EqualTo ("test.xml"));
        throw;
      }
    }

    private XmlSchemaSet GetXmlSchemaSet()
    {
      XmlSchemaSet schemas = new XmlSchemaSet();
      schemas.Add (SampleClass.SchemaUri, SampleClass.GetSchemaReader());
      return schemas;
    }

    private XmlReader GetReaderForDefaultFragment (object value)
    {
      string xmlFragment =
          @"<sampleClass xmlns=""http://www.re-motion.org/core/unitTests"">
            <value>{0}</value>
          </sampleClass>";
      return GetReader (string.Format (xmlFragment, value));
    }

    private XmlReader GetReader (string xmlFragment)
    {
      StringReader stringReader = new StringReader (xmlFragment);

      return XmlReader.Create (stringReader, null, "test.xml");
    }
  }
}
