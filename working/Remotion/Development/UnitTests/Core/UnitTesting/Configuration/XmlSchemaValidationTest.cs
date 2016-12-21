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
using Remotion.Development.UnitTesting.Configuration;

namespace Remotion.Development.UnitTests.Core.UnitTesting.Configuration
{
  [TestFixture]
  public class XmlSchemaValidationTest
  {
    [Test]
    public void Validate_ValidSchema ()
    {
      var xsdContent = 
        @"<?xml version=""1.0"" encoding=""utf-8""?>
          <xs:schema id=""typepipe""
              xmlns:xs=""http://www.w3.org/2001/XMLSchema""
              targetNamespace=""targetNamespace""
              elementFormDefault=""qualified""
              attributeFormDefault=""unqualified"">
            <xs:element name=""tag"">
              <xs:complexType>
                <xs:attribute name=""attribute"" type=""xs:boolean"" use=""required"" />
              </xs:complexType>
            </xs:element>
          </xs:schema>";
      var validFragment = @"<tag xmlns=""targetNamespace"" attribute=""true"" />";
      var invalidFragment = @"<tag xmlns=""targetNamespace"" attribute=""null"" />";

      Assert.That (() => XmlSchemaValidation.Validate (validFragment, xsdContent), Throws.Nothing);
      Assert.That (
          () => XmlSchemaValidation.Validate (invalidFragment, xsdContent),
          Throws.Exception.With.Message.StartsWith ("Validation of the xml fragment did not succeed for schema"));
    }

    [Test]
    [ExpectedException (typeof (XmlSchemaException), ExpectedMessage = "Schema is invalid:", MatchType = MessageMatch.StartsWith)]
    public void Validate_InvalidSchema ()
    {
      var invalidSchema = @"<xs:invalid xmlns:xs=""http://www.w3.org/2001/XMLSchema"" />";
      XmlSchemaValidation.Validate ("does not matter", invalidSchema);
    }
  }
}