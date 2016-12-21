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
using Remotion.ExtensibleEnums.UnitTests.TestDomain;
using Remotion.Reflection;

namespace Remotion.ExtensibleEnums.UnitTests
{
  [TestFixture]
  public class ExtensibleEnumUtilityTest
  {
    [Test]
    public void IsExtensibleEnumType_True ()
    {
      Assert.That (ExtensibleEnumUtility.IsExtensibleEnumType (typeof (Color)), Is.True);
    }

    [Test]
    public void IsExtensibleEnumType_False ()
    {
      Assert.That (ExtensibleEnumUtility.IsExtensibleEnumType (typeof (object)), Is.False);
    }

    [Test]
    public void IsExtensibleEnumType_False_BaseType ()
    {
      Assert.That (ExtensibleEnumUtility.IsExtensibleEnumType (typeof (ExtensibleEnum<>)), Is.False);
      Assert.That (ExtensibleEnumUtility.IsExtensibleEnumType (typeof (ExtensibleEnum<Color>)), Is.False);
      Assert.That (ExtensibleEnumUtility.IsExtensibleEnumType (typeof (IExtensibleEnum)), Is.False);
    }

    [Test]
    public void IsExtensibleEnumType_FromITypeInformation_True ()
    {
      Assert.That (ExtensibleEnumUtility.IsExtensibleEnumType (TypeAdapter.Create (typeof (Color))), Is.True);
    }

    [Test]
    public void IsExtensibleEnumType_FromITypeInformation_False ()
    {
      Assert.That (ExtensibleEnumUtility.IsExtensibleEnumType (TypeAdapter.Create (typeof (object))), Is.False);
    }

    [Test]
    public void IsExtensibleEnumType_FromITypeInformation_False_BaseType ()
    {
      Assert.That (ExtensibleEnumUtility.IsExtensibleEnumType (TypeAdapter.Create (typeof (ExtensibleEnum<>))), Is.False);
      Assert.That (ExtensibleEnumUtility.IsExtensibleEnumType (TypeAdapter.Create (typeof (ExtensibleEnum<Color>))), Is.False);
      Assert.That (ExtensibleEnumUtility.IsExtensibleEnumType (TypeAdapter.Create (typeof (IExtensibleEnum))), Is.False);
    }

    [Test]
    public void GetDefinition ()
    {
      Assert.That (ExtensibleEnumUtility.GetDefinition (typeof (Color)), Is.SameAs (Color.Values));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "Type 'System.Object' is not an extensible enum type derived from ExtensibleEnum<T>.\r\nParameter name: extensibleEnumType")]
    public void GetDefinition_InvalidType ()
    {
      ExtensibleEnumUtility.GetDefinition (typeof (object));
    }
  }
}