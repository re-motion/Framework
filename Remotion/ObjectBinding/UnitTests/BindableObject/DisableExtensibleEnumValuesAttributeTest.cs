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
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class DisableExtensibleEnumValuesAttributeTest
  {
    [Test]
    public void GetEnumerationValueFilter_FromFilterTypeCtor ()
    {
      var attribute = new DisableExtensibleEnumValuesAttribute (typeof (StubEnumerationValueFilter));

      Assert.That (attribute.GetEnumerationValueFilter (), Is.TypeOf (typeof (StubEnumerationValueFilter)));
    }

    [Test]
    public void GetEnumerationValueFilter_FromEnumValueCtorWithArray ()
    {
      var attribute = new DisableExtensibleEnumValuesAttribute (new[] {"Test1", "Test2", "Test3" });

      CheckDisabledIdentifiersFilter(attribute, "Test1", "Test2", "Test3");
    }

    [Test]
    public void GetEnumerationValueFilter_FromEnumValueCtorWithItems_1 ()
    {
      var attribute = new DisableExtensibleEnumValuesAttribute ("Test1");

      CheckDisabledIdentifiersFilter (attribute, "Test1");
    }

    [Test]
    public void GetEnumerationValueFilter_FromEnumValueCtorWithItems_2 ()
    {
      var attribute = new DisableExtensibleEnumValuesAttribute ("Test1", "Test2");

      CheckDisabledIdentifiersFilter (attribute, "Test1", "Test2");
    }
    [Test]
    public void GetEnumerationValueFilter_FromEnumValueCtorWithItems_3 ()
    {
      var attribute = new DisableExtensibleEnumValuesAttribute ("Test1", "Test2", "Test3");

      CheckDisabledIdentifiersFilter (attribute, "Test1", "Test2", "Test3");
    }
    [Test]
    public void GetEnumerationValueFilter_FromEnumValueCtorWithItems_4 ()
    {
      var attribute = new DisableExtensibleEnumValuesAttribute ("Test1", "Test2", "Test3", "Test4");

      CheckDisabledIdentifiersFilter (attribute, "Test1", "Test2", "Test3", "Test4");
    }

    [Test]
    public void GetEnumerationValueFilter_FromEnumValueCtorWithItems_5 ()
    {
      var attribute = new DisableExtensibleEnumValuesAttribute ("Test1", "Test2", "Test3", "Test4", "Test5");

      CheckDisabledIdentifiersFilter (attribute, "Test1", "Test2", "Test3", "Test4", "Test5");
    }

    private void CheckDisabledIdentifiersFilter (DisableExtensibleEnumValuesAttribute attribute, params string[] expectedIDs)
    {
      var filter = attribute.GetEnumerationValueFilter ();
      Assert.That (filter, Is.TypeOf (typeof (DisabledIdentifiersEnumerationFilter)));
      Assert.That (((DisabledIdentifiersEnumerationFilter)filter).DisabledIDs.ToArray(), Is.EqualTo (expectedIDs));
    }
  }
}