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
using NUnit.Framework;
using Remotion.UnitTests.Utilities.AttributeUtilityTests.TestDomain;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.AttributeUtilityTests
{
  [TestFixture]
  public class GetCustomAttributesWithMetadataTest
  {
    [Test]
    public void BaseClass_InheritedFalse ()
    {
      AttributeWithMetadata[] attributes = 
        AttributeUtility.GetCustomAttributesWithMetadata (typeof (BaseClassWithAttribute), typeof (Attribute), false).ToArray ();
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new AttributeWithMetadata (typeof (BaseClassWithAttribute), new BaseInheritedAttribute ("BaseClass")), 
        new AttributeWithMetadata (typeof (BaseClassWithAttribute), new DerivedInheritedAttribute ("BaseClass")), 
        new AttributeWithMetadata (typeof (BaseClassWithAttribute), new BaseNonInheritedAttribute ("BaseClass")), 
        new AttributeWithMetadata (typeof (BaseClassWithAttribute), new DerivedNonInheritedAttribute ("BaseClass")),
        new AttributeWithMetadata (typeof (BaseClassWithAttribute), new InheritedNotMultipleAttribute ("BaseClass")),
      }));
    }

    [Test]
    public void BaseClass_InheritedTrue ()
    {
      AttributeWithMetadata[] attributes =
        AttributeUtility.GetCustomAttributesWithMetadata (typeof (BaseClassWithAttribute), typeof (Attribute), true).ToArray ();
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new AttributeWithMetadata (typeof (BaseClassWithAttribute), new BaseInheritedAttribute ("BaseClass")), 
        new AttributeWithMetadata (typeof (BaseClassWithAttribute), new DerivedInheritedAttribute ("BaseClass")), 
        new AttributeWithMetadata (typeof (BaseClassWithAttribute), new BaseNonInheritedAttribute ("BaseClass")), 
        new AttributeWithMetadata (typeof (BaseClassWithAttribute), new DerivedNonInheritedAttribute ("BaseClass")),
        new AttributeWithMetadata (typeof (BaseClassWithAttribute), new InheritedNotMultipleAttribute ("BaseClass")),
      }));
    }

    [Test]
    public void DerivedClass_InheritedFalse ()
    {
      AttributeWithMetadata[] attributes =
        AttributeUtility.GetCustomAttributesWithMetadata (typeof (DerivedClassWithAttribute), typeof (Attribute), false).ToArray ();
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new AttributeWithMetadata (typeof (DerivedClassWithAttribute), new BaseInheritedAttribute ("DerivedClass")), 
        new AttributeWithMetadata (typeof (DerivedClassWithAttribute), new DerivedInheritedAttribute ("DerivedClass")), 
        new AttributeWithMetadata (typeof (DerivedClassWithAttribute), new BaseNonInheritedAttribute ("DerivedClass")), 
        new AttributeWithMetadata (typeof (DerivedClassWithAttribute), new DerivedNonInheritedAttribute ("DerivedClass")),
        new AttributeWithMetadata (typeof (DerivedClassWithAttribute), new SuppressAttributesAttribute (typeof (InheritedNotMultipleAttribute))),
        new AttributeWithMetadata (typeof (DerivedClassWithAttribute), new InheritedNotMultipleAttribute ("DerivedClass")),
      }));
    }

    [Test]
    public void DerivedClass_InheritedTrue ()
    {
      AttributeWithMetadata[] attributes =
        AttributeUtility.GetCustomAttributesWithMetadata (typeof (DerivedClassWithAttribute), typeof (Attribute), true).ToArray ();
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new AttributeWithMetadata (typeof (BaseClassWithAttribute), new BaseInheritedAttribute ("BaseClass")), 
        new AttributeWithMetadata (typeof (BaseClassWithAttribute), new DerivedInheritedAttribute ("BaseClass")), 
        new AttributeWithMetadata (typeof (DerivedClassWithAttribute), new BaseInheritedAttribute ("DerivedClass")), 
        new AttributeWithMetadata (typeof (DerivedClassWithAttribute), new DerivedInheritedAttribute ("DerivedClass")), 
        new AttributeWithMetadata (typeof (DerivedClassWithAttribute), new BaseNonInheritedAttribute ("DerivedClass")), 
        new AttributeWithMetadata (typeof (DerivedClassWithAttribute), new DerivedNonInheritedAttribute ("DerivedClass")),
        new AttributeWithMetadata (typeof (DerivedClassWithAttribute), new SuppressAttributesAttribute (typeof (InheritedNotMultipleAttribute))),
        new AttributeWithMetadata (typeof (DerivedClassWithAttribute), new InheritedNotMultipleAttribute ("DerivedClass")),
      }));
    }

    [Test]
    public void DerivedClass_WithTypeFilter ()
    {
      AttributeWithMetadata[] attributes =
        AttributeUtility.GetCustomAttributesWithMetadata (typeof (DerivedClassWithAttribute), typeof (BaseInheritedAttribute), true).ToArray ();
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new AttributeWithMetadata (typeof (BaseClassWithAttribute), new BaseInheritedAttribute ("BaseClass")), 
        new AttributeWithMetadata (typeof (BaseClassWithAttribute), new DerivedInheritedAttribute ("BaseClass")), 
        new AttributeWithMetadata (typeof (DerivedClassWithAttribute), new BaseInheritedAttribute ("DerivedClass")), 
        new AttributeWithMetadata (typeof (DerivedClassWithAttribute), new DerivedInheritedAttribute ("DerivedClass")), 
      }));
    }

    [Test]
    public void ObjectClass_InheritedFalse ()
    {
      AttributeWithMetadata[] attributes =
        AttributeUtility.GetCustomAttributesWithMetadata (typeof (object), typeof (Attribute), false).ToArray ();
      Assert.That (attributes.Length, Is.EqualTo (typeof (object).GetCustomAttributes (false).Length));
    }

    [Test]
    public void ObjectClass_InheritedTrue ()
    {
      AttributeWithMetadata[] attributes =
        AttributeUtility.GetCustomAttributesWithMetadata (typeof (object), typeof (Attribute), true).ToArray ();
      Assert.That (attributes.Length, Is.EqualTo (typeof (object).GetCustomAttributes (false).Length));
    }

    [Test]
    public void Type_ReturnsNewInstance ()
    {
      var attributeMetadata1 = AttributeUtility.GetCustomAttributesWithMetadata (typeof (SampleClass), typeof (InheritedAttribute), false).Single ();
      var attributeMetadata2 = AttributeUtility.GetCustomAttributesWithMetadata (typeof (SampleClass), typeof (InheritedAttribute), false).Single ();
      Assert.That (attributeMetadata1.AttributeInstance, Is.Not.SameAs (attributeMetadata2.AttributeInstance));
    }
  }
}
