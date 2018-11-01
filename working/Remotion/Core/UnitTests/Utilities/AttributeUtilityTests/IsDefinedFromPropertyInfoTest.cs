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
using System.Reflection;
using NUnit.Framework;
using Remotion.UnitTests.Utilities.AttributeUtilityTests.TestDomain;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.AttributeUtilityTests
{
  [TestFixture]
  public class IsDefinedFromPropertyInfoTest
  {
    private PropertyInfo _basePropertyWithSingleAttribute;
    private PropertyInfo _derivedPropertyWithSingleAttribute;
    private PropertyInfo _derivedPropertyWithMultipleAttribute;
    private PropertyInfo _derivedProtectedProperty;

    [SetUp]
    public void SetUp ()
    {
      _basePropertyWithSingleAttribute = typeof (SampleClass).GetProperty ("PropertyWithSingleAttribute");
      _derivedPropertyWithSingleAttribute = typeof (DerivedSampleClass).GetProperty ("PropertyWithSingleAttribute");
      _derivedPropertyWithMultipleAttribute = typeof (DerivedSampleClass).GetProperty ("PropertyWithMultipleAttribute");
      _derivedProtectedProperty = typeof (DerivedSampleClass).GetProperty ("ProtectedPropertyWithAttribute",
          BindingFlags.NonPublic | BindingFlags.Instance);
    }

    [Test]
    public void Test_FromBaseWithAttribute ()
    {
      Assert.That (AttributeUtility.IsDefined (_basePropertyWithSingleAttribute, typeof (InheritedAttribute), true), Is.True);
    }

    [Test]
    public void TestGeneric_FromBaseWithAttribute ()
    {
      Assert.That (AttributeUtility.IsDefined<InheritedAttribute> (_basePropertyWithSingleAttribute, true), Is.True);
    }

    [Test]
    public void Test_FromOverrideWithAttribute_ExpectAmbigousMatch ()
    {
      Assert.That (AttributeUtility.IsDefined (_derivedPropertyWithMultipleAttribute, typeof (MultipleAttribute), true), Is.True);
    }

    [Test]
    public void Test_FromBaseWithInterface ()
    {
      Assert.That (AttributeUtility.IsDefined (_basePropertyWithSingleAttribute, typeof (ICustomAttribute), true), Is.True);
    }

    [Test]
    public void TestGeneric_FromBaseWithInterface ()
    {
      Assert.That (AttributeUtility.IsDefined<ICustomAttribute> (_basePropertyWithSingleAttribute, true), Is.True);
    }

    [Test]
    public void Test_FromOverrideWithInterface_ExpectAmbigousMatch ()
    {
      Assert.That (AttributeUtility.IsDefined (_derivedPropertyWithMultipleAttribute, typeof (ICustomAttribute), true), Is.True);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The attribute type must be assignable to System.Attribute or an interface.\r\nParameter name: T")]
    public void TestGeneric_FromBaseWithInvalidType ()
    {
      AttributeUtility.IsDefined<object> (_basePropertyWithSingleAttribute, true);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "The attribute type must be assignable to System.Attribute or an interface.\r\nParameter name: attributeType")]
    public void Test_FromBaseWithInvalidType ()
    {
      AttributeUtility.IsDefined (_basePropertyWithSingleAttribute, typeof (object), true);
    }

    [Test]
    public void Test_FromOverrideWithAttribute ()
    {
      Assert.That (AttributeUtility.IsDefined (_derivedPropertyWithSingleAttribute, typeof (InheritedAttribute), true), Is.True);
    }

    [Test]
    [Ignore ("Not supported at the moment by Attribute.GetCustomAttribute - should we leave this or add a workaround?")]
    public void Test_FromProtectedOverrideWithAttribute ()
    {
      Assert.That (AttributeUtility.IsDefined (_derivedProtectedProperty, typeof (InheritedAttribute), true), Is.True);
    }

    [Test]
    public void Test_FromOverrideWithInterface ()
    {
      Assert.That (AttributeUtility.IsDefined (_derivedPropertyWithSingleAttribute, typeof (ICustomAttribute), true), Is.True);
    }

    [Test]
    public void Test_FromOverrideWithAttributeAndWithoutInherited ()
    {
      Assert.That (AttributeUtility.IsDefined (_derivedPropertyWithSingleAttribute, typeof (InheritedAttribute), false), Is.False);
    }

    [Test]
    public void Test_FromOverrideWithInterfaceAndWithoutInherited ()
    {
      Assert.That (AttributeUtility.IsDefined (_derivedPropertyWithSingleAttribute, typeof (ICustomAttribute), false), Is.False);
    }
  }
}
