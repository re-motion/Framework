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
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.UnitTests.Utilities.AttributeUtilityTests.TestDomain;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.AttributeUtilityTests
{
  [TestFixture]
  public class GetCustomAttributeFromPropertyInfoTest
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
      InheritedAttribute attribute =
          (InheritedAttribute) AttributeUtility.GetCustomAttribute (_basePropertyWithSingleAttribute, typeof (InheritedAttribute), true);
      Assert.That (attribute, Is.Not.Null);
    }

    [Test]
    public void TestGeneric_FromBaseWithAttribute ()
    {
      InheritedAttribute attribute = AttributeUtility.GetCustomAttribute<InheritedAttribute> (_basePropertyWithSingleAttribute, true);
      Assert.That (attribute, Is.Not.Null);
    }

    [Test]
    [ExpectedException (typeof (AmbiguousMatchException), ExpectedMessage = "Multiple custom attributes of the same type found.")]
    public void Test_FromOverrideWithAttribute_ExpectAmbigousMatch ()
    {
      AttributeUtility.GetCustomAttribute (_derivedPropertyWithMultipleAttribute, typeof (MultipleAttribute), true);
    }

    [Test]
    public void Test_FromBaseWithInterface ()
    {
      ICustomAttribute attribute = 
          (ICustomAttribute) AttributeUtility.GetCustomAttribute (_basePropertyWithSingleAttribute, typeof (ICustomAttribute), true);
      Assert.That (attribute, Is.Not.Null);
    }

    [Test]
    public void TestGeneric_FromBaseWithInterface ()
    {
      ICustomAttribute attribute = AttributeUtility.GetCustomAttribute<ICustomAttribute> (_basePropertyWithSingleAttribute, true);
      Assert.That (attribute, Is.Not.Null);
    }

    [Test]
    [ExpectedException (typeof (AmbiguousMatchException), ExpectedMessage = "Multiple custom attributes of the same type found.")]
    public void Test_FromOverrideWithInterface_ExpectAmbigousMatch ()
    {
      AttributeUtility.GetCustomAttribute (_derivedPropertyWithMultipleAttribute, typeof (ICustomAttribute), true);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The attribute type must be assignable to System.Attribute or an interface.\r\nParameter name: T")]
    public void TestGeneric_FromBaseWithInvalidType ()
    {
      AttributeUtility.GetCustomAttribute<object> (_basePropertyWithSingleAttribute, true);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = 
        "The attribute type must be assignable to System.Attribute or an interface.\r\nParameter name: attributeType")]
    public void Test_FromBaseWithInvalidType ()
    {
      AttributeUtility.GetCustomAttribute (_basePropertyWithSingleAttribute, typeof (object), true);
    }

    [Test]
    public void Test_FromOverrideWithAttribute ()
    {
      Assert.That (AttributeUtility.GetCustomAttribute (_derivedPropertyWithSingleAttribute, typeof (InheritedAttribute), true), Is.Not.Null);
    }

    [Test]
    [Ignore ("Not supported at the moment by Attribute.GetCustomAttribute - should we leave this or add a workaround?")]
    public void Test_FromProtectedOverrideWithAttribute ()
    {
      Assert.That (AttributeUtility.GetCustomAttribute (_derivedProtectedProperty, typeof (InheritedAttribute), true), Is.Not.Null);
    }

    [Test]
    public void Test_FromOverrideWithInterface ()
    {
      Assert.That (AttributeUtility.GetCustomAttribute (_derivedPropertyWithSingleAttribute, typeof (ICustomAttribute), true), Is.Not.Null);
    }

    [Test]
    public void Test_FromOverrideWithAttributeAndWithoutInherited ()
    {
      Assert.That (AttributeUtility.GetCustomAttribute (_derivedPropertyWithSingleAttribute, typeof (InheritedAttribute), false), Is.Null);
    }

    [Test]
    public void Test_FromOverrideWithInterfaceAndWithoutInherited ()
    {
      Assert.That (AttributeUtility.GetCustomAttribute (_derivedPropertyWithSingleAttribute, typeof (ICustomAttribute), false), Is.Null);
    }

    [Test]
    public void Test_FromBaseClass_InheritedFalse ()
    {
      object attribute = AttributeUtility.GetCustomAttribute (typeof (BaseClassWithAttribute), typeof (InheritedNotMultipleAttribute), false);
      Assert.That (attribute, Is.EqualTo(new InheritedNotMultipleAttribute ("BaseClass")));
    }

    [Test]
    public void Test_FromBaseClass_InheritedTrue ()
    {
      object attribute = AttributeUtility.GetCustomAttribute (typeof (BaseClassWithAttribute), typeof (InheritedNotMultipleAttribute), true);
      Assert.That (attribute, Is.EqualTo (new InheritedNotMultipleAttribute ("BaseClass")));
    }

    [Test]
    public void Test_FromDerivedClass_InheritedFalse ()
    {
      object attribute = AttributeUtility.GetCustomAttribute (typeof (DerivedClassWithAttribute), typeof (InheritedNotMultipleAttribute), false);
      Assert.That (attribute, Is.EqualTo (new InheritedNotMultipleAttribute ("DerivedClass")));
    }

    [Test]
    public void Test_FromDerivedClass_InheritedTrue ()
    {
      object attribute = AttributeUtility.GetCustomAttribute (typeof (DerivedClassWithAttribute), typeof (InheritedNotMultipleAttribute), true);
      Assert.That (attribute, Is.EqualTo (new InheritedNotMultipleAttribute ("DerivedClass")));
    }
    
    [Test]
    public void Test_ReturnsNewInstanceForType ()
    {
      var attribute1 = AttributeUtility.GetCustomAttributes (typeof (SampleClass), typeof (InheritedAttribute), false).Single();
      var attribute2 = AttributeUtility.GetCustomAttributes (typeof (SampleClass), typeof (InheritedAttribute), false).Single();

      Assert.That (attribute1, Is.Not.SameAs (attribute2));
    }

    [Test]
    public void Test_ReturnsNewInstanceForMemberInfo ()
    {
      var attribute1 = AttributeUtility.GetCustomAttributes (_basePropertyWithSingleAttribute, typeof (InheritedAttribute), false).Single();
      var attribute2 = AttributeUtility.GetCustomAttributes (_basePropertyWithSingleAttribute, typeof (InheritedAttribute), false).Single();

      Assert.That (attribute1, Is.Not.SameAs (attribute2));
    }
  }
}
