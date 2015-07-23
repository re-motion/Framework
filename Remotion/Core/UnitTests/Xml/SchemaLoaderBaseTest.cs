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
using System.Xml.Schema;
using NUnit.Framework;
using Remotion.Xml;

namespace Remotion.UnitTests.Xml
{
  [TestFixture]
  public class SchemaLoaderBaseTest
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public SchemaLoaderBaseTest ()
    {
    }

    // methods and properties

    [Test]
    public void GetSchemaWithSchemaFile ()
    {
      SchemaLoaderBaseMock schemaBaseMock = new SchemaLoaderBaseMock ("http://www.re-motion.org/Core/Test/Xml/SchemaLoaderBaseMock");
      XmlSchema xmlSchema = schemaBaseMock.LoadSchema ("SchemaLoaderBaseMock.xsd");
      Assert.That (xmlSchema, Is.Not.Null);
      Assert.That (xmlSchema.TargetNamespace, Is.EqualTo ("http://www.re-motion.org/Core/Test/Xml/SchemaLoaderBaseMock"));
    }

    [Test]
    public void GetSchemaReaderWithInvalidFileName ()
    {
      try
      {
        SchemaLoaderBaseMock schemaBaseMock = new SchemaLoaderBaseMock ("http://www.re-motion.org/Core/Test/Xml/SchemaLoaderBaseMock");
        schemaBaseMock.LoadSchema ("invalidSchemaFileName.xsd");

        Assert.Fail ("ApplicationException was expected.");
      }
      catch (ApplicationException ex)
      {
        string expectedMessage = string.Format (
            "Error loading schema resource 'invalidSchemaFileName.xsd' from assembly '{0}'.", typeof (SchemaLoaderBaseMock).Assembly.FullName);

        Assert.That (ex.Message, Is.EqualTo (expectedMessage));
      }
    }

    [Test]
    public void GetSchemaSet ()
    {
      SchemaLoaderBase schemaBaseMock = new SchemaLoaderBaseMock ("http://www.re-motion.org/Core/Test/Xml/SchemaLoaderBaseMock");
      XmlSchemaSet xmlSchemaSet = schemaBaseMock.LoadSchemaSet ();
      Assert.That (xmlSchemaSet.Count, Is.EqualTo (1));
      Assert.That (xmlSchemaSet.Contains ("http://www.re-motion.org/Core/Test/Xml/SchemaLoaderBaseMock"), Is.True);
    }
  }
}
