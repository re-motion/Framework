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
  public class GetCustomAttributesFromTypeTest
  {
    [Test]
    public void ReturnSpecificArrayType ()
    {
      object[] attributes = AttributeUtility.GetCustomAttributes (typeof (BaseClassWithAttribute), typeof (BaseInheritedAttribute), false);
      Assert.That (attributes, Is.InstanceOf (typeof (BaseInheritedAttribute[])));
    }

    [Test]
    public void BaseClass_InheritedFalse ()
    {
      object[] attributes = AttributeUtility.GetCustomAttributes (typeof (BaseClassWithAttribute), typeof (Attribute), false);
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new BaseInheritedAttribute ("BaseClass"), 
        new DerivedInheritedAttribute ("BaseClass"), 
        new BaseNonInheritedAttribute ("BaseClass"), 
        new DerivedNonInheritedAttribute ("BaseClass"),
        new InheritedNotMultipleAttribute ("BaseClass"),
      }));
    }

    [Test]
    public void BaseClass_InheritedTrue ()
    {
      object[] attributes = AttributeUtility.GetCustomAttributes (typeof (BaseClassWithAttribute), typeof (Attribute), true);
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new BaseInheritedAttribute ("BaseClass"), 
        new DerivedInheritedAttribute ("BaseClass"), 
        new BaseNonInheritedAttribute ("BaseClass"), 
        new DerivedNonInheritedAttribute ("BaseClass"),
        new InheritedNotMultipleAttribute ("BaseClass"),
      }));
    }

    [Test]
    public void DerivedClass_InheritedFalse ()
    {
      object[] attributes = AttributeUtility.GetCustomAttributes (typeof (DerivedClassWithAttribute), typeof (Attribute), false);
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new BaseInheritedAttribute ("DerivedClass"), 
        new DerivedInheritedAttribute ("DerivedClass"), 
        new BaseNonInheritedAttribute ("DerivedClass"), 
        new DerivedNonInheritedAttribute ("DerivedClass"),
        new InheritedNotMultipleAttribute ("DerivedClass"),
      }));
    }

    [Test]
    public void DerivedClass_InheritedTrue ()
    {
      object[] attributes = AttributeUtility.GetCustomAttributes (typeof (DerivedClassWithAttribute), typeof (Attribute), true);
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new BaseInheritedAttribute ("BaseClass"), 
        new DerivedInheritedAttribute ("BaseClass"), 
        new BaseInheritedAttribute ("DerivedClass"), 
        new DerivedInheritedAttribute ("DerivedClass"), 
        new BaseNonInheritedAttribute ("DerivedClass"), 
        new DerivedNonInheritedAttribute ("DerivedClass"),
        new InheritedNotMultipleAttribute ("DerivedClass"),
      }));
    }

    [Test]
    public void BaseClass_InheritableNotMultipleAttribute ()
    {
      object[] attributes = AttributeUtility.GetCustomAttributes (typeof (BaseWithInheritedNonMultipleAttribute), typeof (Attribute), true);
      Assert.That (attributes, Has.Length.EqualTo (1));
      Assert.That (((InheritedNotMultipleAttribute) attributes[0]).Context, Is.EqualTo ("Base"));
    }

    [Test]
    public void DerivedClass_ShadowingInheritedNotMultipleAttribute ()
    {
      object[] attributes = AttributeUtility.GetCustomAttributes (typeof (DerivedShadowingNonMultipleAttribute), typeof (Attribute), true);
      Assert.That (attributes, Has.Length.EqualTo (1));
      Assert.That (((InheritedNotMultipleAttribute) attributes[0]).Context, Is.EqualTo ("Derived"));
    }
    
    [Test]
    public void Filtering_WithBaseAttributeType ()
    {
      object[] attributes = AttributeUtility.GetCustomAttributes (typeof (BaseClassWithAttribute), typeof (BaseInheritedAttribute), false);
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new BaseInheritedAttribute ("BaseClass"), 
        new DerivedInheritedAttribute ("BaseClass")}));
    }

    [Test]
    public void Filtering_WithDerivedAttributeType ()
    {
      object[] attributes = AttributeUtility.GetCustomAttributes (typeof (BaseClassWithAttribute), typeof (DerivedInheritedAttribute), false);
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new DerivedInheritedAttribute ("BaseClass")}));
    }

    [Test]
    public void Filtering_WithInterfaceType ()
    {
      object[] attributes = AttributeUtility.GetCustomAttributes (typeof (BaseClassWithAttribute), typeof (ICustomAttribute), false);
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new BaseInheritedAttribute ("BaseClass"), 
        new DerivedInheritedAttribute ("BaseClass"),
        new InheritedNotMultipleAttribute ("BaseClass"),
      }));
    }

    [Test]
    public void GetCustomAttributes_WithMemberInfo_DelegatesToTypeVersion ()
    {
      object[] attributes = AttributeUtility.GetCustomAttributes ((MemberInfo) typeof (DerivedWithAttributesAndSuppressed), typeof (Attribute), true);
      Assert.That (attributes, Is.EquivalentTo (new object[] {
        new BaseInheritedAttribute ("DerivedWithAttributesAndSuppressed"), 
        new DerivedInheritedAttribute ("DerivedWithAttributesAndSuppressed")}));
    }

    [Test]
    public void GetCustomAttriubtes_ReturnsNewInstance ()
    {
      var attribute1 = AttributeUtility.GetCustomAttributes (typeof (SampleClass), typeof (InheritedAttribute), false).Single();
      var attribute2 = AttributeUtility.GetCustomAttributes (typeof (SampleClass), typeof (InheritedAttribute), false).Single();

      Assert.That (attribute1, Is.Not.SameAs (attribute2));
    }
  }
}
