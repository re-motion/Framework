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
using NUnit.Framework;
using Remotion.Development.UnitTesting.NUnit;
using Remotion.Xml.UnitTests.XmlSerializationUtilityTests.TestDomain;

namespace Remotion.Xml.UnitTests.XmlSerializationUtilityTests
{
  [TestFixture]
  public class GetNamespaceTest
  {
    [Test]
    public void WithXmlTypeAttribute ()
    {
      Assert.That(XmlSerializationUtility.GetNamespace(typeof(SampleTypeWithXmlType)), Is.EqualTo("http://type-namespace"));
    }

    [Test]
    public void WithXmlTypeAttributeWithoutNamespace ()
    {
      Assert.That(
          () => XmlSerializationUtility.GetNamespace(typeof(SampleTypeWithXmlTypeWithoutNamespace)),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Cannot determine the xml namespace of type 'Remotion.Xml.UnitTests.XmlSerializationUtilityTests.TestDomain.SampleTypeWithXmlTypeWithoutNamespace' "
                  + "because neither an XmlTypeAttribute nor an XmlRootAttribute is used to define a namespace for the type.", "type"));
    }

    [Test]
    public void WithXmlRootAttribute ()
    {
      Assert.That(XmlSerializationUtility.GetNamespace(typeof(SampleTypeWithXmlRoot)), Is.EqualTo("http://root-namespace"));
    }

    [Test]
    public void WithXmlRootAttributeWithoutNamespace ()
    {
      Assert.That(
          () => XmlSerializationUtility.GetNamespace(typeof(SampleTypeWithXmlRootWithoutNamespace)),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Cannot determine the xml namespace of type 'Remotion.Xml.UnitTests.XmlSerializationUtilityTests.TestDomain.SampleTypeWithXmlRootWithoutNamespace' "
                  + "because neither an XmlTypeAttribute nor an XmlRootAttribute is used to define a namespace for the type.", "type"));
    }

    [Test]
    public void WithXmlRootAttributeWithTypeAlsoHavingAnXmlTypeAttribute ()
    {
      Assert.That(XmlSerializationUtility.GetNamespace(typeof(SampleTypeWithXmlRootAndXmlType)), Is.EqualTo("http://root-namespace"));
    }

    [Test]
    public void WithoutXmlRootAttributeAndWithoutXmlTypeAttribute ()
    {
      Assert.That(
          () => XmlSerializationUtility.GetNamespace(typeof(SampleTypeWithoutXmlAttributes)),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Cannot determine the xml namespace of type 'Remotion.Xml.UnitTests.XmlSerializationUtilityTests.TestDomain.SampleTypeWithoutXmlAttributes' "
                  + "because no neither an XmlTypeAttribute nor an XmlRootAttribute has been provided.", "type"));
    }
  }
}
