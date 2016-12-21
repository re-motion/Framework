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
using System.Globalization;
using NUnit.Framework;
using Remotion.Globalization.UnitTests.TestDomain;
using Remotion.Utilities;

namespace Remotion.Globalization.UnitTests.Obsolete
{
  [TestFixture]
  [Obsolete]
  public class EnumDescriptionTest
  {
    [Test]
    public void TestGetDescriptionForEnumWithDescriptions ()
    {
      using (new CultureScope (CultureInfo.InvariantCulture, CultureInfo.InvariantCulture))
      {
        // try twice to test caching
        for (int i = 0; i < 2; ++i)
        {
          Assert.That (EnumDescription.GetDescription (EnumWithDescription.Value1), Is.EqualTo ("Value I"));
          Assert.That (EnumDescription.GetDescription (EnumWithDescription.Value2), Is.EqualTo ("Value II"));
          Assert.That (EnumDescription.GetDescription (EnumWithDescription.ValueWithoutDescription), Is.EqualTo ("ValueWithoutDescription"));
          Assert.That (EnumDescription.GetDescription ((EnumWithDescription) 100), Is.EqualTo ("100"));
        }
      }
    }

    [Test]
    public void TestGetAvailableValuesForEnumWithDescriptions ()
    {
      using (new CultureScope (CultureInfo.InvariantCulture, CultureInfo.InvariantCulture))
      {
        // try twice to test caching
        for (int i = 0; i < 2; ++i)
        {
          EnumValue[] enumValuesInvariant = EnumDescription.GetAllValues (typeof (EnumWithDescription));
          Assert.That (enumValuesInvariant.Length, Is.EqualTo (3));
          Assert.That (enumValuesInvariant[0].Value, Is.EqualTo (EnumWithDescription.Value1));
          Assert.That (enumValuesInvariant[0].Description, Is.EqualTo ("Value I"));
          Assert.That (enumValuesInvariant[1].Value, Is.EqualTo (EnumWithDescription.Value2));
          Assert.That (enumValuesInvariant[1].Description, Is.EqualTo ("Value II"));
          Assert.That (enumValuesInvariant[2].Value, Is.EqualTo (EnumWithDescription.ValueWithoutDescription));
          Assert.That (enumValuesInvariant[2].Description, Is.EqualTo ("ValueWithoutDescription"));

          CultureInfo culture = new CultureInfo ("en-US");
          EnumValue[] enumValuesSpecific = EnumDescription.GetAllValues (typeof (EnumWithDescription), culture);
          Assert.That (enumValuesSpecific.Length, Is.EqualTo (3));
          Assert.That (enumValuesSpecific[0].Value, Is.EqualTo (EnumWithDescription.Value1));
          Assert.That (enumValuesSpecific[0].Description, Is.EqualTo ("Value I"));
          Assert.That (enumValuesSpecific[1].Value, Is.EqualTo (EnumWithDescription.Value2));
          Assert.That (enumValuesSpecific[1].Description, Is.EqualTo ("Value II"));
          Assert.That (enumValuesSpecific[2].Value, Is.EqualTo (EnumWithDescription.ValueWithoutDescription));
          Assert.That (enumValuesSpecific[2].Description, Is.EqualTo ("ValueWithoutDescription"));
        }
      }
    }

    [Test]
    public void TestGetDescriptionForEnumFromResource ()
    {
      using (new CultureScope (CultureInfo.InvariantCulture, CultureInfo.InvariantCulture))
      {
        Assert.That (EnumDescription.GetDescription (EnumWithResources.Value1), Is.EqualTo ("Value 1"));
        Assert.That (EnumDescription.GetDescription (EnumWithResources.Value2), Is.EqualTo ("Value 2"));
        Assert.That (EnumDescription.GetDescription (EnumWithResources.ValueWithoutResource), Is.EqualTo ("ValueWithoutResource"));
        Assert.That (EnumDescription.GetDescription ((EnumWithResources) 100), Is.EqualTo ("100"));

        CultureInfo culture = new CultureInfo ("de-AT");
        Assert.That (EnumDescription.GetDescription (EnumWithResources.Value1, culture), Is.EqualTo ("Wert 1"));
        Assert.That (EnumDescription.GetDescription (EnumWithResources.Value2, culture), Is.EqualTo ("Wert 2"));
        Assert.That (EnumDescription.GetDescription (EnumWithResources.ValueWithoutResource, culture), Is.EqualTo ("ValueWithoutResource"));
        Assert.That (EnumDescription.GetDescription ((EnumWithResources) 100, culture), Is.EqualTo ("100"));
      }
    }

    [Test]
    public void TestGetAvailableValuesForEnumFromResource ()
    {
      using (new CultureScope (CultureInfo.InvariantCulture, CultureInfo.InvariantCulture))
      {
        // try twice to test caching
        for (int i = 0; i < 2; ++i)
        {
          EnumValue[] enumValues = EnumDescription.GetAllValues (typeof (EnumWithResources));
          Assert.That (enumValues.Length, Is.EqualTo (3));
          Assert.That (enumValues[0].Value, Is.EqualTo (EnumWithResources.Value1));
          Assert.That (enumValues[0].Description, Is.EqualTo ("Value 1"));
          Assert.That (enumValues[1].Value, Is.EqualTo (EnumWithResources.Value2));
          Assert.That (enumValues[1].Description, Is.EqualTo ("Value 2"));
          Assert.That (enumValues[2].Value, Is.EqualTo (EnumWithResources.ValueWithoutResource));
          Assert.That (enumValues[2].Description, Is.EqualTo ("ValueWithoutResource"));
        }
      }
    }
  }
}