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
  public class AttributeWithMetadataTest
  {
    [Test]
    public void IsInstanceOfType_True ()
    {
      AttributeWithMetadata attribute = new AttributeWithMetadata(typeof (BaseClassWithAttribute), new DerivedInheritedAttribute("X"));
      Assert.That (attribute.IsInstanceOfType (typeof (Attribute)));
      Assert.That (attribute.IsInstanceOfType (typeof (BaseInheritedAttribute)));
      Assert.That (attribute.IsInstanceOfType (typeof (DerivedInheritedAttribute)));
      Assert.That (attribute.IsInstanceOfType (typeof (ICustomAttribute)));
    }

    [Test]
    public void IsInstanceOfType_False ()
    {
      AttributeWithMetadata attribute = new AttributeWithMetadata (typeof (BaseClassWithAttribute), new DerivedNonInheritedAttribute ("X"));
      Assert.That (attribute.IsInstanceOfType (typeof (BaseInheritedAttribute)), Is.False);
      Assert.That (attribute.IsInstanceOfType (typeof (ICustomAttribute)), Is.False);
    }

    [Test]
    public void IncludeAll ()
    {
      AttributeWithMetadata[] attributes = new AttributeWithMetadata[] {
          new AttributeWithMetadata(typeof (object), new BaseInheritedAttribute("X")),
          new AttributeWithMetadata(typeof (object), new BaseNonInheritedAttribute("X")),
          new AttributeWithMetadata(typeof (object), new DerivedInheritedAttribute("X")),
          new AttributeWithMetadata(typeof (object), new DerivedNonInheritedAttribute("X")),
      };
      AttributeWithMetadata[] expectedAttributes = new AttributeWithMetadata[] {
          new AttributeWithMetadata(typeof (object), new BaseInheritedAttribute("X")),
          new AttributeWithMetadata(typeof (object), new DerivedInheritedAttribute("X")),
      };

      AttributeWithMetadata[] filteredAttributes = AttributeWithMetadata.IncludeAll (attributes, typeof (ICustomAttribute)).ToArray ();
      Assert.That (filteredAttributes, Is.EqualTo (expectedAttributes));
    }

    [Test]
    public void ExcludeAll ()
    {
      AttributeWithMetadata[] attributes = new AttributeWithMetadata[] {
          new AttributeWithMetadata(typeof (object), new BaseInheritedAttribute("X")),
          new AttributeWithMetadata(typeof (object), new BaseNonInheritedAttribute("X")),
          new AttributeWithMetadata(typeof (object), new DerivedInheritedAttribute("X")),
          new AttributeWithMetadata(typeof (object), new DerivedNonInheritedAttribute("X")),
      };
      AttributeWithMetadata[] expectedAttributes = new AttributeWithMetadata[] {
          new AttributeWithMetadata(typeof (object), new BaseNonInheritedAttribute("X")),
          new AttributeWithMetadata(typeof (object), new DerivedNonInheritedAttribute("X")),
      };

      AttributeWithMetadata[] filteredAttributes = AttributeWithMetadata.ExcludeAll (attributes, typeof (ICustomAttribute)).ToArray ();
      Assert.That (filteredAttributes, Is.EqualTo (expectedAttributes));
    }

    [Test]
    public void Suppress_DoesNotSuppressAnythingOnSameType ()
    {
      AttributeWithMetadata[] attributes = new AttributeWithMetadata[] {
          new AttributeWithMetadata(typeof (object), new BaseInheritedAttribute("X")),
      };
      AttributeWithMetadata[] suppressAttributes = new AttributeWithMetadata[] {
          new AttributeWithMetadata(typeof (object), new SuppressAttributesAttribute(typeof (BaseInheritedAttribute))),
      };
      AttributeWithMetadata[] expectedAttributes = new AttributeWithMetadata[] {
          new AttributeWithMetadata(typeof (object), new BaseInheritedAttribute("X")),
      };

      AttributeWithMetadata[] filteredAttributes = AttributeWithMetadata.Suppress (attributes, suppressAttributes).ToArray ();
      Assert.That (filteredAttributes, Is.EqualTo (expectedAttributes));
    }

    [Test]
    public void Suppress_DoesSuppressSpecificAttribute_OnDifferentType ()
    {
      AttributeWithMetadata[] attributes = new AttributeWithMetadata[] {
          new AttributeWithMetadata(typeof (object), new BaseInheritedAttribute("X")),
      };
      AttributeWithMetadata[] suppressAttributes = new AttributeWithMetadata[] {
          new AttributeWithMetadata(typeof (string), new SuppressAttributesAttribute(typeof (BaseInheritedAttribute))),
      };
      AttributeWithMetadata[] expectedAttributes = new AttributeWithMetadata[] {
      };

      AttributeWithMetadata[] filteredAttributes = AttributeWithMetadata.Suppress (attributes, suppressAttributes).ToArray ();
      Assert.That (filteredAttributes, Is.EqualTo (expectedAttributes));
    }

    [Test]
    public void Suppress_DoesSuppressDerivedAttribute_OnDifferentType ()
    {
      AttributeWithMetadata[] attributes = new AttributeWithMetadata[] {
          new AttributeWithMetadata(typeof (object), new DerivedInheritedAttribute("X")),
      };
      AttributeWithMetadata[] suppressAttributes = new AttributeWithMetadata[] {
          new AttributeWithMetadata(typeof (string), new SuppressAttributesAttribute(typeof (BaseInheritedAttribute))),
      };
      AttributeWithMetadata[] expectedAttributes = new AttributeWithMetadata[] {
      };

      AttributeWithMetadata[] filteredAttributes = AttributeWithMetadata.Suppress (attributes, suppressAttributes).ToArray ();
      Assert.That (filteredAttributes, Is.EqualTo (expectedAttributes));
    }

    [Test]
    public void Suppress_DoesNotSuppressUnrelatedAttribute_OnDifferentType ()
    {
      AttributeWithMetadata[] attributes = new AttributeWithMetadata[] {
          new AttributeWithMetadata(typeof (object), new BaseInheritedAttribute("X")),
      };
      AttributeWithMetadata[] suppressAttributes = new AttributeWithMetadata[] {
          new AttributeWithMetadata(typeof (string), new SuppressAttributesAttribute(typeof (DerivedInheritedAttribute))),
      };
      AttributeWithMetadata[] expectedAttributes = new AttributeWithMetadata[] {
          new AttributeWithMetadata(typeof (object), new BaseInheritedAttribute("X")),
      };

      AttributeWithMetadata[] filteredAttributes = AttributeWithMetadata.Suppress (attributes, suppressAttributes).ToArray ();
      Assert.That (filteredAttributes, Is.EqualTo (expectedAttributes));
    }

    [Test]
    public void ExtractInstances ()
    {
      AttributeWithMetadata[] attributes = new AttributeWithMetadata[] {
          new AttributeWithMetadata(typeof (object), new BaseInheritedAttribute("X")),
          new AttributeWithMetadata(typeof (object), new BaseNonInheritedAttribute("X")),
          new AttributeWithMetadata(typeof (object), new DerivedInheritedAttribute("X")),
          new AttributeWithMetadata(typeof (object), new DerivedNonInheritedAttribute("X")),
      };
      object[] expectedInstances = new object[]
      {
        attributes[0].AttributeInstance,
        attributes[1].AttributeInstance,
        attributes[2].AttributeInstance,
        attributes[3].AttributeInstance,
      };

      object[] instances = AttributeWithMetadata.ExtractInstances (attributes).ToArray ();
      Assert.That (instances, Is.EqualTo (expectedInstances));
    }
  }
}
