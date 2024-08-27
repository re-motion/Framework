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
using Remotion.Development.UnitTesting;

namespace Remotion.Development.UnitTests.Core.UnitTesting
{
  [TestFixture]
  public class XmlSerializationHelperTest
  {
    private const char c_bom = (char)65279;

    [Test]
    public void XmlSerialize ()
    {
      int[] array = new int[] { 1, 2, 3 };
      byte[] serializedArray = XmlSerializationHelper.XmlSerialize(array);
      var serializedArrayString = ReplaceKnownXmlNamespaceDeclarations(Encoding.UTF8.GetString(serializedArray));

      Assert.That(serializedArrayString, Is.EqualTo(ReplaceKnownXmlNamespaceDeclarations(GetExpectedXmlString())));
    }

    [Test]
    public void XmlDeserialize ()
    {
      byte[] serializedArray = Encoding.UTF8.GetBytes(GetExpectedXmlString());
      int[] array = XmlSerializationHelper.XmlDeserialize<int[]>(serializedArray);
      Assert.That(array, Is.EqualTo(new int[] { 1, 2, 3 }));
    }

    [Test]
    public void XmlSerializeAndDeserialize ()
    {
      string[] array = XmlSerializationHelper.XmlSerializeAndDeserialize(new string[] { "1", "2", "3" });
      Assert.That(array, Is.EqualTo(new string[] { "1", "2", "3" }));
    }

    private string ReplaceKnownXmlNamespaceDeclarations (string xml)
    {
      return xml
          .Replace(@"xmlns:xsd=""http://www.w3.org/2001/XMLSchema""", "XmlnsDeclaration")
          .Replace(@"xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance""", "XmlnsDeclaration");
    }


    private string GetExpectedXmlString ()
    {
      return
            c_bom + @"<?xml version=""1.0"" encoding=""utf-8""?>"
          + @"
<ArrayOfInt xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
  <int>1</int>
  <int>2</int>
  <int>3</int>
</ArrayOfInt>";
    }
  }
}
